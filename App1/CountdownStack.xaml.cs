using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
            TitleBlock.Text = date.Title;
            final = date.FinalDate;
            TimeSpan difference = date.FinalDate - DateTimeOffset.Now;
            fillDateBeautiful(difference);
            dateset = date;
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
                ThirdDateBlock.Text = Convert.ToString(diff.Minutes) + " M";
            }
            else
            {
                int weeks = Math.Abs(diff.Days / 7);
                int days = diff.Days - 7 * weeks;
                FirstDateBlock.Text = Convert.ToString(weeks) + " W";
                SecondDateBlock.Text = Convert.ToString(days) + " D";
                ThirdDateBlock.Text = Convert.ToString(diff.Hours) + " H";
            }
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
