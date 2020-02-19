using Library;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryTest
{
    public class PointTests
    {
        private Point start;
        private Point end;
        private Point dir;

        [SetUp]
        public void Setup()
        {
            start = new Point(2, 2);
            end = new Point(2, 0);
        }

        [Test]
        public void IsInDirection()
        {
            dir = new Point(0, -1);

            var erg = start.IsInDirection(end, dir);
            Assert.IsTrue(erg);
        }

        [Test]
        public void IsNotInDirection()
        {
            dir = new Point(1, -1);

            var erg = start.IsInDirection(end, dir);
            Assert.IsFalse(erg);
        }

        [Test]
        public void AllMovesWithinDirection()
        {
            Point start = new Point(2, 2);
            Point end = new Point(4, 0);
            Point dir = new Point(1, -1);

            var erg = start.AllMovesWithinDirection(end, dir);
            var res = erg.Count == 2;
            Assert.IsTrue(res);
        }
    }
}
