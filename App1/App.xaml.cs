﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App1
{
    /// <summary>
    /// Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    sealed partial class App : Application
    {
        public static ObservableCollection<Date> myDates;
        private DateSerializer serializer;
        public static Windows.Storage.IStorageFile datesFile;
        Color barBColor;
        Color inBarBColor;
        public enum SortEnum { CreationDate, Date, Title};
        public static SortEnum ChosenSortAlgorithm;
        public static bool AscOrder = true;

        /// <summary>
        /// Initialisiert das Singletonanwendungsobjekt.  Dies ist die erste Zeile von erstelltem Code
        /// und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App()
        {
            myDates = new ObservableCollection<Date>();
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            serializer = DateSerializer.getInstance();
            RestoreDatebase();
            ChosenSortAlgorithm = SortEnum.Date;
        }


        private async void RestoreDatebase()
        {
            try
            {
                myDates = await serializer.read();
            }
            catch (Exception)
            {
                Debug.WriteLine("Ausgabe von myDates:");
                foreach (Date date in myDates)
                {
                    Debug.WriteLine(date.Title);

                }
                Debug.WriteLine("Ausgabe beendet");
            }
        }

        public void RefreshDatebase()
        {
            serializer.write(myDates);
        }


        /// <summary>
        /// Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        /// werden z. B. verwendet, wenn die Anwendung gestartet wird, um eine bestimmte Datei zu öffnen.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {


#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;
            barBColor = Color.FromArgb(0, 67, 131, 255);
            inBarBColor = Color.FromArgb(0, 67, 176, 255);
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = barBColor;
            view.TitleBar.ButtonBackgroundColor = barBColor;
            view.TitleBar.InactiveBackgroundColor = inBarBColor;
            view.TitleBar.ButtonInactiveBackgroundColor = inBarBColor;

            // App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
            // Nur sicherstellen, dass das Fenster aktiv ist.
            if (rootFrame == null)
            {
                // Frame erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Zustand von zuvor angehaltener Anwendung laden
                }

                // Den Frame im aktuellen Fenster platzieren
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
                // und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
                // übergeben werden
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Sicherstellen, dass das aktuelle Fenster aktiv ist
            Window.Current.Activate();
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Navigation auf eine bestimmte Seite fehlschlägt
        /// </summary>
        /// <param name="sender">Der Rahmen, bei dem die Navigation fehlgeschlagen ist</param>
        /// <param name="e">Details über den Navigationsfehler</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        /// ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        /// unbeschädigt bleiben.
        /// </summary>
        /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
        /// <param name="e">Details zur Anhalteanforderung.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            deferral.Complete();
        }
    }
}
