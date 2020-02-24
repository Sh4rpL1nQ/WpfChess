using Library.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Library
{
    public class Board : PropertyChangedBase, ICloneable
    {
        private Color topColor;
        public static int BoardSize = 8;
        public List<Move> PossibleMoves { get; set; }

        public ObservableCollection<Square> Squares { get; set; }

        public Board()
        {
            MakeBoard();

            PossibleMoves = new List<Move>();
        }

        private void MakeBoard()
        {
            Squares = new ObservableCollection<Square>();
            Color toggle = Color.White;
            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    Squares.Add(new Square() { Point = new Point(x, y) });
                    Squares.Last().Color = toggle;
                    if (x != BoardSize - 1)
                        toggle = toggle == Color.White ? Color.Black : Color.White;
                }
            }
        }

        public List<Square> CenterSquares => Squares.Where(x =>
            (x.Point.PosY == 3 && x.Point.PosX == 3) ||
            (x.Point.PosY == 3 && x.Point.PosX == 4) ||
            (x.Point.PosY == 4 && x.Point.PosX == 4) ||
            (x.Point.PosY == 4 && x.Point.PosX == 3)
        ).ToList();

        public Color TopColor
        {
            get { return topColor; }
            set { RaisePropertyChanged(ref topColor, value); }
        }

        public Piece IsKingChecked(Color turn)
        {
            var enemyPieces = GetAllPiecesByColor(GetOtherColor(turn));
            var playerKingSquare = Squares.FirstOrDefault(x => x.Point.Equals(GetKingByColor(turn).Point));

            foreach (var enemyPiece in enemyPieces)
            {
                if (enemyPiece.CanBeMovedToSquare(playerKingSquare))
                {
                    var dir = enemyPiece.ChooseRightDirection(playerKingSquare.Point);
                    if (!IsPieceBlocking(enemyPiece, playerKingSquare, dir))
                        return enemyPiece;
                }
            }

            return null;
        }

        public bool IsPieceBlocking(Piece piece, Square end, Point dir)
        {
            var allPieces = GetAllPieces().Where(x => x != null && !x.Point.Equals(piece.Point));
            var allDirs = piece.Point.AllMovesWithinDirection(end.Point, dir);
            allDirs.Remove(allDirs.Last());

            foreach (var p in allPieces)
                if (allDirs.FirstOrDefault(x => x.Equals(p.Point)) != null)
                    return true;

            return false;
        }

        public Color GetOtherColor(Color color)
        {
            return (color == Color.White) ? Color.Black : Color.White;
        }

        public Piece GetKingByColor(Color turn)
        {
            var allPieces = GetAllPiecesByColor(turn);

            return allPieces.FirstOrDefault(x => x is King);
        }

        public List<Piece> GetAllPiecesByColor(Color color)
        {
            return GetAllPieces().Where(x => x?.Color == color).ToList();
        }

        public List<Piece> GetAllPieces()
        {
            var pieces = new List<Piece>();

            foreach (var s in Squares)
                pieces.Add(s.Piece);

            return pieces;
        }

        public Board PredictBoard(Piece piece, Square end)
        {
            var clone = Clone() as Board;
            clone.ShiftPiece(piece, end);
            return clone;
        }

        public Piece ShiftPiece(Piece piece, Square end)
        {
            var startPiece = piece.Clone() as Piece;
            var endPiece = Squares.FirstOrDefault(x => x.Point.Equals(end.Point))?.Piece;
            var saveEndPiece = endPiece?.Clone() as Piece;

            Squares.FirstOrDefault(x => x.Point.Equals(end.Point)).Piece = Squares.FirstOrDefault(x => x.Point.Equals(piece.Point)).Piece;
            Squares.FirstOrDefault(x => x.Point.Equals(startPiece.Point)).Piece = null;

            return saveEndPiece;
        }

        public void ShowPossibleMoves(Piece piece)
        {
            foreach (var move in CalcPossibleMoves(piece))
            {
                move.End.IsPossibileSquare = true;
                PossibleMoves.Add(move);
            }

        }

        public List<Move> CalcPossibleMoves(Piece piece)
        {
            var res = new List<Move>();
            foreach (var dir in piece.Directions)
            {
                var allMoves = piece.Point.AllMovesWithinDirection(dir);
                foreach (var end in allMoves)
                {
                    var square = Squares.FirstOrDefault(x => x.Point.Equals(end));
                    if (piece.CanMoveWithoutColliding(square, this))
                    {
                        var clonedBoard = PredictBoard(piece, square);
                        if (clonedBoard.IsKingChecked(piece.Color) == null)
                        {
                            var move = new Move(this, piece, square);
                            move.OnCastlePossible += Move_OnCastlePossible;
                            move.OnInitiatePawnPromotion += Move_OnInitiatePawnPromotion;
                            move.OnPieceCaptured += Move_OnPieceCaptured;
                            res.Add(move);
                        }
                    }
                }
            }

            return res;
        }

        #region Events
        public event EventHandler OnCastlePossible;
        public event EventHandler OnInitiatePawnPromotion;
        public event EventHandler OnPieceCaptured;

        private void Move_OnPieceCaptured(object sender, EventArgs e)
        {
            OnPieceCaptured?.Invoke(sender, e);
        }

        private void Move_OnInitiatePawnPromotion(object sender, EventArgs e)
        {
            OnInitiatePawnPromotion?.Invoke(sender, e);
        }

        private void Move_OnCastlePossible(object sender, EventArgs e)
        {
            OnCastlePossible?.Invoke(sender, e);
        }

        #endregion

        public object Clone()
        {
            Board board = new Board();
            board.Squares = new ObservableCollection<Square>();
            foreach (var s in Squares)
                board.Squares.Add(s.Clone() as Square);

            return board;
        }
    }
}
