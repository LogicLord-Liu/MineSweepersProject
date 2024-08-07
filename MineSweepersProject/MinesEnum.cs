using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweepersProject
{
    // enum for the back state of the cell
    public enum BackState
    {
        MINE = -1,
        BLACK = 0
    }
    // enum for the fore state of the cell
    public enum ForeState
    {
        NONE,
        NORMAL,
        FLAG
    }
    // enum for the current state of game  
    public enum GameState
    {
        NONE, // no game
        STOP, // game stop
        START, // game start
        PAUSE // game pause
    }
    // enum for the game level
    public enum GameLevel
    {
        EASY, // easy level
        MEDIUM, // medium level
        HARD // hard level
    }
    public class Game
    {
        public int _row;
        public int _col;
        public int _mineCount;
        public Game(int row, int col, int mineCount)
        {
            _row = row;
            _col = col;
            _mineCount = mineCount;
        }
    }
}
