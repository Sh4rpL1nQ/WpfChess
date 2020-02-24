using Library;
using Library.Pieces;
using NUnit.Framework;

namespace LibraryTest
{
    public class PieceTests
    {
        private Piece start;
        private Square end;

        [SetUp]
        public void Setup()
        {
            end = new Square();

            start = new Pawn();
        }

        [Test]
        public void GetRightDirection()
        {
            start.Point = new Point(2, 2);
            end.Point = new Point(2, 0);

            var erg = start.ChooseRightDirection(end.Point);

            Assert.AreEqual(erg, new Point(0, -1));
        }
    }
}
