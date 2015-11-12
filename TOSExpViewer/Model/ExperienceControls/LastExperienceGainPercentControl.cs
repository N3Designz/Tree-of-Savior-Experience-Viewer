using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceControls
{
    public class LastExperienceGainPercentControl : ExperienceControl
    {
        public LastExperienceGainPercentControl()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 0.0112.ToString("N4");
        }

        public LastExperienceGainPercentControl(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideLastExperienceGainPercent;
            Value = experienceData.LastExperienceGainPercent.ToString("N4");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.LastExperienceGainPercent.ToString("N4");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideLastExperienceGainPercent = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Last Exp Gain %";

        public override string HideComponentText { get; set; } = "Hide Last Exp Gain %";
    }
}
