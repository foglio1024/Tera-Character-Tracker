using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TCTUI;

namespace Tera
{
    /// <summary>
    /// Logica di interazione per MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();

            UI.CharView = this.chView;
            UI.CharListContainer = this.accounts;

            switch (TCTData.TCTProps.Theme)
            {
                case TCTData.Enums.Theme.Light:
                    accGrid.Background = new SolidColorBrush(TCTData.Colors.LightTheme_Card);
                    accGrid.Effect = TCTData.Shadows.LightThemeShadow;
                    break;
                case TCTData.Enums.Theme.Dark:
                    accGrid.Background = new SolidColorBrush(TCTData.Colors.DarkTheme_Card);
                    accGrid.Effect = TCTData.Shadows.DarkThemeShadow;
                    break;
                default:
                    break;
            }
        }
        bool detailsExtended = true;
        public bool DetailsExtended
        {
            get
            {
                return detailsExtended;
            }
            set
            {
                detailsExtended = value;
                if (value)
                {
                    chViewDetails.Width = GridLength.Auto;
                }
                else
                {
                    chViewDetails.Width = new GridLength(0);
                }
            }
        }

        public void ToggleDetails()
        {
            if (detailsExtended)
            {
                DetailsExtended = false;
            }
            else
            {
                DetailsExtended = true;
            }
        }
    }
}
