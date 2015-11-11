using Caliburn.Micro;

namespace TOSExpViewer.Model.ExperienceControls
{
    public abstract class ExperienceControl : Screen
    {
        private string value;
        private bool show = true;
        private string hideComponentText;
        
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