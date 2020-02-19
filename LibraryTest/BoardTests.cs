using Library;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryTest
{
    public class BoardTests
    {
        private Board board;

        [SetUp]
        public void Setup()
        {
            board = new Board();           
        }

        [Test]
        public void IsPieceBlockingYes()
        {
            board = Serializer.FromXml<Board>(@"D:\Chess\LibraryTest\Xml\PieceBlocking.xml");

            var piece = board.Squares.FirstOrDefault(x => x.Piece?.PieceType == PieceType.Bishop)?.Piece;
            var square = board.Squares.FirstOrDefault(x => x.Piece?.PieceType == PieceType.Knight);

            var dir = piece.ChooseRightDirection(square.Point);

            Assert.IsTrue(board.IsPieceBlocking(piece, square, dir));
        }
    }
}
