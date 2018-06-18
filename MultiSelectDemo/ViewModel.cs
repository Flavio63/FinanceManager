using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSelectDemo
{
    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            obsItems = new ObservableCollection<Proprietari>();
            
            obsItems.Add(new Proprietari() { idx = 1, name = "Flavio" });
            obsItems.Add(new Proprietari() { idx = 2, name = "Dany" });
            obsItems.Add(new Proprietari() { idx = 3, name = "Aury" });
            obsItems.Add(new Proprietari() { idx = 4, name = "Cinzia" });
            obsItems.Add(new Proprietari() { idx = 5, name = "Maria" });
            obsItems.Add(new Proprietari() { idx = 6, name = "Cleopatra" });

            obsSelectedItems = new ObservableCollection<Proprietari>();

            obsSelectedItems.Add(new Proprietari() { idx = 1, name = "Flavio" });
            obsSelectedItems.Add(new Proprietari() { idx = 6, name = "Cleopatra" });

            Items = new Dictionary<string, object>();
            Items.Add("Flavio", new Proprietari() { idx = 1, name = "Flavio" });
            Items.Add("Dany", new Proprietari() { idx = 2, name = "Dany" });
            Items.Add("Aury", new Proprietari() { idx = 3, name = "Aury" });
            Items.Add("Cinzia", new Proprietari() { idx = 4, name = "Cinzia" });
            Items.Add("Maria", new Proprietari() { idx = 5, name = "Maria" });
            Items.Add("Cleopatra", new Proprietari() { idx = 6, name = "Cleopatra" });

            SelectedItems = new Dictionary<string, object>();
            SelectedItems.Add("Flavio", new Proprietari() { idx = 1, name = "Flavio" });
            SelectedItems.Add("Cleopatra", new Proprietari() { idx = 6, name = "Cleopatra" });

        }

        public Dictionary<string, object> Items
        {
            get { return GetValue(() => Items); }
            set { SetValue(() => Items, value); }
        }

        public Dictionary<string, object> SelectedItems
        {
            get { return GetValue(() => SelectedItems); }
            set { SetValue(() => SelectedItems, value); }
        }

        public ObservableCollection<Proprietari> obsItems
        {
            get { return GetValue(() => obsItems); }
            set { SetValue(() => obsItems, value); }
        }

        public ObservableCollection<Proprietari> obsSelectedItems
        {
            get { return GetValue(() => obsSelectedItems); }
            set { SetValue(() => obsSelectedItems, value); }
        }
    }

    public class Proprietari
    {
        public int idx { get; set; }
        public string name { get; set; }
    }
}
