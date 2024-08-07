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
using System.Windows.Shapes;

namespace MineSweepersProject
{
    /// <summary>
    /// Interaction logic for RankingScreen.xaml
    /// </summary>
    public partial class RankingScreen : Window
    {
        public RankingScreen()
        {
            InitializeComponent();
        }

        private void ReturnHomeScreen_ClickButton(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen();
            homeScreen.Show();
            this.Close();
        }
    }
}
