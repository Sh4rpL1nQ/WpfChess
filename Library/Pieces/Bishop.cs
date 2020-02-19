using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Pieces
{
    public class Bishop : Piece
    {
        public override PieceType PieceType => PieceType.Bishop;

        public Bishop()
        {
            Directions = new List<Point>
            {
                new Point { PosX = 1, PosY = -1 },
                new Point { PosX = 1, PosY = 1 },
                new Point { PosX = -1, PosY = -1 },
                new Point { PosX = -1, PosY = 1 },
            };
        }

        public override event EventHandler UpgradePiece;

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            //not valid
            if (dir == null) 
                return false;

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
            return new Bishop()
            {
                Color = Color,
                Image = Image,
                Point = Point.Clone() as Point
            };
        }
    }
}
