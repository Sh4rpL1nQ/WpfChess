using Library.Pieces;
using System;
using System.Collections.Generic;
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

        [XmlIgnore]
        public string Image
        {
            get { return image; }
            set { RaisePropertyChanged(ref image, value); }
        }

        public virtual Color Color
        {
            get { return color; }
            set { RaisePropertyChanged(ref color, value); }
        }

        [XmlIgnore]
        public virtual int Weight { get; }

        public Point Point { get; set; }

        [XmlIgnore]
        public List<Point> Directions { get; set; }

        public virtual bool PartOfTopBoard { get; set; }

        public bool IsFirstMove { get; set; } = true;

        public abstract bool CanBeMovedToSquare(Square square);

        private bool CheckCollision(Square end, Board board)
        {
            var dir = ChooseRightDirection(end.Point);
            if (dir == null)
                return false; 

            if (end.Piece != null && Color.Equals(end.Piece.Color))
                return true;

            if (board.IsPieceBlocking(this, end, dir))
                return true;

            return false;
        }

        public bool CanMoveWithoutColliding(Square end, Board board)
        {
            if (CanBeMovedToSquare(end))
                return !CheckCollision(end, board);

            return false;
        }

        public Point ChooseRightDirection(Point end)
        {
            foreach (var dir in Directions)
                if (Point.IsInDirection(end, dir))
                    return dir;

            return null;
        }

        public virtual object Clone()
        {
            return Clone();
        }
    }
}
