using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class Entity : INotifyPropertyChanged
    {
        double measurement;
        DateTime dateTime;

        public Entity()
        {
            measurement = 0;
        }

        public Entity(double measurement, DateTime dateTime)
        {
            this.measurement = measurement;
            this.dateTime = dateTime;
        }

        public double Measurement
        {
            get { return measurement; }
            set
            {
                measurement = value;
                RaisePropertyChanged("Measurement");
                RaisePropertyChanged("Draw");
                RaisePropertyChanged("GraphDate");
                RaisePropertyChanged("Color");
            }
        }
        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                RaisePropertyChanged("DateTime");
                RaisePropertyChanged("GraphDate");
            }
        }
        public string GraphDate
        {
            get
            {
                if(Measurement == 0)
                {
                    return "";
                }
                else
                {
                    return DateTime.ToString("t");
                }
            }
        }

        public string GraphMeasure
        {
            get
            {
                if(Measurement == 0)
                {
                    return "";
                }
                else
                {
                    return Measurement.ToString();
                }
            }
        }

        public double Draw
        {
            get { return 300 - (10 * Measurement); }
        }

        public Brush Color
        {
            get
            {
                if (Measurement == 0)
                    return Brushes.Transparent;
                else if (Measurement < 17)
                    return Brushes.Red;
                else
                    return Brushes.Green;
            }
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
