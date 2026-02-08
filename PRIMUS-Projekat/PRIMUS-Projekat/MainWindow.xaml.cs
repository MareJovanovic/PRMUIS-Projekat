using PRIMUS_Projekat.Src;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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

namespace PRIMUS_Projekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private KnjigaServis knjigaServis;
        new ObservableCollection<Knjiga> knjige;

        private List<TcpClient> connectedClients = new List<TcpClient>();

        TcpListener tcpListener;
        UdpClient udpClient;

        int tcpPort = 5000;
        int udpPort = 5001;
        public MainWindow()
        {
            InitializeComponent();

            //dodavanje knjige i prikazivanje
            knjigaServis = new KnjigaServis();
            knjige = new ObservableCollection<Knjiga>(knjigaServis.getKnjige());

            PrikazKnjiga.ItemsSource = knjige;
            BookLIst.ItemsSource = knjige;
        }
       
        public void StartServer()
        {
            try
            {
                // TCP – PRISTUPNA utičnica
                tcpListener = new TcpListener(IPAddress.Any, tcpPort);
                tcpListener.Start();
                IPEndPoint tcpEndPoint = (IPEndPoint)tcpListener.LocalEndpoint;
                AcceptClientsAsync();

                // UDP – INFO utičnica
                udpClient = new UdpClient(udpPort);
                IPEndPoint udpEndPoint = (IPEndPoint)udpClient.Client.LocalEndPoint;

                // Prava lokalna IP adresa
                string localIp = Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .First(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                    .ToString();

                // Prikaz u TextBlock
                ServerMsg.Text = $"PRISTUPNA utičnica (TCP): {localIp} : {tcpEndPoint.Port}";
                ServerMsg.Text = $"INFO utičnica (UDP): {localIp} : {udpEndPoint.Port}";

                // Log u TextBox
                ServerMsg.AppendText("\nServer upaljen!\r\n");
                ServerMsg.AppendText($"TCP port: {tcpEndPoint.Port}, UDP port: {udpEndPoint.Port}\r\n");
            }
            catch(Exception ex)
            {
                ServerMsg.AppendText($"Greška: {ex.Message}\r\n");
            }
            HostSRV.IsEnabled = false;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Knjiga k = new Knjiga(NaslovK.Text, AutorK.Text, int.Parse(KolicinaK.Text));
                if (knjigaServis.Dodaj(k))
                {
                    knjige.Add(k);
                    MessageBox.Show("Uspesno ste dodali knjigu");
                    NaslovK.Clear();
                    AutorK.Clear();
                    KolicinaK.Clear();
                }
                else
                {
                    MessageBox.Show("Greska pri unosu");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Neispravan format unosa");
            }
        }

        private void DLTBook_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Da li ste sigurni da zelite da obrisete knjigu", "Potvrdi brisanje", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                knjige.RemoveAt(PrikazKnjiga.SelectedIndex);
                MessageBox.Show("Knjiga obrisana");
            }
            else
            {
                MessageBox.Show("Neuspesno brisanje");
            }
        }

        private void HostSRV_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }
        

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string poruka = MsgSend.Text.Trim();
            if (string.IsNullOrEmpty(poruka)) return;

            ServerMsg.AppendText($"[Server]: {poruka}\r\n");
            MsgSend.Clear();

            byte[] data = Encoding.UTF8.GetBytes(poruka);
            foreach (var client in connectedClients.ToArray())
            {
                if (client.Connected)
                {
                    try
                    {
                        await client.GetStream().WriteAsync(data, 0, data.Length);
                    }
                    catch
                    {
                        connectedClients.Remove(client);
                    }
                }
            }
        }
        
        private async void AcceptClientsAsync()
        {
            while (true)
            {
                TcpClient client = await tcpListener.AcceptTcpClientAsync();
                connectedClients.Add(client);
                ServerMsg.AppendText("Novi klijent povezan!\r\n");
                HandleClientAsync(client);
            }
        }
        private async void HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (client.Connected)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ServerMsg.Dispatcher.Invoke(() =>
                    {
                        ServerMsg.AppendText($"[Klijent]: {message}\r\n");
                    });

                    // Echo nazad klijentu
                    byte[] response = Encoding.UTF8.GetBytes($"[Server]: {message}");
                    await stream.WriteAsync(response, 0, response.Length);
                }
                catch
                {
                    break;
                }
            }

            connectedClients.Remove(client);
            ServerMsg.Dispatcher.Invoke(() =>
            {
                ServerMsg.AppendText("Klijent diskonektovan.\r\n");
            });
            client.Close();
        }
    }
}