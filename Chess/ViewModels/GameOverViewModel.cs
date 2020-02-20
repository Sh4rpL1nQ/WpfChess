using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Chess.ViewModels
{
    public class GameOverViewModel : PropertyChangedBase
    {
        public GameOverViewModel(Player player, GameOver gameOver)
        {
            RetryCommand = new ActionCommand(RetryAction);

            Winner = player.UserName;
            GameOver = gameOver;
        }

        public string Winner { get; set; }

        public GameOver GameOver { get; set; }


        public event EventHandler OnRetry;

        public ICommand RetryCommand { get; }

        public void RetryAction(object sender)
        {
            OnRetry?.Invoke(this, new EventArgs());
        }
    }
}
