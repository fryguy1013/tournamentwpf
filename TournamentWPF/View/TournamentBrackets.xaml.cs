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
using TournamentWPF.Model;
using System.ComponentModel;

namespace TournamentWPF.View
{
    /// <summary>
    /// Interaction logic for TournamentBrackets.xaml
    /// </summary>
    public partial class TournamentBrackets : UserControl
    {


        public Tournament SelectedTournament
        {
            get { return (Tournament)GetValue(SelectedTournamentProperty); }
            set { SetValue(SelectedTournamentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTournament.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTournamentProperty =
            DependencyProperty.Register("SelectedTournament", typeof(Tournament), typeof(TournamentBrackets), new UIPropertyMetadata(null));


        public TournamentBrackets()
        {
            InitializeComponent();

            DependencyPropertyDescriptor prop = DependencyPropertyDescriptor.FromProperty(SelectedTournamentProperty, typeof(TournamentBrackets));
            prop.AddValueChanged(this, delegate { UpdateBrackets(); });
        }

        private void UpdateBrackets()
        {
            Brackets.Children.Clear();
            if (SelectedTournament != null)
                AddBracket(SelectedTournament.FinalWinner, 1200, 0, Colors.Silver);
        }


        const double bracketwidth = 110;
        struct BracketLocation
        {
            public double Height { get; set; }
            public double Slotloc { get; set; }
        }
        private BracketLocation AddBracket(MatchSlot slot, double right, double top, Color color)
        {
            Brush brush, border;
            if (slot.Match != null && slot.Robot != null && slot.Robot == slot.Match.Winner)
            {
                brush = new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B));
                border = Brushes.Black;
            }
            else
            {
                brush = new SolidColorBrush(Color.FromArgb(48, color.R, color.G, color.B)); ;
                border = null;
            }

            if (slot.WinnerFrom != null)
            {
                BracketLocation above = AddBracket(slot.WinnerFrom.Robots[0], right - bracketwidth, top, Color.FromArgb(255, 255, 128, 128));
                BracketLocation below = AddBracket(slot.WinnerFrom.Robots[1], right - bracketwidth, top + above.Height + 10, Color.FromArgb(255, 96, 128, 255));
                double loc = (above.Slotloc + below.Slotloc + above.Height + 10) / 2;

                AddBracketSlot(slot, right, top + loc - 10, brush, border);
                //AddBracketSlot(slot, right, top + above.Height - 10);

                Brackets.Children.Add(new Line
                {
                    X1 = right - bracketwidth,
                    X2 = right - bracketwidth,
                    Y1 = top + above.Slotloc - 10,
                    Y2 = top + above.Height + below.Slotloc + 20,
                    Stroke = Brushes.Black
                });

                return new BracketLocation { Height = above.Height + below.Height + 10, Slotloc = loc };
            }
            else
            {
                AddBracketSlot(slot, right, top, brush, border);
                return new BracketLocation { Height = 20, Slotloc = 10 };
            }
        }
        private double AddBracketSlot(MatchSlot slot, double right, double top, Brush color, Brush border)
        {
            //Console.WriteLine("{0} {1} {2}", slot.Robot != null ? slot.Robot.Name : slot.Desc, right, top);
            /*
            Rectangle rect = new Rectangle
            {
                Width = bracketwidth,
                Height = 20,
                Fill = color,
                Stroke = border,
                StrokeThickness = 2,
                RadiusX = 4,
                RadiusY = 4
            };
            rect.SetValue(Canvas.LeftProperty, right - rect.Width);
            rect.SetValue(Canvas.TopProperty, top);

            TextBlock text = new TextBlock
            {
                Width = bracketwidth,
                Height = 20,
                Margin = new Thickness(5, 0, 0, 0),
                Text = slot.Robot != null ? slot.Robot.Name : slot.Desc,
            };
            text.SetValue(Canvas.LeftProperty, right - rect.Width);
            text.SetValue(Canvas.TopProperty, top);

            Brackets.Children.Add(rect);
            Brackets.Children.Add(text);
            */

            ContentPresenter presenter = new ContentPresenter
            {
                Content = slot,
                ContentTemplate = (DataTemplate)Resources["MatchSlotTemplate"],
                Width = bracketwidth,
                Height = 20,
            };
            presenter.SetValue(Canvas.LeftProperty, right - bracketwidth);
            presenter.SetValue(Canvas.TopProperty, top);
            Brackets.Children.Add(presenter);

            return presenter.Height;
        }

    }
}
