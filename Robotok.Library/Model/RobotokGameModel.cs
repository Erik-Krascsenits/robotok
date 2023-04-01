using ELTE.Robotok.Persistence;
using Robotok.Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Robotok.Model
{
    public enum GameDifficulty { Easy, Medium, Hard }
    public class RobotokGameModel
    {
        #region Difficulty constants
        private Int32 _ManhattanDistanceEasy = 4;
        private Int32 _ManhattanDistanceMedium = 3;
        private Int32 _ManhattanDistanceHard = 2;

        #endregion

        #region Fields

        private IRobotokDataAccess _dataAccess; // adatelérés
        private RobotokTable _table; // játéktábla (teljes, a játékvezető láthatja opcionálisan)
        private RobotokTable _tableGreenPlayerOne; // zöld csapataban 1. játékos táblája
        private RobotokTable _tableGreenPlayerTwo; // zöld csapataban 2. játékos táblája
        private RobotokTable _tableRedPlayerOne; // piros csapataban 1. játékos táblája
        private RobotokTable _tableRedPlayerTwo; // piros csapataban 2. játékos táblája
        private RobotokTable _tableNoticeBoardOne; // Hirdetőtábla 1
        private RobotokTable _tableNoticeBoardTwo; // Hirdetőtábla 2
        private GameDifficulty _gameDifficulty; // nehézség
        private Int32 _gameStepCount; // hátralevő lépések száma a játék végéig
        private Int32 _remainingSeconds; // hátralevő másodpercek száma a következő lépésig
        private Int32 _cleaningOperations; // játék nehézségétől függő tisztítási műveletek száma
        private Shape _figure1; // első alakzat
        private Shape _figure2; // másik alakzat
        private Int32 _teams; // csapatok száma

        #endregion

        #region Properties

        /// <summary>
        /// Lépések számának lekérdezése.
        /// </summary>
        public Int32 GameStepCount { get { return _gameStepCount; } }

        /// <summary>
        /// Hátramaradt játékidő lekérdezése.
        /// </summary>
        public Int32 RemainingSeconds { get { return _remainingSeconds; } }
        /// <summary>
        /// Hátramaradt játékidő lekérdezése.
        /// </summary>
        public Int32 Teams { get { return _teams; } }

        /// <summary>
        /// Játéktábla lekérdezése.(játékvezetői)
        /// </summary>
        public RobotokTable Table { get { return _table; } }

        /// <summary>
        /// Játéktábla lekérdezése (zöld csapataban 1. játékosé).
        /// </summary>
        public RobotokTable TableGreenPlayerOne { get { return _tableGreenPlayerOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (zöld csapatban 2. játékosé).
        /// </summary>
        public RobotokTable TableGreenPlayerTwo { get { return _tableGreenPlayerTwo; } }

        /// <summary>
        /// Játéktábla lekérdezése (zöld csapataban 1. játékosé).
        /// </summary>
        public RobotokTable TableRedPlayerOne { get { return _tableRedPlayerOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (zöld csapatban 2. játékosé).
        /// </summary>
        public RobotokTable TableRedPlayerTwo { get { return _tableRedPlayerTwo; } }

        /// <summary>
        /// Játéktábla lekérdezése (hirdetőtábla 1).
        /// </summary>
        public RobotokTable TableNoticeBoardOne { get { return _tableNoticeBoardOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (hirdetőtábla 2).
        /// </summary>
        public RobotokTable TableNoticeBoardTwo { get { return _tableNoticeBoardTwo; } }

        /// <summary>
        /// Játék végének lekérdezése.
        /// </summary>

        public Boolean IsGameOver { get { return (_gameStepCount == 0); } }

        /// <summary>
        /// Játéknehézség lekérdezése, vagy beállítása.
        /// </summary>
        public GameDifficulty GameDifficulty { get { return _gameDifficulty; } set { _gameDifficulty = value; } }

        #endregion

        #region Events

        #endregion

        #region Constructor

        public RobotokGameModel(IRobotokDataAccess dataAccess, int selectedDifficulty, int teams)
        {
            switch(selectedDifficulty)
            {
                case 1:
                    _gameDifficulty = GameDifficulty.Easy;
                    _cleaningOperations = 2;
                    _remainingSeconds = 6;
                    break;
                case 2:
                    _gameDifficulty = GameDifficulty.Medium;
                    _cleaningOperations = 3;
                    _remainingSeconds = 5;
                    break;
                case 3:
                    _gameDifficulty = GameDifficulty.Hard;
                    _cleaningOperations = 4;
                    _remainingSeconds = 4;
                    break;

            }

            _teams = teams;
            _figure1 = new Shape();
            _figure2 = new Shape();
            _tableNoticeBoardOne = new RobotokTable(4, 4);
            _tableNoticeBoardTwo = new RobotokTable(4, 4);
            _dataAccess = dataAccess;
        }

        #endregion

        #region Public game methods

        /// <summary>
        /// Új játék kezdése.
        /// </summary>
        public void NewGame()
        {
            // Táblák létrehozása
            _table = new RobotokTable(17, 28);
            _tableGreenPlayerOne = new RobotokTable(11, 20);
            _tableGreenPlayerTwo = new RobotokTable(11, 20);
            if (_teams == 2) { 
                _tableRedPlayerOne = new RobotokTable(11, 20);
                _tableRedPlayerTwo = new RobotokTable(11, 20);
            }
            _remainingSeconds = 5; // műveletek közötti gondolkodási idő
            _gameStepCount = 300; // játék kezdeti lépésszáma, folyamatosan csökken, 0-nál játék vége
            // Kezdeti értékek generálása a mezőknek
            GenerateFields();
            

            // Beállítjuk a hirdetőtáblákban lévő alakzatoknak a színét
            for (int i = 0; i < _figure1.Figure.GetLength(0); ++i)
            {
                for (int j = 0; j < _figure1.Figure.GetLength(1); ++j)
                {
                    if (_figure1.GetFieldValue(i,j) != 0)
                    {
                        _tableNoticeBoardOne.SetValue(i, j, _figure1.GetColor(), _cleaningOperations);
                    } 
                    else
                    {
                        _tableNoticeBoardOne.SetValue(i, j, 7, _cleaningOperations);
                    }
                }
            }

            for (int i = 0; i < _figure2.Figure.GetLength(0); ++i)
            {
                for (int j = 0; j < _figure2.Figure.GetLength(1); ++j)
                {
                    if (_figure2.GetFieldValue(i, j) != 0)
                    {
                        _tableNoticeBoardTwo.SetValue(i, j, _figure2.GetColor(), _cleaningOperations);
                    }
                    else
                    {
                        _tableNoticeBoardTwo.SetValue(i, j, 7, _cleaningOperations);
                    }
                }
            }
        }

        /// <summary>
        /// Játékidő léptetése.
        /// </summary>
        public void AdvanceTime()
        {
            if (IsGameOver) // ha már vége, nem folytathatjuk
            {
                return;
            }

            _remainingSeconds--; 

            if (_remainingSeconds == -1) // ha lejárt a lépések közötti idő, újraindítjuk a visszaszámlálást, majd végrehajtuk a megadott játékműveletet
            {
                switch (GameDifficulty)
                {
                    case GameDifficulty.Easy:
                        _remainingSeconds = 6;
                        break;
                    case GameDifficulty.Medium:
                        _remainingSeconds = 5;
                        break;
                    case GameDifficulty.Hard:
                        _remainingSeconds = 4;
                        break;
                }
                _gameStepCount--; // csökkenti a hátralevő lépések számát

            }
        }

        #endregion

        #region Private game methods

        /// <summary>
        /// Mezők generálása.
        /// </summary>
        private void GenerateFields()
        {
            Random random = new Random();

            for (Int32 i = 0; i < _table.SizeX; i++)
            {
                for (Int32 j = 0; j < _table.SizeY; j++)
                {
                    // Azoknak a mezőknek az esete, amelyek a pálya határán túl helyezkednek el
                    if (i < 3 || i > 13 || j < 4 || j > 23)
                    {
                        _table.SetValue(i, j, -2, -1); // A pálya határán túli mezők törhetetlenek, ezért -1 a tisztító műveletük
                    }
                    // A határt képező mezők esete
                    else if (i == 3 || i == 13 || j == 4 || j == 23)
                    {
                        _table.SetValue(i, j, -1, -1); // A pálya határán túli mezők törhetetlenek, ezért -1 a tisztító műveletük
                    }
                    // A többi mező kitöltése (általánosan, játékosok, akadályok, kijáratok mezői még nincsenek)
                    else
                    {
                        _table.SetValue(i, j, 7, _cleaningOperations);
                    }
                }
            }
            // Játékosok mezőinek random generálása
            int greenPlayerOne_i, greenPlayerOne_j, greenPlayerTwo_i, greenPlayerTwo_j;
            greenPlayerOne_i = random.Next(4, 13);
            greenPlayerOne_j = random.Next(5, 23);
            greenPlayerTwo_i = random.Next(4, 13);
            greenPlayerTwo_j = random.Next(5, 23);

            while (greenPlayerOne_i == greenPlayerTwo_j && greenPlayerTwo_j == greenPlayerOne_j)
            {
                greenPlayerOne_j = random.Next(5, 23);
            }

            _table.SetValue(greenPlayerOne_i, greenPlayerOne_j, 1, -1);
            _table.SetValue(greenPlayerTwo_i, greenPlayerTwo_j, 8, -1);

            if (_teams == 2)
            {
                int redPlayerOne_i, redPlayerOne_j, redPlayerTwo_i, redPlayerTwo_j;
                redPlayerOne_i = random.Next(4, 13);
                redPlayerOne_j = random.Next(5, 23);
                redPlayerTwo_i = random.Next(4, 13);
                redPlayerTwo_j = random.Next(5, 23);
                

                while (_table.GetFieldValue(redPlayerOne_i ,redPlayerOne_j) != 7)
                {
                    redPlayerOne_i = random.Next(4, 13);
                    redPlayerOne_j = random.Next(5, 23);
                }

                while (_table.GetFieldValue(redPlayerTwo_i, redPlayerTwo_j) != 7)
                {
                    redPlayerTwo_i = random.Next(4, 13);
                    redPlayerTwo_j = random.Next(5, 23);
                }

                _table.SetValue(redPlayerOne_i, redPlayerOne_j, 2, -1);
                _table.SetValue(redPlayerTwo_i, greenPlayerTwo_j, 9, -1);
            }

            //Alakzatoknak az építőelemeinek generálása
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    for (int x = 0; x < _figure1.Figure.GetLength(0); ++x)
                    {
                        for (int y = 0; y < _figure1.Figure.GetLength(1); ++y)
                        {
                            if (_figure1.GetFieldValue(x, y) != 0)
                            {
                                Int32 figureOne_i = random.Next(4, 13);
                                Int32 figureOne_j = random.Next(5, 23);
                                while (_table.GetFieldValue(figureOne_i, figureOne_j) != 7)
                                {
                                    figureOne_i = random.Next(4, 13);
                                    figureOne_j = random.Next(5, 23);
                                }

                                _table.SetValue(figureOne_i, figureOne_j, _figure1.GetColor(), _cleaningOperations);
                            }

                        }
                    }
                }
                else
                {
                    for (int x = 0; x < _figure2.Figure.GetLength(0); ++x)
                    {
                        for (int y = 0; y < _figure2.Figure.GetLength(1); ++y)
                        {
                            if (_figure2.GetFieldValue(x, y) != 0)
                            {
                                Int32 figureTwo_i = random.Next(4, 13);
                                Int32 figureTwo_j = random.Next(5, 23);
                                while (_table.GetFieldValue(figureTwo_i, figureTwo_j) != 7)
                                {
                                    figureTwo_i = random.Next(4, 13);
                                    figureTwo_j = random.Next(5, 23);
                                }

                                _table.SetValue(figureTwo_i, figureTwo_j, _figure2.GetColor(), _cleaningOperations);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Manhattan távolság.
        /// </summary>
        public void ManhattanDistance(int _difficulty, int player)
        {
            int tempX = 0;
            int tempY = 0;

            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (_table.GetFieldValue(i,j) == player)
                    {
                        tempX = i;
                        tempY = j;
                    }
                }
            }

            int tempDistance = _ManhattanDistanceEasy;
            switch (_difficulty)
            {
                case 1:
                    tempDistance = _ManhattanDistanceEasy;
                    break;
                case 2:
                    tempDistance = _ManhattanDistanceMedium;
                    break;
                case 3:
                    tempDistance = _ManhattanDistanceHard;
                    break;
            }


            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (i > 3 && i < 13 && j > 4 && j < 23)
                    {
                        if (Math.Abs(i - tempX) + Math.Abs(j - tempY) < tempDistance)
                        {
                            if (player == 1)
                            {
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i,j));
                            }

                            if (player == 8)
                            {
                                _tableGreenPlayerTwo.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                            }
                            
                            if (_teams == 2)
                            {
                                if (player == 2)
                                {
                                    _tableRedPlayerOne.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                }

                                if (player == 9)
                                {
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                }
                            }
                        } 
                        else
                        {
                            if (player == 1)
                            {
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, 10, _table.GetFieldRemainingCleaningOperations(i, j));
                            }

                            if (player == 8)
                            {
                                _tableGreenPlayerTwo.SetValue(i - 3, j - 4, 10, _table.GetFieldRemainingCleaningOperations(i, j));
                            }
                            if (_teams == 2)
                            {
                                if (player == 2)
                                {
                                    _tableRedPlayerOne.SetValue(i - 3, j - 4, 10, _table.GetFieldRemainingCleaningOperations(i, j));
                                }

                                if (player == 9)
                                {
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, 10, _table.GetFieldRemainingCleaningOperations(i, j));
                                }
                            }
                        }
                    }
                    else if ((i == 3 || i == 13) && (j >= 4 && j <= 23) || (i >= 3 && i <= 13) && (j == 4 || j == 23))
                    {
                        if (player == 1)
                        {
                            _tableGreenPlayerOne.SetValue(i - 3, j - 4, -1, -1);
                        }

                        if (player == 8)
                        {
                            _tableGreenPlayerTwo.SetValue(i - 3, j - 4, -1, -1);
                        }

                        if (_teams == 2)
                        {
                            if (player == 2)
                            {
                                _tableRedPlayerOne.SetValue(i - 3, j - 4, -1, -1);
                            }

                            if (player == 9)
                            {
                                _tableRedPlayerTwo.SetValue(i - 3, j - 4, -1, -1);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Várakozás logikája
        /// </summary>
        public void Wait()
        {
            _remainingSeconds = 0;
        }

        /// <summary>
        /// Lépés logikája
        /// </summary>
        public void Move(String direction, int playerNumber)
        {
            if(direction == "észak")
            {
                for(int i = 4; i < 13; i++)
                {
                    for(int j = 5; j < 23; j++)
                    {
                        if(_table.GetFieldValue(i,j) == playerNumber)
                        {
                            if(_table.GetFieldValue(i - 1, j) == 7)
                            {
                                _table.SetValue(i, j, 7, _cleaningOperations);
                                _table.SetValue(i - 1, j, playerNumber, _cleaningOperations);
                                break;
                            }
                        }
                    }
                }
            }
            else if(direction == "dél")
            {
                for (int i = 12; i > 3; i--)
                {
                    for (int j = 22; j > 4; j--)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (_table.GetFieldValue(i + 1, j) == 7)
                            {
                                _table.SetValue(i, j, 7, _cleaningOperations);
                                _table.SetValue(i + 1, j, playerNumber, _cleaningOperations);
                                break;
                            }
                        }
                    }
                }
            }
            else if(direction == "kelet")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (_table.GetFieldValue(i, j - 1) == 7)
                            {
                                _table.SetValue(i, j, 7, _cleaningOperations);
                                _table.SetValue(i, j - 1, playerNumber, _cleaningOperations);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 12; i > 3; i--)
                {
                    for (int j = 22; j > 4; j--)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (_table.GetFieldValue(i , j + 1) == 7)
                            {
                                _table.SetValue(i, j, 7, _cleaningOperations);
                                _table.SetValue(i , j + 1, playerNumber, _cleaningOperations);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Észlelés.
        /// </summary>
        private void Observation()
        {

        }

        #endregion

        #region Private event methods

        #endregion

    }
}
