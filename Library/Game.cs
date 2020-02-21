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
                var board = Serializer.ImportFromTxt(settings.BoardXmlPathPriority) as Board;
                FillBoard(board);
                return;
            }

            if (Board.TopColor == Color.White || init)
            {
                var board = Serializer.ImportFromTxt(settings.BoardXmlPathSwitch1) as Board;
                FillBoard(board);
            }
            else
            {
                var board = Serializer.ImportFromTxt(settings.BoardXmlPathSwitch2) as Board;
                FillBoard(board);
            }            
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
    }
}
