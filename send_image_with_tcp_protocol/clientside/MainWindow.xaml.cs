
using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;


namespace clientside
{ 
    public partial class MainWindow : Window
    {
        string filePath;


        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                SelectedImage.Text = filePath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        var imageBytes = File.ReadAllBytes(filePath);
                        var base64Image = Convert.ToBase64String(imageBytes);

                        using (var client = new TcpClient(SelectedIp.Text, int.Parse(SelectedPort.Text)))
                        using (var networkStream = client.GetStream())
                        using (var writer = new StreamWriter(networkStream, Encoding.UTF8))
                        {
                            writer.Write(base64Image);
                            writer.Flush();
                        }
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() => MessageBox.Show(ex.Message));
                }
            });
        }
    }


}
