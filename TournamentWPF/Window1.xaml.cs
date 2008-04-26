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
using System.Collections.ObjectModel;

namespace TournamentWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Random rand = new Random();
        private Event mainEvent;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainEvent = new Event("tournament.xml");
            if (mainEvent.Tournaments[0].Matches.Count() == 0)
                mainEvent.LoadMatches();

            //var query = mainEvent.Tournaments.Single().Robots.Values;
            //Robots.ItemsSource = query;

            UpdateMatches();
        }


        class DisplayMatch
        {
            public string RedRobot { get; set; }
            public string BlueRobot { get; set; }
            public string Winner { get; set; }
            public string MatchId { get; set; }
            public bool RedWins { get; set; }
            public bool BlueWins { get; set; }
            public bool RedKnown { get; set; }
            public bool BlueKnown { get; set; }
        }

        private void UpdateMatches()
        {
            bool full = true;

            var query = from match in mainEvent.Tournaments[0].Matches.Values
                        where full || match.Robots.Count(r => r.Robot != null) > 0
                        orderby full ? match.MatchId.PadLeft(4, '0') : "",
                                match.Winner != null ? 0 : 1
                        select new DisplayMatch
                        {
                            RedRobot = match.RedRobot != null ? match.RedRobot.Name : match.Robots[0].Desc,
                            BlueRobot = match.BlueRobot != null ? match.BlueRobot.Name : match.Robots[1].Desc,
                            Winner = match.Winner != null ? match.Winner.Name : "",
                            RedWins = match.Winner != null && match.Winner == match.RedRobot,
                            BlueWins = match.Winner != null && match.Winner == match.BlueRobot,                            
                            MatchId = match.MatchId,
                            RedKnown = match.RedRobot != null,
                            BlueKnown = match.BlueRobot != null,
                        };
            this.Matches.ItemsSource = query.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tournament t = mainEvent.Tournaments.Single();
            Match match = (from m in t.Matches.Values
                           where m.Winner == null &&
                                 m.Robots.Count(r => r.Robot != null) == 2
                           orderby rand.Next()
                           select m).FirstOrDefault();
            if (match == null)
                return;

            int winner = rand.Next(2);
            match.Winner = match.Robots[winner].Robot;
            match.Loser = match.Robots[1 - winner].Robot;

            UpdateMatches();

            mainEvent.Save("tournament.xml");
        }

        private void Matches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainTextBox.Text = (this.Matches.SelectedItem as DisplayMatch).RedRobot;
        }
    }
}
