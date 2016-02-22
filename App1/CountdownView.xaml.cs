using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private CountdownStack lastRightTappedItem;
        private static CountdownStack _lastTappedItem;
        public event EventHandler<PointerRoutedEventArgs> onPoint;
        public event EventHandler<PointerRoutedEventArgs> outPoint;
        public event EventHandler<DeletedEventArgs> deletedStack;
        public static event PropertyChangedEventHandler PropertyChanged;

        public static CountdownStack lastTappedItem
        {
            get
            {
                return _lastTappedItem;
            }
            set
            {
                _lastTappedItem = value;
                PropertyChanged(lastTappedItem, new PropertyChangedEventArgs("lastTappedItem"));
            }
        }

        public CountdownView()
        {
            this.InitializeComponent();
        }

        public void addCountdown (Date date)
        {
            this.InitializeComponent();
            CountdownStack cStack = new CountdownStack(date);
            cStack.RightTapped += CStack_RightTapped;
            cStack.PointerEntered += CStack_PointerEntered;
            cStack.PointerExited += CStack_PointerExited;
            cStack.Tapped += CStack_Tapped;
            gridView.Items.Add(cStack);
        }

        private void CStack_Tapped(object sender, TappedRoutedEventArgs e)
        {
            lastTappedItem = (CountdownStack)sender;
        }

        private void CStack_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.outPoint(sender, e);
        }

        private void CStack_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.onPoint(sender, e);
        }

        private void CStack_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CountdownStack csender = (CountdownStack)sender;
            var point = e.GetPosition(csender);
            gridFlyout.ShowAt(csender, point);
            lastRightTappedItem = csender;
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
                    DeletedEventArgs args = new DeletedEventArgs(stack, lastTappedItem);
                    this.deletedStack(stack, args);
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

        public void UnselectItems()
        {
            gridView.SelectedIndex = -1;
        }

        public void ReorderItems()
        {
            switch (App.ChosenSortAlgorithm)
            {
                case App.SortEnum.Title:
                    IEnumerable<Date> titleSorted = null;
                    if (App.AscOrder == true)
                    {
                       titleSorted = App.myDates.OrderBy(t => t.Title);
                    }
                    else
                    {
                        titleSorted = App.myDates.OrderByDescending(t => t.Title);
                    }
                    App.myDates = new ObservableCollection<Date>(titleSorted);
                    Clear();
                    PopulateCountdownFromCollection(App.myDates);
                    break;
                case App.SortEnum.Date:
                    IEnumerable<Date> dateSorted = null;
                    if (App.AscOrder == true)
                    {
                        dateSorted = App.myDates.OrderBy(t => t.FinalDate);

                    }
                    else
                    {
                        dateSorted = App.myDates.OrderByDescending(t => t.FinalDate);

                    }
                    App.myDates = new ObservableCollection<Date>(dateSorted);
                    Clear();
                    PopulateCountdownFromCollection(App.myDates);
                    break;
                case App.SortEnum.CreationDate:
                    IEnumerable<Date> creationDateSorted = null;
                    if (App.AscOrder == true)
                    {
                        creationDateSorted = App.myDates.OrderBy(t => t.CreationDate);

                    }
                    else
                    {
                        creationDateSorted = App.myDates.OrderByDescending(t => t.CreationDate);

                    }
                    App.myDates = new ObservableCollection<Date>(creationDateSorted);
                    Clear();
                    PopulateCountdownFromCollection(App.myDates);
                    break;
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            gridView.Items.Remove(lastRightTappedItem);
            App.myDates.Remove(lastRightTappedItem.dateset);
            DateSerializer.getInstance().write(App.myDates);
        }

        public class DeletedEventArgs : EventArgs
        {
            public bool isLastTapped;

            public DeletedEventArgs(CountdownStack deleted, CountdownStack lastTapped)
            {
                if (deleted.Equals(lastTapped))
                {
                    isLastTapped = true;
                }
                else
                {
                    isLastTapped = false;
                }
            }
        }

        private void gridView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string sourceType = e.OriginalSource.GetType().ToString();
            if (sourceType == "Windows.UI.Xaml.Controls.Grid")
            {
                UnselectItems();
                lastTappedItem = null;
            }
        }
    }
}
