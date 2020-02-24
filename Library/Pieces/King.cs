using System.Collections.Generic;

namespace Library.Pieces
{
    public class King : Piece
    {
        private Color color;

        public King()
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

            Image = @"Images\King_B.png";
        }

        public override Color Color
        {
            get { return color; }
            set
            {
                if ((color = value) == Color.White)
                    Image = @"Images\King_W.png";
            }
        }

        public override int Weight => 0;

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            if (dir == null) 
                return false;

            var point = Point.GoToDirection(dir);

            return point.Equals(end.Point);
        }

        public override object Clone()
        {
            return new King()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
