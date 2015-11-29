using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TOSExpViewer.Model;
using TOSExpViewer.Service;
using TOSExpViewer.Core;
using TOSExpViewer.Properties;

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
            ExperienceComponents = new BindableCollection<IExperienceControl>(new[]
            {
                new ExperienceControl<ExperienceData>(9123.ToString(), 123.ToString())              { DisplayName = "Current Exp"},
                new ExperienceControl<ExperienceData>(10000.ToString())                             { DisplayName = "Required Exp"},
                new ExperienceControl<ExperienceData>(91.23.ToString("N4"), 1.23.ToString("N4"))    { DisplayName = "Current Exp %"},
                new ExperienceControl<ExperienceData>(112.ToString(), 86.ToString())                { DisplayName = "Last Exp"},
                new ExperienceControl<ExperienceData>(0.0112.ToString("N4"), 0.0086.ToString("N4")) { DisplayName = "Last Exp %"},
                new ExperienceControl<ExperienceData>(8.ToString(), 115.ToString())                 { DisplayName = "Kills TNL"},
                new ExperienceControl<ExperienceData>(1754.ToString(), 956.ToString())              { DisplayName = "Exp/Hr"},
                new ExperienceControl<ExperienceData>("30m", "3hr 02m")                             { DisplayName = "Time TNL"},
                new ExperienceControl<ExperienceData>("12m 05s", null)                              { DisplayName = "Session"},
            });

            foreach (var experienceComponent in ExperienceComponents)
            {
                experienceComponent.ShowClassValue = true;
            }
        }

        public ShellViewModel(SettingsViewModel settingsViewModelViewModel, ExperienceContainer experienceContainer)
        {
            if (settingsViewModelViewModel == null)
            {
                throw new ArgumentNullException(nameof(settingsViewModelViewModel));
            }

            if (experienceContainer == null)
            {
                throw new ArgumentNullException(nameof(experienceContainer));
            }

            ExperienceContainer = experienceContainer;

            SettingsViewModel = settingsViewModelViewModel;
            SettingsViewModel.ActivateWith(this);

            experienceUpdateService = new ExperienceUpdateService();

            ExperienceComponents = new BindableCollection<IExperienceControl>(ExperienceContainer.ExperienceControls);

            timer.Tick += TimerOnTick;
        }

        public override string DisplayName { get; set; } = "Tree of Savior Experience Viewer";

        public ExperienceContainer ExperienceContainer { get; set; }

        public BindableCollection<IExperienceControl> ExperienceComponents { get; set; }

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
                                       (!Settings.Default.HideExperiencePerHour ||
                                       !Settings.Default.HideTimeToLevel);

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

            Settings.Default.Top = window.Top;
            Settings.Default.Left = window.Left;
            Settings.Default.Save();
        }

        protected override async void OnViewReady(object view)
        {
            base.OnViewReady(view);
            await ValidateConfiguration();

            Settings.Default.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Settings.Default.HideExperiencePerHour) ||
                    args.PropertyName == nameof(Settings.Default.HideTimeToLevel))
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
                    RestartSession();
                    ExperienceContainer.BaseExperienceData.Reset();
                    ExperienceContainer.ClassExperienceData.Reset();

                    tosMonitor.Attach();
                }

                if (!tosMonitor.Attached)
                {
                    return; // escape out, the client probably isn't running
                }

                int classTier = tosMonitor.GetClassTier();
                int a = tosMonitor.GetCurrentClassExperienceOffset();

                // TODO - find a way to discover these automatically or at least make them configurable
                const int classRank = 4;
                const int classLevel = 7;

                // TODO: come up with clever solution to just iterate over this and call the right type of experience instead
                experienceUpdateService.UpdateExperienceValues(
                    ExperienceContainer.BaseExperienceData, 
                    currentExperience: tosMonitor.GetCurrentBaseExperience(), 
                    requiredExperience: tosMonitor.GetRequiredExperience());

                experienceUpdateService.UpdateExperienceValues(
                    ExperienceContainer.ClassExperienceData,
                    currentExperience: Constants.GetCurrentClassExperienceForLevelOnly(tosMonitor.GetCurrentClassExperience(), classRank, classLevel),
                    requiredExperience: Constants.GetRequiredClassExperience(classRank, classLevel));
            }
            catch (Exception ex)
            {
                timer.Stop();
                ErrorAndClose("Error polling for experience", ex.Message);
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

            int currentExpMemAddress = await ParseAddress(ConfigurationManager.AppSettings["CurrentExpMemAddress"]);
            int currentClassExperienceAddress = await ParseAddress(ConfigurationManager.AppSettings["CurrentClassExpMemAddress"]);
            int currentClassTierAddress = await ParseAddress(ConfigurationManager.AppSettings["CurrentClassTier"]);

            timer.Interval = TimeSpan.FromMilliseconds(pollingInterval);
            tosMonitor = new TosMonitor(currentExpMemAddress, currentClassExperienceAddress, currentClassTierAddress);
            tosMonitor.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(tosMonitor.Attached)) Attached = tosMonitor.Attached;
            };
            timer.Start();
        }

        private async Task<int> ParseAddress(string address)
        {
            int currentExpMemAddress;

            if (string.IsNullOrWhiteSpace(address))
            {
                await ConfigError("CurrentExpMemAddress setting missing from config file");
                return 0;
            }

            // ignore leading 0x to prevent the c# parser from throwing
            if (address.StartsWith("0x"))
            {
                address = address.Substring(2);
            }

            if (!int.TryParse(address, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out currentExpMemAddress))
            {
                await ConfigError("CurrentExpMemAddress setting value not in expected format (expecting a hex value such as 0x01489F10)");
                return 0;
            }

            return currentExpMemAddress;
        }

        private async Task ConfigError(string errorMessage)
        {
            await ErrorAndClose("Configuration File Invalid", errorMessage);
        }

        private async Task ErrorAndClose(string title, string errorMessage)
        {
            var metroWindow = GetView() as MetroWindow;
            if (metroWindow != null)
            {
                metroWindow.SizeToContent = SizeToContent.Manual; // we need to resize the control manually to fully display the error
                metroWindow.Height = 240;
                await metroWindow.ShowMessageAsync(title, errorMessage);
            }

            await Task.Delay(50);
            TryClose();
        }

        public void RestartSession()
        {
            ExperienceContainer.RestartSession();
        }
    }
}