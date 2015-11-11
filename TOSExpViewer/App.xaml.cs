using MahApps.Metro;
using System;
using System.Windows;
using TOSExpViewer.Core;
using TOSExpViewer.Properties;

namespace TOSExpViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Load the theme set by the user
            string accentColour = GetAccentColour();
            string baseColour = GetBaseThemeColour();

            ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent(accentColour), ThemeManager.GetAppTheme(baseColour));

            base.OnStartup(e);
        }

        private string GetAccentColour()
        {
            string accentColour = Settings.Default.MetroThemeAccentColour;

            if (!Enum.IsDefined(typeof(MetroThemeAccentColour), accentColour))
            {
                // set safe default
                accentColour = MetroThemeAccentColour.Blue.ToString();
                Settings.Default.MetroThemeAccentColour = accentColour;
                Settings.Default.Save();
            }

            return accentColour;
        }

        private string GetBaseThemeColour()
        {
            string baseColour = Settings.Default.MetroThemeBaseColour;

            if (!Enum.IsDefined(typeof(MetroThemeBaseColour), baseColour))
            {
                // set safe default
                baseColour = MetroThemeBaseColour.BaseLight.ToString();
                Settings.Default.MetroThemeBaseColour = baseColour;
                Settings.Default.Save();
            }

            return baseColour;
        }
    }
}
