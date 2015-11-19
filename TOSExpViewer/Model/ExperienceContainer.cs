namespace TOSExpViewer.Model
{
    public class ExperienceContainer
    {
        public ExperienceContainer(ExperienceData experienceData, IExperienceControl[] experienceControls)
        {
            ExperienceData = experienceData;
            IExperienceControls = experienceControls;
        }

        public ExperienceData ExperienceData
        {
            get; set;
        }

        public IExperienceControl[] IExperienceControls
        {
            get; set;
        }

        public void Reset()
        {

        }
    }
}
