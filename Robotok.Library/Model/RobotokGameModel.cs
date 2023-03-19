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
        private Int32 _gameStepCount; // hátralevő lépések száma
        private Int32 _gameTime; // fennmaradó játékidő a következő lépésig

        #endregion

        #region Properties

        /// <summary>
        /// Lépések számának lekérdezése.
        /// </summary>
        public Int32 GameStepCount { get { return _gameStepCount; } }

        /// <summary>
        /// Hátramaradt játékidő lekérdezése.
        /// </summary>
        public Int32 GameTime { get { return _gameTime; } }

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

        public RobotokGameModel(IRobotokDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            //Táblák létrehozása a modellben
            _table = new RobotokTable(28, 17);
            _tablePlayerOne = new RobotokTable(20, 11);
            _tablePlayerTwo = new RobotokTable(20, 11);
            _tableNoticeBoardOne = new RobotokTable(4, 4);
            _tableNoticeBoardTwo = new RobotokTable(4, 4);
            _gameStepCount = 700; //játék kezdeti lépésszáma, folyamatosan csökken, 0-nál játék vége
        }

        #endregion

        #region Public game methods

        public void NewGame()
        {
            _table = new RobotokTable(28, 17);
            _tablePlayerOne = new RobotokTable(20, 11);
            _tablePlayerTwo = new RobotokTable(20, 11);
            _tableNoticeBoardOne = new RobotokTable(4, 4);
            _tableNoticeBoardTwo = new RobotokTable(4, 4);
            _gameStepCount = 700; //játék kezdeti lépésszáma, folyamatosan csökken, 0-nál játék vége
        }

        #endregion

        #region Private game methods

        #endregion

        #region Private event methods

        #endregion

    }
}
