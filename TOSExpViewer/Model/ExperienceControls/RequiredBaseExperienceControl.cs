using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceControls
{
    public class RequiredBaseExperienceControl : ExperienceControl
    {
        public RequiredBaseExperienceControl()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 10000.ToString("N0");
        }

        public RequiredBaseExperienceControl(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideRequiredBaseExperience;
            Value = experienceData.RequiredBaseExperience.ToString("N0");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.RequiredBaseExperience.ToString("N0");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideRequiredBaseExperience = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Required Exp";

        public override string HideComponentText { get; set; } = "Hide Required Exp";
    }
}