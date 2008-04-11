using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TournamentWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private List<Match> matches = new List<Match>();

        public Window1()
        {
            InitializeComponent();

            matches.Add(new Match { Left = "Left", Right = "Right", Winner = "Winner" });
            matches.Add(new Match { Left = "Left", Right = "Right", Winner = "Winner" });
            matches.Add(new Match { Left = "Left", Right = "Right", Winner = "Winner" });

            this.Reviews.ItemsSource = matches;
        }
    }


    public class Match
    {
        public string Left { get; set; }
        public string Right { get; set; }
        public string Winner { get; set; }
    }
}
