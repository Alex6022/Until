using System;
using System.Collections;
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
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
        DispatcherTimer countdownTimer = new DispatcherTimer();


        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(400, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            countdownTimer.Tick += countdownTimer_Tick;
            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Start();
            CountdownView.PopulateCountdownFromCollection(App.myDates);
            CountdownView.onPoint += CountdownView_onPoint;
            CountdownView.outPoint += CountdownView_outPoint;
            CountdownView.deletedStack += CountdownView_deletedStack;
            CountdownView.PropertyChanged += CountdownView_PropertyChanged;
            CreationDateChoser.IsChecked = true;
        }

        private void CountdownView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("lastTappedItem") && sender == null)
            {
                ClearMetaData();
            }
        }

        private void CountdownView_deletedStack(object sender, CountdownView.DeletedEventArgs e)
        {
            if (e.isLastTapped == true)
            {
                ClearMetaData();
            }
        }

        private void CountdownView_outPoint(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (CountdownView.lastTappedItem == null)
                {
                    ClearMetaData();
                }
                else if (CountdownView.lastTappedItem != sender)
                {
                    CountdownStack stack = CountdownView.lastTappedItem;
                    ShowMetaData(stack);
                }
            }
            catch (Exception)
            {
                //DoNothing
            }
        }

        private void CountdownView_onPoint(object sender, PointerRoutedEventArgs e)
        {
            Debug.Write(sender.ToString());
            CountdownStack stack = (CountdownStack)sender;
            ShowMetaData(stack);
        }

        private void ShowMetaData(CountdownStack stack)
        {
            BarBlock.Text = "Erstelldatum: " + stack.GetFormattedCreationDate();
            FinalDateBlock.Text = "Eintrittsdatum: " + stack.GetFormattedFinalDate();
            NameBlock.Text = "Event:\n" + stack.dateset.Title;
        }

        private void ClearMetaData()
        {
            BarBlock.Text = "";
            FinalDateBlock.Text = "";
            NameBlock.Text = "";
        }

        void countdownTimer_Tick(object sender, object e)
        {
            CountdownView.RefreshCountdowns();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            countdownTimer.Start();
            CountdownView.Clear();
            this.Frame.Navigate(typeof(AddPage));
        }

        private void CountdownView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TitleChoser_Click(object sender, RoutedEventArgs e)
        {
            App.ChosenSortAlgorithm = App.SortEnum.Title;
            DateChoser.IsChecked = false;
            CreationDateChoser.IsChecked = false;
            CountdownView.ReorderItems();
        }

        private void DateChoser_Click(object sender, RoutedEventArgs e)
        {
            App.ChosenSortAlgorithm = App.SortEnum.Date;
            TitleChoser.IsChecked = false;
            CreationDateChoser.IsChecked = false;
            CountdownView.ReorderItems();
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            if (App.AscOrder == false)
            {
                App.AscOrder = true;
                Order.Text = "Aufsteigend";
            }
            else
            {
                App.AscOrder = false;
                Order.Text = "Absteigend";
            }
            CountdownView.ReorderItems();
        }

        private void CreationDateChoser_Click(object sender, RoutedEventArgs e)
        {
            App.ChosenSortAlgorithm = App.SortEnum.CreationDate;
            TitleChoser.IsChecked = false;
            DateChoser.IsChecked = false;
            CountdownView.ReorderItems();
        }
    }

    public class Date
    {
        public string Title { get; set; }
        public DateTime FinalDate { get; set; }
        public Color ColorCode { get; set; }
        public DateTime CreationDate { get; set; }

        public Date() { }

        public Date(string name, DateTime release)
        {
            Title = name;
            FinalDate = release;
            ColorCode = Color.FromArgb(0, 255, 0, 0);
            CreationDate = DateTime.Now;
        }

        public Date(string name, DateTime release, Color color)
        {
            Title = name;
            FinalDate = release;
            ColorCode = color;
            CreationDate = DateTime.Now;
        }

        public TimeSpan calculateCountdown()
        {
            TimeSpan countdown = FinalDate - DateTimeOffset.Now;
            return countdown;
        }
    }
}
