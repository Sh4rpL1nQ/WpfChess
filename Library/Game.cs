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
            Board = Serializer.FromXml<Board>(@"D:\Chess\Library\Structure.xml");
        }
    }
}
