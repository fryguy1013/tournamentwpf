using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentWPF.Model
{
    public class MatchSlot
    {
        public Robot Robot { get; set; }
        public Match Match { get; set; }
        public Match WinnerFrom { get; set; }
        public Match LoserFrom { get; set; }
        public Match MatchFrom { get { if (WinnerFrom != null) return WinnerFrom; else return LoserFrom; } }

        public string Desc
        {
            get
            {
                if (WinnerFrom != null)
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
    }
}
