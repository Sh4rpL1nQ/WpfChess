using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Pieces
{
    public class Rook : Piece
    {
        private Color color;

        public Rook()
        {
            Directions = new List<Point>
            {
                new Point { PosX = 1, PosY = 0 },
                new Point { PosX = -1, PosY = 0 },
                new Point { PosX = 0, PosY = -1 },
                new Point { PosX = 0, PosY = 1 },
            };

            Image = @"Images\Rook_B.png";
        }

        public override Color Color
        {
            get { return color; }
            set
            {
                color = value;
                if (Color == Color.White)
                    Image = @"Images\Rook_W.png";
            }
        }

        public override bool CanBeMovedToSquare(Square end)
        {
            return ChooseRightDirection(end.Point) != null;
        }

        public override object Clone()
        {
            return new Rook()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
