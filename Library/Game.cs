using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Game
    {
        public Board Board { get; set; }

        public bool ShowPossibleMoves { get; set; }

        public Game()
        {
            Board = new Board();
            Reset();
        }

        public void Reset()
        {
            Board.Squares.Clear();
            var board = Serializer.FromXml<Board>(@"..\..\..\..\Library\Xml\PromotionTest.xml");
            foreach (var square in board.Squares)
                Board.Squares.Add(square);
        }
    }
}
