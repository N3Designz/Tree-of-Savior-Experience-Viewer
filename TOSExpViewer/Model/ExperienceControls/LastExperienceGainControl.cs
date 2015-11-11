using System;
using Caliburn.Micro;
using TOSExpViewer.Properties;

namespace TOSExpViewer.Model.ExperienceControls
{
    public class LastExperienceGainControl : ExperienceControl
    {
        public LastExperienceGainControl()
        {
            if (!Execute.InDesignMode)
                throw new InvalidOperationException("Constructor only accessible from design time");

            Value = 112.ToString("N0");
        }

        public LastExperienceGainControl(ExperienceData experienceData)
        {
            if (experienceData == null)
            {
                throw new ArgumentNullException(nameof(experienceData));
            }

            // Set initial values
            Show = !Settings.Default.HideLastExperienceGain;
            Value = experienceData.LastExperienceGain.ToString("N0");

            // Hook up property change events to keep everything in sync
            experienceData.PropertyChanged += (sender, args) =>
            {
                Value = experienceData.LastExperienceGain.ToString("N0");
            };

            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Show))
                {
                    Settings.Default.HideLastExperienceGain = !Show;
                    Settings.Default.Save();
                }
            };
        }

        public override string DisplayName { get; set; } = "Last Exp Gain";

        public override string HideComponentText { get; set; } = "Hide Last Exp Gain";
    }
}