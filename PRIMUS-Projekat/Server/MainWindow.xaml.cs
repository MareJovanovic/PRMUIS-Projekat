using System.Net.Sockets;
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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client;
        NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Host_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = IpAddr.Text.Trim();
                int port = int.Parse(BrojPorta.Text.Trim());

                client = new TcpClient();
                await client.ConnectAsync(ip, port);
                stream = client.GetStream();

                ServerMsg.AppendText("Povezano na server!\r\n");
                Send.IsEnabled = true;
                Host.IsEnabled = false;

                ReceiveMessagesAsync(); // Start async receive loop
            }
            catch (Exception ex)
            {
                ServerMsg.AppendText($"Greška pri povezivanju: {ex.Message}\r\n");
            }
        }


        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string poruka = MsgSend.Text.Trim();
            if (!string.IsNullOrEmpty(poruka) && client.Connected)
            {
                if (poruka.StartsWith("Proveri", StringComparison.OrdinalIgnoreCase))
                {
                    string kriterijum = poruka.Substring(7).Trim();
                    await SendUdpQuery(kriterijum);
                }
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes(poruka);
                    await stream.WriteAsync(data, 0, data.Length);

                    ServerMsg.AppendText($"[Ja]: {poruka}\r\n");
                }
                MsgSend.Clear();
            }
        }
        private async void ReceiveMessagesAsync()
        {
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
                        ServerMsg.AppendText($"[Server]: {message}\r\n");
                    });
                }
                catch
                {
                    break;
                }
            }

            ServerMsg.Dispatcher.Invoke(() =>
            {
                ServerMsg.AppendText("Diskonektovan sa servera.\r\n");
                Send.IsEnabled = false;
                Host.IsEnabled = true;
            });
            client.Close();
        }

        private async Task SendUdpQuery(string trazeniNaslov)
        {
            using (UdpClient client = new UdpClient())
            {
                try
                {
                    string upit = "Proveri" + trazeniNaslov;
                    byte[] data = Encoding.UTF8.GetBytes(upit);

                    string serverIp = IpAddr.Text.Trim();
                    int serverPort = int.Parse(BrojPorta.Text.Trim());

                    await client.SendAsync(data, data.Length, serverIp, serverPort);

                    var result = await client.ReceiveAsync();
                    string odgovor = Encoding.UTF8.GetString(result.Buffer);

                    // Parse odgovora i update ListBox
                    string[] knjige = odgovor.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                    ListaKnjigaPrikaz.Dispatcher.Invoke(() =>
                    {
                        ListaKnjigaPrikaz.Items.Clear();

                        if (knjige.Length > 0 && !knjige[0].Contains("Nema dostupnih knjiga"))
                        {
                            foreach (var knjiga in knjige)
                                ListaKnjigaPrikaz.Items.Add(knjiga);
                            

                            ListaKnjigaPrikaz.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            ListaKnjigaPrikaz.Visibility = Visibility.Hidden;
                        }
                    });
                }
                catch (Exception ex)
                {
                    ServerMsg.Dispatcher.Invoke(() =>
                    {
                        ServerMsg.AppendText($"Greška: {ex.Message}\r\n");
                    });
                }
            }

        }
    }
}