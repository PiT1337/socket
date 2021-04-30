using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//aggiunta delle seguenti librerie
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace socket
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            IPEndPoint sourceSocket = new IPEndPoint(IPAddress.Parse("10.73.0.20"),56000);


            
            btnInvia.IsEnabled = true;
            Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));


            ricezione.Start(sourceSocket);


        }
        
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {//aggiunre controlli sul contenuto delle textbox
            string ipaddress = txtindirizzoIp.Text;
            int port =int.Parse( txtport.Text);
            SocketSend(IPAddress.Parse(ipaddress), port, txtmessage.Text);




        }
        public async void SocketReceive(object socksource) 
        {
            IPEndPoint ipendp = (IPEndPoint)socksource;
            Socket t = new Socket(ipendp.AddressFamily,SocketType.Dgram,ProtocolType.Udp);
            t.Bind(ipendp);

            Byte[] byteRicevti = new Byte[256];

            string message;
            int contaCaratteri = 0;

            await Task.Run(() =>
            {
                while(true)
                {
                    if (t.Available > 0)
                    {
                        message = "";

                        contaCaratteri = t.Receive(byteRicevti,byteRicevti.Length,0);
                        message += Encoding.ASCII.GetString(byteRicevti, 0, contaCaratteri);
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblRicevi.Content = message;
                        }));
                    }
                }
            });

        }



        public void SocketSend(IPAddress dest,int destport,string message)
        {
            Byte[] byteTnviati = Encoding.ASCII.GetBytes(message);

            Socket s = new Socket(dest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint remote_endpoint = new IPEndPoint(dest, destport);

            s.SendTo(byteTnviati, remote_endpoint);


        }
    }
}
