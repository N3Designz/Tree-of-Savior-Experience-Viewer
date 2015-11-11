using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceControls
{
    public class CurrentBaseExperienceControl : ExperienceControl
    {
        public CurrentBaseExperienceControl()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 9123.ToString("N0");
        }

        public CurrentBaseExperienceControl(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }
            
            // Set initial values
            Show = !Settings.Default.HideCurrentBaseExperience;
            Value = experienceData.CurrentBaseExperience.ToString("N0");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.CurrentBaseExperience.ToString("N0");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideCurrentBaseExperience = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Current Exp";

        public override string HideComponentText { get; set; } = "Hide Current Experience";
    }
}