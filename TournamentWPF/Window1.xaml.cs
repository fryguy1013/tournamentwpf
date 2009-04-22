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
using System.Windows.Threading;
using TournamentWPF.Model;
using TournamentWPF.ViewModel;
using System.ComponentModel;

namespace TournamentWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, INotifyPropertyChanged
    {
        private Random rand = new Random();
        private Event mainEvent;


        private Tournament selectedTournament;
        public Tournament SelectedTournament
        {
            get { return selectedTournament; }
            set
            {
                selectedTournament = value;

                UpdateBrackets();
                UpdateMatches();
                NotifyPropertyChanged("SelectedTournament");
            }
        }

        public Window1()
        {
             InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainEvent = new Event("tournament.xml");
            
            var tournamentsquery = mainEvent.Tournaments;
            Tournaments.ItemsSource = tournamentsquery;
            Tournaments.SelectedIndex = 0;

            UpdateMatches();
            UpdateBrackets();

            mainEvent.ExportToCsv();
        }

        private void UpdateMatches()
        {
            // this might get called early, if so, wait until later
            if (mainEvent == null || SelectedTournament == null)
                return;

            int index = Matches.SelectedIndex;

            var query = from match in SelectedTournament.Matches.Values
                        select new MatchViewModel(match);

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

        private void MatchFilter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateMatches();
        }

        private void RobotName_KeyDown(object sender, KeyEventArgs e)
        {
            if (Robots.SelectedItem == null)
                return;
            Robot robot = Robots.SelectedItem as Robot;
            robot.Name = RobotName.Text;

            UpdateMatches();
            UpdateBrackets();
        }

        private void RobotTeam_KeyDown(object sender, KeyEventArgs e)
        {
            if (Robots.SelectedItem == null)
                return;
            Robot robot = Robots.SelectedItem as Robot;
            robot.Team = RobotTeam.Text;

            UpdateMatches();
            UpdateBrackets();
        }



        private void UpdateBrackets()
        {
            if (SelectedTournament == null)
                return;

            Brackets.Children.Clear();
            AddBracket(SelectedTournament.FinalWinner, 720, 0, Colors.Silver);
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
                Text = slot.Robot != null ? slot.Robot.Name : slot.Desc,
            };
            text.SetValue(Canvas.LeftProperty, right - rect.Width);
            text.SetValue(Canvas.TopProperty, top);

            Brackets.Children.Add(rect);
            Brackets.Children.Add(text);

            return rect.Height;
        }

        private void btnGenerateMatches_Click(object sender, RoutedEventArgs e)
        {
            if (mainEvent == null || SelectedTournament == null)
                return;

            SelectedTournament.Matches.Clear();
            SelectedTournament.StartTournament();

            UpdateBrackets();
            UpdateMatches();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
