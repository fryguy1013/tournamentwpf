using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TournamentWPF.Model
{
    public class Robot : INotifyPropertyChanged
    {
        private int id;
        private string name;
        private string team;

        public int Id { get { return id; } set { id = value; NotifyPropertyChanged("Id"); } }
        public string Name { get { return name; } set { name = value; NotifyPropertyChanged("Name"); } }
        public string Team { get { return team; } set { team = value; NotifyPropertyChanged("Team"); } }

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
