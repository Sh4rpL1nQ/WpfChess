using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Pieces
{
    public class Knight : Piece
    {
        private Color color;

        public Knight()
        {
            Directions = new List<Point>
            {
                new Point { PosX = 1, PosY = -2 },
                new Point { PosX = -1, PosY = -2 },
                new Point { PosX = 1, PosY = 2 },
                new Point { PosX = -1, PosY = 2 },
                new Point { PosX = -2, PosY = -1 },
                new Point { PosX = 2, PosY = 1 },
                new Point { PosX = 2, PosY = -1 },
                new Point { PosX = -2, PosY = 1 },
            };

            Image = @"Images\Knight_B.png";
        }

        public override Color Color
        {
            get { return color; }
            set
            {
                color = value;
                if (Color == Color.White)
                    Image = @"Images\Knight_W.png";
            }
        }

        public override int Weight => 30;

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            if (dir == null) return false;

            var point = Point.GoToDirection(dir);

            return point.Equals(end.Point);
        }

        public override object Clone()
        {
            return new Knight()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
