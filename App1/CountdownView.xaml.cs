using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace App1
{
    public sealed partial class CountdownView : UserControl
    {
        private CountdownStack lastTappedItem;

        public CountdownView()
        {
            this.InitializeComponent();
        }

        public void addCountdown (Date date)
        {
            this.InitializeComponent();
            CountdownStack cStack = new CountdownStack(date);
            cStack.RightTapped += CStack_RightTapped;
            gridView.Items.Add(cStack);
        }

        private void CStack_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CountdownStack csender = (CountdownStack)sender;
            var point = e.GetPosition(csender);
            gridFlyout.ShowAt(csender, point);
            lastTappedItem = csender;
        }

        public void PopulateCountdownFromCollection (ObservableCollection<Date> dates)
        {
            try
            {
                foreach (Date date in dates)
                {
                    addCountdown(date);
                }
            }
            catch (Exception) { }
        }

        public void RefreshCountdowns ()
        {
            foreach(CountdownStack stack in gridView.Items)
            {
                try
                {
                    stack.RefreshCountdown();
                }
                catch (ExceededException)
                {
                    Debug.WriteLine("Zeit abgelaufen für das Event '" + stack.dateset.Title + "' um " + stack.dateset.FinalDate.ToString());
                    var xmlToastTemplate = "<toast launch=\"app-defined-string\">" +
                         "<visual>" +
                           "<binding template =\"ToastGeneric\">" +
                             "<text>Zeit vorbei</text>" +
                             "<text>" +
                               "Der Countdown '" + stack.dateset.Title + "' ist abgelaufen" +
                             "</text>" +
                           "</binding>" +
                         "</visual>" +
                       "</toast>";
                    var xmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
                    xmlDocument.LoadXml(xmlToastTemplate);
                    var toastNotification = new ToastNotification(xmlDocument);
                    var notification = ToastNotificationManager.CreateToastNotifier();
                    notification.Show(toastNotification);
                    gridView.Items.Remove(stack);
                    App.myDates.Remove(stack.dateset);
                    DateSerializer.getInstance().write(App.myDates);
                }
            }
        }

        public void Clear()
        {
            gridView.Items.Clear();
        }

        public void ReorderItems()
        {
            switch (App.ChosenSortAlgorithm)
            {
                case App.SortEnum.Title:
                    IEnumerable<Date> titleSorted = App.myDates.OrderBy(t => t.Title);
                    App.myDates = new ObservableCollection<Date>(titleSorted);
                    Clear();
                    PopulateCountdownFromCollection(App.myDates);
                    break;
                case App.SortEnum.Date:
                    IEnumerable<Date> dateSorted = App.myDates.OrderBy(t => t.FinalDate);
                    App.myDates = new ObservableCollection<Date>(dateSorted);
                    Clear();
                    PopulateCountdownFromCollection(App.myDates);
                    break;
            }
        }

        private List<int> GetDatePositions ()
        {
            List<int> positionList = new List<int>();
            return positionList;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            gridView.Items.Remove(lastTappedItem);
            App.myDates.Remove(lastTappedItem.dateset);
            DateSerializer.getInstance().write(App.myDates);
        }
    }
}
