using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TournamentWPF.Model;

namespace TournamentWPF.View
{
    /// <summary>
    /// Interaction logic for ActiveMatchView.xaml
    /// </summary>
    public partial class ActiveMatchView : UserControl
    {
        //private DispatcherTimer timer = new DispatcherTimer();

        //timer.Interval = TimeSpan.FromSeconds(1);
        //timer.Tick += new EventHandler(timer_Tick);

        /*
        DateTime startTime = DateTime.Now;
        void timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            TimeSpan diff = startTime - DateTime.Now;
            if (diff.Ticks <= 0)
            {
                TimerValue.Text = "0:00";
                timer.Stop();
                return;
            }
            TimerValue.Text = String.Format("{0}:{1:00}", Math.Floor(diff.TotalMinutes), diff.Seconds);
        }
        private void Timer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startTime = DateTime.Now.AddMinutes(2);
            if (timer.IsEnabled)
            {
                timer.Stop();
                TimerValue.Text = "2:00";
            }
            else
                timer.Start();
        }
        */

        public Match SelectedMatch { get; set; }

        public ActiveMatchView()
        {
            InitializeComponent();
        }


        private void MatchRedRobot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedMatch == null)
                return;
            SelectedMatch.Winner = SelectedMatch.RedRobot;
            SelectedMatch.Loser = SelectedMatch.BlueRobot;
        }
        private void MatchBlueRobot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedMatch == null)
                return;
            SelectedMatch.SetWinner(SelectedMatch.BlueRobot);
        }

        private void ResetMatch_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMatch == null)
                return;

            if ((SelectedMatch.WinnerMatchSlot != null && SelectedMatch.WinnerMatchSlot.Match != null && SelectedMatch.WinnerMatchSlot.Match.Winner != null) ||
                (SelectedMatch.LoserMatchSlot != null && SelectedMatch.LoserMatchSlot.Match != null && SelectedMatch.LoserMatchSlot.Match.Winner != null))
            {
                MessageBox.Show("Unable to reset match because next match already has a winner");
                return;
            }

            SelectedMatch.Winner = null;
            SelectedMatch.Loser = null;
        }
    }
}
