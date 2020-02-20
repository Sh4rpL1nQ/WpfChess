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

        public override event EventHandler UpgradePiece;

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            //not valid
            if (dir == null) return false;

            var points = Point.AllMovesWithinDirection(end.Point, dir);

            //attacking move
            if (end.Piece != null)
                return true;

            //normal move           
            if (end.Piece == null)
                return true;

            return false;
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
