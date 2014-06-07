using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
        public string Url { get { return "http://partner.market.yandex.ru/pages/help/YML.xml"; } }
        public string Id { get { return "12344"; } }
        private yml_catalog _model;
        private string json;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void DownloadButtonClick(object sender, RoutedEventArgs e)
        {
            Task<yml_catalog> task = DownloadAndDeserialize(Url);
            downlodButton.IsEnabled = false;
            downloadedTestBlock.Text = "Downloading...";
            progressBar.IsIndeterminate = true;
            _model = await task;
            sendButton.IsEnabled = true;
            downloadedTestBlock.Text = "Downloaded!";
            progressBar.IsIndeterminate = false;
            progressBar.Value = 100;
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            offer offer = _model.shop.offers.First(x => x.id == Id);
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(yml_catalog));            
            json = jsonSerializer.Serialize(offer);
        }

        private async Task<yml_catalog> DownloadAndDeserialize(string url)
        {
            HttpClient client = new HttpClient();
            Task<Stream> xml = client.GetStreamAsync(url);
            Stream xmlStream = await xml;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(yml_catalog));
            return (yml_catalog) Task.FromResult(xmlSerializer.Deserialize(xmlStream)).Result;
        }
    }
}
