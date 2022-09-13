using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.Model
{
    public class CanvasObjekat : BindableBase
    {
        Zemljiste zemljiste;
        bool taken;

        public CanvasObjekat()
        {
            Taken = false;
            Zemljiste = new Zemljiste();
        }

        public CanvasObjekat(Zemljiste zemljiste, bool taken)
        {
            this.zemljiste = zemljiste;
            this.taken = taken;
        }

        public Brush Background
        {
            get
            {
                if (Zemljiste.Type != null)
                {
                    BitmapImage slika = new BitmapImage();
                    slika.BeginInit();
                    slika.UriSource = new Uri(Zemljiste.Type.ImgSrc);
                    slika.EndInit();
                    return new ImageBrush(slika);
                }
                else
                    return Brushes.GhostWhite;
            }
        }

        public Zemljiste Zemljiste
        {
            get { return zemljiste; }
            set
            {
                zemljiste = value;
                OnPropertyChanged("Zemljiste");
                OnPropertyChanged("Foreground");
                OnPropertyChanged("Text");
            }
        }

        public bool Taken
        {
            get { return taken; }
            set
            {
                if(taken != value)
                {
                    taken = value;
                    OnPropertyChanged("Taken");
                }    
            }
        }

        public string Foreground
        {
            get
            {
                if (Zemljiste.Value < 17)
                    return "Red";
                else
                    return "Blue";
            }
        }
        public string Text { get => Zemljiste.Name != null ? "ID: " + Zemljiste.Id + "Name: " + Zemljiste.Name : ""; }
    }
}
