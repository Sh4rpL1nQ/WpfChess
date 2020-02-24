using Library;
using System;
using System.Windows.Input;

namespace Chess.ViewModels
{
    public class GameOverViewModel : PropertyChangedBase
    {
        public GameOverViewModel(Player winner, Player looser, GameOver gameOver)
        {
            RetryCommand = new ActionCommand(RetryAction);

            Winner = winner.UserName;
            Looser = looser.UserName;
            GameOver = gameOver;
        }

        public string Winner { get; set; }

        public string Looser { get; set; }

        public GameOver GameOver { get; set; }


        public event EventHandler OnRetry;

        public string GameOverMessage => GameOver switch
        {
            GameOver.Checkmate => Winner + " has won, because of checkmate!",
            GameOver.Time => Winner + " has won, because the " + Looser + " ran out of time",
            GameOver.Stalemate => "Stalemate! The king of " + Looser + " has no valid move, but isn't checked.",
            GameOver.Draw => "Both have agreed, that it should be a draw.",
            GameOver.Surrender => Winner + " has won, because the " + Looser + " has surrendered",
            _ => string.Empty
        };

        public ICommand RetryCommand { get; }

        public void RetryAction(object sender)
        {
            OnRetry?.Invoke(this, new EventArgs());
        }
    }
}
