using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
                    gridView.Items.Remove(stack);
                    App.myDates.Remove(stack.dateset);
                }
            }
        }

        public void Clear()
        {
            gridView.Items.Clear();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            gridView.Items.Remove(lastTappedItem);
            App.myDates.Remove(lastTappedItem.dateset);
        }
    }
}
