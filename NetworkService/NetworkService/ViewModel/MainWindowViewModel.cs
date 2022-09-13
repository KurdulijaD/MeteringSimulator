using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {

        public MyICommand<string> NavCommand { get; private set; }
        private static NetworkEntitiesViewModel networkEntitiesViewModel = new NetworkEntitiesViewModel();
        private static NetworkDisplayViewModel networkDisplayViewModel = new NetworkDisplayViewModel();
        private static MeasurementGraphViewModel measurementGraphViewModel = new MeasurementGraphViewModel();
        private static BindableBase currentViewModel;


        private string path = AppDomain.CurrentDomain.BaseDirectory + "log.txt";

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        public static bool UseToolTips { get; set; } = true;

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "NetworkEntities":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "NetworkDisplay":
                    CurrentViewModel = networkDisplayViewModel;
                    break;
                case "MeasurementGraph":
                    CurrentViewModel = measurementGraphViewModel;
                    break;
                    //Messenger.Default.Send<ObservableCollection<Ventil>>(Ventils);

            }

        }

        //private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom

            NavCommand = new MyICommand<string>(OnNav);
            currentViewModel = networkEntitiesViewModel;

        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25566);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            //Byte[] data = System.Text.Encoding.ASCII.GetBytes(count.ToString());
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntitiesViewModel.Zemljista.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"
                            //entitet_
                            //1:
                            //272
                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            string[] dio = incomming.Split('_', ':');
                            int objectIndex = Int32.Parse(dio[1]);
                            int objectValue = Int32.Parse(dio[2]);
                            Console.WriteLine(objectIndex);
                            Console.WriteLine(objectValue);
                            NetworkEntitiesViewModel.Zemljista[objectIndex].Value = objectValue;
                            NetworkEntitiesViewModel.Filtrirani[objectIndex].Value = objectValue;
                            NetworkDisplayViewModel.UpdateList(NetworkEntitiesViewModel.Zemljista[objectIndex]);
                            MeasurementGraphViewModel.Update("Entitet_" + dio[1], new Entity(objectValue, DateTime.Now));

                            OnPropertyChanged("Zemljista");
                            OnPropertyChanged("Filtrirani");
                            OnPropertyChanged("listaObjekata");
                            OnPropertyChanged("ZemljistaGraf");

                            //string ispis = "ID: " + dio[1] + ", " + DateTime.Now + ", CHANGED STATE: " + dio[2] + Environment.NewLine;
                            string ispis = DateTime.Now + " Entitet_" + dio[1] + ", " + dio[2] + Environment.NewLine;
                            File.AppendAllText(path, ispis);

                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }
    }
}
