using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using TOSExpViewer.Core;
using TOSExpViewer.Model;
using TOSExpViewer.Model.ExperienceControls;
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
            container.Singleton<ExperienceData>();

            var experienceData = container.GetInstance<ExperienceData>();
            var experienceControls = new ExperienceControl[]
            {
                new CurrentBaseExperienceControl(experienceData),
                new RequiredBaseExperienceControl(experienceData),
                new CurrentBaseExperiencePercentControl(experienceData),
                new LastExperienceGainControl(experienceData),
                new KillsTilNextLevelControl(experienceData),
                new ExperiencePerHourControl(experienceData),
                new TimeToLevelControl(experienceData),
            };

            container.Handler<ShellViewModel>(simpleContainer =>
                                              new ShellViewModel(
                                                  container.GetInstance<SettingsViewModel>(),
                                                  experienceData,
                                                  experienceControls)
                );

            container.Handler<SettingsViewModel>(simpleContainer => new SettingsViewModel(experienceControls));
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