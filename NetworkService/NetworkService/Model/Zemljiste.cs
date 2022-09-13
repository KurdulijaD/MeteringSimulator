using NetworkService.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Zemljiste : ValidationBase
    {
        private int id;
        private string name;
        private Type type;
        private double val;
        private string idString;

        public Zemljiste(int id, string name, Type type, double value)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.val = value;
        }

        public Zemljiste()
        {

        }

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public Type Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        public double Value
        {
            get { return val; }
            set
            {
                if (val != value)
                {
                    val = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public string IdString
        {
            get { return idString; }
            set
            {
                if (idString != value)
                {
                    idString = value;
                }
            }
        }

        protected override void ValidateSelf()
        {

            if (string.IsNullOrWhiteSpace(this.idString))
            {
                this.ValidationErrors["Id"] = "ID is required!";
            }
            else if(!int.TryParse(this.idString, out int num))
            {
                this.ValidationErrors["Id"] = "ID has to be an integer!";
            }
            else if(num < 0)
            {
                this.ValidationErrors["Id"] = "ID has to be a positive number!";
            }
            else
            {
                for (int i = 0; i < NetworkEntitiesViewModel.Zemljista.Count; i++)
                {
                    if (NetworkEntitiesViewModel.Zemljista[i].Id == num)
                    {
                        this.ValidationErrors["Id"] = "Id is already being used!";
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(this.name))
            {
                this.ValidationErrors["Name"] = "Name is required!";
            }

            if(this.type == null)
            {
                this.ValidationErrors["Type"] = "Type is required!";
            }
        }

        public override string ToString()
        {
            return $"ID: {Id}  Name: {Name} ";
        }
    }
}
