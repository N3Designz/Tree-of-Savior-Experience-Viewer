using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TOSExpViewer.Core;
using TOSExpViewer.Model;
using TOSExpViewer.Model.ExperienceControls;
using TOSExpViewer.Service;

namespace TOSExpViewer.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly ExperienceDataToTextService experienceDataToTextService;
        private TosMonitor tosMonitor;
        private bool firstUpdate = true;
        private bool attached;
        private bool showTitleBar = true;

        public ShellViewModel()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Attached = true;
            ExperienceComponents = new BindableCollection<ExperienceControl>(new ExperienceControl[]
            {
                new CurrentBaseExperienceControl(),
                new RequiredBaseExperienceControl(), 
                new CurrentBaseExperiencePercentControl(), 
                new LastExperienceGainControl(), 
                new LastExperienceGainPercentControl(), 
                new KillsTilNextLevelControl(), 
                new ExperiencePerHourControl(), 
                new TimeToLevelControl(), 
            });
        }

        public ShellViewModel(
            SettingsViewModel settingsViewModelViewModel,
            ExperienceData experienceData,
            ExperienceControl[] experienceControls)
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
            ExperienceData = experienceData;
            SettingsViewModel.ActivateWith(this);
            experienceDataToTextService = new ExperienceDataToTextService(); // must not be initialized in design time
            ExperienceComponents = new BindableCollection<ExperienceControl>(experienceControls);

            timer.Tick += TimerOnTick;
        }

        public override string DisplayName { get; set; } = "Tree of Savior Experience Viewer";

        public ExperienceData ExperienceData { get; }

        public BindableCollection<ExperienceControl> ExperienceComponents { get; set; }

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

        public void Reset()
        {
            ExperienceData.GainedBaseExperience = 0;
            ExperienceData.StartTime = DateTime.Now;
            ExperienceData.TimeToLevel = CalculateTimeToLevel(ExperienceData);
            NotifyOfPropertyChange(() => ExperienceData.ExperiencePerHour);
        }

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
                    Reset();
                    ExperienceData.Reset();
                    firstUpdate = true;
                    tosMonitor.Attach();
                }

                if (!tosMonitor.Attached)
                {
                    return; // escape out, the client probably isn't running
                }

                UpdateExperienceValues();
            }
            catch (Exception ex)
            {
                (GetView() as MetroWindow).ShowMessageAsync("Error", ex.Message);
            }
        }

        private void UpdateExperienceValues()
        {
            var newCurrentBaseExperience = tosMonitor.GetCurrentBaseExperience();
            var requiredBasedExp = tosMonitor.GetRequiredExperience();

            if (newCurrentBaseExperience == int.MinValue || requiredBasedExp == int.MinValue ||
                requiredBasedExp == 15) // for some reason required base exp returns as 15 when char not selected, no idea why
            {
                Reset();
                ExperienceData.Reset();
                return;
            }

            ExperienceData.RequiredBaseExperience = requiredBasedExp;

            if (firstUpdate)
            {
                ExperienceData.PreviousRequiredBaseExperience = requiredBasedExp;
                ExperienceData.CurrentBaseExperience = newCurrentBaseExperience;
                firstUpdate = false;
            }
            else if (newCurrentBaseExperience != ExperienceData.CurrentBaseExperience) // exp hasn't changed, nothing else to do
            {
                if (ExperienceData.RequiredBaseExperience > ExperienceData.PreviousRequiredBaseExperience) // handle level up scenarios
                {
                    ExperienceData.LastExperienceGain = (ExperienceData.PreviousRequiredBaseExperience - ExperienceData.CurrentBaseExperience) + newCurrentBaseExperience;
                    ExperienceData.PreviousRequiredBaseExperience = requiredBasedExp;
                }
                else
                {
                    ExperienceData.LastExperienceGain = newCurrentBaseExperience - ExperienceData.CurrentBaseExperience;
                }

                ExperienceData.CurrentBaseExperience = newCurrentBaseExperience;
                ExperienceData.GainedBaseExperience += ExperienceData.LastExperienceGain;
            }

            ExperienceData.ExperiencePerHour = ExperienceData.GainedBaseExperience * (int)(TimeSpan.FromHours(1).TotalMilliseconds / ExperienceData.ElapsedTime.TotalMilliseconds);

            ExperienceData.TimeToLevel = CalculateTimeToLevel(ExperienceData);

            experienceDataToTextService.writeToFile(ExperienceData);
        }

        private string CalculateTimeToLevel(ExperienceData experienceData)
        {
            if (experienceData.LastExperienceGain == 0)
            {
                return Constants.INFINITY;
            }

            var totalExperienceRequired = experienceData.RequiredBaseExperience - experienceData.CurrentBaseExperience;
            var experiencePerSecond = experienceData.GainedBaseExperience / experienceData.ElapsedTime.TotalSeconds;

            if (experiencePerSecond == 0 || double.IsNaN(experiencePerSecond))
            {
                return Constants.INFINITY;
            }

            var estimatedTimeToLevel = TimeSpan.FromSeconds(totalExperienceRequired / experiencePerSecond);

            if (estimatedTimeToLevel >= TimeSpan.FromDays(1) || estimatedTimeToLevel < TimeSpan.Zero)
            {
                return Constants.INFINITY;
            }

            if (estimatedTimeToLevel >= TimeSpan.FromHours(1))
            {
                return $"{estimatedTimeToLevel.Hours:00}h {estimatedTimeToLevel.Minutes:00}m";
            }

            if (estimatedTimeToLevel >= TimeSpan.FromMinutes(1))
            {
                return $"~{estimatedTimeToLevel.Minutes}m";
            }

            return $"~{estimatedTimeToLevel.Seconds}s";
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