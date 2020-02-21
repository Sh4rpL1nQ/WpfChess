using Library.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Library
{
    public class Board : PropertyChangedBase, ICloneable
    {
        private Color topColor;

        public ObservableCollection<Square> Squares { get; set; }

        public Board()
        {
            Squares = new ObservableCollection<Square>();
        }

        public void MakeBoard()
        {
            Color toggle = Color.White;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares.Add(new Square() { Point = new Point(j, i) });

                    Squares.Last().Color = toggle;

                    if (j != 7)
                        toggle = toggle == Color.White ? Color.Black : Color.White;
                }
            }
        }

        public Color TopColor
        {
            get { return topColor; }
            set { RaisePropertyChanged(ref topColor, value); }
        }

        public Piece IsKingChecked(Color turn)
        {
            var enemyPieces = GetAllPiecesByColor(GetOtherColor(turn));
            var playerKingSquare = Squares.FirstOrDefault(x => x.Point.Equals(GetKing(turn).Point));

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
            {
                if (allDirs.FirstOrDefault(x => x.Equals(p.Point)) != null)
                    return true;
            }

            return false;
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

        public void CheckPiecePromotable(Piece piece)
        {
            if ((piece is Pawn) && (piece.Point.PosY == 0 || piece.Point.PosY == 7))
                OnInitiatePawnPromotion?.Invoke(piece, new EventArgs());
        }

        public event EventHandler OnInitiatePawnPromotion;

        public Color GetOtherColor(Color color)
        {
            return (color == Color.White) ? Color.Black : Color.White;
        }

        public Piece GetKing(Color turn)
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
