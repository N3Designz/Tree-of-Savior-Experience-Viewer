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

            container.Singleton<IWindowManager, MetroWindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();

            var baseExperienceData = new ExperienceData() { DisplayName = "Base Experience" };
            var classExperienceData = new ExperienceData() { DisplayName = "Class Experience" };
            IExperienceControl[] baseExperienceControls = GetExperienceControls(baseExperienceData, classExperienceData);

            ExperienceContainer experienceContainer = new ExperienceContainer(baseExperienceData, classExperienceData, baseExperienceControls);

            container.Handler<ShellViewModel>(simpleContainer =>
                                              new ShellViewModel(
                                                  container.GetInstance<SettingsViewModel>(),
                                                  experienceContainer,
                                                  new ExpCardCalculatorViewModel(experienceContainer),
                                                  container.GetInstance<IWindowManager>()));

            // TODO - refactor settings view model to just take a collection of menuitems
            container.Handler<SettingsViewModel>(simpleContainer => new SettingsViewModel(baseExperienceControls));
        }

        private IExperienceControl[] GetExperienceControls(ExperienceData baseExperienceData, ExperienceData classExperienceData)
        {
            IExperienceControl[] experienceControls = new IExperienceControl[]
            {
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideCurrentBaseExperience,
                    baseExperienceData,
                    baseData => baseData.CurrentExperience,
                    baseData => baseData.CurrentExperience.ToString("N0"),
                    classExperienceData)
                {
                    DisplayName = "Current Exp",
                    HideComponentText = "Hide Current Experience"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideRequiredBaseExperience,
                    baseExperienceData,
                    baseData => baseData.RequiredExperience,
                    baseData => baseData.RequiredExperience.ToString("N0"),
                    classExperienceData)
                {
                    DisplayName = "Required Exp",
                    HideComponentText = "Hide Required Exp"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideCurrentBaseExperencePercent,
                    baseExperienceData,
                    baseData => baseData.CurrentExperiencePercent,
                    baseData => baseData.CurrentExperiencePercent.ToString("N4"),
                    classExperienceData)
                {
                    DisplayName = "Current Exp %",
                    HideComponentText = "Hide Current Experience %"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideLastExperienceGain,
                    baseExperienceData,
                    baseData => baseData.LastExperienceGain,
                    baseData => baseData.LastExperienceGain.ToString("N0"),
                    classExperienceData)
                {
                    DisplayName = "Last Exp",
                    HideComponentText = "Hide Last Exp Gain"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideLastExperienceGainPercent,
                    baseExperienceData,
                    baseData => baseData.LastExperienceGainPercent,
                    baseData => baseData.LastExperienceGainPercent.ToString("N4"),
                    classExperienceData)
                {
                    DisplayName = "Last Exp %",
                    HideComponentText = "Hide Last Exp Gain %"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideKillsTilNextLevel,
                    baseExperienceData,
                    baseData => baseData.KillsTilNextLevel,
                    baseData => baseData.KillsTilNextLevel.ToString("N0"),
                    classExperienceData)
                {
                    DisplayName = "Kills TNL",
                    HideComponentText = "Hide Kills TNL"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideExperiencePerHour,
                    baseExperienceData,
                    baseData => baseData.ExperiencePerHour,
                    baseData => baseData.ExperiencePerHour.ToString("N0"),
                    classExperienceData)
                {
                    DisplayName = "Exp/Hr",
                    HideComponentText = "Hide Exp/Hr"
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideTimeToLevel,
                    baseExperienceData,
                    baseData => baseData.TimeToLevel,
                    baseData => baseData.TimeToLevel,
                    classExperienceData)
                {
                    DisplayName = "Time TNL",
                    HideComponentText = "Hide Time TNL",
                },
                new ExperienceControl<ExperienceData>(
                    settings => settings.HideSessionTime,
                    baseExperienceData,
                    data => data.ElapsedTime,
                    data => data.ElapsedTime.ToShortDisplayFormat())
                {
                    DisplayName = "Session",
                    HideComponentText = "Hide Session Time",
                    CanShowClassValue = false
                }, 
            };

            foreach (var experienceControl in experienceControls)
            {
                experienceControl.ShowClassValue = Settings.Default.ShowClassExperienceRow;
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