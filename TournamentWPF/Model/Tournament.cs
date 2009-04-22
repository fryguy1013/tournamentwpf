using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TournamentWPF.Model
{
    public class Tournament
    {
        public string WeightClass { get; set; }
        public Dictionary<int, Robot> Robots { get; set; }
        public Dictionary<string, Match> Matches { get; set; }
        public MatchSlot FinalWinner { get; set; }
        public string FinalWinnerId { get; set; }

        public Tournament()
        {
            Robots = new Dictionary<int, Robot>();
            Matches = new Dictionary<string, Match>();
            FinalWinner = new MatchSlot();
        }

        public void StartTournament()
        {
            XElement templates = XElement.Load("brackettemplates.xml");

            string matchidstr = "";
            if (Robots.Count <= 4)
                matchidstr = "DE4";
            else if (Robots.Count <= 8)
                matchidstr = "DE8";
            else if (Robots.Count <= 16)
                matchidstr = "DE16";
            else
                throw new Exception("Don't know how to make that kind of bracket");


            var bracket = (from c in templates.Descendants("bracket")
                           where (string)c.Attribute("id") == matchidstr
                           select c).Single();

            var matches = from m in bracket.Descendants("match")
                          select new
                          {
                              id = (string)m.Attribute("id"),
                              winner = (string)m.Element("winner"),
                              loser = (string)m.Element("loser")
                          };

            foreach (var match in matches)
            {
                Matches[match.id] = new Match
                {
                    MatchId = match.id,
                    WinnerMatchSlotId = match.winner,
                    LoserMatchSlotId = match.loser,
                };
            }
            foreach (Match match in Matches.Values)
            {
                if (match.WinnerMatchSlotId == "Winner")
                    match.WinnerMatchSlot = FinalWinner;
                else if (match.WinnerMatchSlotId != null)
                    match.WinnerMatchSlot = Matches[match.WinnerMatchSlotId].AddMatchSlot();
                if (match.LoserMatchSlotId != null)
                    match.LoserMatchSlot = Matches[match.LoserMatchSlotId].AddMatchSlot();
            }
            
            while (Robots.Count < bracket.Descendants("seed").Count())
            {
                Robots.Add(Robots.Count + 1, new Robot
                {
                    Id = Robots.Count + 1,
                    Name = "Bye",
                    Team = "Bye",
                });
            }

            int seed = 1;
            Random rand = new Random();
            foreach (Robot r in Robots.Values.OrderBy(r => r.Name == "Bye").ThenBy(r => rand.Next(100000)))
            {
                string matchid = (string)bracket.Descendants("seed").Single(s => (int)s.Attribute("id") == seed).Attribute("match");
                //Console.WriteLine("seed {0} = {1}", seed, matchid);
                Matches[matchid].AddMatchSlot().Robot = r;
                seed++;
            }
        }

        public override string ToString()
        {
            string ret = "";
            ret += String.Format("---Tournament: {0}---\n", WeightClass);
            foreach (Robot r in Robots.Values)
            {
                ret += r.ToString() + "\n";
            }
            ret += "Matches:\n";
            foreach (Match m in Matches.Values)
            {
                ret += m.ToString() + "\n";
            }


            return ret;
        }
    }
}
