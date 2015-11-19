using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TOSExpViewer.Model;
using TOSExpViewer.Service;

namespace TOSExpViewer.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly ExperienceUpdateService experienceUpdateService;
        
        private TosMonitor tosMonitor;
        private bool attached;
        private bool showTitleBar = true;

        public ShellViewModel()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Attached = true;
            BaseExperienceComponents = new BindableCollection<IExperienceControl>(new[]
            {
                new ExperienceControl<ExperienceData>(9123.ToString()) { DisplayName = "Current Exp"},
                new ExperienceControl<ExperienceData>(91.23.ToString("N4")) { DisplayName = "Required Exp"},
                new ExperienceControl<ExperienceData>(1754.ToString("N0")) { DisplayName = "Current Exp %"},
                new ExperienceControl<ExperienceData>(112.ToString("N0")) { DisplayName = "Last Exp Gain"},
                new ExperienceControl<ExperienceData>(0.0112.ToString("N4")) { DisplayName = "Last Exp Gain %"},
                new ExperienceControl<ExperienceData>(8.ToString("N0")) { DisplayName = "Kills TNL"},
                new ExperienceControl<ExperienceData>(1754.ToString("N0")) { DisplayName = "Exp/Hr"},
                new ExperienceControl<ExperienceData>("~30m") { DisplayName = "Time TNL"},
                new ExperienceControl<ExperienceData>("1h 30m") { DisplayName = "Run Time"},
            });
            
            ClassExperienceComponents = new BindableCollection<IExperienceControl>(new[]
            {
                new ExperienceControl<ExperienceData>(9123.ToString()) { DisplayName = ""},
                new ExperienceControl<ExperienceData>(91.23.ToString("N4")) { DisplayName = ""},
                new ExperienceControl<ExperienceData>(1754.ToString("N0")) { DisplayName = ""},
                new ExperienceControl<ExperienceData>(112.ToString("N0")) { DisplayName = ""},
                new ExperienceControl<ExperienceData>(0.0112.ToString("N4")) { DisplayName = ""},
                new ExperienceControl<ExperienceData>(8.ToString("N0")) { DisplayName = ""},
                new ExperienceControl<ExperienceData>(1754.ToString("N0")) { DisplayName = ""},
                new ExperienceControl<ExperienceData>("~30m") { DisplayName = ""},
                new ExperienceControl<ExperienceData>("1h 30m") { DisplayName = ""},
            });
        }

        public ShellViewModel(SettingsViewModel settingsViewModelViewModel, ExperienceData experienceData, IExperienceControl[] experienceControls)
        {
            if (settingsViewModelViewModel == null)
            {
                throw new ArgumentNullException(nameof(settingsViewModelViewModel));
            }

            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            if (experienceControls == null)
            {
                throw new ArgumentNullException(nameof(experienceControls));
            }

            SettingsViewModel = settingsViewModelViewModel;
            BaseExperienceData = experienceData;
            SettingsViewModel.ActivateWith(this);
            experienceUpdateService = new ExperienceUpdateService();
            BaseExperienceComponents = new BindableCollection<IExperienceControl>(experienceControls);
            ClassExperienceComponents = new BindableCollection<IExperienceControl>(experienceControls);

            timer.Tick += TimerOnTick;
        }

        public override string DisplayName { get; set; } = "Tree of Savior Experience Viewer";

        public ExperienceData BaseExperienceData { get; }
        public ExperienceData ClassExperienceData { get; }

        public BindableCollection<IExperienceControl> BaseExperienceComponents { get; set; }
        public BindableCollection<IExperienceControl> ClassExperienceComponents { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        public bool Attached
        {
            get { return attached; }
            set
            {
                if (value == attached)
                {
                    return;
                }

                attached = value;
                NotifyOfPropertyChange(() => Attached);
                NotifyOfPropertyChange(() => ShowResetButton);
            }
        }

        public bool ShowTitleBar
        {
            get { return showTitleBar; }
            set
            {
                if (value == showTitleBar)
                {
                    return;
                }

                showTitleBar = value;
                NotifyOfPropertyChange(() => ShowTitleBar);
            }
        }

        public bool ShowResetButton => Attached &&
                                       (!Properties.Settings.Default.HideExperiencePerHour ||
                                       !Properties.Settings.Default.HideTimeToLevel);

        public void InterceptWindowDoubleClick(MouseButtonEventArgs args)
        {
            ShowTitleBar = !ShowTitleBar;
            args.Handled = true; // prevents the maximize event being triggered for the window
        }

        public void WindowMoved()
        {
            var window = GetView() as MetroWindow;

            if (window == null)
            {
                return;
            }

            Properties.Settings.Default.Top = window.Top;
            Properties.Settings.Default.Left = window.Left;
            Properties.Settings.Default.Save();
        }

        protected override async void OnViewReady(object view)
        {
            base.OnViewReady(view);
            await ValidateConfiguration();

            Properties.Settings.Default.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Properties.Settings.Default.HideExperiencePerHour) ||
                    args.PropertyName == nameof(Properties.Settings.Default.HideTimeToLevel))
                {
                    NotifyOfPropertyChange(() => ShowResetButton);
                }
            };
        }

        public override void TryClose(bool? dialogResult = null)
        {
            timer.Stop();
            base.TryClose(dialogResult);
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (!IsActive || tosMonitor == null)
            {
                return;
            }

            try
            {
                if (!tosMonitor.Attached)
                {
                    experienceUpdateService.Reset(BaseExperienceData);
                    BaseExperienceData.Reset();
                    BaseExperienceData.FirstUpdate = true;
                    tosMonitor.Attach();
                }

                if (!tosMonitor.Attached)
                {
                    return; // escape out, the client probably isn't running
                }

                experienceUpdateService.UpdateExperienceValues(BaseExperienceData, tosMonitor.GetCurrentBaseExperience(), tosMonitor.GetRequiredExperience());
            }
            catch (Exception ex)
            {
                (GetView() as MetroWindow).ShowMessageAsync("Error", ex.Message);
            }
        }

        private async Task ValidateConfiguration()
        {
            int pollingInterval;
            if (!int.TryParse(ConfigurationManager.AppSettings["PollingIntervalMs"], out pollingInterval))
            {
                await ConfigError("PollingIntervalMs setting missing from config file");
                return;
            }

            int currentExpMemAddress;
            var currentExpMemAddressValue = ConfigurationManager.AppSettings["CurrentExpMemAddress"];
            if (string.IsNullOrWhiteSpace(currentExpMemAddressValue))
            {
                await ConfigError("CurrentExpMemAddress setting missing from config file");
                return;
            }

            // ignore leading 0x to prevent the c# parser from throwing
            if (currentExpMemAddressValue.StartsWith("0x"))
                currentExpMemAddressValue = currentExpMemAddressValue.Substring(2);

            if (!int.TryParse(currentExpMemAddressValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out currentExpMemAddress))
            {
                await ConfigError("CurrentExpMemAddress setting value not in expected format (expecting a hex value such as 0x01489F10)");
                return;
            }

            timer.Interval = TimeSpan.FromMilliseconds(pollingInterval);
            tosMonitor = new TosMonitor(currentExpMemAddress);
            tosMonitor.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(tosMonitor.Attached)) Attached = tosMonitor.Attached;
            };
            timer.Start();
        }

        private async Task ConfigError(string errorMessage)
        {
            await (GetView() as MetroWindow).ShowMessageAsync("Configuration File Invalid", errorMessage);
            await Task.Delay(50);
            TryClose();
        }
    }
}