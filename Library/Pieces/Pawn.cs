using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Pieces
{
    public class Pawn : Piece
    {
        private Color color;

        public override event EventHandler UpgradePiece;

        public Pawn()
        {
            Directions = new List<Point>
            {
                new Point { PosX = 1, PosY = -1 },
                new Point { PosX = -1, PosY = -1 },
                new Point { PosX = 0, PosY = -1 },
            };

            Image = @"Images\Pawn_B.png";
        }

        public override Color Color
        {
            get { return color; }
            set 
            { 
                color = value;
                if (Color == Color.White)
                {
                    Directions = new List<Point>
                    {
                        new Point { PosX = 1, PosY = 1 },
                        new Point { PosX = -1, PosY = 1 },
                        new Point { PosX = 0, PosY = 1 },
                    };

                    Image = @"Images\Pawn_W.png";
                }
            }
        }

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            //not valid
            if (dir == null) return false;

            //attacking move
            if (dir.PosX != 0 && Point.GoToDirection(dir).Equals(end.Point) && end.Piece != null)
            {
                CheckPawnPositionForUpgrade(end);
                return true;
            }

            //normal move
            var points = Point.AllMovesWithinDirection(end.Point, dir);
            if (dir.PosX == 0 && (points.Count == 1 ^ (points.Count == 2 && IsFirstMove)) && end.Piece == null)
            {
                CheckPawnPositionForUpgrade(end);
                return true;
            }

            return false;
        }

        private void CheckPawnPositionForUpgrade(Square end)
        {
            if (end.Point.PosY == 0 || end.Point.PosY == 7)
                UpgradePiece?.Invoke(this, new EventArgs());
        }

        public override object Clone()
        {
            return new Pawn()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
