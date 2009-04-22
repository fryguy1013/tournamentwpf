using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TournamentWPF.Model
{
    public class Match
    {
        public string MatchId { get; set; }
        public List<MatchSlot> Robots { get; set; }

        public IEnumerable<Robot> GetRobotsInSubTree()
        {
            foreach (MatchSlot ms in Robots)
            {
                if (ms.Robot != null)
                    yield return ms.Robot;
                if (ms.WinnerFrom != null)
                    foreach (Robot r in ms.WinnerFrom.GetRobotsInSubTree())
                        yield return r;
            }
        }
        public int GetLeafOrRobotsInSubTree()
        {
            int ret = 0;
            foreach (MatchSlot ms in Robots)
            {
                if (ms.Robot != null)
                    ret += 1;
                else if (ms.WinnerFrom != null)
                    ret += ms.WinnerFrom.GetLeafOrRobotsInSubTree();
                else
                    ret += 1;
            }
            return ret;
        }

        private MatchSlot winnerMatchSlot;
        public MatchSlot WinnerMatchSlot
        {
            get { return winnerMatchSlot; }
            set
            {
                winnerMatchSlot = value;
                value.WinnerFrom = this;
            }
        }
        private MatchSlot loserMatchSlot;
        public MatchSlot LoserMatchSlot
        {
            get { return loserMatchSlot; }
            set
            {
                loserMatchSlot = value;
                value.LoserFrom = this;
            }
        }

        public string WinnerMatchSlotId { get; set; }
        public string LoserMatchSlotId { get; set; }

        public Robot Winner
        {
            get { return WinnerMatchSlot == null ? null : WinnerMatchSlot.Robot; }
            set { if (WinnerMatchSlot != null) WinnerMatchSlot.Robot = value; }
        }
        public Robot Loser
        {
            get { return LoserMatchSlot == null ? null : LoserMatchSlot.Robot; }
            set { if (LoserMatchSlot != null) LoserMatchSlot.Robot = value; }
        }
        public Robot RedRobot
        {
            get { return Robots[0].Robot; }
        }
        public Robot BlueRobot
        {
            get { return Robots[1].Robot; }
        }

        public Match()
        {
            Robots = new List<MatchSlot>();
        }

        public MatchSlot AddMatchSlot()
        {
            Robots.Add(new MatchSlot
            {
                Match = this
            });
            return Robots.Last();
        }

        public override string ToString()
        {
            return String.Format("Match[{0}]: {1} vs {2} (advances to: {3}; loser to: {4})", MatchId, Robots.ElementAtOrDefault(0), Robots.ElementAtOrDefault(1), WinnerMatchSlotId, LoserMatchSlotId);
        }

        public void SetWinner(Robot robot)
        {
            if (robot == null)
            {
                Winner = Loser = null;
            }
            else if (robot == RedRobot)
            {
                Winner = robot;
                Loser = BlueRobot;
            }
            else if (robot == BlueRobot)
            {
                Winner = robot;
                Loser = RedRobot;
            }
            else
                throw new ArgumentException("Winner must be one of the robots in the match!");
        }
    }
}
