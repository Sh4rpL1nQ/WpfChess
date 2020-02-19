﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Pieces
{
    public class Knight : Piece
    {
        public override PieceType PieceType => PieceType.Knight;

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
        }

        public override event EventHandler UpgradePiece;

        public override bool CanBeMovedToSquare(Square end)
        {
            var dir = ChooseRightDirection(end.Point);

            //not valid
            if (dir == null) return false;

            var point = Point.GoToDirection(dir);

            //attacking move
            if (point.Equals(end.Point))
                return true;

            return false;
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