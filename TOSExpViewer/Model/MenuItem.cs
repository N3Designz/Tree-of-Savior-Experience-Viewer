using System.Collections.Generic;
using Caliburn.Micro;
using Action = System.Action;

namespace TOSExpViewer.Model
{
    public class MenuItem : PropertyChangedBase
    {
        private static readonly Action EmptyAction = () => { };

        private readonly Action action;
        private string menuItemText;
        private bool isChecked;
        private bool isCheckable;
        private bool staysOpenOnClick;

        public MenuItem() : this(EmptyAction)
        {
        }

        public MenuItem(IEnumerable<MenuItem> menuItems) : this(EmptyAction, menuItems)
        {
        }

        public MenuItem(Action action) : this(action, new MenuItem[] { })
        {
            this.action = action;
        }

        public MenuItem(Action action, IEnumerable<MenuItem> menuItems)
        {
            this.action = action;
            MenuItems = new BindableCollection<MenuItem>(menuItems);
        }

        public string MenuItemText
        {
            get { return menuItemText; }
            set
            {
                if (value == menuItemText) return;
                menuItemText = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (value == isChecked) return;
                isChecked = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsCheckable
        {
            get { return isCheckable; }
            set
            {
                if (value == isCheckable) return;
                isCheckable = value;
                NotifyOfPropertyChange();
            }
        }

        public bool StaysOpenOnClick
        {
            get { return staysOpenOnClick; }
            set
            {
                if (value == staysOpenOnClick) return;
                staysOpenOnClick = value;
                NotifyOfPropertyChange(() => StaysOpenOnClick);
            }
        }

        public BindableCollection<MenuItem> MenuItems { get; }

        public void Execute()
        {
            action();
        }

        public override string ToString() => $"{MenuItemText}, Items={MenuItems.Count}";
    }
}