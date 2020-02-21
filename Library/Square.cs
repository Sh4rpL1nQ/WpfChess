using Library.Pieces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Library
{
    public class Square : PropertyChangedBase, ICloneable
    {
        private Color color;
        private Point point;
        private bool isSelected;
        private Piece piece;
        private bool isPossibileSquare;

        public Point Point
        {
            get { return point; }
            set 
            { 
                point = value; 
                if (Piece != null)
                    Piece.Point = new Point(point.PosX, point.PosY);
            }
        }

        public Color Color
        {
            get { return color; }
            set { RaisePropertyChanged(ref color, value); }
        }

        public bool IsPossibileSquare
        { 
            get { return isPossibileSquare; } 
            set { RaisePropertyChanged(ref isPossibileSquare, value); }
        }

        [XmlElement(nameof(Rook), typeof(Rook))]
        [XmlElement(nameof(Knight), typeof(Knight))]
        [XmlElement(nameof(Bishop), typeof(Bishop))]
        [XmlElement(nameof(Queen), typeof(Queen))]
        [XmlElement(nameof(King), typeof(King))]
        [XmlElement(nameof(Pawn), typeof(Pawn))]
        public Piece Piece
        {
            get { return piece; }
            set 
            {
                RaisePropertyChanged(ref piece, value);
                if (Point != null && piece != null)
                {
                    piece.Point = new Point()
                    {
                        PosX = Point.PosX,
                        PosY = Point.PosY
                    };
                }
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { RaisePropertyChanged(ref isSelected, value); }
        }

        public object Clone()
        {
            return new Square()
            {
                Color = Color,
                IsSelected = IsSelected,
                Piece = Piece?.Clone() as Piece,
                Point = Point.Clone() as Point
            };
        }
    }

    public enum Color
    {
        Black,
        White
    }
}
