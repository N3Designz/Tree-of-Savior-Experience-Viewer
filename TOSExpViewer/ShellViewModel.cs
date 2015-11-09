using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TOSExpViewer.Model;
using TOSExpViewer.Properties;

namespace TOSExpViewer
{
    public class ShellViewModel : Screen
    {
        private const string INFINITY = "\u221E";

        private readonly DispatcherTimer timer = new DispatcherTimer();
        private TosMonitor tosMonitor;
        private bool firstUpdate = true;
        private bool attached;

        public ShellViewModel()
        {
            if (Execute.InDesignMode)
            {
                Attached = true;
                return;
            }

            timer.Tick += TimerOnTick;
        }

        public override string DisplayName { get; set; } = "Tree of Savior Experience Viewer";

        public ExperienceData ExperienceData { get; } = new ExperienceData();

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
            }
        }

        public string TimeToLevel
        {
            get
            {
                if (ExperienceData.LastExperienceGain == 0)
                {
                    return INFINITY;
                }
                
                var totalExperienceRequired = ExperienceData.RequiredBaseExperience - ExperienceData.CurrentBaseExperience;
                var experiencePerSecond = ExperienceData.GainedBaseExperience / ExperienceData.ElapsedTime.TotalSeconds;

                if(experiencePerSecond == 0)
                {
                    return INFINITY;
                }
                
                var estimatedTimeToLevel = TimeSpan.FromSeconds(totalExperienceRequired / experiencePerSecond);

                if (estimatedTimeToLevel >= TimeSpan.FromDays(1) || estimatedTimeToLevel < TimeSpan.Zero)
                {
                    return INFINITY;
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
        }

        public double ExpPerHour
        {
            get
            {
                return ExperienceData.GainedBaseExperience * (TimeSpan.FromHours(1).TotalMilliseconds / ExperienceData.ElapsedTime.TotalMilliseconds);
            }
        }

        public void Reset()
        {
            ExperienceData.GainedBaseExperience = 0;
            ExperienceData.StartTime = DateTime.Now;
            NotifyOfPropertyChange(() => TimeToLevel);
            NotifyOfPropertyChange(() => ExpPerHour);
        }

        public void InterceptWindowDoubleClick(MouseButtonEventArgs args)
        {
            var window = GetView() as MetroWindow;

            if (window != null)
            {
                window.ShowTitleBar = !window.ShowTitleBar;
                window.ShowMinButton = window.ShowTitleBar;
                window.ShowMaxRestoreButton = window.ShowTitleBar;
                window.ShowCloseButton = window.ShowTitleBar;
            }

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
            var newBaseExp = tosMonitor.GetBaseExperience();
            var requiredBasedExp = tosMonitor.GetRequiredExperience();

            if (newBaseExp == int.MinValue || requiredBasedExp == int.MinValue ||
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
                ExperienceData.CurrentBaseExperience = newBaseExp;
                firstUpdate = false;
            }
            else if (newBaseExp != ExperienceData.CurrentBaseExperience) // exp hasn't changed, nothing else to do
            {
                if (ExperienceData.RequiredBaseExperience > ExperienceData.PreviousRequiredBaseExperience) // handle level up scenarios
                {
                    ExperienceData.LastExperienceGain = (ExperienceData.RequiredBaseExperience - ExperienceData.CurrentBaseExperience) + newBaseExp;
                    ExperienceData.PreviousRequiredBaseExperience = requiredBasedExp;
                }
                else
                {
                    ExperienceData.LastExperienceGain = newBaseExp - ExperienceData.CurrentBaseExperience;
                }

                ExperienceData.CurrentBaseExperience = newBaseExp;
                ExperienceData.GainedBaseExperience += ExperienceData.LastExperienceGain;
            }

            NotifyOfPropertyChange(() => ExpPerHour);
            NotifyOfPropertyChange(() => TimeToLevel);
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