using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using TOSExpViewer.Model;
using TOSExpViewer.Service;

namespace TOSExpViewer.ViewModels
{
    public class ExpCardCalculatorViewModel : Conductor<ExpCard>.Collection.AllActive
    {
        private const int MAX_EXP_CARD_LEVEL = 10;
        private readonly List<ExpCard> expCards = new List<ExpCard>();

        private ExperienceContainer experienceContainer;

        public ExpCardCalculatorViewModel(ExperienceContainer experienceContainer)
        {
            this.experienceContainer = experienceContainer;
        }

        protected override void OnInitialize()
        {
            for (int i = 0; i < MAX_EXP_CARD_LEVEL; i++)
            {
                var expCard = ExpCard.CreateCardLevel(i + 1);
                expCards.Add(expCard);

                expCard.PropertyChanged += ExpCardOnPropertyChanged;
            }

            base.OnInitialize();
        }

        protected override void OnActivate()
        {
            expCards.ForEach(x => x.Amount = 0);
            Items.AddRange(expCards);
            base.OnActivate();
        }

        public override string DisplayName { get; set; } = "Card Calculator";

        public int BaseLevel { get; set; } = 2;

        public long TotalBaseExperience => Items.Sum(x => x.BaseExperience * x.Amount);

        public long TotalClassExperience => Items.Sum(x => x.ClassExperience * x.Amount);

        public ExperienceCardData BaseExperienceCardData { get; set; } = new ExperienceCardData();

        public ExperienceCardData ClassExperienceCardData { get; set; } = new ExperienceCardData();

        private void ExpCardOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExpCard.Amount))
            { 
                NotifyOfPropertyChange(() => TotalBaseExperience);
                NotifyOfPropertyChange(() => TotalClassExperience);

                CardCalculatorService.calculateLevel(BaseLevel, 1, experienceContainer.BaseExperienceData.CurrentExperience, TotalBaseExperience, ExperienceType.BASE, BaseExperienceCardData);

                // TODO: update calculations to work with class levels as well
                //CardCalculatorService.calculateLevel(8, 1, currentExperience, TotalClassExperience, ExperienceType.CLASS, ClassExperienceCardData);

                NotifyOfPropertyChange(() => BaseExperienceCardData);
                NotifyOfPropertyChange(() => ClassExperienceCardData);
            }
        }
    }
}
