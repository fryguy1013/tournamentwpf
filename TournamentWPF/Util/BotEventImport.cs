using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.IO;
using TournamentWPF.Model;

namespace TournamentWPF.Util
{
    public class BotEventImport
    {
        public BotEventImport()
        {
        }

        public Event GetEvent(string filename)
        {
            var client = new WebClient();
            var str = client.DownloadString(@"http://robogames.net/registration/event/entries_xml/9");
            //var str = File.ReadAllText(@"C:\Users\kevin\Desktop\9.xml");
            var xml = XDocument.Parse(str);

            int i = 0;
            var tournaments =
                (from t in xml.Descendants("division")
                 select new Tournament
                 {
                     WeightClass = (string)t.Attribute("name"),
                     Robots =
                         (from entry in t.Descendants("entry")
                          select new Robot
                          {
                              Id = i++,
                              Name = (string)entry.Attribute("name"),
                              Team = (string)entry.Attribute("teamname"),
                              ImagePath = DownloadImage((string)entry.Attribute("thumbnail_url")),
                          }).ToDictionary(r => r.Id)
                 }).ToList();

            return new Event(filename, "Combots 2010", "10/23/10", tournaments);
        }

        private string DownloadImage(string url)
        {
            var file = Path.GetFileName(url);

            if (file == String.Empty)
                return file;

            if (File.Exists(file))
                return file;

            var client = new WebClient();
            client.DownloadFile(url, file);

            return file;
        }
    }
}
