using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Library;

namespace Chess.ViewModels
{
    public class ApplicationViewModel : PropertyChangedBase
    {
        private Square selectedSquare;
        private GameOverViewModel gameOverModel;

        public Game Chess { get; set; }

        public ApplicationViewModel()
        {
            Chess = new Game();

            PlayerModel1 = new PlayerViewModel(Chess.Board, Color.White);
            PlayerModel1.Player.UserName = "Player1";
            PlayerModel2 = new PlayerViewModel(Chess.Board, Color.Black);
            PlayerModel2.Player.UserName = "Player2";

            PlayerModel1.Player.OnGameOver += Player_OnGameOver;
            PlayerModel2.Player.OnGameOver += Player_OnGameOver;
            PlayerModel1.TimeIsOver += Player_OnGameOver; 
            PlayerModel2.TimeIsOver += Player_OnGameOver;

            SquareCommand = new ActionCommand(SquareAction);

            PlayerModel1.Player.IsMyTurn = true;
        }

        private void Player_OnGameOver(object sender, EventArgs e)
        {
            var winner = sender as Player;
            GameOverModel = new GameOverViewModel(winner.Color == PlayerModel1.Player.Color ? PlayerModel1.Player : PlayerModel2.Player, (e as GameOverEventArgs).GameOver);
            PlayerModel1.StopTimer();
            PlayerModel2.StopTimer();
            GameOverModel.OnRetry += GameOverModel_OnRetry;
        }

        private void GameOverModel_OnRetry(object sender, EventArgs e)
        {
            GameOverModel = null;
            Chess.Reset();
            PlayerModel1.Reset();
            PlayerModel2.Reset();
            PlayerModel1.Player.IsMyTurn = true;
        }

        public PlayerViewModel PlayerModel1 { get; set; }

        public PlayerViewModel PlayerModel2 { get; set; }

        public GameOverViewModel GameOverModel
        {
            get { return gameOverModel; }
            set { RaisePropertyChanged(ref gameOverModel, value); }
        }

        public Player PlayerAtTurn => PlayerModel1.Player.IsMyTurn ? PlayerModel1.Player : PlayerModel2.Player;

        public ICommand SquareCommand { get; }

        public void SquareAction(object sender)
        {
            var square = (sender as Square);
            
            if (selectedSquare != null && !square.Point.Equals(selectedSquare.Point))
            {
                if (PlayerAtTurn.Move(selectedSquare.Piece, square))
                    ChangeTurns();

                ClearSelections();

                return;
            }

            if (square.Piece == null) return;

            ClearSelections();

            if (square.Piece.Color == PlayerAtTurn.Color)
            {
                square.IsSelected = true;
                selectedSquare = square;

                if (PlayerAtTurn.ShowPossibleMoves)
                    PlayerAtTurn.PossibleMoves(selectedSquare.Piece);
            }      
        }

        private void ClearSelections()
        {
            foreach (var s in Chess.Board.Squares)
            {
                s.IsSelected = false;
                s.IsPossibileSquare = false;
            }                

            selectedSquare = null;
        }

        public void ChangeTurns()
        {
            if (PlayerModel1.Player.IsMyTurn)
            {
                PlayerModel1.StopTimer();
                PlayerModel2.StartTimer();
                PlayerModel1.Player.IsMyTurn = false;
                PlayerModel2.Player.IsMyTurn = true;                
            }
             else
            {
                PlayerModel2.StopTimer();
                PlayerModel1.StartTimer();
                PlayerModel2.Player.IsMyTurn = false;
                PlayerModel1.Player.IsMyTurn = true;                
            }   
        }
    }
}
