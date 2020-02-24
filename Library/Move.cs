using Library.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library
{
    public class Move
    {
        private Piece start;
        private Square end;
        private Board board;

        public Move(Board board, Piece start, Square end)
        {
            this.start = start;
            this.end = end;
            this.board = board;
        }

        public Square End => end;

        public int Weight { get; set; }

        public void Execute()
        {
            //Castle
            var squares = CanCastle();
            if (squares != null && squares.Count != 0)
                OnCastlePossible?.Invoke(squares, new EventArgs());

            //Promotion
            if (CanPieceBePromoted())
                OnInitiatePawnPromotion?.Invoke(start, new EventArgs());

            var lostPiece = board.ShiftPiece(start, end);
            if (lostPiece != null)
                OnPieceCaptured?.Invoke(lostPiece, new EventArgs());
        }

        public int CalculateWeight()
        {
            var opponentKing = board.GetKingByColor(board.GetOtherColor(start.Color));
            var pieces = board.GetAllPiecesByColor(start.Color);
            var squares = CanCastle();
            var enemyPieces = board.GetAllPiecesByColor(board.GetOtherColor(start.Color));

            var result = 0;

            result += PieceCapture();
            if (squares != null && squares.Count != 0)
                result += 10;

            if (IsCenterSquare())
                result += 2;

            if (IsNextToKing(opponentKing))
                result += 2;

            if (IsDoublePawn(pieces))
                result += -10;

            if (IsPassedPawn(enemyPieces))
                result += 10;

            if (IsOpponentPieceAttacked(enemyPieces))
                result += 1;

            if (GoodBishopRange(pieces))
                result += 2;

            if (IsRookOn7th())
                result += 10;

            if (IsRookOnOpenFile())
                result += 10;

            if (IsCentralKnight())
                result += 5;

            return result;
        }

        public bool CanPieceBePromoted()
        {
            if (!(start is Pawn))
                return false;

            return end.Point.PosY == 0 || end.Point.PosY == 7;
        }

        #region Events
        public event EventHandler OnCastlePossible;
        public event EventHandler OnInitiatePawnPromotion;
        public event EventHandler OnPieceCaptured;
        #endregion

        public bool IsEndPosition(Square square)
        {
            return end.Point.Equals(square.Point);
        }

        #region Weight Calculation
        private bool IsCenterSquare()
        {
            return board.CenterSquares.FirstOrDefault(x => x.Point.Equals(end.Point)) != null;
        }

        private bool IsCentralKnight()
        {
            return (start is Knight) && IsCenterSquare();
        }

        private bool IsNextToKing(Piece opponentKing)
        {
            var pieces = board.GetAllPiecesByColor(start.Color);
            foreach (var dir in opponentKing.Directions)
            {
                foreach (var piece in pieces)
                {
                    var possibleMoves = board.PossibleMoves;
                    var square = board.Squares.FirstOrDefault(x => x.Point.Equals(start.Point + dir));
                    if (possibleMoves.FirstOrDefault(x => x.IsEndPosition(square)) != null)
                        return true;
                }
            }

            return false;
        }

        private bool IsDoublePawn(IEnumerable<Piece> pieces)
        {
            var pawns = pieces.Where(x => x is Pawn);
            return pawns.FirstOrDefault(x => x.Point.PosX == end.Point.PosX) != null;
        }

        private bool IsPassedPawn(IEnumerable<Piece> enemies)
        {
            var enemyPawns = enemies.Where(x => x is Pawn);
            var towards = enemyPawns.FirstOrDefault(x => x.Point.PosX == end.Point.PosX) != null;
            var diagonals = start.Directions.Where(x => x.PosX != 0);

            if (!towards)
                foreach (var dir in diagonals)
                    if (enemyPawns.FirstOrDefault(x => x.Point.Equals(start.Point + dir)) != null)
                        return false;

            return true;
        }

        public bool IsOpponentPieceAttacked(IEnumerable<Piece> enemies)
        {
            var clone = board.PredictBoard(start, end);
            foreach (var move in clone.PossibleMoves)
                if (enemies.FirstOrDefault(x => x.Point.Equals(move.End.Point)) != null)
                    return true;

            return false;
        }

        public List<Square> CanCastle()
        {
            if (!(start is Rook))
                return null;

            var king = board.GetKingByColor(start.Color);
            var squares = new List<Square>();
            if (start.IsFirstMove && king.IsFirstMove)
            {
                var clone = board.Clone() as Board;
                var dir = king.ChooseRightDirection(start.Point);
                var allMoves = king.Point.AllMovesWithinDirection(start.Point, dir).Take(2);
                foreach (var move in allMoves)
                {
                    var end = clone.Squares.FirstOrDefault(x => x.Point.Equals(move));
                    clone.ShiftPiece(clone.GetKingByColor(king.Color), end);
                    if (clone.IsKingChecked(start.Color) != null)
                        return null;

                    squares.Add(board.Squares.FirstOrDefault(x => x.Point.Equals(move)));
                }

                return squares;
            }

            return null;
        }

        public int PieceCapture()
        {
            if (end.Piece == null)
                return 0;

            return end.Piece.Weight;
        }

        public bool IsEarlyQueen()
        {
            //TODO
            if (!(start is Queen))
                return false;

            return false;
        }

        public bool GoodBishopRange(IEnumerable<Piece> olds)
        {
            olds = olds.Where(x => x is Bishop);

            var clone = board.PredictBoard(start, end);

            var news = clone.GetAllPiecesByColor(start.Color).Where(x => x is Bishop);

            for (int i = 0; i < olds.Count(); i++)
            {
                var newMoves = clone.CalcPossibleMoves(news.ElementAt(i));
                if (clone.CalcPossibleMoves(olds.ElementAt(i)).Count < newMoves.Count)
                    return true;
            }

            return false;
        }

        public bool IsRookOn7th()
        {
            if (!(start is Rook))
                return false;

            if (board.TopColor == start.Color)
                return end.Point.PosY == 6;
            else
                return end.Point.PosY == 1;
        }

        public bool IsRookOnOpenFile()
        {
            if (!(start is Rook))
                return false;

            var line1 = board.Squares.Where(x => x.Point.PosY == end.Point.PosY);
            var line2 = board.Squares.Where(x => x.Point.PosX == end.Point.PosX);

            return
                !(line1.FirstOrDefault(x => x.Piece?.Color == start.Color) != null ||
                line2.FirstOrDefault(x => x.Piece?.Color == start.Color) != null);
        }
        #endregion


        public override string ToString()
        {
            return start.ToString() + "->" + end.ToString();
        }
    }
}
