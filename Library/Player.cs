using Library.Pieces;
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
        private Color color;

        public string UserName { get; set; }

        public Color Color
        {
            get { return color; }
            set  { RaisePropertyChanged(ref color, value); }
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
            foreach (var square in CalcPossibleMoves(piece))
            {
                if (square.Point.Equals(end.Point))
                {
                    if (piece is Rook)
                    {
                        var squares = CanCastle(piece);
                        if (squares != null && squares.Count != 0)
                            if (squares[0].Point.Equals(end.Point))
                                OnCastlePossible?.Invoke(squares, new EventArgs());
                            
                    }
                    var p = board.ShiftPiece(piece, end);
                    board.CheckPiecePromotable(piece);
                    piece.IsFirstMove = false;
                    if (p != null)
                        OnPieceRemoved?.Invoke(p, new EventArgs());

                    return true;
                }
            }

            return false;
        }

        public event EventHandler OnCastlePossible;

        public List<Square> CanCastle(Piece rook)
        {
            var king = board.GetKing(rook.Color);
            var squares = new List<Square>();
            if (rook.IsFirstMove && king.IsFirstMove)
            {
                var clone = board.Clone() as Board;
                var dir = king.ChooseRightDirection(rook.Point);
                var allMoves = king.Point.AllMovesWithinDirection(rook.Point, dir).Take(2);
                foreach (var move in allMoves)
                {
                    var end = clone.Squares.FirstOrDefault(x => x.Point.Equals(move));
                    clone.ShiftPiece(clone.GetKing(king.Color), end);
                    if (clone.IsKingChecked(rook.Color) != null) 
                        return null;

                    squares.Add(board.Squares.FirstOrDefault(x => x.Point.Equals(move)));
                }

                return squares;
            }

            return null;
        }

        public void ExecuteCastle(List<Square> square)
        {
            var king = board.GetKing(Color);
            var clone = CheckPredictionBoard(king, square[1]);
            if (clone.IsKingChecked(king.Color) == null)
            {
                board.Squares.FirstOrDefault(x => x.Point.Equals(king.Point)).Piece = null;
                square[1].Piece = king;
            }            
        }

        public event EventHandler OnPieceRemoved;

        public GameOver IsCheckMate()
        {
            var king = board.GetKing(Color);
            var enemyPiece = board.IsKingChecked(Color);
            var moves = CalcPossibleMoves(king);
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
                {
                    if (CalcPossibleMoves(piece).Count != 0)
                        return GameOver.None;
                }

                return GameOver.Stalemate;
            }
        }

        public void PossibleMoves(Piece piece)
        {
            var all = CalcPossibleMoves(piece);
            foreach (var square in all)
                if (ShowPossibleMoves)
                    square.IsPossibileSquare = true;
        }

        public List<Square> CalcPossibleMoves(Piece piece)
        {
            var res = new List<Square>();
            foreach (var dir in piece.Directions)
            {
                var allMoves = piece.Point.AllMovesWithinDirection(dir);
                foreach (var end in allMoves)
                {
                    var square = board.Squares.FirstOrDefault(x => x.Point.Equals(end));
                    if (piece.CanMoveWithoutColliding(square, board))
                    {
                        var clonedBoard = CheckPredictionBoard(piece, square);
                        if (clonedBoard.IsKingChecked(Color) == null)
                            res.Add(square);
                    }
                }
            }

            return res;
        }

        private Board CheckPredictionBoard(Piece piece, Square end)
        {
            var clone = board.Clone() as Board;
            clone.ShiftPiece(piece, end);
            return clone;
        }
    }
}
