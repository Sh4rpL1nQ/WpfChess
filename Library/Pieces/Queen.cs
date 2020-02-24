using System.Collections.Generic;

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
                if ((color = value) == Color.White)
                    Image = @"Images\Queen_W.png";
            }
        }

        public override int Weight => 1000;

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
