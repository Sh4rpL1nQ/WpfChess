using Library;
using Library.Pieces;
using NUnit.Framework;

namespace LibraryTest
{
    public class PawnTests
    {
        private Square start;
        private Square end;

        [SetUp]
        public void Setup()
        {
            start = new Square();
            start.Piece = new Pawn();

            end = new Square();
        }

        [Test]
        public void PawnMovesNormalTwoFields()
        {
            start.Point = new Point(2, 2);
            end.Point = new Point(2, 0);

            var erg = start.Piece.CanBeMovedToSquare(end);

            Assert.IsTrue(erg);
        }

        [Test]
        public void PawnMovesNormalOneField()
        {
            start.Point = new Point(2, 2);
            end.Point = new Point(2, 1);

            var erg = start.Piece.CanBeMovedToSquare(end);

            Assert.IsTrue(erg);
        }

        [Test]
        public void PawnAttacksField()
        {
            end.Piece = new Bishop();

            start.Point = new Point(2, 2);
            end.Point = new Point(3, 1);

            var erg = start.Piece.CanBeMovedToSquare(end);

            Assert.IsTrue(erg);
        }

        [Test]
        public void OtherPieceIsBlockingInOneStep()
        {
            end.Piece = new Bishop();

            start.Point = new Point(2, 2);
            end.Point = new Point(2, 1);

            var erg = start.Piece.CanBeMovedToSquare(end);

            Assert.IsFalse(erg);
        }

        [Test]
        public void OtherPieceIsBlockingInTwoSteps()
        {
            end.Piece = new Bishop();

            start.Point = new Point(2, 2);
            end.Point = new Point(2, 0);

            var erg = start.Piece.CanBeMovedToSquare(end);

            Assert.IsFalse(erg);
        }
    }
}
