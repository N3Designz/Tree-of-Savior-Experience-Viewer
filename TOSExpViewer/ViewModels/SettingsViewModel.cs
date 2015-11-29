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
using System.Globalization;
using Action = System.Action;

namespace TOSExpViewer.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private readonly List<MenuItem> baseThemeMenuItems = new List<MenuItem>();
        private readonly List<MenuItem> accentColorMenuItems = new List<MenuItem>();
        private readonly List<MenuItem> classLevelMenuItems = new List<MenuItem>();
        private readonly List<MenuItem> classRankMenuItems = new List<MenuItem>();

        public SettingsViewModel()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            MenuItems.Add(new MenuItem() { MenuItemText = "View" });
            InitializeThemeMenuItems();
        }

        public SettingsViewModel(IExperienceControl[] experienceControls)
        {
            if (experienceControls == null)
            {
                throw new ArgumentNullException(nameof(experienceControls));
            }

            InitializeExperienceControlMenuItems(experienceControls);
            InitializeThemeMenuItems();
            InitializeClassRankMenuItems();
            InitializeClassLevelMenuItems();
        }

        public BindableCollection<MenuItem> MenuItems { get; set; } = new BindableCollection<MenuItem>();

        protected override void OnActivate()
        {
            Settings.Default.PropertyChanged += SettingsOnPropertyChanged;
            base.OnActivate();
        }

        private void InitializeExperienceControlMenuItems(IExperienceControl[] experienceControls)
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
            }).ToList();

            // Toggle showing the class experience row menu item
            Action toggleShowClassExpRow = () =>
            {
                foreach (var experienceControl in experienceControls)
                {
                    experienceControl.ShowClassValue = !experienceControl.ShowClassValue;
                }

                Settings.Default.ShowClassExperienceRow = experienceControls[0].ShowClassValue;
                Settings.Default.Save();
            };

            var toggleShowClassExpMenuItem = new MenuItem(toggleShowClassExpRow)
            {
                MenuItemText = "Show class exp row",
                StaysOpenOnClick = true,
                IsCheckable = true,
                IsChecked = Settings.Default.ShowClassExperienceRow
            };

            // Keep the show class experience row settings value and menu item in sync
            experienceControls[0].PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(IExperienceControl.ShowClassValue))
                {
                    toggleShowClassExpMenuItem.IsChecked = experienceControls[0].ShowClassValue;
                }
            };

            var rootExperienceControlMenuItem = new MenuItem(experienceControlMenuItems) { MenuItemText = "View" };
            rootExperienceControlMenuItem.MenuItems.Add(toggleShowClassExpMenuItem);

            MenuItems.Add(rootExperienceControlMenuItem);
        }

        private void InitializeThemeMenuItems()
        {
            foreach (MetroThemeBaseColor themeBaseColor in Enum.GetValues(typeof(MetroThemeBaseColor)))
            {
                Action action = () =>
                {
                    var currentTheme = ThemeManager.DetectAppStyle(Application.Current.MainWindow);
                    var baseTheme = ThemeManager.GetAppTheme(themeBaseColor.ToString());
                    ChangeTheme(baseTheme, currentTheme.Item2);

                    Settings.Default.MetroThemeBaseColor = themeBaseColor.ToString();
                    Settings.Default.Save();
                };

                var menuItem = new MenuItem(action)
                {
                    MenuItemText = themeBaseColor.ToString().ToFriendlyString(),
                    IsCheckable = true,
                    StaysOpenOnClick = true
                };
                menuItem.IsChecked = string.Equals(Settings.Default.MetroThemeBaseColor, menuItem.MenuItemText.ReverseFriendlyString());

                baseThemeMenuItems.Add(menuItem);
            }

            foreach (MetroThemeAccentColor themeAccentColor in Enum.GetValues(typeof(MetroThemeAccentColor)))
            {
                Action action = () =>
                {
                    var currentTheme = ThemeManager.DetectAppStyle(Application.Current.MainWindow);
                    var accent = ThemeManager.GetAccent(themeAccentColor.ToString());
                    ChangeTheme(currentTheme.Item1, accent);

                    Settings.Default.MetroThemeAccentColor = themeAccentColor.ToString();
                    Settings.Default.Save();
                };

                var menuItem = new MenuItem(action)
                {
                    MenuItemText = themeAccentColor.ToString().ToFriendlyString(),
                    IsCheckable = true,
                    StaysOpenOnClick = true
                };
                menuItem.IsChecked = string.Equals(Settings.Default.MetroThemeAccentColor, menuItem.MenuItemText);
                accentColorMenuItems.Add(menuItem);
            }

            var baseThemeRootMenuItem = new MenuItem(baseThemeMenuItems) { MenuItemText = "Base Theme" };

            MenuItem accentColorRootMenuItem = null;

            var currentCulture = CultureInfo.CurrentCulture;

            if(currentCulture.Name == "en-US")
            {
                accentColorRootMenuItem = new MenuItem(accentColorMenuItems) { MenuItemText = "Accent Color" };
            } else {
                accentColorRootMenuItem = new MenuItem(accentColorMenuItems) { MenuItemText = "Accent Colour" };
            }

            MenuItems.AddRange(new[] { baseThemeRootMenuItem, accentColorRootMenuItem });
        }

        // TODO: these two initialize class level/rank methods can be refactored into one method...
        private void InitializeClassRankMenuItems()
        {
            for (int classRank = 0; classRank < Constants.MAX_CLASS_RANK; classRank++)
            {
                // I don't know why using the for loop index instead of this breaks it... probably something I don't realize about C#.
                int currentRank = classRank + 1;

                Action action = () =>
                {
                    Settings.Default.ClassRank = currentRank.ToString();
                    Settings.Default.Save();
                };

                var menuItem = new MenuItem(action)
                {
                    MenuItemText = currentRank.ToString(),
                    IsCheckable = true,
                    StaysOpenOnClick = true
                };

                menuItem.IsChecked = string.Equals(Settings.Default.ClassRank, menuItem.MenuItemText);

                classRankMenuItems.Add(menuItem);
            }

            var baseClassRankMenuItem = new MenuItem(classRankMenuItems) { MenuItemText = "Class Rank" };

            MenuItems.Add(baseClassRankMenuItem);
        }

        private void InitializeClassLevelMenuItems()
        {
            for(int classLevel = 0; classLevel < Constants.MAX_CLASS_LEVEL; classLevel++)
            {
                // I don't know why using the for loop index instead of this breaks it... probably something I don't realize about C#.
                int currentLevel = classLevel + 1;

                Action action = () =>
                {
                    Settings.Default.ClassLevel = currentLevel.ToString();
                    Settings.Default.Save();
                };

                var menuItem = new MenuItem(action)
                {
                    MenuItemText = currentLevel.ToString(),
                    IsCheckable = true,
                    StaysOpenOnClick = true
                };

                menuItem.IsChecked = string.Equals(Settings.Default.ClassLevel, menuItem.MenuItemText);

                classLevelMenuItems.Add(menuItem);
            }

            var baseClassLevelMenuItem = new MenuItem(classLevelMenuItems) { MenuItemText = "Class Level" };

            MenuItems.Add(baseClassLevelMenuItem);
        }

        private void ChangeTheme(AppTheme baseColor, Accent accentColor)
        {
            // change the theme for the main window so the update is immediate
            ThemeManager.ChangeAppStyle(Application.Current.MainWindow, accentColor, baseColor);

            // change the default theme for all future windows opened
            ThemeManager.ChangeAppStyle(Application.Current, accentColor, baseColor);
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // make sure we check which theme options are currently selected
            if (e.PropertyName == nameof(Settings.Default.MetroThemeAccentColor))
            {
                accentColorMenuItems.ForEach(x => x.IsChecked = false);
                var menuItem = accentColorMenuItems.FirstOrDefault(x => string.Equals(x.MenuItemText, Settings.Default.MetroThemeAccentColor.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
            else if (e.PropertyName == nameof(Settings.Default.MetroThemeBaseColor))
            {
                baseThemeMenuItems.ForEach(x => x.IsChecked = false);
                var menuItem = baseThemeMenuItems.FirstOrDefault(x => string.Equals(x.MenuItemText.ReverseFriendlyString(), Settings.Default.MetroThemeBaseColor.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
            else if (e.PropertyName == nameof(Settings.Default.ClassLevel))
            {
                classLevelMenuItems.ForEach(x => x.IsChecked = false);
                var menuItem = classLevelMenuItems.FirstOrDefault(x => string.Equals(x.MenuItemText, Settings.Default.ClassLevel.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
            else if (e.PropertyName == nameof(Settings.Default.ClassRank))
            {
                classRankMenuItems.ForEach(x => x.IsChecked = false);
                var menuItem = classRankMenuItems.FirstOrDefault(x => string.Equals(x.MenuItemText, Settings.Default.ClassRank.ToString()));

                if (menuItem != null)
                {
                    menuItem.IsChecked = true;
                }
            }
        }
    }
}
