using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    class MeasurementGraphViewModel : BindableBase
    {
        public static ObservableCollection<Zemljiste> ZemljistaGraf { get; set; }
        public static ObservableCollection<Measurement> measurements { get; set; }
        public MyICommand ButtonGraphCommand { get; set; }

        static string path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";
        private Zemljiste selectedZemljiste;
        public static Zemljiste saveSelected = null;
        private static Measurement savePicked = null;
        private Measurement picked;
        private bool toolTips;
        
        public MeasurementGraphViewModel()
        {
            if (measurements == null)
            {
                measurements = new ObservableCollection<Measurement>();
                ReadFromFile();
            }

            if (ZemljistaGraf == null)
                ZemljistaGraf = new ObservableCollection<Zemljiste>();
            

            ToolTips = MainWindowViewModel.UseToolTips;

            ButtonGraphCommand = new MyICommand(ButtonGraph, CanGraph);

            if (saveSelected != null)
                SelectedZemljiste = saveSelected;
            if (savePicked != null)
                Picked = savePicked;
        }

        public bool ToolTips
        {
            get => toolTips;
            set
            {
                toolTips = value;
                MainWindowViewModel.UseToolTips = value;
                OnPropertyChanged("ToolTips");
            }
        }

        public Measurement Picked
        {
            get { return picked; }
            set
            {
                picked = value;
                savePicked = value;
                OnPropertyChanged("Picked");
            }
        }
        public Zemljiste SelectedZemljiste
        {
            get { return selectedZemljiste; }
            set
            {
                if(selectedZemljiste != value)
                {
                    selectedZemljiste = value;
                    saveSelected = value;
                    OnPropertyChanged("SelectedZemljiste");
                    ButtonGraphCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public static bool DoesEntityExist(string name)
        {
            foreach (Measurement mst in measurements)
            {
                if (mst.Name == name)
                    return true;
            }

            return false;
        }

        public static void DeleteEntity(string name)
        {
            for (int i = 0; i < measurements.Count; i++)
            {
                if(measurements[i].Name == name)
                {
                    if(measurements[i].Entities.Count > 5)
                    {
                        measurements[i].Entities.RemoveAt(0);
                        return;
                    }
                }
            }
        }
        public static void AddEntity(string name, Entity entity, bool delete = true)
        {
            for (int i = 0; i < measurements.Count; i++)
            {
                if(measurements[i].Entities.Count >= 5 && delete)
                {
                    return;
                }
                if(measurements[i].Name == name)
                {
                    measurements[i].Entities.Add(entity);
                    return;
                }
            }
        }
        public static void Update(string name, Entity entity)
        {
            if(DoesEntityExist(name))
            {
                AddEntity(name, entity, false);
                DeleteEntity(name);
            }
            else
            {
                measurements.Add(new Measurement(name, new ObservableCollection<Entity>()));
                AddEntity(name, entity);
            }
        }

        private void ReadFromFile()
        {
            var file = File.ReadAllLines(path).Reverse();
            foreach(string str in file)
            {
                string[] split = str.Split(' ');
                split[1] = split[1].Substring(0, split[1].Length);
                split[3] = split[3].Substring(0, split[3].Length - 1);

                if(!DoesEntityExist(split[3]))
                {
                    measurements.Add(new Measurement(split[3], new ObservableCollection<Entity>()));
                }

                DateTime dateTime = DateTime.Parse(split[0] + " " + split[1]);
                AddEntity(split[3], new Entity(double.Parse(split[4]), dateTime));
            }

            foreach(Measurement measurement in measurements)
            {
                measurement.Entities.Reverse();
            }
        }

        private string EntityId()
        {
            for (int i = 0; i < NetworkEntitiesViewModel.Zemljista.Count; i++)
                if (NetworkEntitiesViewModel.Zemljista[i].Id == SelectedZemljiste.Id)
                    return "Entitet_" + i;

            return null;
        }

        private Measurement Pick()
        {
            string id = EntityId();
            foreach(Measurement m in measurements)
            {
                if(m.Name == id)
                {
                    return m;
                }
            }
            return null;
        }
        private void ButtonGraph()
        {
            Picked = Pick();
        }

        private bool CanGraph()
        {
            return SelectedZemljiste != null;
        }
    }
}
