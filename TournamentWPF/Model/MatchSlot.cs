using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TournamentWPF.Model
{
    public class MatchSlot : INotifyPropertyChanged
    {
        private Robot robot;
        private Match match;

        public Robot Robot
        {
            get { return robot; }
            set
            {
                robot = value;
                NotifyPropertyChanged("Robot");
                NotifyPropertyChanged("Desc");
                if (robot != null)
                {
                    robot.PropertyChanged += (s, e) =>
                    {
                        NotifyPropertyChanged("Robot");
                        NotifyPropertyChanged("Desc");
                    };
                }
            }
        }
        public Match Match {
            get { return match; }
            set
            {
                match = value;
                match.PropertyChanged += (s, e) =>
                {
                    NotifyPropertyChanged("IsWinner");
                    NotifyPropertyChanged("IsLoser");
                };
            }
        }
        public Match WinnerFrom { get; set; }
        public Match LoserFrom { get; set; }
        public Match MatchFrom { get { if (WinnerFrom != null) return WinnerFrom; else return LoserFrom; } }

        public bool IsWinner
        {
            get
            {
                if (Match == null) throw new Exception("xxx");
                return Match.Winner != null && Match.Winner == Robot;
            }
        }
        public bool IsLoser { get { return Match.Winner != null && Match.Winner != Robot; } }

        public string Desc
        {
            get
            {
                if (Robot != null)
                    return Robot.Name;
                else if (WinnerFrom != null)
                    return "(W" + WinnerFrom.MatchId + ")";
                else if (LoserFrom != null)
                    return "(L" + LoserFrom.MatchId + ")";
                else
                    return "";
            }
        }

        public override string ToString()
        {
            return Robot != null ? Robot.Name : "n/a";
        }

        public string RobotId { get; set; }
        public string MatchFromId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
