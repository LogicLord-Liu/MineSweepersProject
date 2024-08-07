using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// HomeScreen.xaml 的交互逻辑
    /// </summary>
    public partial class HomeScreen : Window
    {
        public HomeScreen()
        {
            InitializeComponent();
        }
        // Set the button to enter the game screen and exit the game
        private void gameButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the game screen
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        // Set the button to enter the instruction screen
        private void instructionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Assign the link to the Wikipedia page for Minesweeper
            string url = "https://en.wikipedia.org/wiki/Minesweeper_(video_game)";
            try
            {
                // Try to open the link in the default browser
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // If the link cannot be opened, display an error message
                MessageBox.Show("Can not open instructions: " + ex.Message);
            }
        }
        // Set the button to exit the game
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the application
            Application.Current.Shutdown();
        }

        private void rankingButton_Click(object sender, RoutedEventArgs e)
        {
            RankingScreen rankingScreen = new RankingScreen();
            rankingScreen.Show();
            this.Close();
        }
    }
}
