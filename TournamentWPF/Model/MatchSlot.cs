using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

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
                NotifyPropertyChanged("ImagePath");
                NotifyPropertyChanged("Image");
                if (robot != null)
                {
                    robot.PropertyChanged += (s, e) =>
                    {
                        NotifyPropertyChanged("Robot");
                        NotifyPropertyChanged("Desc");
                        NotifyPropertyChanged("ImagePath");
                        NotifyPropertyChanged("Image");
                    };
                }
            }
        }
        public Match Match {
            get { return match; }
            set
            {
                match = value;
                if (match != null)
                {
                    match.PropertyChanged += (s, e) =>
                    {
                        NotifyPropertyChanged("IsWinner");
                        NotifyPropertyChanged("IsLoser");
                    };
                }
            }
        }

        private int points;
        public int Points { get { return points; } set { points = value; NotifyPropertyChanged("Points"); } }

        public Match WinnerFrom { get; set; }
        public Match LoserFrom { get; set; }
        public Match MatchFrom { get { if (WinnerFrom != null) return WinnerFrom; else return LoserFrom; } }       

        public bool IsWinner
        {
            get
            {
                return Match != null && Match.Winner != null && Match.Winner == Robot;
            }
        }
        public bool IsLoser { get { return Match != null && Match.Winner != null && Match.Winner != Robot; } }

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
        public string ImagePath
        {
            get
            {
                if (Robot == null || String.IsNullOrEmpty(Robot.ImagePath))
                    return "nopicture-entry.png";
                else
                    return Robot.ImagePath;
            }
        }
        public object Image
        {
            get
            {
                BitmapImage image = new BitmapImage();

                try
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.UriSource = new Uri(ImagePath, UriKind.Relative);
                    image.EndInit();
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }

                return image;
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
