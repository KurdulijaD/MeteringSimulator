using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = NetworkService.Model.Type;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public static ObservableCollection<Zemljiste> Zemljista { get; private set; }
        public static ObservableCollection<Zemljiste> Filtrirani { get; private set; }
        public ObservableCollection<Type> Tipovi { get; set; }
        public ObservableCollection<Type> FilterTipovi { get; set; }

        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }

        private Zemljiste selectedZemljiste = new Zemljiste();
        private Zemljiste currentZemljiste = new Zemljiste();
        private Type selectedType = new Type();

        private bool isGreaterChecked;
        private bool isLessChecked;
        private string filterText;
        public string filterError;
        private bool toolTips;
       
        Type tipZitarice = new Type { Name = "Zitarice", ImgSrc = AppDomain.CurrentDomain.BaseDirectory + "Images\\zitarice.jpg" };
        Type tipIndBiljke = new Type { Name = "Industrijske Biljke", ImgSrc = AppDomain.CurrentDomain.BaseDirectory + "Images\\indBiljke.jpg" };
       
        public NetworkEntitiesViewModel()
        {
            if(Zemljista == null)
            {
                Zemljista = new ObservableCollection<Zemljiste>();
            }

            if(Filtrirani == null)
            {
                Filtrirani = new ObservableCollection<Zemljiste>();
            }

            Tipovi = new ObservableCollection<Type>();
            FilterTipovi = new ObservableCollection<Type>();

            Tipovi.Add(tipZitarice);
            Tipovi.Add(tipIndBiljke);
            FilterTipovi.Add(tipZitarice);
            FilterTipovi.Add(tipIndBiljke);
            FilterTipovi.Add(new Type { Name = "Oba" });

            Zemljista = new ObservableCollection<Zemljiste>();
            AddCommand = new MyICommand(OnAdd);
            DeleteCommand = new MyICommand(OnDelete, CanDelete);
            FilterCommand = new MyICommand(Filter);
            ToolTips = MainWindowViewModel.UseToolTips;
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

        public Zemljiste SelectedZemljiste
        {
            get { return selectedZemljiste; }
            set
            {
                    selectedZemljiste = value;
                    DeleteCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged("SelectedZemljiste");
            }
        }
        public Zemljiste CurrentZemljiste
        {
            get { return currentZemljiste; }
            set
            {
                currentZemljiste = value;
                //DeleteCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("CurrentZemljiste");
            }
        }


        public Type SelectedType
        {
            get
            {
                return selectedType;
            }
            set
            {
                selectedType = value;
                OnPropertyChanged("SelectedType");

            }
        }
        public bool IsGreaterChecked
        {
            get { return isGreaterChecked; }
            set
            {
                if(isGreaterChecked != value)
                {
                    isGreaterChecked = value;
                    OnPropertyChanged("IsGreaterChecked");
                }
            }
        }
        public bool IsLessChecked
        {
            get { return isLessChecked; }
            set
            {
                if (isLessChecked != value)
                {
                    isLessChecked = value;
                    OnPropertyChanged("IsLessChecked");
                }
            }
        }
        public string FilterText
        {
            get { return filterText; }
            set
            {

                if (filterText != value)
                {
                    filterText = value;
                    OnPropertyChanged("FilterText");

                }

            }
        }
        public string FilterError
        {
            get { return filterError; }
            set
            {
                if (filterError != value)
                {
                    filterError = value;
                    OnPropertyChanged("FilterError");
                }
            }
        }
        public void OnAdd()
        {
            CurrentZemljiste.Validate();
            if (CurrentZemljiste.IsValid)
            {
                int id = int.Parse(CurrentZemljiste.IdString);
                CurrentZemljiste.Id = id;

                if (ExistById(id))
                {
                    CurrentZemljiste.ValidationErrors["Id"] = "ID exists in list";
                    return;
                }

                Zemljista.Add(new Zemljiste 
                {
                    Id = CurrentZemljiste.Id, 
                    Name = CurrentZemljiste.Name, 
                    Type = CurrentZemljiste.Type, 
                    Value = CurrentZemljiste.Value
                });

                Filtrirani.Add(new Zemljiste
                {
                    Id = CurrentZemljiste.Id,
                    Name = CurrentZemljiste.Name,
                    Type = CurrentZemljiste.Type,
                    Value = CurrentZemljiste.Value
                });

                NetworkDisplayViewModel.listaObjekata.Add(new Zemljiste
                {
                    Id = CurrentZemljiste.Id,
                    Name = CurrentZemljiste.Name,
                    Type = CurrentZemljiste.Type,
                    Value = CurrentZemljiste.Value
                });


               //MeasurementGraphViewModel.ZemljistaGraf = Zemljista;
               MeasurementGraphViewModel.ZemljistaGraf.Add(new Zemljiste
              {
                    Id = CurrentZemljiste.Id,
                    Name = CurrentZemljiste.Name,
                    Type = CurrentZemljiste.Type,
                    Value = CurrentZemljiste.Value
              });

                OnPropertyChanged("Zemljista");
                OnPropertyChanged("Filtrirani");
                OnPropertyChanged("listaObjekata");
                OnPropertyChanged("ZemljistaGraf");
            }
        }

        public void OnDelete()
        {
            NetworkDisplayViewModel.RemoveFromList(SelectedZemljiste);
            Zemljista.Remove(SelectedZemljiste);
            Filtrirani.Remove(SelectedZemljiste);           
            MeasurementGraphViewModel.ZemljistaGraf = Zemljista;
            OnPropertyChanged("Zemljista");
            OnPropertyChanged("Filtrirani");
            OnPropertyChanged("listaObjekata");
        }

        private bool CanDelete()
        {
            return SelectedZemljiste != null;
        }

        private bool ExistById(int id)
        {
            foreach(Zemljiste z in Zemljista)
            {
                if (z.Id == id)
                    return true;
            }

            return false;
        }
        public void Filter()
        {
            int num = 0;
            bool noNum = false;
            bool unvalidNumber = false;
            if(string.IsNullOrEmpty(FilterText))
            {
                noNum = true;
            }
            else
            {
                if(!int.TryParse(FilterText, out num))
                {
                   FilterError = "Filter value must be a number";
                    noNum = true;
                    unvalidNumber = true;
                }
                else if(int.Parse(FilterText) < 0)
                {
                    FilterError = "Filter value must be a positive number";
                    noNum = true;
                    unvalidNumber = true;
                }
                else
                {
                    num = int.Parse(FilterText);
                    unvalidNumber = false;
                }
            }
            if(!unvalidNumber)
            {
                FilterError = "";
            }

            if(noNum)
            {
                Filtrirani.Clear();
                for (int i = 0; i < Zemljista.Count; i++)
                {
                    if (SelectedType.Name == "Oba")
                    {
                        Filtrirani.Add(Zemljista[i]);
                    }
                    else if (SelectedType.Name == Zemljista[i].Type.Name)
                    {
                        Filtrirani.Add(Zemljista[i]);
                    }
                }
            }
            else
            {
                Filtrirani.Clear();
                for (int i = 0; i < Zemljista.Count; i++)
                {
                    if (SelectedType.Name == "Oba")
                    {
                        if (isGreaterChecked)
                        {
                            if (Zemljista[i].Id > num)
                            {
                                Filtrirani.Add(Zemljista[i]);
                            }
                        }
                        else if(isLessChecked)
                        {
                            if (Zemljista[i].Id < num)
                            {
                                Filtrirani.Add(Zemljista[i]);
                            }
                        }
                    }
                    else if(SelectedType.Name == Zemljista[i].Type.Name)
                    {
                        if (isGreaterChecked)
                        {
                            if (Zemljista[i].Id > num)
                            {
                                Filtrirani.Add(Zemljista[i]);
                            }
                        }
                        else if (isLessChecked)
                        {
                            if (Zemljista[i].Id < num)
                            {
                                Filtrirani.Add(Zemljista[i]);
                            }
                        }
                    }
                }
            }
            OnPropertyChanged("Filtrirani");
        }
    }
}
