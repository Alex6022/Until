﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace App1
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        public AddPage()
        {
            this.InitializeComponent();
            Title.Focus(FocusState.Programmatic);
            DatePicker.MinYear = System.DateTimeOffset.Now;
        }

        private async void AddB_Click(object sender, RoutedEventArgs e)
        {
            int year = DatePicker.Date.Year;
            int month = DatePicker.Date.Month;
            int day = DatePicker.Date.Day;
            int hour = TimePicker.Time.Hours;
            int minute = TimePicker.Time.Minutes;
            DateTime selectedTime = new DateTime(year, month, day, hour, minute, 0);
            if (selectedTime < DateTime.Now)
            {
                string messageText = "Der gewählte Zeitpunkt liegt in der Vergangenheit";
                MessageDialog dateDialog = new MessageDialog(messageText, "Anderen Zeitpunkt wählen");
                try
                {
                    await dateDialog.ShowAsync();
                }
                catch (Exception)
                {
                    Debug.WriteLine(messageText);
                }
            }
            else if (String.IsNullOrWhiteSpace(Title.Text) == true)
            {
                MessageDialog titleDialog = new MessageDialog("Bitte gib einen Titel für das Event an", "Titel fehlt");
                await titleDialog.ShowAsync();
                Title.Focus(FocusState.Programmatic);
            }
            else
            {
                App.myDates.Add(new Date(Title.Text, selectedTime));
                this.Frame.Navigate(typeof(MainPage));
                DateSerializer.getInstance().write(App.myDates);
            }
        }

        private void currentTimeButton_Click(object sender, RoutedEventArgs e)
        {
            TimePicker.Time = DateTimeOffset.Now.AddMinutes(1).TimeOfDay;
            DatePicker.Date = DateTimeOffset.Now;
            Title.Focus(FocusState.Programmatic);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AddB_Click(sender, e);
            }
        }

        private void Outlook_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
