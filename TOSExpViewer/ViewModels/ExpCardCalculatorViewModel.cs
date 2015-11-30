using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
using TOSExpViewer.Model;

namespace TOSExpViewer.ViewModels
{
    public class ExpCardCalculatorViewModel : Conductor<ExpCard>.Collection.AllActive
    {
        private const int MAX_EXP_CARD_LEVEL = 10;
        private readonly List<ExpCard> expCards = new List<ExpCard>();

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

        public long TotalBaseExperience => Items.Sum(x => x.BaseExperience * x.Amount);

        public long TotalClassExperience => Items.Sum(x => x.ClassExperience * x.Amount);
        
        private void ExpCardOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExpCard.Amount))
            {
                NotifyOfPropertyChange(() => TotalBaseExperience);
                NotifyOfPropertyChange(() => TotalClassExperience);
            }
        }
    }
}
