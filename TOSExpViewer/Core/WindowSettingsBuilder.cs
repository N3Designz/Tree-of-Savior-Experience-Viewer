﻿using System.Dynamic;
using System.Windows;
using System.Windows.Media;

namespace TOSExpViewer.Core
{
    public class WindowSettingsBuilder
    {
        private readonly dynamic settings = new ExpandoObject();

        public WindowSettingsBuilder WithTopLeft(double top = 0, double left = 0)
        {
            if (top == 0 && left == 0)
            {
                return this;
            }

            WithStartupLocation(WindowStartupLocation.Manual);
            settings.Left = left;
            settings.Top = top;
            return this;
        }

        /// <summary> Convenience for setting WithResizeMode noresize and WindowStyle none </summary>
        public WindowSettingsBuilder NoResizeBorderless()
        {
            WithResizeMode(ResizeMode.NoResize);
            WithWindowStyle(WindowStyle.None);
            return this;
        }

        public WindowSettingsBuilder SizeToContent(SizeToContent size = System.Windows.SizeToContent.WidthAndHeight)
        {
            settings.SizeToContent = size;
            return this;
        }

        public WindowSettingsBuilder WithResizeMode(ResizeMode resizeMode)
        {
            settings.ResizeMode = resizeMode;
            return this;
        }

        public WindowSettingsBuilder WithWindowStyle(WindowStyle windowStyle)
        {
            if (windowStyle != WindowStyle.None)
            {
                settings.AllowsTransparency = false; // transparency + windows style other than none will trigger an exception
            }

            settings.WindowStyle = windowStyle;
            return this;
        }

        public WindowSettingsBuilder WithStartupLocation(WindowStartupLocation startupLocation)
        {
            settings.WindowStartupLocation = startupLocation;
            return this;
        }

        public WindowSettingsBuilder AsTopmost()
        {
            settings.Topmost = true;
            return this;
        }

        public WindowSettingsBuilder TransparentBackground()
        {
            settings.AllowsTransparency = true;
            settings.Background = Brushes.Transparent;
            return this;
        }

        public ExpandoObject Create()
        {
            return settings;
        }
    }
}
