using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Game
    {
        private string xmlPath;

        public Board Board { get; set; }

        public bool ShowPossibleMoves { get; set; }

        public Game(string xmlPath)
        {
            this.xmlPath = xmlPath;

            Board = new Board();
            Reset();
        }

        public void Reset()
        {
            Board.Squares.Clear();
            var board = Serializer.FromXml<Board>(xmlPath);
            foreach (var square in board.Squares)
                Board.Squares.Add(square);
        }
    }
}
