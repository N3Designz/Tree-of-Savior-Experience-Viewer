using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro;
using TOSExpViewer.Core;
using TOSExpViewer.Model;
using TOSExpViewer.Properties;
using Action = System.Action;

namespace TOSExpViewer.ViewModels
{
    public class ThemeSelectorViewModel : Screen
    {
        public ThemeSelectorViewModel()
        {
            if (Execute.InDesignMode)
            {
                return; // don't do anything in design time
            }

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
                    Name = themeBaseColour.ToString().ToFriendlyString()
                };
                menuItem.IsChecked = string.Equals(Settings.Default.MetroThemeBaseColour, menuItem.Name.ReverseFriendlyString());

                BaseThemes.Add(menuItem);
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
                    Name = themeAccentColour.ToString().ToFriendlyString(),
                };
                menuItem.IsChecked = string.Equals(Settings.Default.MetroThemeAccentColour, menuItem.Name);
                AccentColours.Add(menuItem);
            }
        }

        public List<MenuItem> BaseThemes { get; } = new List<MenuItem>();

        public List<MenuItem> AccentColours { get; } = new List<MenuItem>();

        protected override void OnActivate()
        {
            Settings.Default.PropertyChanged += SettingsOnPropertyChanged;
            base.OnActivate();
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
                AccentColours.ForEach(x => x.IsChecked = false);
                var menuItem = AccentColours.FirstOrDefault(x => string.Equals(x.Name, Settings.Default.MetroThemeAccentColour.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
            else if (e.PropertyName == nameof(Settings.Default.MetroThemeBaseColour))
            {
                BaseThemes.ForEach(x => x.IsChecked = false);
                var menuItem = BaseThemes.FirstOrDefault(x => string.Equals(x.Name.ReverseFriendlyString(), Settings.Default.MetroThemeBaseColour.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
        }
    }
}
