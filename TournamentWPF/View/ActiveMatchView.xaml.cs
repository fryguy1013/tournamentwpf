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
using System.ComponentModel;
using TournamentWPF.ViewModel;

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


        public MatchViewModel SelectedMatch
        {
            get { return (MatchViewModel)GetValue(SelectedMatchProperty); }
            set { SetValue(SelectedMatchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedMatch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedMatchProperty =
            DependencyProperty.Register("SelectedMatch", typeof(MatchViewModel), typeof(ActiveMatchView), new UIPropertyMetadata(null));


        public ActiveMatchView()
        {
            InitializeComponent();
        }


        private void MatchRedRobot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetWinner(SelectedMatch.Red.Robot);
        }
        private void MatchBlueRobot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetWinner(SelectedMatch.Blue.Robot);
        }

        private void ResetMatch_Click(object sender, RoutedEventArgs e)
        {
            SetWinner(null);
        }

        private void SetWinner(Robot r)
        {
            try
            {
                if (SelectedMatch != null)
                    SelectedMatch.SetWinner(r);
            }
            catch
            {
                MessageBox.Show("Unable to clear winner of match");
            }
        }

    }
}
