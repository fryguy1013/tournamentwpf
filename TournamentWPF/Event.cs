using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace TournamentWPF
{
    public class Event
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public List<Tournament> Tournaments { get; set; }

        public Event(string filename)
        {
            try
            {
                XElement events = XElement.Load(filename);

                var thiselem =
                    (from e in events.Descendants("event")
                    select new
                    {
                        Name = (string)e.Element("name"),
                        Date = (string)e.Element("date"),
                        Tournaments =
                            (from tournament in e.Descendants("tournament")
                            select new Tournament
                            {
                                WeightClass = (string)tournament.Element("weightclass"),
                                FinalWinnerId = (string)tournament.Element("finalwinner"),
                                Robots =
                                    (from robot in e.Descendants("robot")
                                    select new Robot
                                    {
                                        Id = (int)robot.Attribute("id"),
                                        Name = (string)robot.Element("name"),
                                        Team = (string)robot.Element("team"),
                                        Weight = (string)robot.Element("weight"),
                                        Channel1 = (string)robot.Element("freq1"),
                                        Channel2 = (string)robot.Element("freq2"),
                                    }).ToDictionary(r => r.Id),
                                Matches =
                                    (from match in e.Descendants("match")
                                    select new Match
                                    {
                                        MatchId = (string)match.Attribute("matchid"),
                                        WinnerMatchSlotId = (string)match.Attribute("winnerto"),
                                        LoserMatchSlotId = (string)match.Attribute("loserto"),
                                        Robots =
                                            (from matchslot in match.Descendants("slot")
                                            select new MatchSlot
                                            {
                                                RobotId = (string)matchslot.Attribute("robot"),
                                                MatchFromId = (string)matchslot.Attribute("from")
                                            }).ToList()
                                    }).ToDictionary(m => m.MatchId)
                            }).ToList(),
                    }).Single();

                Name = thiselem.Name;
                Date = thiselem.Date;
                Tournaments = thiselem.Tournaments;

                foreach (Tournament t in Tournaments)
                {
                    if (t.FinalWinnerId != null && t.FinalWinnerId.Length > 0)
                        t.FinalWinner = new MatchSlot { Robot = t.Robots[Int32.Parse(t.FinalWinnerId)] };
                    else
                        t.FinalWinner = new MatchSlot();

                    foreach (Match match in t.Matches.Values)
                    {
                        foreach (MatchSlot ms in match.Robots)
                        {
                            ms.Match = match;
                            if (ms.RobotId.Length > 0)
                                ms.Robot = t.Robots[Int32.Parse(ms.RobotId)];
                        }

                        if (match.WinnerMatchSlotId != null && match.WinnerMatchSlotId.Length > 0)
                        {
                            if (match.WinnerMatchSlotId == "Winner")
                            {
                                match.WinnerMatchSlot = t.FinalWinner;
                            }
                            else
                            {
                                match.WinnerMatchSlot = (from ms in t.Matches[match.WinnerMatchSlotId].Robots
                                                         where ms.MatchFromId == match.MatchId
                                                         select ms).Single();
                            }
                        }
                        if (match.LoserMatchSlotId != null && match.LoserMatchSlotId.Length > 0)
                        {
                            match.LoserMatchSlot = (from ms in t.Matches[match.LoserMatchSlotId].Robots
                                                    where ms.MatchFromId == match.MatchId
                                                    select ms).Single();
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Save(string filename)
        {
            Console.WriteLine("------");
            XElement xml = new XElement("events",
              new XElement("event",
                new XElement("name", Name),
                new XElement("date", Date),
                new XElement("tournaments",
                    from t in Tournaments
                    select new XElement("tournament",
                        new XElement("weightclass", t.WeightClass),
                        new XElement("finalwinner", t.FinalWinner != null && t.FinalWinner.Robot != null ? t.FinalWinner.Robot.Id.ToString() : ""),
                        new XElement("robots",
                            from r in t.Robots.Values
                            select new XElement("robot",
                                new XAttribute("id", r.Id),
                                new XElement("name", r.Name),
                                new XElement("team", r.Team),
                                new XElement("weight", r.Weight),
                                new XElement("freq1", r.Channel1),
                                new XElement("freq2", r.Channel2)
                            )
                        ),
                        new XElement("matches",
                            from m in t.Matches.Values
                            select new XElement("match",
                                new XAttribute("matchid", m.MatchId),
                                new XAttribute("winnerto", m.WinnerMatchSlotId != null ? m.WinnerMatchSlotId : ""),
                                new XAttribute("loserto", m.LoserMatchSlotId != null ? m.LoserMatchSlotId : ""),
                                new XElement("slots",
                                    from ms in m.Robots
                                    select new XElement("slot",
                                        new XAttribute("robot", ms.Robot != null ? ms.Robot.Id.ToString() : ""),
                                        new XAttribute("from", ms.MatchFrom != null ? ms.MatchFrom.MatchId : "")
                                    )
                                )
                            )
                        )
                    )
                )
              )
            );

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, xws))
            {
                xml.WriteTo(writer);
            }
        }

        public override string ToString()
        {
            string ret = "";
            ret += String.Format("Event: {0}; {1}\n", Name, Date);

            foreach (Tournament b in Tournaments)
            {
                ret += b.ToString() + "\n";
            }

            return ret;
        }

        public void LoadMatches()
        {
            XElement templates = XElement.Load("brackettemplates.xml");

            var bracket = (from c in templates.Descendants("bracket")
                           where (string)c.Attribute("id") == "DE8"
                           select c).Single();

            var matches = from m in bracket.Descendants("match")
                          select new
                          {
                              id = (string)m.Attribute("id"),
                              winner = (string)m.Element("winner"),
                              loser = (string)m.Element("loser")
                          };

            Tournament t = Tournaments.Single();

            foreach (var match in matches)
            {
                t.Matches[match.id] = new Match
                {
                    MatchId = match.id,
                    WinnerMatchSlotId = match.winner,
                    LoserMatchSlotId = match.loser,
                };
            }
            foreach (Match match in t.Matches.Values)
            {
                if (match.WinnerMatchSlotId == "Winner")
                    match.WinnerMatchSlot = t.FinalWinner;
                else if (match.WinnerMatchSlotId != null)
                    match.WinnerMatchSlot = t.Matches[match.WinnerMatchSlotId].AddMatchSlot();
                if (match.LoserMatchSlotId != null)
                    match.LoserMatchSlot = t.Matches[match.LoserMatchSlotId].AddMatchSlot();
            }

            while (t.Robots.Count < bracket.Descendants("seed").Count())
            {
                t.Robots.Add(t.Robots.Count + 1, new Robot
                {
                    Id = t.Robots.Count + 1,
                    Name = "Bye",
                    Team = "Bye",
                    Channel1 = "n/a",
                    Channel2 = "n/a",
                    Weight = "0",
                });
            }

            int seed = 1;
            foreach (Robot r in t.Robots.Values)
            {
                string matchid = (string)bracket.Descendants("seed").Single(s => (int)s.Attribute("id") == seed).Attribute("match");
                Console.WriteLine("seed {0} = {1}", seed, matchid);
                t.Matches[matchid].AddMatchSlot().Robot = r;
                seed++;
            }            
        }
    }

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

    public class Robot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public string Weight { get; set; }
        public string Channel1 { get; set; }
        public string Channel2 { get; set; }

        public Robot()
        {
        }
        public override string ToString()
        {
            return Name;
            //return String.Format("Robot[{5}]: {0} ({1}); Freq: {2}/{3}. Weight: {4}", Name, Team, Channel1, Channel2, Weight, Id);
        }
    }

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
    }

    public class MatchSlot
    {
        public Robot Robot { get; set; }
        public Match Match { get; set; }
        public Match WinnerFrom { get; set; }
        public Match LoserFrom { get; set; }
        public Match MatchFrom { get { if (WinnerFrom != null) return WinnerFrom; else return LoserFrom; } }

        public override string ToString()
        {
            return Robot != null ? Robot.Name : "n/a";
        }

        public string RobotId { get; set; }
        public string MatchFromId { get; set; }
    }
}
