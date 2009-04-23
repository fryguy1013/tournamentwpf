using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace TournamentWPF.Model
{
    public class Event
    {
        public string Name { get; set; }
        public string Date { get; set; }
        public List<Tournament> Tournaments { get; set; }

        public static Action MatchChanged;

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
                                    (from robot in tournament.Descendants("robot")
                                    select new Robot
                                    {
                                        Id = (int)robot.Attribute("id"),
                                        Name = (string)robot.Element("name"),
                                        Team = (string)robot.Element("team"),
                                    }).ToDictionary(r => r.Id),
                                Matches =
                                    (from match in tournament.Descendants("match")
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
                                                MatchFromId = (string)matchslot.Attribute("from"),
                                                Points = (int)matchslot.Attribute("points"),
                                            }).ToList(),
                                        Result = (MatchResultType)Enum.Parse(typeof(MatchResultType), (string)match.Attribute("result")),
                                        MatchTime = (string)match.Attribute("duration"),
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

            MatchChanged += delegate { Save(filename); };
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
                                new XElement("team", r.Team)
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
                                        new XAttribute("from", ms.MatchFrom != null ? ms.MatchFrom.MatchId : ""),
                                        new XAttribute("points", ms.Points.ToString())
                                    )
                                ),
                                new XAttribute("result", m.Result),
                                new XAttribute("duration", m.MatchTime != null ? m.MatchTime : "")
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


        public void ExportToCsv()
        {
            Console.WriteLine("------");
            foreach (Tournament t in Tournaments)
            {
                var matches =
                    from m in t.Matches.Values
                    where m.RedRobot.Name != "Bye" &&
                          m.BlueRobot.Name != "Bye"
                    select m;

                foreach (var m in matches)
                    Console.WriteLine("{0},{1},{2},{3}", t.WeightClass, m.RedRobot, m.BlueRobot, m.Winner.Name);
            }
        }


    }


}
