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
using System.Collections.ObjectModel;

namespace TournamentWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private ObservableCollection<Match> matches = new ObservableCollection<Match>();

        public Window1()
        {
            InitializeComponent();

            matches.Add(new Match { Left = "Left", Right = "Right", Winner = "Winner" });
            matches.Add(new Match { Left = "Left", Right = "Right", Winner = "Winner" });
            matches.Add(new Match { Left = "Left", Right = "Right", Winner = "Winner" });

            this.Reviews.ItemsSource = matches;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            matches[0].Right = "Purple";
        }
    }


    public class Match : DependencyObject
    {


        public string Left
        {
            get { return (string)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Left.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(string), typeof(Match), new UIPropertyMetadata(""));


        public string Right { get; set; }
        public string Winner { get; set; }
    }
}
