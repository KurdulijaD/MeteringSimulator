using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        public static ObservableCollection<Zemljiste> listaObjekata { get; set; }
        public static ObservableCollection<CanvasObjekat> CanvasObjekti { get; set; }

        public MyICommand<ListView> SelectionChangedCommand { get; set; }
        public MyICommand MouseLeftButtonUpCommand { get; set; }
        public MyICommand<Canvas> MouseLeftButtonDownCommand { get; set; }
        public MyICommand<Canvas> ButtonCommand { get; set; }
        public MyICommand<Canvas> DropCommand { get; set; }
        public MyICommand<Canvas> DragOverCommand { get; set; }

        private Zemljiste selectedZemljiste;
        private CanvasObjekat currentCanvas;
        private bool dragging = false;
        private bool toolTips = false;

        public NetworkDisplayViewModel()
        {
            if (listaObjekata == null)
                listaObjekata = new ObservableCollection<Zemljiste>();
            if (CanvasObjekti == null)
            {
                CanvasObjekti = new ObservableCollection<CanvasObjekat>();
                for (int i = 0; i < 16; i++)
                    CanvasObjekti.Add(new CanvasObjekat());
            }

            SelectionChangedCommand = new MyICommand<ListView>(SelectionChanged);
            ButtonCommand = new MyICommand<Canvas>(ButtonFreeCommand);
            MouseLeftButtonUpCommand = new MyICommand(MouseLeftButtonUp);
            MouseLeftButtonDownCommand = new MyICommand<Canvas>(MouseLeftButtonDown);
            DropCommand = new MyICommand<Canvas>(Drop);
            DragOverCommand = new MyICommand<Canvas>(DragOver);
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
                if(selectedZemljiste != value)
                {
                    selectedZemljiste = value;
                    OnPropertyChanged("SelectedZemljiste");
                }
            }
        }
        public CanvasObjekat CurrentCanvas
        {
            get { return currentCanvas; }
            set 
            {
                currentCanvas = value;
                OnPropertyChanged("CurrentCanvas");
            }
        }

        public void SelectionChanged(ListView list)
        {
            if (!dragging)
            {
                dragging = true;
                SelectedZemljiste = SelectedZemljiste;
                DragDrop.DoDragDrop(list, SelectedZemljiste, DragDropEffects.Copy | DragDropEffects.Move);

            }

        }

        private void MouseLeftButtonUp()
        {
            CurrentCanvas = null;
            SelectedZemljiste = null;
            dragging = false;
        }

        private void MouseLeftButtonDown(Canvas canvas)
        {
            int id = int.Parse(canvas.Name.Substring(6));
            if (CanvasObjekti[id].Taken)
            {
                CurrentCanvas = CanvasObjekti[id];
                if (!dragging)
                {
                    dragging = true;
                    DragDrop.DoDragDrop(canvas, CurrentCanvas, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        private void DragOver(Canvas obj)
        {
            int id = int.Parse(obj.Name.Substring(6));
            if (!CanvasObjekti[id].Taken)
                obj.AllowDrop = true;
            else
                obj.AllowDrop = false;
        }

        public void ButtonFreeCommand(Canvas canvas)
        {
            int id = int.Parse(canvas.Name.Substring(6));
            if (CanvasObjekti[id].Taken)
            {
                listaObjekata.Add(CanvasObjekti[id].Zemljiste);
                CanvasObjekti[id] = new CanvasObjekat();
            }
        }

        public void Drop(Canvas canvas)
        {
            if (SelectedZemljiste != null)
            {
                int id = int.Parse(canvas.Name.Substring(6));
                if (!CanvasObjekti[id].Taken)
                {
                    CanvasObjekti[id] = new CanvasObjekat(SelectedZemljiste, true);
                    listaObjekata.Remove(SelectedZemljiste);
                }
                SelectedZemljiste = null;
                dragging = false;
            }
            else if (CurrentCanvas != null)
            {
                int id = int.Parse(canvas.Name.Substring(6));
                if (!CanvasObjekti[id].Taken)
                {
                    for (int i = 0; i < 16; i++)
                        if (CanvasObjekti[i] == CurrentCanvas)
                            CanvasObjekti[i] = new CanvasObjekat();
                    CanvasObjekti[id] = CurrentCanvas;
                }
                SelectedZemljiste = null;
                dragging = false;
            }
        }

        public static void RemoveFromList(Zemljiste z)
        {
            foreach (Zemljiste zemljiste in listaObjekata)
                if (zemljiste.Id == z.Id)
                {
                    listaObjekata.Remove(zemljiste);
                    return;
                }

            for (int i = 0; i < 16; i++)
                if (CanvasObjekti[i].Zemljiste.Id == z.Id)
                {
                    CanvasObjekti[i] = new CanvasObjekat();
                    return;
                }
        }

        public static void UpdateList(Zemljiste z)
        {
            for (int i = 0; i < listaObjekata.Count; i++)
                if (listaObjekata[i].Id == z.Id)
                {
                    listaObjekata[i].Value = z.Value;
                    return;
                }

            for (int i = 0; i < 16; i++)
                if (CanvasObjekti[i].Zemljiste.Id == z.Id)
                {
                    CanvasObjekti[i].Zemljiste = z;
                    return;
                }
        }
    }
}
