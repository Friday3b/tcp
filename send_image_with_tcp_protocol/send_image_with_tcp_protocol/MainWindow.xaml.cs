using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace send_image_with_tcp_protocol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static int port = 27001;
        static IPAddress ip = IPAddress.Parse("192.168.1.8");
        private TcpListener? _listener;

        static IPEndPoint _endpoint = new IPEndPoint (ip ,port);




        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; 
            Start();    
            
        }


        private async void Start()
        {
            try
            {
                _listener = new TcpListener(ip, port);
                _listener.Start();
                Dispatcher.Invoke(() => ListBoxInformation.Items.Add($"Listening on: {_listener.LocalEndpoint}"));

                while (true)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private async void HandleClient(TcpClient client)
        {
            try
            {
                Dispatcher.Invoke(() => ListBoxInformation.Items.Add($"{client.Client.RemoteEndPoint} connected"));
                using var networkRead = client.GetStream();
                using (var reader = new StreamReader(networkRead, Encoding.UTF8))
                {
                    var baseImage = await reader.ReadToEndAsync();
                    var image = BitmapImageFromBase64(baseImage);
                    Dispatcher.Invoke(() => listboximages.Items.Add(image));
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private BitmapImage BitmapImageFromBase64(string base64Image)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64Image);
                var imageStream = new MemoryStream(imageBytes);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null!;
            }


        }
    }
}
