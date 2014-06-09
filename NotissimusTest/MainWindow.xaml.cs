using System;
using System.Windows;

namespace NotissimusTest
{
    public partial class MainWindow
    {
        private yml_catalog _model;
        private string _postTo = "http://httpbin.org/post";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        ///     Url to GET xml
        /// </summary>
        public string Url
        {
            get { return "http://partner.market.yandex.ru/pages/help/YML.xml"; }
        }

        /// <summary>
        ///     Id of orger to POST to another url
        /// </summary>
        public string Id
        {
            get { return "12344"; }
        }

        /// <summary>
        ///     Url to POST result JSON
        /// </summary>
        public string PostTo
        {
            get { return _postTo; }
            set { _postTo = value; }
        }

        public string Request { get; private set; }

        /// <summary>
        ///     Response to POST request
        /// </summary>
        public string Response { get; private set; }

        private async void GetButtonClick(object sender, RoutedEventArgs e)
        {
            //Update UI
            getButton.IsEnabled = false;
            progressBar.IsIndeterminate = true;
            statusTestBlock.Text = "Getting and deserializing...";
            responseLink.Visibility = Visibility.Hidden;

            //Sending GET request and deserializing response to model
            try
            {
                _model = await Utils.Get(Url);
            }
            catch (Exception ex)
            {
                getButton.IsEnabled = true;
                progressBar.IsIndeterminate = false;
                statusTestBlock.Text = "Error.";
                MessageBox.Show(String.Format("Failed to GET.{0}\n Exception message: {1}\n StackTrace: {2}",
                    ex.GetType(), ex.Message, ex.StackTrace));
                return;
            }

            //Update UI
            postButton.IsEnabled = true;
            progressBar.IsIndeterminate = false;
            statusTestBlock.Text = "Done!";
        }

        private async void SendButtonClick(object sender, RoutedEventArgs e)
        {
            Uri uri = null;
            //Trying to parse string to Uri
            if (String.IsNullOrEmpty(PostTo))
            {
                MessageBox.Show("Please specify URL.");
                return;
            }
            Uri.TryCreate(PostTo, UriKind.Absolute, out uri);
            if (uri == null)
            {
                Uri.TryCreate("http://" + PostTo, UriKind.Absolute, out uri);
            }

            if (uri == null)
            {
                MessageBox.Show("Please specify valid URL.");
                return;
            }

            //Update UI
            statusTestBlock.Text = "Sending POST request...";
            progressBar.IsIndeterminate = true;

            //Sending POST request in background
            Response = await Utils.Post(_model, Id, uri);

            //Update UI
            statusTestBlock.Text = "Done!";
            responseLink.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = false;
            getButton.IsEnabled = true;
            postButton.IsEnabled = false;
            //Reset model
            _model = null;
        }

        private void ResponseLinkClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Response);
        }

        private void RequestLinkClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Utils.Json);
        }
    }
}