using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Game
    {
        private ApplicationSettings settings;       

        public Board Board { get; set; }

        public bool ShowPossibleMoves { get; set; }

        public Game(ApplicationSettings settings)
        {
            this.settings = settings;

            Board = new Board();
            Reset(true);
        }

        public void Reset(bool init = false)
        {
            Board.Squares.Clear();
            if (settings.Priority != string.Empty)
            {
                GenerateBoard(settings.BoardXmlPathPriority);
                return;
            }

            if (Board.TopColor == Color.White || init)
                GenerateBoard(settings.BoardXmlPathSwitch1);
            else
                GenerateBoard(settings.BoardXmlPathSwitch2);    
        }

        public void Reset(Board board)
        {
            Board.Squares.Clear();
            FillBoard(board);
        }

        private void FillBoard(Board board)
        {
            foreach (var square in board.Squares)
                Board.Squares.Add(square);
        }

        private void GenerateBoard(string path)
        {
            var board = Serializer.ImportFromTxt(settings.BoardXmlPathSwitch2) as Board;
            FillBoard(board);
        }
    }
}
