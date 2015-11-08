using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using TOSExpViewer.Core;
using TOSExpViewer.Properties;

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
            container.PerRequest<ShellViewModel>();
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
            var settings = new WindowSettingsBuilder()
                .WithTopLeft(Settings.Default.Top, Settings.Default.Left)
                .AsTopmost()
                .SizeToContent()
                .Create();

            DisplayRootViewFor<ShellViewModel>(settings);
        }
    }
}