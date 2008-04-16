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
        private Event mainEvent = new Event("tournament.xml");

        public Window1()
        {
            InitializeComponent();

            mainEvent.LoadMatches();

            var query = mainEvent.Tournaments.Single().Robots.Values;
            Robots.ItemsSource = query;

            //UpdateMatches();
        }

        /*
        private void UpdateMatches()
        {
            var query = from match in mainEvent.Tournaments[0].Matches.Values
                        where match.Robots.Count(r => r.Robot != null) > 0
                        orderby match.Winner != null ? 0 : 1
                        select new
                        {
                            RedRobot = match.Robots[0].Robot != null ? match.Robots[0].Robot.Name : "",
                            BlueRobot = match.Robots[1].Robot != null ? match.Robots[1].Robot.Name : "",
                            Winner = match.Winner != null ? match.Winner.Name : "",
                            RedWins = match.Winner != null && match.Winner == match.Robots[0].Robot,
                            BlueWins = match.Winner != null && match.Winner == match.Robots[1].Robot,                            
                        };
            this.Matches.ItemsSource = query;
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
        }
        */
    }
}
