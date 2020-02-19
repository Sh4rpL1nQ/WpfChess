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
        private PlayerViewModel winner;

        public Game Chess { get; set; }

        public ApplicationViewModel()
        {
            Chess = new Game();

            PlayerModel1 = new PlayerViewModel(Chess.Board, Color.White);
            PlayerModel1.Player.UserName = "Player1";
            PlayerModel2 = new PlayerViewModel(Chess.Board, Color.Black);
            PlayerModel2.Player.UserName = "Player2";

            PlayerModel1.TimeIsOver += PlayerModel_TimeIsOver; 
            PlayerModel2.TimeIsOver += PlayerModel_TimeIsOver;

            SquareCommand = new ActionCommand(SquareAction);
            AgainCommand = new ActionCommand(AgainAction);

            PlayerModel1.Player.IsMyTurn = true;
        }

        private void PlayerModel_TimeIsOver(object sender, EventArgs e)
        {
            Winner = sender as PlayerViewModel;            
        }

        public PlayerViewModel PlayerModel1 { get; set; }

        public PlayerViewModel PlayerModel2 { get; set; }

        public PlayerViewModel Winner
        {
            get { return winner; }
            set { RaisePropertyChanged(ref winner, value); }
        }

        public Player PlayerAtTurn => PlayerModel1.Player.IsMyTurn ? PlayerModel1.Player : PlayerModel2.Player;

        public ICommand SquareCommand { get; }

        public ICommand AgainCommand { get; }

        public void AgainAction(object sender)
        {

        }

        public void SquareAction(object sender)
        {
            var square = (sender as Square);
            
            if (selectedSquare != null && !square.Point.Equals(selectedSquare.Point))
            {
                if (PlayerAtTurn.Move(selectedSquare.Piece, square))
                {
                    ChangeTurns();
                }

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
                PlayerModel1.Player.IsMyTurn = false;
                PlayerModel2.Player.IsMyTurn = true;
                PlayerModel1.StopTimer();
                PlayerModel2.StartTimer();
            }
             else
            {
                PlayerModel2.Player.IsMyTurn = false;
                PlayerModel1.Player.IsMyTurn = true;
                PlayerModel2.StopTimer();
                PlayerModel1.StartTimer();
            }   
        }
    }
}
