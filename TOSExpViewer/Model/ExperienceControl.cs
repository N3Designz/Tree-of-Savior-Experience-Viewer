using System;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model
{
    public class ExperienceControl<T> : Screen, IExperienceControl where T : class, INotifyPropertyChangedEx
    {
        private string baseValue;
        private bool show = true;
        private bool canShowClassValue = true;
        private bool showClassValue = true;
        private string hideComponentText;
        private string classValue;

        #region Design time constructors

        public ExperienceControl(string designTimeBaseValue) : this(designTimeBaseValue, designTimeBaseValue)
        {
        }

        public ExperienceControl(string designTimeBaseValue, string designTimeClassValue)
        {
            if (!Execute.InDesignMode)
            {
                throw new InvalidOperationException("Constructor only accessible from design time");
            }

            if (designTimeBaseValue == null)
            {
                throw new ArgumentNullException(nameof(designTimeBaseValue));
            }

            BaseValue = designTimeBaseValue;
            ClassValue = designTimeClassValue;
        }

        #endregion

        /// <param name="settingsPropertySelector">The property in the <see cref="Settings"/> class to bind to for saving the hide state</param>
        /// <param name="baseExperienceObject">The class for which to listen to base experience property change notifications</param>
        /// <param name="propertyChangedPropertySelector">The property to watch for property change notifications</param>
        /// <param name="valueFunc">A function to format the experience value for display purposes</param>
        /// <param name="classExperienceObject">The class for which to listen to class experience property change notifications</param>
        public ExperienceControl(
            Expression<Func<Settings, bool>> settingsPropertySelector,
            T baseExperienceObject,
            Expression<Func<T, object>> propertyChangedPropertySelector,
            Func<T, string> valueFunc,
            T classExperienceObject = null)
        {
            Show = !settingsPropertySelector.Compile()(Settings.Default);

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    var propertyInfo = (PropertyInfo)((MemberExpression)settingsPropertySelector.Body).Member;
                    propertyInfo.SetValue(Settings.Default, !Show);
                    Settings.Default.Save();
                }
            };
            
            var propertyChangedPropertyName = GetPropertyName(propertyChangedPropertySelector);
            if (string.IsNullOrWhiteSpace(propertyChangedPropertyName))
            {
                throw new InvalidOperationException(
                    "Unable to discover experience property name." +
                    $"{Environment.NewLine}Type: {typeof(T).Name}" +
                    $"{Environment.NewLine}Failed property selector: {propertyChangedPropertySelector}");
            }

            // Hookup base experience data
            baseExperienceObject.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == propertyChangedPropertyName)
                {
                    BaseValue = valueFunc(baseExperienceObject);
                }
            };

            // Hookup class experience data
            if (classExperienceObject != null)
            {
                classExperienceObject.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == propertyChangedPropertyName)
                    {
                        ClassValue = valueFunc(classExperienceObject);
                    }
                };
            }
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

        /// <summary>
        /// Set to <b>true</b> by default <para />
        /// Show or hide the <see cref="ExperienceControl{T}"/>
        /// </summary>
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

        public string BaseValue
        {
            get { return baseValue; }
            set
            {
                if (value == this.baseValue) return;
                this.baseValue = value;
                NotifyOfPropertyChange(() => BaseValue);
            }
        }

        public string ClassValue
        {
            get { return classValue; }
            set
            {
                if (value == classValue) return;
                classValue = value;
                NotifyOfPropertyChange(() => ClassValue);
            }
        }

        /// <summary>
        /// Show or hide the <see cref="ClassValue"/> <para />
        /// Set to <b>true</b> by default
        /// </summary>
        public bool ShowClassValue
        {
            get { return CanShowClassValue && showClassValue; }
            set
            {
                if (value == showClassValue) return;
                showClassValue = value;
                NotifyOfPropertyChange(() => ShowClassValue);
            }
        }

        /// <summary>
        /// Hack to allow for "base value only" controls that will ignore <see cref="ShowClassValue"/> <para />
        /// Set to <b>true</b> by default  
        /// </summary>
        public bool CanShowClassValue
        {
            get { return canShowClassValue; }
            set
            {
                if (value == canShowClassValue) return;
                canShowClassValue = value;
                NotifyOfPropertyChange(() => CanShowClassValue);
                NotifyOfPropertyChange(() => ShowClassValue);
            }
        }

        public void Hide()
        {
            Show = false;
        }

        private static string GetPropertyName(Expression<Func<T, object>> expression)
        {
            var memberExpression = GetMemberExpression(expression);
            return memberExpression?.Member.Name ?? string.Empty;
        }

        private static MemberExpression GetMemberExpression(Expression<Func<T, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            var unary = expression.Body as UnaryExpression;
            return member ?? unary?.Operand as MemberExpression;
        }
    }
}