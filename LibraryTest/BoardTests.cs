using Library;
using Library.Pieces;
using NUnit.Framework;
using System.Linq;

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
            board = Serializer.FromXml<Board>(DirectoryInfos.GetPath("PieceBlocking.xml"));

            var piece = board.Squares.FirstOrDefault(x => x.Piece is Bishop)?.Piece;
            var square = board.Squares.FirstOrDefault(x => x.Piece is Knight);

            var dir = piece.ChooseRightDirection(square.Point);

            Assert.IsTrue(board.IsPieceBlocking(piece, square, dir));
        }

        [Test]
        public void MakeBoardTest()
        {
            Board board = new Board();
        }
    }
}
