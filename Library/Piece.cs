using Library.Pieces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Library
{
    [XmlInclude(typeof(King))]
    [XmlInclude(typeof(Queen))]
    [XmlInclude(typeof(Knight))]
    [XmlInclude(typeof(Rook))]
    [XmlInclude(typeof(Pawn))]
    [XmlInclude(typeof(Bishop))]
    public abstract class Piece : PropertyChangedBase, ICloneable
    {
        private Color color;
        private string image;

        public string Image
        {
            get { return image; }
            set { RaisePropertyChanged(ref image, value); }
        }

        [XmlIgnore]
        public abstract PieceType PieceType { get; }

        public virtual Color Color
        {
            get { return color; }
            set { RaisePropertyChanged(ref color, value); }
        }

        public Point Point { get; set; }

        public abstract event EventHandler UpgradePiece;

        public List<Point> Directions { get; set; }

        public abstract bool CanBeMovedToSquare(Square square);

        public bool CheckCollision(Square end, Board board)
        {
            var dir = ChooseRightDirection(end.Point);
            if (dir == null) return false;

            if (PieceType != PieceType.Pawn)
            {
                if (end.Piece != null && Color.Equals(end.Piece.Color)) 
                    return true;

                if (board.IsPieceBlocking(this, end, dir)) 
                    return true;
            }
            else
            {
                if (end.Piece != null && dir.PosX == 0) 
                    return true;

                if (end.Piece != null && Color.Equals(end.Piece.Color)) 
                    return true;
            }

            return false;
        }

        public Point ChooseRightDirection(Point end)
        {
            foreach (var dir in Directions)
            {
                if (Point.IsInDirection(end, dir))
                    return dir;
            }

            return null;
        }

        public virtual object Clone()
        {
            return Clone();
        }
    }

    public enum PieceType
    {
        None,
        King,
        Queen,
        Pawn,
        Bishop,
        Knight,
        Rook
    }
}
