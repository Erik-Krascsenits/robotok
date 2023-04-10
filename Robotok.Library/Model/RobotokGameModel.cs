using ELTE.Robotok.Persistence;
using Robotok.Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

class Coordinates // az összekapcsolt kockáknak a struktúrája
{
    public Int32 x;
    public Int32 y;
    public Int32 color;
}

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
        private Int32[,] _greenTeamObservation;
        private Int32[,] _redTeamObservation;
        private Boolean _SyncGreenPlayerOne; // zöld csapatban 1. játékos látta-e a zöld 2. játékost
        private Boolean _SyncGreenPlayerTwo; // zöld csapatban 2. játékos látta-e a zöld 1. játékost
        private Boolean _SyncRedPlayerOne; // piros csapatban 1. játékos látta-e a piros 2. játékost
        private Boolean _SyncRedPlayerTwo; // piros csapatban 2. játékos látta-e a piros 1. játékost
        private List<Coordinates> _playerOneTeamGreenSpace; // 1. zöld játékosnak a kockai
        private List<Coordinates> _playerTwoTeamGreenSpace; // 2. zöld játékosnak a kockai
        private List<Coordinates> _playerOneTeamRedSpace; // 1. piros játékosnak a kockai
        private List<Coordinates> _playerTwoTeamRedSpace; // 2. piros játékosnak a kockai

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
        /// Csapatak széma lekérdezése
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
        /// Játéktábla lekérdezése (piros csapataban 1. játékosé).
        /// </summary>
        public RobotokTable TableRedPlayerOne { get { return _tableRedPlayerOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (piros csapatban 2. játékosé).
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
            switch (selectedDifficulty)
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
            _greenTeamObservation = new Int32[11, 20];
            _redTeamObservation = new Int32[11, 20];
            _playerOneTeamGreenSpace = new List<Coordinates>();
            _playerTwoTeamGreenSpace = new List<Coordinates>();
            if (_teams == 2)
            {
                _tableRedPlayerOne = new RobotokTable(11, 20);
                _tableRedPlayerTwo = new RobotokTable(11, 20);
                _playerOneTeamRedSpace = new List<Coordinates>();
                _playerTwoTeamRedSpace = new List<Coordinates>();
            }
            _remainingSeconds = 5; // műveletek közötti gondolkodási idő
            _gameStepCount = 300; // játék kezdeti lépésszáma, folyamatosan csökken, 0-nál játék vége

            _SyncGreenPlayerOne = false;
            _SyncGreenPlayerTwo = false;
            _SyncRedPlayerOne = false;
            _SyncRedPlayerTwo = false;
            for (int i = 0; i < 11; i++) // játékosok tábláját feltöltjük nem látható mezőkkel
            {
                for (int j = 0; j < 20; j++)
                {
                    _greenTeamObservation[i, j] = 0;
                    _tableGreenPlayerOne.SetValue(i, j, 10, _cleaningOperations);
                    _tableGreenPlayerTwo.SetValue(i, j, 10, _cleaningOperations);
                    if (_teams == 2)
                    {
                        _redTeamObservation[i, j] = 0;
                        _tableRedPlayerOne.SetValue(i, j, 10, _cleaningOperations);
                        _tableRedPlayerTwo.SetValue(i, j, 10, _cleaningOperations);
                    }
                }
            }

            // Kezdeti értékek generálása a mezőknek
            GenerateFields();


            // Beállítjuk a hirdetőtáblákon lévő alakzatoknak a színét
            for (int i = 0; i < _figure1.Figure.GetLength(0); ++i)
            {
                for (int j = 0; j < _figure1.Figure.GetLength(1); ++j)
                {
                    if (_figure1.GetFieldValue(i, j) != 0)
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
        /// Manhattan távolság.
        /// </summary>
        public void ManhattanDistance(int _difficulty, int player)
        {
            int tempX = 0;
            int tempY = 0;

            for (int i = 0; i < _table.SizeX; i++) // megkeressük a táblán a játékosat
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (_table.GetFieldValue(i, j) == player)
                    {
                        tempX = i;
                        tempY = j;
                    }
                }
            }

            int tempDistance = _ManhattanDistanceEasy; // nézzük, hogy milyen a nehézség
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




            Boolean toMerge = false;

            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (i > 3 && i < 13 && j > 4 && j < 23) // játék pályán vagyunk-e
                    {
                        if (Math.Abs(i - tempX) + Math.Abs(j - tempY) < tempDistance) // ha igen, akkor megnézzük, hogy benne van a mező a Manhattan távolságban.
                        {
                            if (player == 1)
                            {
                                _greenTeamObservation[i - 3, j - 4] = 1;
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j)); // minden játékosnak külön van egy saját "pálya", amin megjelenítjük a Manhattan távolságot
                                if (_table.GetFieldValue(i, j) == 8) // Ha a csapattárs benne van a manhattan távolságban, akkor a két játékos nézetét egyesíteni kell 
                                {
                                    toMerge = true;
                                }
                            }

                            if (player == 8)
                            {
                                _greenTeamObservation[i - 3, j - 4] = 8;
                                _tableGreenPlayerTwo.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                if (_table.GetFieldValue(i, j) == 1)
                                {
                                    toMerge = true;
                                }
                            }

                            if (_teams == 2)
                            {
                                if (player == 2)
                                {
                                    _redTeamObservation[i - 3, j - 4] = 2;
                                    _tableRedPlayerOne.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                    if (_table.GetFieldValue(i, j) == 9)
                                    {
                                        toMerge = true;
                                    }
                                }

                                if (player == 9)
                                {
                                    _redTeamObservation[i - 3, j - 4] = 9;
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                    if (_table.GetFieldValue(i, j) == 2)
                                    {
                                        toMerge = true;
                                    }
                                }
                            }
                        }  // Ez a rész nem kell abban az esetben, ha előre feltöltjük a mezőket üresre
                        /*else // abban az esetben ha Manhattan távolságon kívül vagyunk, akkor minket nem érdekel hogy milyen mező van ott, ezért 10-esekkel jelöljük őket
                        {
                            if (_gameStepCount == 300 ) //Ha ez az első kör, akkor az észlelési területen kívül szürke lesz minden
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
                        }*/
                    }
                    else if ((i == 3 || i == 13) && (j >= 4 && j <= 23) || (i >= 3 && i <= 13) && (j == 4 || j == 23)) // de ugy a pálya határa az mindenképp kell, hogy megjelenjen
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
            if (toMerge) //Ha volt Manhattan távolságon belül csapattárs, akkor belépünk az if-be, és egyesítünk
            {
                Merge(player);
            }

            switch (player) //Ha az egyik játékos már látta a másikat, akkor frissítjük a másik csapattárs területét a jelenleg észlelt területtel
            {
                case 1:
                    if (_SyncGreenPlayerTwo)
                    {
                        Observation(8, 1, tempX, tempY, tempDistance);
                    }
                    break;
                case 8:

                    if (_SyncGreenPlayerOne)
                    {
                        Observation(1, 8, tempX, tempY, tempDistance);
                    }
                    break;
                case 2:
                    if (_SyncRedPlayerOne)
                    {
                        Observation(9, 2, tempX, tempY, tempDistance);
                    }
                    break;
                case 9:
                    if (_SyncRedPlayerTwo)
                    {
                        Observation(2, 9, tempX, tempY, tempDistance);
                    }
                    break;
            }

        }
        /// <summary>
        /// Játékidő léptetése.
        /// </summary>
        public void AdvanceTime(int player)
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
                if (player == 1)
                {
                    _gameStepCount--; // csökkenti a hátralevő lépések számát, ha az első játékos következik
                }

            }
        }
        /// <summary>
        /// Tisztítás logikája
        /// </summary>
        public Boolean Clear(String direction, int playerNumber)
        {
            int num = 0;
            Boolean success = false;
            switch (playerNumber)
            {
                case 1:
                    num = 1;
                    break;
                case 2:
                    num = 8;
                    break;
                case 3:
                    num = 2;
                    break;

                case 4:
                    num = 9;
                    break;
            }


            if (direction == "észak")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i - 1, j) == 0 || _table.GetFieldValue(i - 1, j) == 3 || _table.GetFieldValue(i - 1, j) == 4 || _table.GetFieldValue(i - 1, j) == 5 || _table.GetFieldValue(i - 1, j) == 6)
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i - 1, j) != 1)
                                {
                                    _table.SetValue(i - 1, j, _table.GetFieldValue(i - 1, j), _table.GetFieldRemainingCleaningOperations(i - 1, j) - 1);
                                }
                                else
                                {
                                    _table.SetValue(i - 1, j, 7, _cleaningOperations);
                                }
                                success = true;
                                break;
                            }
                        }
                    }
                }
            }
            else if (direction == "dél")
            {
                for (int i = 12; i > 3; i--)
                {
                    for (int j = 22; j > 4; j--)
                    {
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i + 1, j) == 0 || _table.GetFieldValue(i + 1, j) == 3 || _table.GetFieldValue(i + 1, j) == 4 || _table.GetFieldValue(i + 1, j) == 5 || _table.GetFieldValue(i + 1, j) == 6)
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i + 1, j) != 1)
                                {
                                    _table.SetValue(i + 1, j, _table.GetFieldValue(i + 1, j), _table.GetFieldRemainingCleaningOperations(i + 1, j) - 1);
                                }
                                else
                                {
                                    _table.SetValue(i + 1, j, 7, _cleaningOperations);
                                }
                                success = true;
                                break;
                            }
                        }
                    }
                }
            }
            else if (direction == "nyugat")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i, j - 1) == 0 || _table.GetFieldValue(i, j - 1) == 3 || _table.GetFieldValue(i, j - 1) == 4 || _table.GetFieldValue(i, j - 1) == 5 || _table.GetFieldValue(i, j - 1) == 6)
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i, j - 1) != 1)
                                {
                                    _table.SetValue(i, j - 1, _table.GetFieldValue(i, j - 1), _table.GetFieldRemainingCleaningOperations(i, j - 1) - 1);
                                }
                                else
                                {
                                    _table.SetValue(i, j - 1, 7, _cleaningOperations);
                                }
                                success = true;
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
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i, j + 1) == 0 || _table.GetFieldValue(i, j + 1) == 3 || _table.GetFieldValue(i, j + 1) == 4 || _table.GetFieldValue(i, j + 1) == 5 || _table.GetFieldValue(i, j + 1) == 6)
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i, j + 1) != 1)
                                {
                                    _table.SetValue(i, j + 1, _table.GetFieldValue(i, j + 1), _table.GetFieldRemainingCleaningOperations(i, j + 1) - 1);
                                }
                                else
                                {
                                    _table.SetValue(i, j + 1, 7, _cleaningOperations);
                                }
                                success = true;
                                break;
                            }
                        }
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Lekapcsolás logikája
        /// </summary>
        public Boolean Dettach(String direction, int playerNumber)
        {
            int num = 0;
            Boolean success = false;
            switch (playerNumber)
            {
                case 1:
                    num = 1;
                    break;
                case 2:
                    num = 8;
                    break;
                case 3:
                    num = 2;
                    break;

                case 4:
                    num = 9;
                    break;
            }

            Coordinates tempCubeToDettach = new Coordinates();

            if (num == 1 && _playerOneTeamGreenSpace.Count() >= 1)
            {
                tempCubeToDettach.color = _playerOneTeamGreenSpace.Last().color;
                tempCubeToDettach.x = _playerOneTeamGreenSpace.Last().x;
                tempCubeToDettach.y = _playerOneTeamGreenSpace.Last().y;
            }
            else if (num == 8 && _playerTwoTeamGreenSpace.Count() >= 1)
            {
                tempCubeToDettach.color = _playerTwoTeamGreenSpace.Last().color;
                tempCubeToDettach.x = _playerTwoTeamGreenSpace.Last().x;
                tempCubeToDettach.y = _playerTwoTeamGreenSpace.Last().y;
            }

            if (_teams == 2)
            {
                if (num == 2 && _playerOneTeamRedSpace.Count() >= 1)
                {
                    tempCubeToDettach.color = _playerOneTeamRedSpace.Last().color;
                    tempCubeToDettach.x = _playerOneTeamRedSpace.Last().x;
                    tempCubeToDettach.y = _playerOneTeamRedSpace.Last().y;
                }
                else if (num == 9 && _playerTwoTeamRedSpace.Count() >= 1)
                {
                    tempCubeToDettach.color = _playerTwoTeamRedSpace.Last().color;
                    tempCubeToDettach.x = _playerTwoTeamRedSpace.Last().x;
                    tempCubeToDettach.y = _playerTwoTeamRedSpace.Last().y;
                }
            }

            if (direction == "észak")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (i == tempCubeToDettach.x && j == tempCubeToDettach.y)
                        {
                            success = true;
                            _table.SetValue(i, j, tempCubeToDettach.color, _cleaningOperations);

                            if (num == 1)
                            {
                                _playerOneTeamGreenSpace.RemoveAt(_playerOneTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 8)
                            {
                                _playerTwoTeamGreenSpace.RemoveAt(_playerTwoTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 2)
                            {
                                _playerOneTeamRedSpace.RemoveAt(_playerOneTeamRedSpace.Count() - 1);
                            }
                            else
                            {
                                _playerTwoTeamRedSpace.RemoveAt(_playerTwoTeamRedSpace.Count() - 1);
                            }
                        }
                    }
                }
            }
            else if (direction == "dél")
            {
                for (int i = 12; i > 3; i--)
                {
                    for (int j = 22; j > 4; j--)
                    {
                        if (i == tempCubeToDettach.x && j == tempCubeToDettach.y)
                        {
                            success = true;
                            _table.SetValue(i, j, tempCubeToDettach.color, _cleaningOperations);

                            if (num == 1)
                            {
                                _playerOneTeamGreenSpace.RemoveAt(_playerOneTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 8)
                            {
                                _playerTwoTeamGreenSpace.RemoveAt(_playerTwoTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 2)
                            {
                                _playerOneTeamRedSpace.RemoveAt(_playerOneTeamRedSpace.Count() - 1);
                            }
                            else
                            {
                                _playerTwoTeamRedSpace.RemoveAt(_playerTwoTeamRedSpace.Count() - 1);
                            }
                        }
                    }
                }
            }
            else if (direction == "nyugat")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (i == tempCubeToDettach.x && j == tempCubeToDettach.y)
                        {
                            success = true;
                            _table.SetValue(i, j, tempCubeToDettach.color, _cleaningOperations);

                            if (num == 1)
                            {
                                _playerOneTeamGreenSpace.RemoveAt(_playerOneTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 8)
                            {
                                _playerTwoTeamGreenSpace.RemoveAt(_playerTwoTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 2)
                            {
                                _playerOneTeamRedSpace.RemoveAt(_playerOneTeamRedSpace.Count() - 1);
                            }
                            else
                            {
                                _playerTwoTeamRedSpace.RemoveAt(_playerTwoTeamRedSpace.Count() - 1);
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
                        if (i == tempCubeToDettach.x && j == tempCubeToDettach.y)
                        {
                            success = true;
                            _table.SetValue(i, j, tempCubeToDettach.color, _cleaningOperations);

                            if (num == 1)
                            {
                                _playerOneTeamGreenSpace.RemoveAt(_playerOneTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 8)
                            {
                                _playerTwoTeamGreenSpace.RemoveAt(_playerTwoTeamGreenSpace.Count() - 1);
                            }
                            else if (num == 2)
                            {
                                _playerOneTeamRedSpace.RemoveAt(_playerOneTeamRedSpace.Count() - 1);
                            }
                            else
                            {
                                _playerTwoTeamRedSpace.RemoveAt(_playerTwoTeamRedSpace.Count() - 1);
                            }
                        }
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Rákapcsolás logikája
        /// </summary>
        public Boolean Attach(String direction, int playerNumber)
        {
            int num = 0;
            Boolean success = false;
            switch (playerNumber)
            {
                case 1:
                    num = 1;
                    break;
                case 2:
                    num = 8;
                    break;
                case 3:
                    num = 2;
                    break;

                case 4:
                    num = 9;
                    break;
            }

            Coordinates tempCubeToConnectTo = new Coordinates();
            tempCubeToConnectTo.color = num;
            for (int i = 4; i < 13; i++)
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_table.GetFieldValue(i,j) == num)
                    {
                        tempCubeToConnectTo.x = i;
                        tempCubeToConnectTo.y = j;
                    }
                }
            }

            if (num == 1 && _playerOneTeamGreenSpace.Count() >= 1)
            {
                tempCubeToConnectTo.color = _playerOneTeamGreenSpace.Last().color;
                tempCubeToConnectTo.x = _playerOneTeamGreenSpace.Last().x;
                tempCubeToConnectTo.y = _playerOneTeamGreenSpace.Last().y;
            }
            else if (num == 8 && _playerTwoTeamGreenSpace.Count() >= 1)
            {
                tempCubeToConnectTo.color = _playerTwoTeamGreenSpace.Last().color;
                tempCubeToConnectTo.x = _playerTwoTeamGreenSpace.Last().x;
                tempCubeToConnectTo.y = _playerTwoTeamGreenSpace.Last().y;
            }

            if (_teams == 2)
            {
                if (num == 2 && _playerOneTeamRedSpace.Count() >= 1)
                {
                    tempCubeToConnectTo.color = _playerOneTeamRedSpace.Last().color;
                    tempCubeToConnectTo.x = _playerOneTeamRedSpace.Last().x;
                    tempCubeToConnectTo.y = _playerOneTeamRedSpace.Last().y;
                }
                else if (num == 9 && _playerTwoTeamRedSpace.Count() >= 1)
                {
                    tempCubeToConnectTo.color = _playerTwoTeamRedSpace.Last().color;
                    tempCubeToConnectTo.x = _playerTwoTeamRedSpace.Last().x;
                    tempCubeToConnectTo.y = _playerTwoTeamRedSpace.Last().y;
                }
            }

            if (direction == "észak")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (i == tempCubeToConnectTo.x && j == tempCubeToConnectTo.y)
                        {
                            if (_table.GetFieldValue(i - 1, j) == 3 || _table.GetFieldValue(i - 1, j) == 4 || _table.GetFieldValue(i - 1, j) == 5 
                                || _table.GetFieldValue(i - 1, j) == 6)
                            {
                                success = true;
                                Coordinates temp = new Coordinates();
                                temp.x = i - 1;
                                temp.y = j;
                                temp.color = _table.GetFieldValue(temp.x, temp.y);

                                if (num == 1)
                                {
                                    _playerOneTeamGreenSpace.Add(temp);
                                }
                                else if (num == 8)
                                {
                                    _playerTwoTeamGreenSpace.Add(temp);
                                }
                                else if (num == 2)
                                {
                                    _playerOneTeamRedSpace.Add(temp);
                                }
                                else
                                {
                                    _playerTwoTeamRedSpace.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
            else if (direction == "dél")
            {
                for (int i = 12; i > 3; i--)
                {
                    for (int j = 22; j > 4; j--)
                    {
                        if (i == tempCubeToConnectTo.x && j == tempCubeToConnectTo.y)
                        {
                            if (_table.GetFieldValue(i + 1, j) == 3 || _table.GetFieldValue(i + 1, j) == 4 || _table.GetFieldValue(i + 1, j) == 5 || _table.GetFieldValue(i + 1, j) == 6)
                            {
                                success = true;
                                Coordinates temp = new Coordinates();
                                temp.x = i + 1;
                                temp.y = j;
                                temp.color = _table.GetFieldValue(temp.x, temp.y);

                                if (num == 1)
                                {
                                    _playerOneTeamGreenSpace.Add(temp);
                                }
                                else if (num == 8)
                                {
                                    _playerTwoTeamGreenSpace.Add(temp);
                                }
                                else if (num == 2)
                                {
                                    _playerOneTeamRedSpace.Add(temp);
                                }
                                else
                                {
                                    _playerTwoTeamRedSpace.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
            else if (direction == "nyugat")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (i == tempCubeToConnectTo.x && j == tempCubeToConnectTo.y)
                        {
                            if (_table.GetFieldValue(i, j - 1) == 3 || _table.GetFieldValue(i, j - 1) == 4 || _table.GetFieldValue(i, j - 1) == 5 || _table.GetFieldValue(i, j - 1) == 6)
                            {
                                success = true;
                                Coordinates temp = new Coordinates();
                                temp.x = i;
                                temp.y = j - 1;
                                temp.color = _table.GetFieldValue(temp.x, temp.y);

                                if (num == 1)
                                {
                                    _playerOneTeamGreenSpace.Add(temp);
                                }
                                else if (num == 8)
                                {
                                    _playerTwoTeamGreenSpace.Add(temp);
                                }
                                else if (num == 2)
                                {
                                    _playerOneTeamRedSpace.Add(temp);
                                }
                                else
                                {
                                    _playerTwoTeamRedSpace.Add(temp);
                                }
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
                        if (i == tempCubeToConnectTo.x && j == tempCubeToConnectTo.y)
                        {
                            if (_table.GetFieldValue(i, j + 1) == 3 || _table.GetFieldValue(i, j + 1) == 4 || _table.GetFieldValue(i, j + 1) == 5 || _table.GetFieldValue(i, j + 1) == 6)
                            {
                                success = true;
                                Coordinates temp = new Coordinates();
                                temp.x = i;
                                temp.y = j + 1;
                                temp.color = _table.GetFieldValue(temp.x, temp.y);

                                if (num == 1)
                                {
                                    _playerOneTeamGreenSpace.Add(temp);
                                }
                                else if (num == 8)
                                {
                                    _playerTwoTeamGreenSpace.Add(temp);
                                }
                                else if (num == 2)
                                {
                                    _playerOneTeamRedSpace.Add(temp);
                                }
                                else
                                {
                                    _playerTwoTeamRedSpace.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
            return success;
        }
        /// <summary>
        /// Várakozás logikája
        /// </summary>
        public void Wait()
        {
            _remainingSeconds = 1;
        }
        /// <summary>
        /// Lépés logikája
        /// </summary>
        public Boolean Move(String direction, int playerNumber) // még nem csináltam meg végig
        {
            Boolean success = false;
            int num = 0;
            switch (playerNumber)
            {
                case 1:
                    num = 1;
                    break;
                case 2:
                    num = 8;
                    break;
                case 3:
                    num = 2;
                    break;

                case 4:
                    num = 9;
                    break;
            }

            if (direction == "észak")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i - 1, j) == 7)
                            {
                                if (num == 1 && _playerOneTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x - 1, coord.y) != 7 && _table.GetFieldValue(coord.x - 1, coord.y) != 1)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x - 1, coord.y, coord.color, -1);
                                        coord.x = coord.x - 1;
                                    }
                                }
                                else if (num == 8 && _playerTwoTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x - 1, coord.y) != 7 && _table.GetFieldValue(coord.x - 1, coord.y) != 8)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x - 1, coord.y, coord.color, -1);
                                        coord.x = coord.x - 1;
                                    }
                                }
                                if (_teams == 2)
                                {
                                    if (num == 2 && _playerOneTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x - 1, coord.y) != 7 && _table.GetFieldValue(coord.x - 1, coord.y) != 2)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x - 1, coord.y, coord.color, -1);
                                            coord.x = coord.x - 1;

                                        }
                                    }
                                    else if (num == 9 && _playerTwoTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x - 1, coord.y) != 7 && _table.GetFieldValue(coord.x - 1, coord.y) != 9)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x - 1, coord.y, coord.color, -1);
                                            coord.x = coord.x - 1;
                                        }
                                    }
                                }
                                if (_table.GetFieldValue(i, j) != 3 && _table.GetFieldValue(i, j) != 4 && _table.GetFieldValue(i, j) != 5 && _table.GetFieldValue(i, j) != 6)
                                {
                                    _table.SetValue(i, j, 7, -1);
                                }
                                success = true;
                                _table.SetValue(i - 1, j, num, -1);
                                break;
                            }
                        }
                    }
                }
            }
            else if (direction == "dél")
            {
                for (int i = 12; i > 3; i--)
                {
                    for (int j = 22; j > 4; j--)
                    {
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i + 1, j) == 7)
                            {
                                if (num == 1 && _playerOneTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x + 1, coord.y) != 7 && _table.GetFieldValue(coord.x + 1, coord.y) != 1)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x + 1, coord.y, coord.color, -1);
                                        coord.x = coord.x + 1;
                                    }
                                }
                                else if (num == 8 && _playerTwoTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x + 1, coord.y) != 7 && _table.GetFieldValue(coord.x + 1, coord.y) != 8)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x + 1, coord.y, coord.color, -1);
                                        coord.x = coord.x + 1;
                                    }
                                }

                                if (_teams == 2)
                                {
                                    if (num == 2 && _playerOneTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x + 1, coord.y) != 7 && _table.GetFieldValue(coord.x + 1, coord.y) != 2)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x + 1, coord.y, coord.color, -1);
                                            coord.x = coord.x + 1;
                                        }
                                    }
                                    else if (num == 9 && _playerTwoTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x + 1, coord.y) != 7 && _table.GetFieldValue(coord.x + 1, coord.y) != 9)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x + 1, coord.y, coord.color, -1);
                                            coord.x = coord.x + 1;
                                        }
                                    }
                                }

                                if (_table.GetFieldValue(i, j) != 3 && _table.GetFieldValue(i, j) != 4 && _table.GetFieldValue(i, j) != 5 && _table.GetFieldValue(i, j) != 6)
                                {
                                    _table.SetValue(i, j, 7, -1);
                                }
                                success = true;
                                _table.SetValue(i + 1, j, num, -1);
                                break;
                            }
                        }
                    }
                }
            }
            else if (direction == "nyugat")
            {
                for (int i = 4; i < 13; i++)
                {
                    for (int j = 5; j < 23; j++)
                    {
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i, j - 1) == 7)
                            {
                                if (num == 1 && _playerOneTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x, coord.y - 1) != 7 && _table.GetFieldValue(coord.x, coord.y - 1) != 1)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x, coord.y - 1, coord.color, -1);
                                        coord.y = coord.y - 1;
                                    }
                                }
                                else if (num == 8 && _playerTwoTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x, coord.y - 1) != 7 && _table.GetFieldValue(coord.x, coord.y - 1) != 8)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x, coord.y - 1, coord.color, -1);
                                        coord.y = coord.y - 1;
                                    }
                                }

                                if (_teams == 2)
                                {
                                    if (num == 2 && _playerOneTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x, coord.y - 1) != 7 && _table.GetFieldValue(coord.x, coord.y - 1) != 2)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x, coord.y - 1, coord.color, -1);
                                            coord.y = coord.y - 1;
                                        }
                                    }
                                    else if (num == 9 && _playerTwoTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x, coord.y - 1) != 7 && _table.GetFieldValue(coord.x, coord.y - 1) != 9)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x, coord.y - 1, coord.color, -1);
                                            coord.y = coord.y - 1;
                                        }
                                    }
                                }
                                if (_table.GetFieldValue(i, j) != 3 && _table.GetFieldValue(i, j) != 4 && _table.GetFieldValue(i, j) != 5 && _table.GetFieldValue(i, j) != 6)
                                {
                                    _table.SetValue(i, j, 7, -1);
                                }
                                success = true;
                                _table.SetValue(i, j - 1, num, -1);
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
                        if (_table.GetFieldValue(i, j) == num)
                        {
                            if (_table.GetFieldValue(i, j + 1) == 7)
                            {
                                if (num == 1 && _playerOneTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x, coord.y + 1) != 7 && _table.GetFieldValue(coord.x, coord.y + 1) != 1)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerOneTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x, coord.y + 1, coord.color, -1);
                                        coord.y = coord.y + 1;
                                    }
                                }
                                else if (num == 8 && _playerTwoTeamGreenSpace.Count() >= 1)
                                {
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        if (_table.GetFieldValue(coord.x, coord.y + 1) != 7 && _table.GetFieldValue(coord.x, coord.y + 1) != 8)
                                        {
                                            return success;
                                        }
                                    }
                                    foreach (Coordinates coord in _playerTwoTeamGreenSpace)
                                    {
                                        _table.SetValue(coord.x, coord.y, 7, -1);
                                        _table.SetValue(coord.x, coord.y + 1, coord.color, -1);
                                        coord.y = coord.y + 1;
                                    }
                                }

                                if (_teams == 2)
                                {
                                    if (num == 2 && _playerOneTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x, coord.y + 1) != 7 && _table.GetFieldValue(coord.x, coord.y + 1) != 2)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerOneTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x, coord.y + 1, coord.color, -1);
                                            coord.y = coord.y + 1;
                                        }
                                    }
                                    else if (num == 9 && _playerTwoTeamRedSpace.Count() >= 1)
                                    {
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            if (_table.GetFieldValue(coord.x, coord.y + 1) != 7 && _table.GetFieldValue(coord.x, coord.y + 1) != 9)
                                            {
                                                return success;
                                            }
                                        }
                                        foreach (Coordinates coord in _playerTwoTeamRedSpace)
                                        {
                                            _table.SetValue(coord.x, coord.y, 7, -1);
                                            _table.SetValue(coord.x, coord.y + 1, coord.color, -1);
                                            coord.y = coord.y + 1;
                                        }
                                    }
                                }
                                if (_table.GetFieldValue(i, j) != 3 && _table.GetFieldValue(i, j) != 4 && _table.GetFieldValue(i, j) != 5 && _table.GetFieldValue(i, j) != 6)
                                {
                                    _table.SetValue(i, j, 7, -1);
                                }
                                success = true;
                                _table.SetValue(i, j + 1, num, -1);
                                break;
                            }
                        }
                    }
                }
            }
            return success;
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

            while (greenPlayerOne_i == greenPlayerTwo_i && greenPlayerTwo_j == greenPlayerOne_j)
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


                while (_table.GetFieldValue(redPlayerOne_i, redPlayerOne_j) != 7) // megnézzük, hogy a táblán a véletlenszerűen kiválasztott mezőnek mi az értéke, ha nem(7), azaz üres mező, akkor tovább generálunk egy új értéket
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

            //Alakzatoknak az építőkockainek generálása
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
        /// Észlelés.
        /// </summary>
        private void Observation(int player, int player2, int posX, int posY, int distance)
        {
            int playerTwoPosX = 0;
            int playerTwoPosY = 0;
            for (int i = 0; i < _table.SizeX; i++) // megkeressük a táblán a csapattársat
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (_table.GetFieldValue(i, j) == player2)
                    {
                        playerTwoPosX = i; // eltároljuk a pozícióját
                        playerTwoPosY = j;
                    }
                }
            }


            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (i > 3 && i < 13 && j > 4 && j < 23) // játék pályán vagyunk-e
                    {
                        if (Math.Abs(i - playerTwoPosX) + Math.Abs(j - playerTwoPosY) < distance) // ha igen, akkor megnézzük, hogy benne van-e a mező a Manhattan távolságban.
                        {
                            if (player == 1) //frissítjük az egyes játékosok látómezőjét a csapattárs aktuális látómezőjével
                            {
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                            }
                            if (player == 8)
                            {
                                _tableGreenPlayerTwo.SetValue(i - 3, j - 4, _tableGreenPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                            }
                            if (_teams == 2)
                            {
                                if (player == 2)
                                {
                                    _tableRedPlayerOne.SetValue(i - 3, j - 4, _tableRedPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                }

                                if (player == 9)
                                {
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, _tableRedPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                }
                            }

                        }
                    }
                }
            }
        }

        //Szürke részek frissülnek, viszont az egyik robot által jelenleg nem látott, de  a másik által látott terület nem.

        /// <summary>
        /// Első találkozás során a pálya frissítése
        /// </summary>
        private void Merge(int player)
        {
            if (player == 1) // Megnézzük, hogy melyik játékos észlelte a másikat, és az ő területét frissítjük
            {
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (_greenTeamObservation[i, j] == 8)
                        {
                            TableGreenPlayerOne.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i, j));
                        }
                    }
                }
                _SyncGreenPlayerOne = true;  // Innentől kezdve folyamatosan szinkronizálják majd a térképeket

            }
            else if (player == 8)
            {
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (_greenTeamObservation[i, j] == 1)
                        {
                            TableGreenPlayerTwo.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i, j));
                        }
                    }
                }
                _SyncGreenPlayerTwo = true;
            }
            else if (player == 2)
            {
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (_redTeamObservation[i, j] == 9)
                        {
                            TableRedPlayerOne.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i, j));
                        }
                    }
                }
                _SyncRedPlayerOne = true;
            }
            else
            {
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (_redTeamObservation[i, j] == 2)
                        {
                            TableRedPlayerTwo.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i, j));
                        }
                    }
                }
                _SyncRedPlayerTwo = true;
            }




        }

        #endregion

        #region Private event methods

        #endregion

    }
}
