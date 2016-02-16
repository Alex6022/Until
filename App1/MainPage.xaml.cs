using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace App1
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(400, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            try
            {
                foreach (Date date in App.myDates)
                {
                    CountdownView.addCountdown(date);
                }
            }
            catch (Exception) { }
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            CountdownView.RefreshCountdowns();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
            CountdownView.Clear();
            this.Frame.Navigate(typeof(AddPage));
        }

        private void CountdownView_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Date // : IXmlSerializable
    {
        public Date() { }

        public Date(string name, DateTimeOffset release)
        {
            Title = name;
            FinalDate = release;
        }

        public TimeSpan calculateCountdown()
        {
            TimeSpan countdown = FinalDate - DateTimeOffset.Now;
            return countdown;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        /**
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Title = reader.GetAttribute("SavedTitle");
            Boolean isEmptyElement = reader.IsEmptyElement; // (1)
            reader.ReadStartElement();
            if (!isEmptyElement) // (1)
            {
                FinalDate = DateTimeOffset.ParseExact(reader.
                    ReadElementContentAsString(), "yyyy-MM-dd", null);
                reader.ReadEndElement();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("SavedTitle", Title);
            writer.WriteElementString("SavedDate", XmlConvert.ToString(FinalDate));
        }
    */
        public string Title { get; set; }
        public DateTimeOffset FinalDate { get; set; }
    }
}
