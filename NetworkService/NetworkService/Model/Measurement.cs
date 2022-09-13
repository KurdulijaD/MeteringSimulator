using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Measurement : BindableBase
    {
        string name;
        ObservableCollection<Entity> entities;

        public Measurement(string name, ObservableCollection<Entity> entities)
        {
            this.name = name;
            this.entities = entities;
        }

        public Measurement()
        {
            name = "";
            entities = new ObservableCollection<Entity>();
        }

        public ObservableCollection<Entity> Entities
        {
            get { return entities; }
            set
            {
                entities = value;
                OnPropertyChanged("Entities");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Entities");
            }
        }
    }
}
