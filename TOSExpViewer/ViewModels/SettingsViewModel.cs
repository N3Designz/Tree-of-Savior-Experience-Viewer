using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using TOSExpViewer.Core;
using TOSExpViewer.Model;
using TOSExpViewer.Model.ExperienceControls;
using TOSExpViewer.Properties;
using Action = System.Action;

namespace TOSExpViewer.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private readonly List<MenuItem> baseThemeMenuItems = new List<MenuItem>();
        private readonly List<MenuItem> accentColourMenuItems = new List<MenuItem>();

        public SettingsViewModel()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            MenuItems.Add(new MenuItem() { MenuItemText = "View" });
            InitializeMenuItems();
        }

        public SettingsViewModel(ExperienceControl[] experienceControls)
        {
            var experienceControlMenuItems = experienceControls.Select(x =>
            {
                var menuItem = new MenuItem(() => x.Show = !x.Show)
                {
                    IsChecked = x.Show,
                    IsCheckable = true,
                    MenuItemText = x.DisplayName,
                    StaysOpenOnClick = true
                };

                x.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(x.Show))
                    {
                        menuItem.IsChecked = x.Show;
                    }
                };

                return menuItem;
            });

            var rootExperienceControlMenuItem = new MenuItem(experienceControlMenuItems) { MenuItemText = "View" };

            MenuItems.Add(rootExperienceControlMenuItem);
            InitializeMenuItems();
        }

        public BindableCollection<MenuItem> MenuItems { get; set; } = new BindableCollection<MenuItem>();

        protected override void OnActivate()
        {
            Settings.Default.PropertyChanged += SettingsOnPropertyChanged;
            base.OnActivate();
        }

        private void InitializeMenuItems()
        {
            foreach (MetroThemeBaseColour themeBaseColour in Enum.GetValues(typeof(MetroThemeBaseColour)))
            {
                Action action = () =>
                {
                    var currentTheme = ThemeManager.DetectAppStyle(Application.Current.MainWindow);
                    var baseTheme = ThemeManager.GetAppTheme(themeBaseColour.ToString());
                    ChangeTheme(baseTheme, currentTheme.Item2);

                    Settings.Default.MetroThemeBaseColour = themeBaseColour.ToString();
                    Settings.Default.Save();
                };

                var menuItem = new MenuItem(action)
                {
                    MenuItemText = themeBaseColour.ToString().ToFriendlyString(),
                    IsCheckable = true,
                    StaysOpenOnClick = true
                };
                menuItem.IsChecked = string.Equals(Settings.Default.MetroThemeBaseColour, menuItem.MenuItemText.ReverseFriendlyString());

                baseThemeMenuItems.Add(menuItem);
            }

            foreach (MetroThemeAccentColour themeAccentColour in Enum.GetValues(typeof(MetroThemeAccentColour)))
            {
                Action action = () =>
                {
                    var currentTheme = ThemeManager.DetectAppStyle(Application.Current.MainWindow);
                    var accent = ThemeManager.GetAccent(themeAccentColour.ToString());
                    ChangeTheme(currentTheme.Item1, accent);

                    Settings.Default.MetroThemeAccentColour = themeAccentColour.ToString();
                    Settings.Default.Save();
                };

                var menuItem = new MenuItem(action)
                {
                    MenuItemText = themeAccentColour.ToString().ToFriendlyString(),
                    IsCheckable = true,
                    StaysOpenOnClick = true
                };
                menuItem.IsChecked = string.Equals(Settings.Default.MetroThemeAccentColour, menuItem.MenuItemText);
                accentColourMenuItems.Add(menuItem);
            }

            var baseThemeRootMenuItem = new MenuItem(baseThemeMenuItems) { MenuItemText = "Base Theme" };
            var accentColourRootMenuItem = new MenuItem(accentColourMenuItems) { MenuItemText = "Accent Colour" };

            MenuItems.AddRange(new[] { baseThemeRootMenuItem, accentColourRootMenuItem });
        }

        private void ChangeTheme(AppTheme baseColour, Accent accentColour)
        {
            // change the theme for the main window so the update is immediate
            ThemeManager.ChangeAppStyle(Application.Current.MainWindow, accentColour, baseColour);

            // change the default theme for all future windows opened
            ThemeManager.ChangeAppStyle(Application.Current, accentColour, baseColour);
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // make sure we check which theme options are currently selected
            if (e.PropertyName == nameof(Settings.Default.MetroThemeAccentColour))
            {
                accentColourMenuItems.ForEach(x => x.IsChecked = false);
                var menuItem = accentColourMenuItems.FirstOrDefault(x => string.Equals(x.MenuItemText, Settings.Default.MetroThemeAccentColour.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
            else if (e.PropertyName == nameof(Settings.Default.MetroThemeBaseColour))
            {
                baseThemeMenuItems.ForEach(x => x.IsChecked = false);
                var menuItem = baseThemeMenuItems.FirstOrDefault(x => string.Equals(x.MenuItemText.ReverseFriendlyString(), Settings.Default.MetroThemeBaseColour.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
        }
    }
}
