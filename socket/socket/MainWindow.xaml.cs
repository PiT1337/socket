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
        //variabile per aggirare un errore
        int startscimmia = 0;

        private void btnCreaSocket_Click(object sender, RoutedEventArgs e)
        {
            //scimmia forte insieme (gestione del errore del creare 2 socket)
            if (startscimmia==0) 
            {
                //visione a video del proprio ip nella label 
                lbloutput.Content= "il tuo ip è :" + Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
                //avvio del Thread
                Thread ricezione = new Thread(new ParameterizedThreadStart(SocketReceive));
                ricezione.Start(new IPEndPoint(IPAddress.Parse(Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString()), 56000));
                startscimmia++;
            }
            //attivazione del altro bottone
            btnInvia.IsEnabled = true;
        }
        
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            //aggiunre controlli sul contenuto delle textbox
            IPAddress provaip;
            int provaint;
            //controllo del input delle textbox
            if (IPAddress.TryParse(txtindirizzoIp.Text, out provaip) && int.TryParse(txtport.Text, out provaint) && provaint > 0 && provaint < 65536) 
            { 
                SocketSend(IPAddress.Parse(txtindirizzoIp.Text), int.Parse(txtport.Text) , txtmessage.Text);
            }
            else
            {
                MessageBox.Show("errore nel inserimento dei campi ip-address o della porta");
            }
        }
        public async void SocketReceive(object socksource) 
        {
            IPEndPoint ipendp = (IPEndPoint)socksource;
            Socket t = new Socket(ipendp.AddressFamily,SocketType.Dgram,ProtocolType.Udp);
            t.Bind(ipendp);
            //max Byte per messaggio
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
