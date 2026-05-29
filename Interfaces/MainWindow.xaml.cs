using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;



namespace Interfaces
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        const string BACKGROUND_COLOR_NAVBUTTON = "Transparent";
        const string BACKGROUND_HOVER_COLOR_NAVBUTTON = "#2563EB";

        string SelectedPage = "Accueil";
        object? currentNavButton;

        public MainWindow()
        {
            InitializeComponent();

            currentNavButton = BtnAccueil;

            FadeButtonColor(BtnAccueil, "In");

            ChangePage();
        }

        public void ChangePage()
        {
            object page = null;

            switch(SelectedPage)
            {
                case "Accueil":
                    page = new Accueil();
                    break;

                case "Calculateur":
                    page = new Calculateur();
                    break;

                case "RFC":
                    page = new RFC();
                    break;

                default:
                    break;
            }

            if (page == null) return;

            MainFrame.Navigate(page);

        }
        public void FadeButtonColor(Border border, string Mode)
        {

            if (border == null) return;

            ColorAnimation fade = new ColorAnimation();

            fade.From = (Color)ColorConverter.ConvertFromString(Mode == "In" ? BACKGROUND_COLOR_NAVBUTTON : BACKGROUND_HOVER_COLOR_NAVBUTTON);
            fade.To = (Color)ColorConverter.ConvertFromString(Mode == "In" ? BACKGROUND_HOVER_COLOR_NAVBUTTON : BACKGROUND_COLOR_NAVBUTTON);
            fade.Duration = TimeSpan.FromSeconds(0.15);

            SolidColorBrush brush = new SolidColorBrush();
            border.Background = brush;

            brush.BeginAnimation(SolidColorBrush.ColorProperty, fade);
        }

        public void ChangeButtonColorOnMouseEnter(object sender, MouseEventArgs e)
        {

            Border border = (Border)sender;

            if (SelectedPage == (string)border.Tag) return;

            FadeButtonColor(border, "In");

        }

        public void ChangeButtonColorOnMouseLeave(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;

            if (SelectedPage == (string)border.Tag) return;

            FadeButtonColor(border, "Out");
        }

        public void NavButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            string ButtonName = (string)button.Tag;

            if (SelectedPage == ButtonName) return;

            SelectedPage = ButtonName;

            FadeButtonColor((Border)currentNavButton, "Out");

            currentNavButton = VisualTreeHelper.GetParent(button);

            ChangePage();
        }
    }
}