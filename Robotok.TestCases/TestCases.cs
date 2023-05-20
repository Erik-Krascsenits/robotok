using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;

namespace Robotok.TestCases
{
    [TestClass]
    public class TestCases
    {
        private RobotokGameModel _model = null!;
        private RobotokTable _mockedTable = null!;

        #region Initializasion

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new RobotokTable(17, 28);
        }

        #endregion

        #region Testing Model

        #region Testing setups

        //Megnézzük, hogy helyes a pálya mérete
        [TestMethod]
        public void GeneratedFieldsAmount()
        {
            _model = new RobotokGameModel(2, 1);

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

        //Megnézzük, hogy helyesen állítja be a táblát NewGame függvény egy csapat esetén, vagy mégse
        [TestMethod]
        public void CheckSetUpForOneTeam()
        {
            _model = new RobotokGameModel( 2, 1);

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

                    if (_model.Table.GetFieldValue(i, j) == 3 || _model.Table.GetFieldValue(i, j) == 4 || _model.Table.GetFieldValue(i, j) == 5 || _model.Table.GetFieldValue(i, j) == 6 || _model.Table.GetFieldValue(i, j) == 11 || _model.Table.GetFieldValue(i, j) == 12)
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

        //Megnézzük, hogy helyesen állítja be a táblát NewGame függvény két csapat esetén, vagy mégse
        [TestMethod]
        public void CheckSetUpForTwoTeam()
        {
            _model = new RobotokGameModel(2, 2);

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

                    if (_model.Table.GetFieldValue(i, j) == 3 || _model.Table.GetFieldValue(i, j) == 4 || _model.Table.GetFieldValue(i, j) == 5 || _model.Table.GetFieldValue(i, j) == 6 || _model.Table.GetFieldValue(i, j) == 11 || _model.Table.GetFieldValue(i, j) == 12)
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

        #endregion

        #region Cheking move function

        //Megnézzük, hogy tudunk lépni északra kocka/kockák nélkül
        [TestMethod]
        public void CheckMoveFunctionWithoutCubesNorth() {
            _model = new RobotokGameModel(2, 1);

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

            int moved = _model.Move("észak", 1);

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

            if (moved == 1)
            {
                Assert.AreEqual(1, (greenPlayerCoordinateXBeforeMove - greenPlayerCoordinateXAfterMove));
            } else
            {
                Assert.AreEqual(greenPlayerCoordinateXAfterMove, greenPlayerCoordinateXBeforeMove);
            }
        }

        //Megnézzük, hogy tudunk lépni délre kocka/kockák nélkül
        [TestMethod]
        public void CheckMovFunctionWithoutCubesSouth()
        {
            _model = new RobotokGameModel(2, 1);    

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

            int moved = _model.Move("dél", 1);

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

            if (moved == 1)
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
        public void CheckMoveFunctionWithoutCubesEast()
        {
            _model = new RobotokGameModel(2, 1);

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

            int moved = _model.Move("kelet", 1);

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

            if (moved == 1)
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
        public void CheckMoveFunctionWithoutCubesWest()
        {
            _model = new RobotokGameModel(2, 1);

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

            int moved = _model.Move("nyugat", 1);

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

            if (moved == 1)
            {
                Assert.AreEqual(1, (greenPlayerCoordinateYBeforeMove - greenPlayerCoordinateYAfterMove));
            }
            else
            {
                Assert.AreEqual(greenPlayerCoordinateYBeforeMove, greenPlayerCoordinateYAfterMove);
            }
        }

        // Megnézzük, hogy kockákkal tudunk északra lépni
        [TestMethod]
        public void CheckMoveWithCubesNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(8, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 8;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 8;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            int succes = _model.Move("észak", 1);

            Assert.AreEqual(1, succes);
            Assert.AreEqual(4, _model.Table.GetFieldValue(7, 10));
            Assert.AreEqual(4, _model.Table.GetFieldValue(8, 10));
            Assert.AreEqual(1, _model.Table.GetFieldValue(9, 10));
        }

        [TestMethod]
        // Megnézzük, hogy kockákkal tudunk keletre lépni
        public void CheckMoveWithCubesEast()
        {
            _model = new RobotokGameModel(2, 1);

            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(9, 11, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 9;
            _model.Cube2YPlayer1TeamGreen = 11;
            _model.Cube2XPlayer2TeamGreen = 9;
            _model.Cube2YPlayer2TeamGreen = 11;

            Assert.IsTrue(_model.AttachCubes("green"));

            int succes = _model.Move("kelet", 1);

            Assert.AreEqual(1, succes);
            Assert.AreEqual(4, _model.Table.GetFieldValue(9, 12));
            Assert.AreEqual(4, _model.Table.GetFieldValue(9, 11));
            Assert.AreEqual(1, _model.Table.GetFieldValue(10, 11));
        }

        // Megnézzük, hogy kockákkal tudunk délre lépni
        [TestMethod]
        public void CheckMoveWithCubesSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(12, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 12;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 12;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            int succes = _model.Move("dél", 1);

            Assert.AreEqual(1, succes);
            Assert.AreEqual(4, _model.Table.GetFieldValue(13, 10));
            Assert.AreEqual(4, _model.Table.GetFieldValue(12, 10));
            Assert.AreEqual(1, _model.Table.GetFieldValue(11, 10));
        }

        // Megnézzük, hogy kockákkal tudunk nyugatra lépni
        [TestMethod]
        public void CheckMoveWithCubesWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(11, 9, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 11;
            _model.Cube2YPlayer1TeamGreen = 9;
            _model.Cube2XPlayer2TeamGreen = 11;
            _model.Cube2YPlayer2TeamGreen = 9;

            Assert.IsTrue(_model.AttachCubes("green"));

            int succes = _model.Move("nyugat", 1);

            Assert.AreEqual(1, succes);
            Assert.AreEqual(4, _model.Table.GetFieldValue(11, 8));
            Assert.AreEqual(4, _model.Table.GetFieldValue(11, 9));
            Assert.AreEqual(1, _model.Table.GetFieldValue(10, 9));
        }

        //Az alábbi tesztekben megnézzük, hogy helyesen működik a Movw függvény, abban az esetben, ha megyünk bele valamiylen akadályba/falra/robotba
        [TestMethod]
        public void CheckingMoveWithoutCubesInObstaclesNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 0, 3);
            Assert.AreEqual(0, _model.Move("észak", 1));

            _model.Table.SetValue(9, 10, -1, -1);
            Assert.AreEqual(0, _model.Move("észak", 1)); 
            _model.Table.SetValue(9, 10, 0, 3);
            Assert.AreEqual(0, _model.Move("észak", 1));
            _model.Table.SetValue(9, 10, 5, 3);
            Assert.AreEqual(0, _model.Move("észak", 1));
        }

        [TestMethod]
        public void CheckingMoveWithoutCubesInObstaclesSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 0, 3);
            _model.Table.SetValue(9, 10, 1, -1);

            Assert.AreEqual(0, _model.Move("dél", 1));

            _model.Table.SetValue(10, 10, -1, -1);
            Assert.AreEqual(0, _model.Move("dél", 1));
            _model.Table.SetValue(10, 10, 0, 3);
            Assert.AreEqual(0, _model.Move("dél", 1));
            _model.Table.SetValue(10, 10, 5, 3);
            Assert.AreEqual(0, _model.Move("dél", 1));
        }

        [TestMethod]
        public void CheckingMoveWithoutCubesInObstaclesWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(10, 11, 0, 3);

            Assert.AreEqual(0, _model.Move("kelet", 1));

            _model.Table.SetValue(10, 11, -1, -1);
            Assert.AreEqual(0, _model.Move("kelet", 1));
            _model.Table.SetValue(10, 11, 5, 3);
            Assert.AreEqual(0, _model.Move("kelet", 1));
            _model.Table.SetValue(10, 11, 0, 3);
            Assert.AreEqual(0, _model.Move("kelet", 1));
        }

        [TestMethod]
        public void CheckingMoveWithoutCubesInObstaclesEast()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(10, 9, 0, 3);

            Assert.AreEqual(0, _model.Move("nyugat", 1));

            _model.Table.SetValue(10, 9, -1, -1);
            Assert.AreEqual(0, _model.Move("nyugat", 1));
            _model.Table.SetValue(10, 9, 0, 3);
            Assert.AreEqual(0, _model.Move("nyugat", 1));
            _model.Table.SetValue(10, 9, 5, 3);
            Assert.AreEqual(0, _model.Move("nyugat", 1));
        }

        [TestMethod]
        public void CheckingMoveWithCubesInObstaclesNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(8, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 8;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 8;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(7, 10, 0, 3);
            Assert.AreEqual(0, _model.Move("észak", 1));
            _model.Table.SetValue(7, 10, -1, -1);
            Assert.AreEqual(0, _model.Move("észak", 1));
            _model.Table.SetValue(7, 10, 5, 3);
            Assert.AreEqual(0, _model.Move("észak", 1));
        }

        [TestMethod]
        public void CheckingMoveWithCubesInObstaclesSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(8, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(10, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 10;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 10;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(11, 10, 0, 3);
            Assert.AreEqual(0, _model.Move("dél", 1));
            _model.Table.SetValue(11, 10, -1, -1);
            Assert.AreEqual(0, _model.Move("dél", 1));
            _model.Table.SetValue(11, 10, 5, 3);
            Assert.AreEqual(0, _model.Move("dél", 1));
        }

        [TestMethod]
        public void CheckingMoveWithCubesInObstaclesWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(11, 11, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 11;
            _model.Cube2YPlayer1TeamGreen = 11;
            _model.Cube2XPlayer2TeamGreen = 11;
            _model.Cube2YPlayer2TeamGreen = 11;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(11, 12, 0, 3);
            Assert.AreEqual(0, _model.Move("kelet", 1));
            _model.Table.SetValue(11, 12, -1, -1);
            Assert.AreEqual(0, _model.Move("kelet", 1));
            _model.Table.SetValue(11, 12, 5, 3);
            Assert.AreEqual(0, _model.Move("kelet", 1));
        }

        [TestMethod]
        public void CheckingMoveWithCubesInInObstaclesEast()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(11, 9, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 11;
            _model.Cube2YPlayer1TeamGreen = 9;
            _model.Cube2XPlayer2TeamGreen = 11;
            _model.Cube2YPlayer2TeamGreen = 9;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(11, 8, 0, 3);
            Assert.AreEqual(0, _model.Move("nyugat", 1));
            _model.Table.SetValue(11, 8, -1, -1);
            Assert.AreEqual(0, _model.Move("nyugat", 1));
            _model.Table.SetValue(11, 8, 5, 3);
            Assert.AreEqual(0, _model.Move("nyugat", 1));
        }

        #endregion

        #region Checking wait function

        // Megnézzük a várakozást
        [TestMethod]
        public void CheckWaitFunction()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Wait(1, 1);

            Assert.AreEqual(1, _model.RemainingSeconds);
        }

        #endregion

        #region Checking attach function

        // Megnézzük a rákapcsolást északi írányba
        [TestMethod]
        public void CheckAttachRobotToCubeNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);

            _model.Table.SetValue(9, 10, 4, 3);
            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Table.SetValue(9, 10, -1, -1);
            Assert.IsFalse(_model.Attach("észak", 1));

            _model.Table.SetValue(9, 10, 0, 3);
            Assert.IsFalse(_model.Attach("észak", 1));

            _model.Table.SetValue(9, 10, 8, -1);
            Assert.IsFalse(_model.Attach("észak", 1));
        }

        // Megnézzük a rákapcsolást déli írányba
        [TestMethod]
        public void CheckAttachRobotToCubeSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(9, 10, 1, -1);

            _model.Table.SetValue(10, 10, 4, 3);
            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Table.SetValue(10, 10, -1, -1);
            Assert.IsFalse(_model.Attach("dél", 1));

            _model.Table.SetValue(10, 10, 0, 3);
            Assert.IsFalse(_model.Attach("dél", 1));

            _model.Table.SetValue(10, 10, 8, -1);
            Assert.IsFalse(_model.Attach("dél", 1));
        }

        // Megnézzük a rákapcsolást nyugati írányba
        [TestMethod]
        public void CheckAttachRobotToCubeWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);

            _model.Table.SetValue(10, 9, 4, 3);
            Assert.IsTrue(_model.Attach("nyugat", 1));
            
            _model.Table.SetValue(10, 9, -1, -1);
            Assert.IsFalse(_model.Attach("nyugat", 1));
            
            _model.Table.SetValue(10, 9, 0, 3);
            Assert.IsFalse(_model.Attach("nyugat", 1));
            
            _model.Table.SetValue(10, 9, 8, -1);
            Assert.IsFalse(_model.Attach("nyugat", 1));
        }

        // Megnézzük a rákapcsolást keleti írányba
        [TestMethod]
        public void CheckAttachRobotToCubeEast()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 9, 1, -1);

            _model.Table.SetValue(10, 10, 4, 3);
            Assert.IsTrue(_model.Attach("kelet", 1));

            _model.Table.SetValue(10, 10, -1, -1);
            Assert.IsFalse(_model.Attach("kelet", 1));

            _model.Table.SetValue(10, 10, 0, 3);
            Assert.IsFalse(_model.Attach("kelet", 1));
            
            _model.Table.SetValue(10, 10, 8, -1);
            Assert.IsFalse(_model.Attach("kelet", 1));
        }

        // Az alábbi tesztekben megnézzük, hogy helyesen műkodik Attach, illetve AttachCubes fv. több kocks esetén
        [TestMethod]
        public void CheckAttachWithCubesNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(8, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 8;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 8;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(8, 10, 0, 3);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(8, 10, -1, -1);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(8, 10, 8, -1);
            Assert.IsFalse(_model.AttachCubes("green"));
        }

        [TestMethod]
        public void CheckAttachWithCubesEast()
        {
            _model = new RobotokGameModel( 2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(9, 11, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 9;
            _model.Cube2YPlayer1TeamGreen = 11;
            _model.Cube2XPlayer2TeamGreen = 9;
            _model.Cube2YPlayer2TeamGreen = 11;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(9, 11, 0, 3);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(9, 11, -1, -1);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(9, 11, 8, -1);
            Assert.IsFalse(_model.AttachCubes("green"));
        }

        [TestMethod]
        public void CheckAttachWithCubesSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(12, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 12;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 12;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(12, 10, 0, 3);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(12, 10, -1, -1);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(12, 10, 8, -1);
            Assert.IsFalse(_model.AttachCubes("green"));
        }

        [TestMethod]
        public void CheckAttachWithCubesWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(11, 9, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 11;
            _model.Cube2YPlayer1TeamGreen = 9;
            _model.Cube2XPlayer2TeamGreen = 11;
            _model.Cube2YPlayer2TeamGreen = 9;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.Table.SetValue(11, 9, 0, 3);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(11, 9, -1, -1);
            Assert.IsFalse(_model.AttachCubes("green"));

            _model.Table.SetValue(11, 9, 8, -1);
            Assert.IsFalse(_model.AttachCubes("green"));
        }

        #endregion

        #region Checking detach function

        // Megnézzük a lekapcsolást északi írányba
        [TestMethod]
        public void CheckDetachRobotFromCubeNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Attach("észak", 1);
            bool successDetach = _model.Detach("észak", 1);
            Assert.IsTrue(successDetach);
        }

        // Megnézzük a lekapcsolást déli írányba
        [TestMethod]
        public void CheckDetachRobotFromCubeSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(9, 10, 1, -1);
            _model.Table.SetValue(10, 10, 4, 3);

            _model.Attach("dél", 1);
            bool successDetach = _model.Detach("dél", 1);
            Assert.IsTrue(successDetach);
        }

        // Megnézzük a lekapcsolást nyugati írányba
        [TestMethod]
        public void CheckDetachRobotFromCubeWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(10, 9, 4, 3);
            _model.Attach("nyugat", 1);
            bool successDetach = _model.Detach("nyugat", 1);
            Assert.IsTrue(successDetach);
        }

        // Megnézzük a lekapcsolást keleti írányba
        [TestMethod]
        public void CheckDetachRobotFromCubeEast()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 9, 1, -1);
            _model.Table.SetValue(10, 10, 4, 3);
            _model.Attach("kelet", 1);
            bool successDetach = _model.Detach("kelet", 1);
            Assert.IsTrue(successDetach);
        }

        // Az alábbi tesztekben megnézzük, hogy helyesen műkodik Detach fv. több kocka esetén
        [TestMethod]
        public void CheckDetachWithCubesNorth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(8, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 8;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 8;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.CubeToDetach1X = 9;
            _model.CubeToDetach1Y = 10;
            _model.CubeToDetach2X = 8;
            _model.CubeToDetach2Y = 10;

            bool succes = _model.DetachCubes(1);

            Assert.IsTrue(succes);
        }

        [TestMethod]
        public void CheckDetachWithCubesEast()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(9, 10, 4, 3);
            _model.Table.SetValue(9, 11, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 9;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 9;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("észak", 1));

            _model.Cube2XPlayer1TeamGreen = 9;
            _model.Cube2YPlayer1TeamGreen = 11;
            _model.Cube2XPlayer2TeamGreen = 9;
            _model.Cube2YPlayer2TeamGreen = 11;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.CubeToDetach1X = 9;
            _model.CubeToDetach1Y = 10;
            _model.CubeToDetach2X = 9;
            _model.CubeToDetach2Y = 11;

            bool succes = _model.DetachCubes(1);

            Assert.IsTrue(succes);
        }

        [TestMethod]
        public void CheckDetachWithCubesSouth()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(12, 10, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 12;
            _model.Cube2YPlayer1TeamGreen = 10;
            _model.Cube2XPlayer2TeamGreen = 12;
            _model.Cube2YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.CubeToDetach1X = 11;
            _model.CubeToDetach1Y = 10;
            _model.CubeToDetach2X = 12;
            _model.CubeToDetach2Y = 10;

            bool succes = _model.DetachCubes(1);

            Assert.IsTrue(succes);
        }

        [TestMethod]
        public void CheckDetachWithCubesWest()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);
            _model.Table.SetValue(11, 9, 4, 3);
            _model.Cube1XPlayer1TeamGreen = 11;
            _model.Cube1YPlayer1TeamGreen = 10;
            _model.Cube1XPlayer2TeamGreen = 11;
            _model.Cube1YPlayer2TeamGreen = 10;

            Assert.IsTrue(_model.Attach("dél", 1));

            _model.Cube2XPlayer1TeamGreen = 11;
            _model.Cube2YPlayer1TeamGreen = 9;
            _model.Cube2XPlayer2TeamGreen = 11;
            _model.Cube2YPlayer2TeamGreen = 9;

            Assert.IsTrue(_model.AttachCubes("green"));

            _model.CubeToDetach1X = 11;
            _model.CubeToDetach1Y = 10;
            _model.CubeToDetach2X = 11;
            _model.CubeToDetach2Y = 9;

            bool succes = _model.DetachCubes(1);

            Assert.IsTrue(succes);
        }

        #endregion

        #region Checking rotate function

        // Megnézzük a forgást óramutatóval megegyező írányba
        [TestMethod]
        public void CheckRotatePlayerWithoutCubesClockwise()
        {
            _model = new RobotokGameModel(2, 1);
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

        // Megnézzük a forgást óramutatóval ellenkező írányba
        [TestMethod]
        public void CheckRotatePlayerWithoutCubesNotClockwise()
        {
            _model = new RobotokGameModel(2, 1);
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

        [TestMethod]
        // Megnézzük a forgást óramutatóval megegyező írányba több kocka esetén
        public void CheckRotatePlayerWithCubesClockwise()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);

            Assert.IsTrue(_model.Attach("dél", 1));
            bool faceToSouth = _model.Table.GetFaceSouth(10, 10);
            Assert.IsTrue(faceToSouth);

            _model.Rotate("óramutatóval megegyező", 1);
            bool faceToWest = _model.Table.GetFaceWest(10, 10);
            Assert.IsTrue(faceToWest);

            _model.Rotate("óramutatóval megegyező", 1);
            bool faceToNorth = _model.Table.GetFaceNorth(10, 10);
            Assert.IsTrue(faceToNorth);

            _model.Rotate("óramutatóval megegyező", 1);
            bool faceToEast = _model.Table.GetFaceEast(10, 10);
            Assert.IsTrue(faceToEast);

            _model.Rotate("óramutatóval megegyező", 1);
            faceToSouth = _model.Table.GetFaceSouth(10, 10);
            Assert.IsTrue(faceToSouth);

            _model.Table.SetValue(10, 9, 0, 3);
            Assert.IsFalse(_model.Rotate("óramutatóval megegyező", 1));
            _model.Table.SetValue(10, 9, 8, -1);
            Assert.IsFalse(_model.Rotate("óramutatóval megegyező", 1));
            _model.Table.SetValue(10, 9, -1, -1);
            Assert.IsFalse(_model.Rotate("óramutatóval megegyező", 1));

        }

        // Megnézzük a forgást óramutatóval ellenkező írányba több kocka esetén
        [TestMethod]
        public void CheckRotatePlayerWithCubesNotClockwise()
        {
            _model = new RobotokGameModel(2, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, 3);

            Assert.IsTrue(_model.Attach("dél", 1));
            bool faceToSouth = _model.Table.GetFaceSouth(10, 10);
            Assert.IsTrue(faceToSouth);

            _model.Rotate("óramutatóval ellenkező", 1);
            bool faceToEast = _model.Table.GetFaceEast(10, 10);
            Assert.IsTrue(faceToEast);

            _model.Rotate("óramutatóval ellenkező", 1);
            bool faceToNorth = _model.Table.GetFaceNorth(10, 10);
            Assert.IsTrue(faceToNorth);

            _model.Rotate("óramutatóval ellenkező", 1);
            bool faceToWest = _model.Table.GetFaceWest(10, 10);
            Assert.IsTrue(faceToWest);

            _model.Rotate("óramutatóval ellenkező", 1);
            faceToSouth = _model.Table.GetFaceSouth(10, 10);
            Assert.IsTrue(faceToSouth);

            _model.Table.SetValue(10, 11, 0, 3);
            Assert.IsFalse(_model.Rotate("óramutatóval ellenkező", 1));
            _model.Table.SetValue(10, 11, 8, -1);
            Assert.IsFalse(_model.Rotate("óramutatóval ellenkező", 1));
            _model.Table.SetValue(10, 11, -1, -1);
            Assert.IsFalse(_model.Rotate("óramutatóval ellenkező", 1));
        }

        #endregion

        #region Cheking clean function

        [TestMethod]
        public void CheckCleaningWithoutCubes()
        {
            _model = new RobotokGameModel(3, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, _model.CleaningOperetions);
            _model.Table.SetValue(10, 11, 4, _model.CleaningOperetions);
            _model.Table.SetValue(9, 10, 4, _model.CleaningOperetions);
            _model.Table.SetValue(10, 9, 4, _model.CleaningOperetions);

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("dél", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("kelet", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("nyugat", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("észak", 1));
            }

            _model.Table.SetValue(11, 10, 2, _model.CleaningOperetions);
            _model.Table.SetValue(10, 11, 8, _model.CleaningOperetions);
            _model.Table.SetValue(10, 9, 9, _model.CleaningOperetions);

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsFalse(_model.Clear("kelet", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsFalse(_model.Clear("nyugat", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsFalse(_model.Clear("dél", 1));
            }

            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 0, _model.CleaningOperetions);
            _model.Table.SetValue(10, 11, 0, _model.CleaningOperetions);
            _model.Table.SetValue(9, 10, 0, _model.CleaningOperetions);
            _model.Table.SetValue(10, 9, 0, _model.CleaningOperetions);

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("dél", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("kelet", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("nyugat", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("észak", 1));
            }
        }

        [TestMethod]
        public void CheckCleaningWithCubes()
        {
            _model = new RobotokGameModel(3, 1);
            _model.Table = _mockedTable;
            for (int i = 0; i < _model.Table.SizeX; ++i)
            {
                for (int j = 0; j < _model.Table.SizeY; ++j)
                {
                    _model.Table.SetValue(i, j, 7, -1);
                }
            }
            _model.Table.SetValue(10, 10, 1, -1);
            _model.Table.SetValue(11, 10, 4, _model.CleaningOperetions);
            _model.Table.SetValue(10, 11, 4, _model.CleaningOperetions);
            _model.Table.SetValue(9, 10, 4, _model.CleaningOperetions);
            _model.Table.SetValue(10, 9, 4, _model.CleaningOperetions);

            Assert.IsTrue(_model.Attach("dél", 1));

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsFalse(_model.Clear("dél", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("kelet", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("nyugat", 1));
            }

            for (int i = 0; i < _model.CleaningOperetions; i++)
            {
                Assert.IsTrue(_model.Clear("észak", 1));
            }
        }

        #endregion

        #region Testing View



        #endregion

        #endregion

        #region Testing View

        #region Testing Manhattan distance

        [TestMethod]
        public void CheckManhattanDistanceEasy()
        {
            _model = new RobotokGameModel(1, 1);

            for (int i = 0; i < _model.Table.SizeX; i++)
            {
                for (int j = 0; j < _model.Table.SizeY; j++)
                {
                    if (i > 3 && i < 13 && j > 4 && j < 23)
                    {
                        _model.Table.SetValue(i, j, 7, -1);
                    }
                }
            }

            _model.Table.SetValue(14, 12, 1, -1);
            _model.Table.SetValue(12, 11, 4, 4);
            _model.Table.SetValue(14, 11, 0, 4);
            _model.Table.SetValue(15, 11, 8, -1);

            _model.ManhattanDistance(1, 1);

            for (int i = 0; i < _model.Table.SizeX; i++)
            {
                for (int j = 0; j < _model.Table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                    {
                        if (Math.Abs(i - 14) + Math.Abs(j - 12) < 6)
                        {
                            Assert.AreEqual(_model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4), _model.Table.GetFieldValue(i, j));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void CheckManhattanDistanceMedium()
        {
            _model = new RobotokGameModel(2, 1);

            for (int i = 0; i < _model.Table.SizeX; i++)
            {
                for (int j = 0; j < _model.Table.SizeY; j++)
                {
                    if (i > 3 && i < 13 && j > 4 && j < 23)
                    {
                        _model.Table.SetValue(i, j, 7, -1);
                    }
                }
            }

            _model.Table.SetValue(9, 9, 1, -1);
            _model.Table.SetValue(10, 9, 4, 4);
            _model.Table.SetValue(11, 11, 0, 4);
            _model.Table.SetValue(9, 10, 8, -1);

            _model.ManhattanDistance(2, 1);

            for (int i = 0; i < _model.Table.SizeX; i++)
            {
                for (int j = 0; j < _model.Table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                    {
                        if (Math.Abs(i - 9) + Math.Abs(j - 9) < 5)
                        {
                            Assert.AreEqual(_model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4), _model.Table.GetFieldValue(i, j));
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void CheckManhattanDistanceHard()
        {
            _model = new RobotokGameModel(2, 1);

            for (int i = 0; i < _model.Table.SizeX; i++)
            {
                for (int j = 0; j < _model.Table.SizeY; j++)
                {
                    if (i > 3 && i < 13 && j > 4 && j < 23)
                    {
                        _model.Table.SetValue(i, j, 7, -1);
                    }
                }
            }

            _model.Table.SetValue(6, 13, 1, -1);
            _model.Table.SetValue(7, 13, 4, 4);
            _model.Table.SetValue(5, 12, 0, 4);
            _model.Table.SetValue(9, 10, 8, -1);

            _model.ManhattanDistance(2, 1);

            for (int i = 0; i < _model.Table.SizeX; i++)
            {
                for (int j = 0; j < _model.Table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                    {
                        if (Math.Abs(i - 6) + Math.Abs(j - 13) < 4)
                        {
                            Assert.AreEqual(_model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4), _model.Table.GetFieldValue(i, j));
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

    }
}