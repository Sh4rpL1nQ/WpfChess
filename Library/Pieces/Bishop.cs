using System.Collections.Generic;

namespace Library.Pieces
{
    public class Bishop : Piece
    {
        private Color color;

        public Bishop()
        {
            Directions = new List<Point>
            {
                new Point { PosX = 1, PosY = -1 },
                new Point { PosX = 1, PosY = 1 },
                new Point { PosX = -1, PosY = -1 },
                new Point { PosX = -1, PosY = 1 },
            };

            Image = @"Images\Bishop_B.png";
        }

        public override Color Color
        {
            get { return color; }
            set
            {
                if ((color = value) == Color.White)
                    Image = @"Images\Bishop_W.png";
            }
        }

        public override int Weight => 300;

        public override bool CanBeMovedToSquare(Square end)
        {
            return ChooseRightDirection(end.Point) != null;
        }

        public override object Clone()
        {
            return new Bishop()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
