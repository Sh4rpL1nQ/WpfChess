using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class GameOverEventArgs : EventArgs
    {
        private GameOver gameOver;

        public GameOverEventArgs(GameOver gameOver)
        {
            this.gameOver = gameOver;
        }

        public GameOver GameOver
        {
            get { return gameOver; }
            set { gameOver = value; }
        }
    }

    public enum GameOver
    {
        None = 0,
        Checkmate = 1,
        Patt = 2,
        Time
    }
}
