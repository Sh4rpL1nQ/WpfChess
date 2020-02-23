using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Pieces
{
    public class Queen : Piece
    {
        private Color color;

        public Queen()
        {
            Directions = new List<Point>
            {
                new Point { PosX = 1, PosY = -1 },
                new Point { PosX = 1, PosY = 1 },
                new Point { PosX = 1, PosY = 0 },
                new Point { PosX = -1, PosY = -1 },
                new Point { PosX = -1, PosY = 1 },
                new Point { PosX = -1, PosY = 0 },
                new Point { PosX = 0, PosY = -1 },
                new Point { PosX = 0, PosY = 1 },
            };

            Image = @"Images\Queen_B.png";
        }

        public override Color Color
        {
            get { return color; }
            set
            {
                color = value;
                if (Color == Color.White)
                    Image = @"Images\Queen_W.png";
            }
        }

        public override bool CanBeMovedToSquare(Square end)
        {
            return ChooseRightDirection(end.Point) != null;
        }

        public override object Clone()
        {
            return new Queen()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
