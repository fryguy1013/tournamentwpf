using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TournamentWPF.Model;
using System.ComponentModel;

namespace TournamentWPF.ViewModel
{
    class MatchViewModel : INotifyPropertyChanged
    {
        public MatchViewModel(Match m)
        {
            match = m;
        }
        private Match match;

        public string RedRobot { get { return match.RedRobot != null ? match.RedRobot.Name : match.Robots[0].Desc; } }
        public string BlueRobot { get { return match.BlueRobot != null ? match.BlueRobot.Name : match.Robots[1].Desc; } }
        public string Winner { get { return match.Winner != null ? match.Winner.Name : ""; } }
        public string MatchId { get { return match.MatchId; } }
        public bool RedWins { get { return match.Winner != null && match.Winner == match.RedRobot; } }
        public bool BlueWins { get { return match.Winner != null && match.Winner == match.BlueRobot; } }
        public bool RedKnown { get { return match.RedRobot != null; } }
        public bool BlueKnown { get { return match.BlueRobot != null; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
