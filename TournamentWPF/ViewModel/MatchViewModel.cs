using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TournamentWPF.Model;
using System.ComponentModel;

namespace TournamentWPF.ViewModel
{
    public class MatchViewModel : INotifyPropertyChanged
    {
        public MatchViewModel(Match m)
        {
            match = m;

            match.Robots[0].PropertyChanged += (s, e) =>
            {
                NotifyPropertyChanged("RedRobot");
                NotifyPropertyChanged("RedKnown");
                NotifyPropertyChanged("Winner");
            };
            match.Robots[1].PropertyChanged += (s, e) =>
            {
                NotifyPropertyChanged("BlueRobot");
                NotifyPropertyChanged("BlueKnown");
                NotifyPropertyChanged("Winner");
            };
            match.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Winner" || e.PropertyName == "Loser")
                {
                    NotifyPropertyChanged("RedWins");
                    NotifyPropertyChanged("BlueWins");
                }
                else if (e.PropertyName == "Result")
                    NotifyPropertyChanged("Result");
                else if (e.PropertyName == "MatchTime")
                    NotifyPropertyChanged("MatchTime");
            };
        }

        private Match match;

        public MatchSlot Red { get { return match.Robots[0]; } }
        public MatchSlot Blue { get { return match.Robots[1]; } }

        //public string RedRobot { get { return match.RedRobot != null ? match.RedRobot.Name : match.Robots[0].Desc; } }
        //public string BlueRobot { get { return match.BlueRobot != null ? match.BlueRobot.Name : match.Robots[1].Desc; } }
        //public string Winner { get { return match.Winner != null ? match.Winner.Name : ""; } }
        public string MatchId { get { return match.MatchId; } }
        //public bool RedWins { get { return match.Robots[0].IsWinner; } }
        //public bool BlueWins { get { return match.Robots[1].IsWinner; } }
        //public bool RedKnown { get { return match.Robots[0].Robot != null; } }
        //public bool BlueKnown { get { return match.Robots[1].Robot != null; } }

        public MatchResultType Result { get { return match.Result; } set { match.Result = value; } }
        public string MatchTime { get { return match.MatchTime; } set { match.MatchTime = value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        internal void SetWinner(Robot robot)
        {
            match.SetWinner(robot);
        }
    }
}
