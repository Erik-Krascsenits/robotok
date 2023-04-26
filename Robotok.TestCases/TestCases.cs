using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;
using Moq;

namespace Robotok.TestCases
{
    [TestClass]
    public class TestCases
    {
        private RobotokGameModel _model = null!;
        private RobotokTable _mockedTable = null!;
        private Mock<IRobotokDataAccess> _mock = null!;
        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new RobotokTable(17, 28);
            _mock = new Mock<IRobotokDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));
        }
        //Megnézzük, hogy helyes a pálya mérete
        [TestMethod]
        public void GeneratedFieldsAmount()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();

            int playingField = 0;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    playingField++;
                }
            }

            Assert.AreEqual(476, playingField);
        }

        //Megnézzük, hogy helyesen állítódik be a tábla egy csapat esetén, vagy mégse
        [TestMethod]
        public void CheckSetUpForOneTeam()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();

            int players = 0;
            int walls = 0;
            int blocks = 0;

            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1 || _model.Table.GetFieldValue(i, j) == 8)
                        players++;

                    if (_model.Table.GetFieldValue(i, j) == 0)
                        walls++;

                    if (_model.Table.GetFieldValue(i, j) == 3 || _model.Table.GetFieldValue(i, j) == 4 || _model.Table.GetFieldValue(i, j) == 5 || _model.Table.GetFieldValue(i, j) == 6)
                        blocks++;
                }
            }

            bool wallsAmount = true;
            bool blocksAmount = true;

            if (walls < 5 || walls > 7)
            {
                wallsAmount = false;
            }

            if (blocks < 6 || blocks > 12)
            {
                blocksAmount = false;
            }

            Assert.IsTrue(wallsAmount);
            Assert.IsTrue(blocksAmount);

            Assert.AreEqual(2, players);
        }
        //Megnézzük, hogy helyesen állítódik be a tábla két csapat esetén, vagy mégse
        [TestMethod]
        public void CheckSetUpForTwoTeam()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 2);
            _model.NewGame();

            int players = 0;
            int walls = 0;
            int blocks = 0;

            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1 || _model.Table.GetFieldValue(i, j) == 8 || _model.Table.GetFieldValue(i, j) == 2 || _model.Table.GetFieldValue(i, j) == 9)
                        players++;

                    if (_model.Table.GetFieldValue(i, j) == 0)
                        walls++;

                    if (_model.Table.GetFieldValue(i, j) == 3 || _model.Table.GetFieldValue(i, j) == 4 || _model.Table.GetFieldValue(i, j) == 5 || _model.Table.GetFieldValue(i, j) == 6)
                        blocks++;
                }
            }

            bool wallsAmount = true;
            bool blocksAmount = true;

            if (walls < 5 || walls > 7)
            {
                wallsAmount = false;
            }

            if (blocks < 6 || blocks > 12)
            {
                blocksAmount = false;
            }

            Assert.IsTrue(wallsAmount);
            Assert.IsTrue(blocksAmount);

            Assert.AreEqual(4, players);
        }
        //Megnézzük, hogy tudunk lépni északra kocka/kockák nélkül
        [TestMethod]
        public void CheckMoveUpFunctionWithoutCubes() {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();

            int greenPlayerCoordinateXBeforeMove = 0;
            int greenPlayerCoordinateXAfterMove = 0;

            for (int i = 4; i < 13; i++)
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateXBeforeMove = i;
                    }
                }
            }

            bool moved = _model.Move("észak", 1);

            for (int i = 3; i < 14; i++) // itt előfordulhat, hogy kijáraton lesz a robot
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateXAfterMove = i;
                    }
                }
            }

            if (moved)
            {
                Assert.AreEqual(1, (greenPlayerCoordinateXBeforeMove - greenPlayerCoordinateXAfterMove));
            } else
            {
                Assert.AreEqual(greenPlayerCoordinateXAfterMove, greenPlayerCoordinateXBeforeMove);
            }
        }
        //Megnézzük, hogy tudunk lépni délre kocka/kockák nélkül
        [TestMethod]
        public void CheckMoveDownFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();

            int greenPlayerCoordinateXBeforeMove = 0;
            int greenPlayerCoordinateXAfterMove = 0;

            for (int i = 4; i < 13; i++)
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateXBeforeMove = i;
                    }
                }
            }

            bool moved = _model.Move("dél", 1);

            for (int i = 3; i < 14; i++) // itt előfordulhat, hogy kijáraton lesz a robot
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateXAfterMove = i;
                    }
                }
            }

            if (moved)
            {
                Assert.AreEqual(-1, (greenPlayerCoordinateXBeforeMove - greenPlayerCoordinateXAfterMove));
            }
            else
            {
                Assert.AreEqual(greenPlayerCoordinateXAfterMove, greenPlayerCoordinateXBeforeMove);
            }
        }
        //Megnézzük, hogy tudunk lépni nyugatra kocka/kockák nélkül
        [TestMethod]
        public void CheckMoveRightFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();

            int greenPlayerCoordinateYBeforeMove = 0;
            int greenPlayerCoordinateYAfterMove = 0;

            for (int i = 4; i < 13; i++)
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateYBeforeMove = j;
                    }
                }
            }

            bool moved = _model.Move("kelet", 1);

            for (int i = 4; i < 13; i++)
            {
                for (int j = 4; j < 24; j++) // itt előfordulhat, hogy kijáraton lesz a robot
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateYAfterMove = j;
                    }
                }
            }

            if (moved)
            {
                Assert.AreEqual(-1, (greenPlayerCoordinateYBeforeMove - greenPlayerCoordinateYAfterMove));
            }
            else
            {
                Assert.AreEqual(greenPlayerCoordinateYBeforeMove, greenPlayerCoordinateYAfterMove);
            }
        }
        //Megnézzük, hogy tudunk lépni keletre kocka/kockák nélkül
        [TestMethod]
        public void CheckMoveLefttFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();

            int greenPlayerCoordinateYBeforeMove = 0;
            int greenPlayerCoordinateYAfterMove = 0;

            for (int i = 4; i < 13; i++)
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateYBeforeMove = j;
                    }
                }
            }

            bool moved = _model.Move("nyugat", 1);

            for (int i = 4; i < 13; i++)
            {
                for (int j = 4; j < 24; j++) // itt előfordulhat, hogy kijáraton lesz a robot
                {
                    if (_model.Table.GetFieldValue(i, j) == 1)
                    {
                        greenPlayerCoordinateYAfterMove = j;
                    }
                }
            }

            if (moved)
            {
                Assert.AreEqual(1, (greenPlayerCoordinateYBeforeMove - greenPlayerCoordinateYAfterMove));
            }
            else
            {
                Assert.AreEqual(greenPlayerCoordinateYBeforeMove, greenPlayerCoordinateYAfterMove);
            }
        }
        // Megnézzük a várakozást
        [TestMethod]
        public void CheckWaitFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.NewGame();
            _model.Wait();

            Assert.AreEqual(1, _model.RemainingSeconds);
        }
        // Megnézzük a rákapcsolást északi írányba
        [TestMethod]
        public void CheckAttachRobotToCubeUpFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);

            bool successAttach = _model.Attach("észak", 1);

            Assert.IsTrue(successAttach);
        }
        // Megnézzük a rákapcsolást déli írányba
        [TestMethod]
        public void CheckAttachRobotToCubeDownFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(9, 10, 1, -1);
            _model.Table.SetValue(10, 10, 4, 3);

            bool success = _model.Attach("dél", 1);

            Assert.IsTrue(success);
        }
        // Megnézzük a rákapcsolást nyugati írányba
        [TestMethod]
        public void CheckAttachRobotToCubeLeftFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(10, 9, 4, 3);

            bool success = _model.Attach("nyugat", 1);

            Assert.IsTrue(success);
        }
        // Megnézzük a rákapcsolást keleti írányba
        [TestMethod]
        public void CheckAttachRobotToCubeRightFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 9, 1, -1);
            _model.Table.SetValue(10, 10, 4, 3);

            bool success = _model.Attach("kelet", 1);

            Assert.IsTrue(success);
        }
        // Megnézzük a lekapcsolást északi írányba
        [TestMethod]
        public void CheckDettachRobotToCubeUpFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Attach("észak", 1);
            bool successDetach = _model.Dettach("észak", 1);
            Assert.IsTrue(successDetach);
        }
        // Megnézzük a lekapcsolást déli írányba
        [TestMethod]
        public void CheckDettachRobotToCubeDownFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(9, 10, 1, -1);
            _model.Table.SetValue(10, 10, 4, 3);

            _model.Attach("dél", 1);
            bool successDetach = _model.Dettach("dél", 1);
            Assert.IsTrue(successDetach);
        }
        // Megnézzük a lekapcsolást nyugati írányba
        [TestMethod]
        public void CheckDettachRobotToCubeLeftFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(10, 9, 4, 3);
            _model.Attach("nyugat", 1);
            bool successDetach = _model.Dettach("nyugat", 1);
            Assert.IsTrue(successDetach);
        }
        // Megnézzük a lekapcsolást keleti írányba
        [TestMethod]
        public void CheckDettachRobotToCubeRightFunction()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 9, 1, -1);
            _model.Table.SetValue(10, 10, 4, 3);
            _model.Attach("kelet", 1);
            bool successDetach = _model.Dettach("kelet", 1);
            Assert.IsTrue(successDetach);
        }
        // Megnézzük a forgatást óramutatóval megegyező írányba
        [TestMethod]
        public void CheckRotatePlayerWithoutCubesClockwise()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 9, 1, -1);
            bool faceToSouth = _model.Table.GetFaceSouth(10, 9);
            Assert.IsTrue(faceToSouth);

            _model.Rotate("óramutatóval megegyező", 1);
            bool faceToWest = _model.Table.GetFaceWest(10, 9);
            Assert.IsTrue(faceToWest);

            _model.Rotate("óramutatóval megegyező", 1);
            bool faceToNorth = _model.Table.GetFaceNorth(10, 9);
            Assert.IsTrue(faceToNorth);

            _model.Rotate("óramutatóval megegyező", 1);
            bool faceToEast = _model.Table.GetFaceEast(10, 9);
            Assert.IsTrue(faceToEast);

            _model.Rotate("óramutatóval megegyező", 1);
            faceToSouth = _model.Table.GetFaceSouth(10, 9);
            Assert.IsTrue(faceToSouth);
        }
        // Megnézzük a forgatást óramutatóval ellenkező írányba
        [TestMethod]
        public void CheckRotatePlayerWithoutCubesNotClockwise()
        {
            _model = new RobotokGameModel(_mock.Object, 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 9, 1, -1);
            bool faceToSouth = _model.Table.GetFaceSouth(10, 9);
            Assert.IsTrue(faceToSouth);

            _model.Rotate("óramutatóval ellenkező", 1);
            bool faceToEast = _model.Table.GetFaceEast(10, 9);
            Assert.IsTrue(faceToEast);

            _model.Rotate("óramutatóval ellenkező", 1);
            bool faceToNorth = _model.Table.GetFaceNorth(10, 9);
            Assert.IsTrue(faceToNorth);

            _model.Rotate("óramutatóval ellenkező", 1);
            bool faceToWest = _model.Table.GetFaceWest(10, 9);
            Assert.IsTrue(faceToWest);

            _model.Rotate("óramutatóval ellenkező", 1);
            faceToSouth = _model.Table.GetFaceSouth(10, 9);
            Assert.IsTrue(faceToSouth);
        }
    }
}