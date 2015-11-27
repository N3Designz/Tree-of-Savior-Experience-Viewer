using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using TOSExpViewer.Core;
using TOSExpViewer.Model;
using TOSExpViewer.Properties;
using TOSExpViewer.ViewModels;

namespace TOSExpViewer
{
    public class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.Singleton<ExperienceData>("baseExperienceData");
            container.Singleton<ExperienceData>("classExperienceData");

            ExperienceData baseExperienceData = container.GetInstance<ExperienceData>("baseExperienceData");
            IExperienceControl[] baseExperienceControls = GetExperienceControls(baseExperienceData, false);

            ExperienceData classExperienceData = container.GetInstance<ExperienceData>("classExperienceData");
            IExperienceControl[] classExperienceControls = GetExperienceControls(classExperienceData, false);

            List<ExperienceContainer> experienceContainers = new List<ExperienceContainer>();

            experienceContainers.Add(new ExperienceContainer(baseExperienceData, baseExperienceControls));
            experienceContainers.Add(new ExperienceContainer(classExperienceData, classExperienceControls));

            container.Handler<ShellViewModel>(simpleContainer => new ShellViewModel(container.GetInstance<SettingsViewModel>(), experienceContainers));

            container.Handler<SettingsViewModel>(simpleContainer => new SettingsViewModel(baseExperienceControls));
        }

        private IExperienceControl[] GetExperienceControls(ExperienceData experienceData, bool hideHeader)
        {
            IExperienceControl[] experienceControls = new IExperienceControl[]
            {
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideCurrentBaseExperience,
                    experienceData,
                    data => data.CurrentBaseExperience,
                    data => data.CurrentBaseExperience.ToString("N0"))
                {
                    DisplayName = "Current Exp",
                    HideComponentText = "Hide Current Experience"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideRequiredBaseExperience,
                    experienceData,
                    data => data.RequiredBaseExperience,
                    data => data.RequiredBaseExperience.ToString("N0"))
                {
                    DisplayName = "Required Exp",
                    HideComponentText = "Hide Required Exp"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideCurrentBaseExperencePercent,
                    experienceData,
                    data => data.CurrentBaseExperiencePercent,
                    data => data.CurrentBaseExperiencePercent.ToString("N4"))
                {
                    DisplayName = "Current Exp %",
                    HideComponentText = "Hide Current Experience %"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideLastExperienceGain,
                    experienceData,
                    data => data.LastExperienceGain,
                    data => data.LastExperienceGain.ToString("N0"))
                {
                    DisplayName = "Last Exp Gain",
                    HideComponentText = "Hide Last Exp Gain"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideLastExperienceGainPercent,
                    experienceData,
                    data => data.LastExperienceGainPercent,
                    data => data.LastExperienceGainPercent.ToString("N4"))
                {
                    DisplayName = "Last Exp Gain %",
                    HideComponentText = "Hide Last Exp Gain %"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideKillsTilNextLevel,
                    experienceData,
                    data => data.KillsTilNextLevel,
                    data => data.KillsTilNextLevel.ToString("N0"))
                {
                    DisplayName = "Kills TNL",
                    HideComponentText = "Hide Kills TNL"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideExperiencePerHour,
                    experienceData,
                    data => data.ExperiencePerHour,
                    data => data.ExperiencePerHour.ToString("N0"))
                {
                    DisplayName = "Exp/Hr",
                    HideComponentText = "Hide Exp/Hr"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideTimeToLevel,
                    experienceData,
                    data => data.TimeToLevel,
                    data => data.TimeToLevel)
                {
                    DisplayName = "Time TNL",
                    HideComponentText = "Hide Time TNL"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideSessionTime,
                    experienceData,
                    data => data.ElapsedTime,
                    data => data.ElapsedTime.ToShortDisplayFormat())
                {
                    DisplayName = "Session",
                    HideComponentText = "Hide Session Time"
                }, 
            };

            // TODO: this doesn't work like I had hoped. figure out a better way to hide the header.
            if(hideHeader)
            {
                foreach(var experienceControl in experienceControls)
                {
                    experienceControl.DisplayName = "";
                }
            }

            return experienceControls;
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
            {
                return instance;
            }

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var settingsBuilder = new WindowSettingsBuilder()
                .AsTopmost()
                .SizeToContent();

            // Can't use SystemParameters.WorkArea as that defines only the primary monitor, virtual screen defines the multi-monitor work area
            var virtualTopLeft = new Point(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop);
            var virtualSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
            var virtualRect = new Rect(virtualTopLeft, virtualSize);

            // Guard against the existing window position being outside the screen, if it is just center the app to teh screen
            if ((Settings.Default.Top == 0 && Settings.Default.Left == 0) ||
                !virtualRect.Contains(new Point(Settings.Default.Left, Settings.Default.Top)))
            {
                settingsBuilder.WithStartupLocation(WindowStartupLocation.CenterScreen);
            }
            else
            {
                settingsBuilder.WithTopLeft(Settings.Default.Top, Settings.Default.Left);
            }

            var settings = settingsBuilder.Create();
            DisplayRootViewFor<ShellViewModel>(settings);
        }
    }
}