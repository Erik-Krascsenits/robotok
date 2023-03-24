using ELTE.Robotok.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Robotok.Model
{
    public enum GameDifficulty { Easy, Medium, Hard }
    public class RobotokGameModel
    {
        #region Difficulty constants

        #endregion

        #region Fields

        private IRobotokDataAccess _dataAccess; // adatelérés
        private RobotokTable _table; // játéktábla (teljes, a játékvezető láthatja opcionálisan)
        private RobotokTable _tablePlayerOne; // 1. játékos táblája
        private RobotokTable _tablePlayerTwo; // 2. játékos táblája
        private RobotokTable _tableNoticeBoardOne; // Hirdetőtábla 1
        private RobotokTable _tableNoticeBoardTwo; // Hirdetőtábla 2
        private GameDifficulty _gameDifficulty; // nehézség
        private Int32 _gameStepCount; // hátralevő lépések száma a játék végéig
        private Int32 _remainingSeconds; // hátralevő másodpercek száma a következő lépésig
        private Int32 _cleaningOperations; // játék nehézségétől függő tisztítási műveletek száma

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
        /// Játéktábla lekérdezése.(játékvezetői)
        /// </summary>
        public RobotokTable Table { get { return _table; } }

        /// <summary>
        /// Játéktábla lekérdezése (1. játékosé).
        /// </summary>
        public RobotokTable TablePlayerOne { get { return _tablePlayerOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (2. játékosé).
        /// </summary>
        public RobotokTable TablePlayerTwo { get { return _tablePlayerTwo; } }

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

        public RobotokGameModel(IRobotokDataAccess dataAccess, int selectedDifficulty)
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
            _tablePlayerOne = new RobotokTable(11, 20);
            _tablePlayerTwo = new RobotokTable(11, 20);
            _tableNoticeBoardOne = new RobotokTable(4, 4);
            _tableNoticeBoardTwo = new RobotokTable(4, 4);
            _remainingSeconds = 5; // műveletek közötti gondolkodási idő
            _gameStepCount = 300; // játék kezdeti lépésszáma, folyamatosan csökken, 0-nál játék vége

            // Kezdeti értékek generálása a mezőknek
            GenerateFields();
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
        }

        #endregion

        #region Private event methods

        #endregion

    }
}
