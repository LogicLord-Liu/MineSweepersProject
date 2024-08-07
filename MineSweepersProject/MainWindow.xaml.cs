using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Threading;
using System.Xml.Serialization;

namespace MineSweepersProject
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Timer control for the game
        /// </summary>
        private DispatcherTimer _timer; // Timer for the game
        private DateTime _startTime; // Time played
        /// <summary>
        /// Initialize the number of mines and the size of the grid for the game
        /// </summary>
        System.Drawing.Size _cellSize = new System.Drawing.Size(32, 32);
        /// <summary>
        /// Initialize the difficulty level and Data for the game
        /// </summary>
        private GameLevel level = GameLevel.EASY;
        Game _game = null; // Game object
        // Set the difficulty array for the game
        int[,] _backData = null;
        Image[,] _backImages = null;
        private GameState _gameState = GameState.NONE; // Game state
        Random rnd = new Random(); // Random number generator
        // Set fore state
        int[,] _foreData = null; // Set the data of the fore
        Image[,] _foreImage = null; // Set the image of the fore


        // Main Function of the game
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen; // Set the window to the center of the screen when it is opened.
            InitializeTimer();
            InitializeDifficulty();
            InitializeMinesweeperGrid();

            // Start the timer when the game starts.
            _startTime = DateTime.Now;
            timeTextBox.Text = "Time: " + FormatTime(_startTime.TimeOfDay);
            _timer.Start();
        }
        //Begin to initialize the game
        private void InitializeMinesweeperGrid()
        {
            gameStateUI.Text = "Started";
            _gameState = GameState.START;
            // On the base of the difficulty level, initialize the grid
            switch (level)
            {
                case GameLevel.EASY:
                    _game = new Game(10,10,10);
                    break;
                case GameLevel.MEDIUM:
                    _game = new Game(16,16,40);
                    break;
                case GameLevel.HARD:
                    _game = new Game(16,30,99);
                    break;
            }
            // Add a counter for the number of mines.
            mineCountTextBox.Text += _game._mineCount.ToString();
            mineCanvas.Width = _game._col * _cellSize.Width; // Set the width of the back canvas
            mineCanvas.Height = _game._row * _cellSize.Height; // Set the height of the back canvas
            SetmineCanvasBackground(); // Set the background color of the canvas

            foreStateCanvas.Width = _game._col * _cellSize.Width; // Set the width of the fore canvas
            foreStateCanvas.Height = _game._row * _cellSize.Height; // Set the height of the fore canvas

            this.Width = mineCanvas.Width + 500; // Set the width of the window
            this.Height = mineCanvas.Height + 10 + 175; // Set the height of the window

            // Gain the size of the window.
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;

            _backData = new int[_game._row, _game._col]; // Initialize the back data array
            _backImages = new Image[_game._row, _game._col]; // Initialize the back image array

            _foreData = new int[_game._row, _game._col]; // Initialize the fore data array
            _foreImage = new Image[_game._row, _game._col]; // Initialize the fore image array
            // Initialize the date of the background and foreground.
            for (int i = 0; i < _game._row; i++)
            {
                for(int j = 0; j < _game._col; j++)
                {
                    _backData[i, j] = (int)BackState.BLACK; // Set the default value of the back data to 0
                    _foreData[i, j] = (int)ForeState.NORMAL; // Set the default value of the fore data to 0
                }
            }
            // Clear the grid Now
            mineCanvas.Children.Clear();
            foreStateCanvas.Children.Clear();

            SetRandomMines();
        }
        // Set the background color of the game
        private void SetmineCanvasBackground()
        {
            // Set the background color of the canvas by using RGB
            byte r = 255;
            byte g = 245;
            byte b = 247;
            // Create a color object
            Color color = Color.FromRgb(r, g, b);
            // Create SolidColorBrush and set in the canvas
            SolidColorBrush brush = new SolidColorBrush(color);

            mineCanvas.Background = brush;
        }
        // Start to initialize the mines
        private void SetRandomMines()
        {
            for (int k = 0; k < _game._mineCount; k++)
            {
                // Find the position to put the mine
                int blackNumber = 0; // Count the number of black
                for (int x = 0; x < _game._row; x++)
                {
                    for (int y = 0; y < _game._col; y++)
                    {
                        if (_backData[x, y] == (int)BackState.BLACK)
                        {
                            blackNumber++;
                        }
                    }
                }
                int index = rnd.Next(1, blackNumber); // Get a random number between 1 and blackNumber
                blackNumber = 0;
                for (int x = 0; x < _game._row; x++)
                {
                    for (int y = 0; y < _game._col; y++)
                    {
                        if (_backData[x, y] == (int)BackState.BLACK)
                        {
                            blackNumber++;
                        }
                        else if(_backData[x, y] == (int)BackState.MINE)
                        {
                            continue;
                        }
                        if (index == blackNumber)
                        {
                            _backData[x, y] = (int)BackState.MINE; // Set the random position to be a mine position
                        }
                    }
                }
            }
            CountMinesAround();
        }
        // Caculate the number of mines around the cell
        private void CountMinesAround()
        {
            for (int x = 0; x < _game._row; x++)
            {
                for (int y = 0; y < _game._col; y++)
                {
                    if ( _backData[x, y] != (int)BackState.MINE)
                    {
                        if (x - 1 >= 0 && y - 1 >= 0 && _backData[x - 1, y - 1] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (x - 1 >= 0 && _backData[x - 1, y] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (x - 1 >= 0 && y + 1 < _game._col && _backData[x - 1, y + 1] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (y + 1 < _game._col && _backData[x, y + 1] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (y - 1 >= 0 && _backData[x, y - 1] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (x + 1 < _game._row && y - 1 >= 0 && _backData[x + 1, y - 1] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (x + 1 < _game._row && _backData[x + 1, y] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                        if (x + 1 < _game._row && y + 1 < _game._col && _backData[x + 1, y + 1] == (int)BackState.MINE)
                        {
                            _backData[x, y]++;
                        }
                    }
                }
            }
            // Call the function to create the foreGrid and backGrid grid
            CreateBackGrid();
            CreateForeGrid();
        }
        // Initialize the Images
        private void CreateBackGrid()
        {
            mineCanvas.Children.Clear();
            for (int row = 0; row < _game._row; row++)
            {
                for(int col = 0; col < _game._col; col++)
                {
                    _backImages[row, col] = new Image();
                    if (_backData[row, col] == (int)BackState.MINE)
                    {
                        // Create the image of the mines
                        BitmapImage bitmap = new BitmapImage(new Uri("E:\\编程课代码\\MineSweepersProject\\MineSweepersProject\\Image\\mines.png"));
                        _backImages[row, col].Source = bitmap;
                    }
                    else if( _backData[row, col] == (int)BackState.BLACK)
                    {
                        // Create the image of the space
                        BitmapImage bitmap = new BitmapImage(new Uri("E:\\编程课代码\\MineSweepersProject\\MineSweepersProject\\Image\\space.png"));
                        _backImages[row, col].Source = bitmap;
                    }
                    else
                    {
                        // Create the image of the numbers
                        BitmapImage bitmap = new BitmapImage(new Uri($"E:\\编程课代码\\MineSweepersProject\\MineSweepersProject\\Image\\numbers\\{_backData[row,col]}.png"));
                        _backImages[row, col].Source = bitmap;
                    }
                    // Set the position of the image
                    _backImages[row, col].SetValue(TopProperty,(double)row * _cellSize.Height);
                    _backImages[row, col].SetValue(LeftProperty,(double)col * _cellSize.Width);

                    mineCanvas.Children.Add(_backImages[row, col]);
                }
            }
        }
        private void CreateForeGrid()
        {
            foreStateCanvas.Children.Clear();
            for (int row = 0; row < _game._row; row++)
            {
                for (int col = 0; col < _game._col; col++)
                {
                    _foreImage[row, col] = new Image();
                    
                    // Create the image of the foreImage.
                    BitmapImage bitmap = new BitmapImage(new Uri("E:\\编程课代码\\MineSweepersProject\\MineSweepersProject\\Image\\foreImg.png"));
                    _foreImage[row, col].Source = bitmap;
                    
                    // Set the position of the image.
                    _foreImage[row, col].SetValue(TopProperty, (double)row * _cellSize.Height);
                    _foreImage[row, col].SetValue(LeftProperty, (double)col * _cellSize.Width);

                    foreStateCanvas.Children.Add(_foreImage[row, col]);
                }
            }
        }
        // Start click event for the foreground state canvas
        private void foreStateCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_gameState == GameState.NONE || _gameState == GameState.STOP)
                return;
            Point point = e.MouseDevice.GetPosition(foreStateCanvas);
            // Acquire the position of the cell
            int valueX = (int)point.Y / _cellSize.Height;
            int valueY = (int)point.X / _cellSize.Width;
            // If the cell is not a mine, remove images and the game continues
            if (_backData[valueX, valueY] > (int)BackState.BLACK)
            {
                // Remove the image from the canvas when the cell is clicked.
                if (foreStateCanvas.Children.Contains(_foreImage[valueX, valueY])) // Check if the image is in the canvas.
                {
                    foreStateCanvas.Children.Remove(_foreImage[valueX, valueY]); // Remove the fore image from the canvas.
                    _foreData[valueX, valueY] = (int)ForeState.NONE;
                }
            }
            // If the cell is a mine, the game is over
            if(_backData[valueX, valueY] == (int)BackState.MINE)
            {
                // Remove the image from the canvas when the cell is clicked.
                if (foreStateCanvas.Children.Contains(_foreImage[valueX, valueY])) // Check if the image is in the canvas.
                {
                    foreStateCanvas.Children.Remove(_foreImage[valueX, valueY]); // Remove the fore image from the canvas.
                    _foreData[valueX, valueY] = (int)ForeState.NONE;
                }
                // Decide the outcome of the game.
                if (!IsWin())
                {
                    gameStateUI.Text = "Lose";
                    // Modify the state of the game when click the mine.
                    _gameState = GameState.STOP;
                    // Modify the style of the message box.
                    MessageBox.Show(
                        messageBoxText: "You Lose, Do You Want To Try Again ? ",
                        caption: "Game Over",
                        button: MessageBoxButton.YesNo,
                        icon: MessageBoxImage.Error
                    );
                    _timer.Stop();
                }
            }
            // If the cell is a black cell, Call the OpenBalck method.
            if (_backData[valueX, valueY] == (int)BackState.BLACK)
            {
                this.OpenBalck(valueX, valueY);
            }
        }
        // Add the function to judge if the game is over
        private Boolean IsWin()
        {
            bool flag = true;
            for(int x = 0; x < _game._row; x++)
            {
                for( int y = 0; y < _game._col; y++)
                {
                    if( _backData[x, y] == (int)BackState.MINE && _foreData[x, y] == (int)ForeState.NONE)
                    {
                        flag = false;
                        return flag;
                    }
                }
            }
            if(_game._mineCount == 0)
            {
                for(int x = 0; x < _game._row; x++)
                {
                    for(int y = 0; y < _game._col; y++)
                    {
                        if(_foreData[x, y] == (int)ForeState.FLAG && _backData[x, y] != (int)BackState.MINE)
                        {
                            flag = false;
                            return flag;
                        }
                    }
                }
            }
            else
            {
                flag = false;
            }

            return flag;
        }
        // Deal with the open black cell by recursion
        private void OpenBalck(int x,int y)
        {
            // Make sure this position is black.
            if (_foreData[x,y] == (int)ForeState.NORMAL && _backData[x,y] == (int)BackState.BLACK)
            {
                // Open the cell when it's clicked first
                if (foreStateCanvas.Children.Contains(_foreImage[x, y])) // Check if the image is in the canvas.
                {
                    _foreData[x, y] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x, y]); // Remove the fore image from the canvas.
                }
                if(y - 1 >= 0)
                {
                    OpenBalck(x, y - 1);
                }
                if(x - 1 >= 0 && y - 1 >= 0)
                {
                    OpenBalck(x - 1, y - 1);
                }
                if(x - 1 >= 0)
                {
                    OpenBalck(x - 1, y);
                }
                if(x - 1 >= 0 && y + 1 <= _game._col - 1)
                {
                    OpenBalck(x - 1, y + 1);
                }
                if(x + 1 <= _game._row - 1 && y - 1 >= 0)
                {
                    OpenBalck(x + 1, y - 1);
                }
                if(x + 1 <= _game._row - 1 && y + 1 <= _game._col - 1)
                {
                    OpenBalck(x + 1, y + 1);
                }
                if(x + 1 <= _game._row - 1)
                {
                    OpenBalck(x + 1, y);
                }
                if(y + 1 <= _game._col - 1)
                {
                    OpenBalck(x, y + 1);
                }

                this.OpenAroundCell(x, y);
            }
        }
        // Open the cell around the black cell and display the number image.
        private void OpenAroundCell(int x,int y)
        {
            if (y - 1 >= 0 && _backData[x, y - 1] > (int)BackState.BLACK && _foreData[x, y - 1] == (int)ForeState.NORMAL)
            {
                if (foreStateCanvas.Children.Contains(_foreImage[x, y - 1])) // Check if the image is in the canvas.
                {
                    _foreData[x, y - 1] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x, y - 1]); // Remove the fore image from the canvas.
                }
            }
            if (y - 1 >= 0 && x - 1 >= 0)
            {
                if (_backData[x - 1, y - 1] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x - 1, y - 1] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x - 1, y - 1]); // Remove the fore image from the canvas.
                }
            }
            if( x - 1 >= 0)
            {
                if (_backData[x - 1, y] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x - 1, y] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x - 1, y]); // Remove the fore image from the canvas.
                }
            }
            if (x - 1 >= 0 && y + 1 <= _game._col - 1)
            {
                if (_backData[x - 1, y + 1] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x - 1, y + 1] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x - 1, y + 1]); // Remove the fore image from the canvas.
                }
            }
            if( x + 1 <= _game._row - 1 && y - 1 >= 0)
            {
                if (_backData[x + 1, y - 1] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x + 1, y - 1] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x + 1, y - 1]); // Remove the fore image from the canvas.
                }
            }
            if( x + 1 <= _game._row - 1 && y + 1 <= _game._col - 1)
            {
                if (_backData[x + 1, y + 1] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x + 1, y + 1] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x + 1, y + 1]); // Remove the fore image from the canvas.
                }
            }
            if( x + 1 <= _game._row - 1)
            {
                if (_backData[x + 1, y] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x + 1, y] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x + 1, y]); // Remove the fore image from the canvas.
                }
            }
            if(y + 1 <= _game._col - 1)
            {
                if (_backData[x, y + 1] != (int)BackState.MINE) // Check if the image is in the canvas.
                {
                    _foreData[x, y + 1] = (int)ForeState.NONE;
                    foreStateCanvas.Children.Remove(_foreImage[x, y + 1]); // Remove the fore image from the canvas.
                }
            }
        }
        private void foreStateCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_gameState == GameState.NONE || _gameState == GameState.STOP) return;
            Point point = e.MouseDevice.GetPosition(foreStateCanvas);
            // Acquire the position of the cell
            int valueX = (int)point.Y / _cellSize.Height;
            int valueY = (int)point.X / _cellSize.Width;
            // Start to place the flag.
            if (_foreData[valueX, valueY] == (int)ForeState.FLAG) // If click the flag again, remove the flag.
            {
                if(foreStateCanvas.Children.Contains(_foreImage[valueX, valueY]))
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri("E:\\编程课代码\\MineSweepersProject\\MineSweepersProject\\Image\\foreImg.png"));
                    _foreImage[valueX, valueY].Source = bitmapImage;
                    _foreData[valueX, valueY] = (int)ForeState.NORMAL;
                    _game._mineCount++;
                    mineCountTextBox.Text = $"Mines: {_game._mineCount}";
                }
            }else if(_game._mineCount > 0 && _foreData[valueX, valueY] == (int)ForeState.NORMAL)
            {
                if(foreStateCanvas.Children.Contains(_foreImage[valueX, valueY]))
                {
                    BitmapImage bitmapImage = new BitmapImage(new Uri("E:\\编程课代码\\MineSweepersProject\\MineSweepersProject\\Image\\flag.png"));
                    _foreImage[valueX, valueY].Source = bitmapImage;
                    _foreData[valueX, valueY] = (int)ForeState.FLAG;
                    _game._mineCount--;
                    mineCountTextBox.Text = $"Mines: {_game._mineCount}";
                }
            }
            // Judge the game win or lose
            if (IsWin())
            {
                _gameState = GameState.STOP;
                gameStateUI.Text = "You Win";
                for(int x = 0; x < _game._row; x++)
                {
                    for(int y = 0; y < _game._col; y++)
                    {
                        foreStateCanvas.Children.Remove(_foreImage[x, y]);
                    }
                }
                MessageBox.Show(
                    messageBoxText: "Congratulation, you have won the game! Do you want to play again?",
                    caption: "Congratulation",
                    button: MessageBoxButton.OK,
                    icon: MessageBoxImage.Question
                    );
            }
        }
        // Realize the timer functions.
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1); // Set the timer interval to 1 time as one second
            _timer.Tick += Timer_Tick;
            // Initialize don't start the timer
            _timer.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Calculate the elapsed time
            TimeSpan elapsedTime = DateTime.Now - _startTime;
            // Update the UI
            timeTextBox.Text = "Time: " + FormatTime(elapsedTime);
        }
        private string FormatTime(TimeSpan timeSpan)
        {
            // Format the time as mm:ss.fff format
            return timeSpan.ToString(@"mm\:ss\.fff");
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Make sure the timer is stopped when the window is closed
            _timer.Stop();
            base.OnClosing(e);
        }
        // Add a Start timer button
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _gameState = GameState.START;
            // Restart the game
            _startTime = DateTime.Now;
            timeTextBox.Text = "Time: " + FormatTime(_startTime.TimeOfDay);
            _timer.Start();
            //InitializeMinesweeperGrid();
        }
        // Add menu items here
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            // Open a web browser to the official website of the game
            string url = "https://www.saolei123.com/jiaocheng/";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        // Game difficulty level selection buttons
        private void InitializeDifficulty()
        {
            // Default difficulty level is Easy
            easyLevel.IsChecked = true;
        }
        // Just a simple difficulty level selection,if choose easy,the other two will be unchecked
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedItem = sender as MenuItem;
            if (clickedItem != null)
            {
                foreach (MenuItem item in difficultyMenu.Items)
                {
                    item.IsChecked = item == clickedItem;
                }
            }
            gameStateUI.Text = "Started";
            // Start the timer when choose a difficulty level
            _startTime = DateTime.Now;
            timeTextBox.Text = "Time: " + FormatTime(_startTime.TimeOfDay);
            _timer.Start();
        }
        // Add a return button to return to the HomeScreen
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen();
            homeScreen.Show();
            this.Close();
            _timer.Stop();
        }
        // Add a Icon button to as a resart button
        private void Icon_Click(object sender, RoutedEventArgs e)
        {
            // Reset and restart the timer.
            _startTime = DateTime.Now;
            timeTextBox.Text = "Time: " + FormatTime(_startTime.TimeOfDay);
            _timer.Start();
            // Reset the counter
            _game._mineCount = 10;
            mineCountTextBox.Text = $"Mines: {_game._mineCount}";
            // Reset the game state。
            _gameState = GameState.START;
            InitializeMinesweeperGrid();
        }
    }
}
