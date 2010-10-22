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
using TournamentWPF.Util;

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

            Event.MatchChanged += UpdateMatches;
            mainEvent.ExportToCsv();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Event.MatchChanged != null)
                Event.MatchChanged();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var import = new BotEventImport();
            mainEvent = import.GetEvent("tournament.xml");
            Event.MatchChanged();
        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            mainEvent.ExportToCsv();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void UpdateMatches()
        {
            // this might get called early, if so, wait until later
            if (mainEvent == null || SelectedTournament == null)
                return;

            int index = Matches.SelectedIndex;

            var query = SelectedTournament.Matches.Values.AsQueryable();

            if (radRemainingMatches.IsChecked == true)
            {
                query = query.Where(m => m.Winner == null);
            }
            else if (radAllMatches.IsChecked == true)
            {
            }
            else if (radNonemptyMatches.IsChecked == true)
            {
                query = query.Where(m => m.Winner == null && (m.RedRobot != null || m.BlueRobot != null));
                query = query.OrderBy(m => m.RedRobot != null && m.BlueRobot != null ? 0 : 1);
            }

            Matches.ItemsSource = query.Select(m => new MatchViewModel(m)).ToList();
            Matches.SelectedIndex = index == -1 ? 0 : index;
        }

        private void MatchFilter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateMatches();
        }




        private void btnGenerateMatches_Click(object sender, RoutedEventArgs e)
        {
            if (mainEvent == null || SelectedTournament == null)
                return;

            SelectedTournament.Matches.Clear();
            SelectedTournament.FinalWinner.Robot = null;
            SelectedTournament.StartTournament();
            Event.MatchChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
