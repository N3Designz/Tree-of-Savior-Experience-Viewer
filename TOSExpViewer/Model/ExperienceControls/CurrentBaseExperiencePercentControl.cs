using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceControls
{
    public class CurrentBaseExperiencePercentControl : ExperienceControl
    {
        public CurrentBaseExperiencePercentControl()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 91.23.ToString("N4");
        }

        public CurrentBaseExperiencePercentControl(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideCurrentBaseExperencePercent;
            Value = experienceData.CurrentBaseExperiencePercent.ToString("N4");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.CurrentBaseExperiencePercent.ToString("N4");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideCurrentBaseExperencePercent = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Current %";

        public override string HideComponentText { get; set; } = "Hide Current Base %";
    }
}