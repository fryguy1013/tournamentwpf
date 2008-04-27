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

        private Tournament SelectedTournament { get { return Tournaments.SelectedItem as Tournament; } }

        public Window1()
        {
             InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainEvent = new Event("tournament.xml");

            foreach (Tournament t in mainEvent.Tournaments)
                if (t.Matches.Count() == 0)
                    mainEvent.LoadMatches();

            var tournamentsquery = mainEvent.Tournaments;
            Tournaments.ItemsSource = tournamentsquery;
            Tournaments.SelectedIndex = 0;

            UpdateRobots();
            UpdateMatches();
            UpdateBrackets();
        }

        private void UpdateRobots()
        {
            var query = SelectedTournament.Robots.Values;
            Robots.ItemsSource = query.ToList();
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
            // this might get called early, if so, wait until later
            if (mainEvent == null || SelectedTournament == null)
                return;

            int index = Matches.SelectedIndex;

            var query = from match in SelectedTournament.Matches.Values
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
            if (radRemainingMatches.IsChecked == true)
            {
                query = query.Where(m => m.Winner == "");
                //query = query.OrderBy(m => m.Winner == "" ? 0 : 1);
            }
            else if (radAllMatches.IsChecked == true)
            {
                //query = query.OrderBy(m => m.MatchId.PadLeft(4, '0'));
            }
            else if (radNonemptyMatches.IsChecked == true)
            {
                query = query.Where(m => m.Winner == "" && (m.RedKnown || m.BlueKnown));
                query = query.OrderBy(m => m.RedKnown && m.BlueKnown ? 0 : 1);
            }

            Matches.ItemsSource = query.ToList();
            Matches.SelectedIndex = index; 
            mainEvent.Save("tournament.xml");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Match match = (from m in SelectedTournament.Matches.Values
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
            if (this.Matches.SelectedItem == null)
                return;

            //MainTextBox.Text = (this.Matches.SelectedItem as DisplayMatch).RedRobot;
            MatchTab.DataContext = this.Matches.SelectedItem as DisplayMatch;
        }

        private void MatchFilter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateMatches();
        }

        private void Robots_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Robots.SelectedItem == null)
                return;
            Robot robot = Robots.SelectedItem as Robot;
            RobotName.Text = robot.Name;
        }

        private void RobotName_KeyDown(object sender, KeyEventArgs e)
        {
            if (Robots.SelectedItem == null)
                return;
            Robot robot = Robots.SelectedItem as Robot;
            robot.Name = RobotName.Text;

            UpdateRobots();
            UpdateMatches();
            UpdateBrackets();
        }

        private void MatchRedRobot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Matches.SelectedItem == null)
                return;
            Match match = SelectedTournament.Matches[(Matches.SelectedItem as DisplayMatch).MatchId];
            match.Winner = match.RedRobot;
            match.Loser = match.BlueRobot;
            UpdateMatches();
            UpdateBrackets();
        }
        private void MatchBlueRobot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Matches.SelectedItem == null)
                return;
            Match match = SelectedTournament.Matches[(Matches.SelectedItem as DisplayMatch).MatchId];
            match.Winner = match.BlueRobot;
            match.Loser = match.RedRobot;

            UpdateMatches();
            UpdateBrackets();
        }

        private void ResetMatch_Click(object sender, RoutedEventArgs e)
        {
            if (Matches.SelectedItem == null)
                return;
            Match match = SelectedTournament.Matches[(Matches.SelectedItem as DisplayMatch).MatchId];

            if ((match.WinnerMatchSlot != null && match.WinnerMatchSlot.Match != null && match.WinnerMatchSlot.Match.Winner != null) ||
                (match.LoserMatchSlot  != null && match.LoserMatchSlot.Match  != null && match.LoserMatchSlot.Match.Winner  != null))
            {
                MessageBox.Show("Unable to reset match because next match already has a winner");
                return;
            }

            match.Winner = null;
            match.Loser = null;

            UpdateMatches();
            UpdateBrackets();
        }

        private void UpdateBrackets()
        {
            Brackets.Children.Clear();
            AddBracket(SelectedTournament.FinalWinner, 540, 0, Colors.Silver);
        }


        const double bracketwidth = 90;
        struct BracketLocation
        {
            public double Height { get; set; }
            public double Slotloc { get; set; }
        }
        private BracketLocation AddBracket(MatchSlot slot, double right, double top, Color color)
        {
            Brush brush, border;
            if (slot.Match != null && slot.Robot != null && slot.Robot == slot.Match.Winner)
            {
                brush = new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B));
                border = Brushes.Black;
            }
            else
            {
                brush = new SolidColorBrush(Color.FromArgb(48, color.R, color.G, color.B)); ;
                border = null;
            }

            if (slot.WinnerFrom != null)
            {
                BracketLocation above = AddBracket(slot.WinnerFrom.Robots[0], right - bracketwidth, top, Color.FromArgb(255, 255, 128, 128));
                BracketLocation below = AddBracket(slot.WinnerFrom.Robots[1], right - bracketwidth, top + above.Height + 10, Color.FromArgb(255, 96, 128, 255));
                double loc = (above.Slotloc + below.Slotloc + above.Height + 10) / 2;

                AddBracketSlot(slot, right, top + loc - 10, brush, border);
                //AddBracketSlot(slot, right, top + above.Height - 10);

                Brackets.Children.Add(new Line
                {
                    X1 = right - bracketwidth,
                    X2 = right - bracketwidth,
                    Y1 = top + above.Slotloc - 10,
                    Y2 = top + above.Height + below.Slotloc + 20,
                    Stroke = Brushes.Black
                });

                return new BracketLocation { Height = above.Height + below.Height + 10, Slotloc = loc };
            }
            else
            {
                AddBracketSlot(slot, right, top, brush, border);
                return new BracketLocation { Height = 20, Slotloc = 10 };
            }
        }
        private double AddBracketSlot(MatchSlot slot, double right, double top, Brush color, Brush border)
        {
            Console.WriteLine("{0} {1} {2}", slot.Robot != null ? slot.Robot.Name : slot.Desc, right, top);
            Rectangle rect = new Rectangle
            {
                Width = bracketwidth,
                Height = 20,
                Fill = color,
                Stroke = border,
                StrokeThickness = 2,
                RadiusX = 4,
                RadiusY = 4
            };
            rect.SetValue(Canvas.LeftProperty, right - rect.Width);
            rect.SetValue(Canvas.TopProperty, top);

            TextBlock text = new TextBlock
            {
                Width = bracketwidth,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 0),
                Text = slot.Robot != null ? slot.Robot.Name : slot.Desc
            };
            text.SetValue(Canvas.LeftProperty, right - rect.Width);
            text.SetValue(Canvas.TopProperty, top);

            Brackets.Children.Add(rect);
            Brackets.Children.Add(text);

            return rect.Height;
        }

        private void Tournaments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBrackets();
            UpdateMatches();
            UpdateRobots();
        }
    }
}
