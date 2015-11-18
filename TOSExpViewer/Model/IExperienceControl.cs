using Caliburn.Micro;

namespace TOSExpViewer.Model
{
    public interface IExperienceControl : IScreen
    {
        /// <summary> The text to display to user when they right click on the control to hide it </summary>
        string HideComponentText { get; set; }

        bool Show { get; set; }

        string Value { get; set; }

        void Hide();
    }
}