using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Library.Pieces
{
    public class King : Piece
    {
        public override PieceType PieceType => PieceType.King;

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
        }

        public override event EventHandler UpgradePiece;

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            //not valid
            if (dir == null) return false;

            var point = Point.GoToDirection(dir);

            //attacking move
            if (point.Equals(end.Point) && end.Piece != null)
                return true;

            //normal move
            if (point.Equals(end.Point) && end.Piece == null)
                return true;

            return false;
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
