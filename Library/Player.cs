using Library.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Library
{
    public class Player : PropertyChangedBase
    {
        private Board board;
        private bool isMyTurn;
        private bool showPossibleMoves;
        private Color color;

        public string UserName { get; set; }

        public Color Color
        {
            get { return color; }
            set { RaisePropertyChanged(ref color, value); }
        }

        public bool IsMyTurn
        {
            get { return isMyTurn; }
            set
            {
                RaisePropertyChanged(ref isMyTurn, value);
                if (isMyTurn)
                {
                    var gameOver = IsCheckMate();
                    switch (gameOver)
                    {
                        case GameOver.Checkmate:
                            OnGameOver?.Invoke(this, new GameOverEventArgs(gameOver));
                            break;
                        case GameOver.Stalemate:
                            OnGameOver?.Invoke(this, new GameOverEventArgs(gameOver));
                            break;
                        default: break;
                    }
                }
            }
        }

        public bool ShowPossibleMoves
        {
            get { return showPossibleMoves; }
            set { RaisePropertyChanged(ref showPossibleMoves, value); }
        }

        public int MoveCounter { get; set; } = 0;

        public event EventHandler OnGameOver;

        public ObservableCollection<Piece> LostPieces { get; set; }

        public Player(Board board, Color color)
        {
            this.board = board;
            Color = color;

            showPossibleMoves = true;

            LostPieces = new ObservableCollection<Piece>();
        }

        public bool Move(Piece piece, Square end)
        {
            foreach (var move in board.CalcPossibleMoves(piece))
            {
                if (move.IsEndPosition(end))
                {
                    move.Execute();

                    return true;
                }
            }

            return false;
        }

        public void ExecuteCastle(List<Square> square)
        {
            var king = board.GetKingByColor(Color);
            var clone = board.PredictBoard(king, square[1]);
            if (clone.IsKingChecked(king.Color) == null)
            {
                board.Squares.FirstOrDefault(x => x.Point.Equals(king.Point)).Piece = null;
                square[1].Piece = king;
            }

            king.IsFirstMove = false;
        }

        public GameOver IsCheckMate()
        {
            var king = board.GetKingByColor(Color);
            var enemyPiece = board.IsKingChecked(Color);
            var moves = board.CalcPossibleMoves(king);
            if (enemyPiece != null)
            {
                var enemyPieces = board.GetAllPiecesByColor(Color).Where(x => !(x is King));

                //Can be blocked?
                foreach (var piece in enemyPieces)
                {
                    var pieceMoves = king.Point.AllMovesWithinDirection(enemyPiece.Point, king.ChooseRightDirection(enemyPiece.Point));
                    foreach (var end in pieceMoves)
                    {
                        var square = board.Squares.FirstOrDefault(x => x.Point.Equals(end));
                        if (piece.CanMoveWithoutColliding(square, board))
                            return GameOver.None;
                    }
                }

                //Can be moved?
                if (moves.Count != 0)
                    return GameOver.None;

                return GameOver.Checkmate;
            }
            else
            {
                foreach (var piece in board.GetAllPiecesByColor(Color))
                    if (board.CalcPossibleMoves(piece).Count != 0)
                        return GameOver.None;

                return GameOver.Stalemate;
            }
        }
    }
}
