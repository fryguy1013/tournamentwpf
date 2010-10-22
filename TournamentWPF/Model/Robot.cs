using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TournamentWPF.Model
{
    public class Robot : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _team;
        private string _imagePath;

        public int Id { get { return _id; } set { _id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return _name; } set { _name = value; NotifyPropertyChanged("Name"); } }
        public string Team { get { return _team; } set { _team = value; NotifyPropertyChanged("Team"); } }
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                NotifyPropertyChanged("ImagePath");
            }
        }

        public Robot()
        {
        }
        public override string ToString()
        {
            return Name;
            //return String.Format("Robot[{5}]: {0} ({1}); Freq: {2}/{3}. Weight: {4}", Name, Team, Channel1, Channel2, Weight, Id);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

    }

}
