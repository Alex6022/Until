﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;

namespace App1
{
    class DateSerializer
    {
        public static DataContractSerializer serializer;
        private static Windows.Storage.StorageFolder roamingFolder;
        public static DateSerializer instance = new DateSerializer();
        private string fileName;

        public static DateSerializer getInstance ()
        {
            return instance;
        }

        private DateSerializer()
        {
            roamingFolder =
                  Windows.Storage.ApplicationData.Current.RoamingFolder;
            serializer = new DataContractSerializer(typeof(ObservableCollection<Date>));
            fileName = "db.xml";
        }

        public async void write(ObservableCollection<Date> dates)
        {
            Debug.WriteLine("Schreibvorgang gestartet...");
            StorageFile dateFile = await roamingFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            Stream file = await dateFile.OpenStreamForWriteAsync();
            serializer.WriteObject(file, dates);
            file.Dispose();
            Debug.WriteLine("...Schreibvorgang abgeschlossen!");
        }

        public async Task<ObservableCollection<Date>> read ()
        {
            Debug.WriteLine("Lesevorgang gestartet...");
            StorageFile dateFile = await roamingFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            Stream file = await dateFile.OpenStreamForReadAsync();
            Debug.WriteLine("Lesevorgang möglich: " + file.CanRead);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(file, new XmlDictionaryReaderQuotas());
            ObservableCollection<Date> date = (ObservableCollection<Date>)serializer.ReadObject(reader, false);
            file.Dispose();
            Debug.WriteLine("...Lesevorgang abgeschlossen!");
            return date;
        }

        public async void showWritten ()
        {
            try
            {
                ObservableCollection<Date> dates = await read();
                Debug.WriteLine("Zeige Collection aus XML im DataSerializer: ");
                foreach (Date date in dates)
                {
                    Debug.WriteLine(date.Title);
                    Debug.WriteLine(date.FinalDate);
                }
                Debug.WriteLine("// Serializer Aufruf beendet");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("showWritten fehlgeschlagen: " + ex.ToString());
            }
        }

        /**
        public static async void saveDates()
        {
            // datesFile =
            // await roamingFolder.CreateFileAsync("dates.xml",
            // Windows.Storage.CreationCollisionOption.ReplaceExisting);
            datesSerializer.Serialize(writer, myDates);
            System.Diagnostics.Debug.WriteLine(writer.ToString());
            datesSerializer.Serialize(writer, myDates);
            await Windows.Storage.FileIO.WriteTextAsync(datesFile, writer.ToString());
        }
    */
    }
}
