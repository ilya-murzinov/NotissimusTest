using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
using System.Xml.Serialization;

namespace NotissimusTest
{
    public partial class MainWindow : Window
    {
        public string Url 
        { 
            get { return "http://partner.market.yandex.ru/pages/help/YML.xml"; }
        }
        public string Id 
        { 
            get { return "12344"; } 
        }
        private string _sendTo = "http://google.com/";
        public string SendTo
        {
            get { return _sendTo; }
            set { _sendTo = value; }
        }
        private yml_catalog _model;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            downlodButton.IsEnabled = false;
            progressBar.IsIndeterminate = true;
            statusTestBlock.Text = "Downloading...";

            Stream xmlStream;
            using (HttpClient client = new HttpClient())
            {
                Task<Stream> xml = client.GetStreamAsync(Url);
                xmlStream = await xml;
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(yml_catalog));
            await Task.Run(() => _model = (yml_catalog) xmlSerializer.Deserialize(xmlStream));
            
            sendButton.IsEnabled = true;
            progressBar.IsIndeterminate = false;
            statusTestBlock.Text = "Downloaded!";
        }

        private async void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(SendTo))
            {
                MessageBox.Show("Please specify URL.");
                return;
            }

            Uri uri = null;
            Uri.TryCreate(SendTo, UriKind.Absolute, out uri);
            if (uri == null)
            {
                Uri.TryCreate("http://" + SendTo, UriKind.Absolute, out uri);
            }

            if (uri == null)
            {
                MessageBox.Show("Please specify valid URL.");
                return;
            }
            
            offer offer = _model.shop.offers.First(x => x.id == Id);

            string json = JsonConvert.SerializeObject(offer, 
                Formatting.Indented, 
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore});

            statusTestBlock.Text = "Sending...";
            progressBar.IsIndeterminate = true;
            using (HttpClient client = new HttpClient())
            {                
                HttpContent content = new StringContent(json);
                HttpResponseMessage responseMessage = await client.PostAsync(uri, content);
                string response = await responseMessage.Content.ReadAsStringAsync();
                statusTestBlock.Text = "Sent!";
                progressBar.IsIndeterminate = false;
                MessageBox.Show(response);
            }
            _model = null;
            json = null;
            downlodButton.IsEnabled = true;
            sendButton.IsEnabled = false;
        }
    }
}
