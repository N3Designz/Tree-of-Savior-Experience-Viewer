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
            string accentColor = GetAccentColor();
            string baseColor = GetBaseThemeColor();

            ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent(accentColor), ThemeManager.GetAppTheme(baseColor));

            base.OnStartup(e);
        }

        private string GetAccentColor()
        {
            string accentColor = Settings.Default.MetroThemeAccentColor;

            if (!Enum.IsDefined(typeof(MetroThemeAccentColor), accentColor))
            {
                // set safe default
                accentColor = MetroThemeAccentColor.Blue.ToString();
                Settings.Default.MetroThemeAccentColor = accentColor;
                Settings.Default.Save();
            }

            return accentColor;
        }

        private string GetBaseThemeColor()
        {
            string baseColor = Settings.Default.MetroThemeBaseColor;

            if (!Enum.IsDefined(typeof(MetroThemeBaseColor), baseColor))
            {
                // set safe default
                baseColor = MetroThemeBaseColor.BaseLight.ToString();
                Settings.Default.MetroThemeBaseColor = baseColor;
                Settings.Default.Save();
            }

            return baseColor;
        }
    }
}
