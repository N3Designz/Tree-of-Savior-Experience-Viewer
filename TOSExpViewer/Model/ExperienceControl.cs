using System;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model
{
    public class ExperienceControl<T> : Screen, IExperienceControl where T : INotifyPropertyChangedEx
    {
        private string value;
        private bool show = true;
        private string hideComponentText;

        public ExperienceControl(string designTimeValue)
        {
            if (!Execute.InDesignMode)
            {
                throw new InvalidOperationException("Constructor only accessible from design time");
            }

            if (designTimeValue == null)
            {
                throw new ArgumentNullException(nameof(designTimeValue));
            }

            Value = designTimeValue;
        }

        /// <summary> Generic ui control data class </summary>
        /// <param name="settingsPropertySelector">The property in the <see cref="Settings"/> class to bind to for saving the hide state</param>
        /// <param name="propertyChanged">The class for which to listen to property change notifications</param>
        /// <param name="valueFunc">A function to format the value from the <param name="propertyChanged" /> class </param>
        public ExperienceControl(
            Expression<Func<Settings, bool>> settingsPropertySelector,
            T propertyChanged,
            Func<T, string> valueFunc)
        {
            Show = settingsPropertySelector.Compile()(Settings.Default);

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    var propertyInfo = (PropertyInfo)((MemberExpression)settingsPropertySelector.Body).Member;
                    propertyInfo.SetValue(Settings.Default, Show);
                    Settings.Default.Save();
                }
            };

            propertyChanged.PropertyChanged += (sender, args) =>
            {
                Value = valueFunc(propertyChanged);
            };
        }

        /// <summary> The text to display to user when they right click on the control to hide it </summary>
        public virtual string HideComponentText
        {
            get { return hideComponentText; }
            set
            {
                if (value == hideComponentText) return;
                hideComponentText = value;
                NotifyOfPropertyChange(() => HideComponentText);
            }
        }

        public virtual bool Show
        {
            get { return show; }
            set
            {
                if (value == show) return;
                show = value;
                NotifyOfPropertyChange(() => Show);
            }
        }

        public string Value
        {
            get { return value; }
            set
            {
                if (value == this.value) return;
                this.value = value;
                NotifyOfPropertyChange(() => Value);
            }
        }

        public void Hide()
        {
            Show = false;
        }
    }
}