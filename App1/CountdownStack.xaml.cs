using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace App1
{
    public sealed partial class CountdownStack : UserControl
    {
        private DateTimeOffset final;
        public Date dateset;

        public CountdownStack()
        {
            this.InitializeComponent();
        }

        public CountdownStack(String title, String date)
        {
            this.InitializeComponent();
            TitleBlock.Text = title;
            FirstDateBlock.Text = date;
        }

        public CountdownStack(Date date)
        {
            this.InitializeComponent();
            //Stack.Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
            TitleBlock.Text = date.Title;
            final = date.FinalDate;
            TimeSpan difference = date.FinalDate - DateTimeOffset.Now;
            fillDateBeautiful(difference);
            dateset = date;
        }

        public void SetColor(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            Stack.Background = brush;
        }

        private void fillDateBeautiful(TimeSpan diff)
        {
            if (diff.TotalDays < 1)
            {
                FirstDateBlock.Text = Convert.ToString(diff.Hours) + " H";
                SecondDateBlock.Text = Convert.ToString(diff.Minutes) + " M";
                ThirdDateBlock.Text = Convert.ToString(diff.Seconds) + " S";
            }
            else if (diff.TotalDays < 7)
            {
                FirstDateBlock.Text = Convert.ToString(diff.Days) + " D";
                SecondDateBlock.Text = Convert.ToString(diff.Hours) + " H";
                ThirdDateBlock.Text = Convert.ToString(diff.Minutes) + " M,  " + Convert.ToString(diff.Seconds) + " S";
            }
            else
            {
                int weeks = Math.Abs(diff.Days / 7);
                int days = diff.Days - 7 * weeks;
                FirstDateBlock.Text = Convert.ToString(weeks) + " W";
                SecondDateBlock.Text = Convert.ToString(days) + " D";
                ThirdDateBlock.Text = Convert.ToString(diff.Hours) + " H,  " + Convert.ToString(diff.Minutes) + " M,  " + Convert.ToString(diff.Seconds) + " S";
            }
        }

        public string GetFormattedCreationDate ()
        {
            string formatted;
            DateTime cDate = dateset.CreationDate;
            formatted = formatDate(cDate);
            return formatted;
        }

        public string GetFormattedFinalDate ()
        {
            string formatted;
            DateTime fDate = dateset.FinalDate;
            formatted = formatDate(fDate);
            return formatted;
        }

        private string formatDate (DateTime date)
        {
            string formattedDate = date.Day + "." + date.Month + "." + date.Year;
            return formattedDate;
        }

        public void RefreshCountdown()
        {
            TimeSpan difference = final - DateTimeOffset.Now;
            if (difference.Seconds < 0)
            {
                throw new ExceededException();
            }
            else
            {
                fillDateBeautiful(difference);
            }
        }

        public void setTitle(String title)
        {
            TitleBlock.Text = title;
        }

        public void setFirstDate (String date)
        {
            FirstDateBlock.Text = date;
        }
    }
}
