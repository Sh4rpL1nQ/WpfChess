using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Library
{
    public class Player : PropertyChangedBase
    {
        private Board board;
        private bool isMyTurn;
        private bool showPossibleMoves;

        public string UserName { get; set; }

        public Color Color { get; set; }

        public bool IsMyTurn
        {
            get { return isMyTurn; }
            set { RaisePropertyChanged(ref isMyTurn, value); }
        }

        public bool ShowPossibleMoves
        {
            get { return showPossibleMoves; }
            set { RaisePropertyChanged(ref showPossibleMoves, value); }
        }

        public ObservableCollection<Piece> LostPieces { get; set; }

        public Player(Board board, Color color)
        {
            this.board = board;
            Color = color;

            LostPieces = new ObservableCollection<Piece>();
        }

        public bool Move(Piece piece, Square end)
        {
            if (piece.CanBeMovedToSquare(end))
            {
                if (piece.CheckCollision(end, board)) return false;

                var clonedBoard = CheckPredictionBoard(piece, end);
                if (clonedBoard.IsKingChecked(this))
                    return false;

                var p = board.ShiftPiece(piece, end);
                if (p != null)
                    LostPieces.Add(p);

                return true;
            }

            return false;
        }

        public void PossibleMoves(Piece piece)
        {
            var res = new List<Square>();
            foreach (var dir in piece.Directions)
            {
                var allMoves = piece.Point.AllMovesWithinDirection(dir);
                foreach (var end in allMoves)
                {
                    var square = board.Squares.FirstOrDefault(x => x.Point.Equals(end));
                    if (piece.CanBeMovedToSquare(square))
                        if (!piece.CheckCollision(square, board))
                        {
                            var clonedBoard = CheckPredictionBoard(piece, square);
                            if (!clonedBoard.IsKingChecked(this))
                            {
                                square.IsPossibileSquare = true;
                            }
                        }
                }
            }
        }

        private Board CheckPredictionBoard(Piece piece, Square end)
        {
            var clone = board.Clone() as Board;
            clone.ShiftPiece(piece, end);
            return clone;
        }
    }
}
