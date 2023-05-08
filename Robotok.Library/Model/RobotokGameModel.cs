using ELTE.Robotok.Persistence;
using Robotok.Library.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Azoknak a kockáknak a struktúrája, amiket léptetni kell
struct CubeToMove
{
    public int x;
    public int y;
    public int value;
    public bool northAttachment;
    public bool southAttachment;
    public bool eastAttachment;
    public bool westAttachment;
    public string direction;
    public int remainingCleaningOperations;

    public CubeToMove(int x, int y, int value, bool n, bool s, bool e, bool w, string direction, int remainingCleaningOperations)
    {
        this.x = x;
        this.y = y;
        this.northAttachment = n;
        this.southAttachment = s;
        this.eastAttachment = e;
        this.westAttachment = w;
        this.value = value;
        this.direction = direction;
        this.remainingCleaningOperations = remainingCleaningOperations;
    }
}

struct CubeToEvaluate
{
    public int x;
    public int y;
    public int value;

    public CubeToEvaluate(int x, int y, int value)
    {
        this.x = x;
        this.y = y;
        this.value = value;
    }
}

namespace ELTE.Robotok.Model
{
    public enum GameDifficulty { Easy, Medium, Hard }
    public class RobotokGameModel
    {
        #region Difficulty constants
        private Int32 _ManhattanDistanceEasy = 6;
        private Int32 _ManhattanDistanceMedium = 3;
        private Int32 _ManhattanDistanceHard = 4;

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
        private List<CubeToMove> _cubesOldPosition = new List<CubeToMove>(); // lépés végrehajtásakor a régi helyek eltárolása
        private List<CubeToMove> _cubesNewPosition = new List<CubeToMove>(); // lépés végrehajtásakor az új helyek eltárolása
        private List<CubeToEvaluate> _cubesToEvaluate = new List<CubeToEvaluate>(); // ebbe a listába pakoljuk be a játék határán kívüli kockákat, amit ki szeretnénk értékelni, hogy helyes alakzat-e
        private List<CubeToEvaluate> _figureToEvaluate = new List<CubeToEvaluate>(); // ebbe a listába pakoljuk be a hirdetőtáblán szereplő alakzat építőkockáit (külön-külön lista az egyes tükrözésekhez)
        private List<CubeToEvaluate> _figureToEvaluateMirrorX = new List<CubeToEvaluate>();
        private List<CubeToEvaluate> _figureToEvaluateMirrorY = new List<CubeToEvaluate>();
        private List<CubeToEvaluate> _figureToEvaluateMirrorXY = new List<CubeToEvaluate>();
        private Int32 _firstTaskDeadline; // első feladat határideje
        private Int32 _firstTaskPoints; // első feladatért járó pont
        private Int32 _secondTaskDeadline; // második feladat határideje
        private Int32 _secondTaskPoints; // második feladatért járó pont
        private Int32 _greenTeamPoints; // zöld csapat pontszáma
        private Int32 _redTeamPoints; // piros csapat pontszáma
        private List<CubeToMove> _improvedObservationGreenPlayerOne = new List<CubeToMove>();
        private List<CubeToMove> _improvedObservationGreenPlayerTwo = new List<CubeToMove>();
        private List<CubeToMove> _improvedObservationRedPlayerOne = new List<CubeToMove>();
        private List<CubeToMove> _improvedObservationRedPlayerTwo = new List<CubeToMove>();
        private List<CubeToMove> _cubesPlayerOldPosition = new List<CubeToMove>(); // lépés végrehajtásakor a régi helyek eltárolása
        private List<CubeToMove> _cubesPlayerNewPosition = new List<CubeToMove>(); // lépés végrehajtásakor az új helyek eltárolása


        /* Eltároljuk minden játékosról, hogy milyen műveletet végzet legutoljára sikerességtől függetlenül
        0 - még nem végzett műveletet (alapállapot játék elején), 1 - várakozás, 2 - mozgás, 3 - forgás, 4 - kocka csatolása robothoz, 5 - kocka lecsatolása robotról, 6 - kocka-kocka összekapcsolás, 7 - kocka-kocka szétválasztás, 8 - tisztítás 
        */
        public int lastOperationTypePlayer1TeamGreen = 0, lastOperationTypePlayer2TeamGreen = 0, lastOperationTypePlayer1TeamRed = 0, lastOperationTypePlayer2TeamRed = 0;

        /* Eltároljuk mindkét játékos által megadott összekapcsolni kívánt kockák x és y koordinátáját mindkét csapat esetében*/
        public int cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen, cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen, cube1XPlayer2TeamGreen, cube1YPlayer2TeamGreen, cube2XPlayer2TeamGreen, cube2YPlayer2TeamGreen, cube1XPlayer1TeamRed, cube1YPlayer1TeamRed, cube2XPlayer1TeamRed, cube2YPlayer1TeamRed, cube1XPlayer2TeamRed, cube1YPlayer2TeamRed, cube2XPlayer2TeamRed, cube2YPlayer2TeamRed;

        /* Annak eltárolása, hogy hol tart az összekapcsolás művelet csapatonként, 0 - nincs kezdeményezve, 1 - az első csapattárs megadta a koordinátákat, 2 - a második csapattárs megadta a koordinátákat, >2 - a második csapattárs más műveletet kezdeményezett (így érvénytelen lesz az összekapcsolás)  */
        public int greenTeamCubeAttachState, redTeamCubeAttachState;

        /* Eltároljuk a szétválasztani kívánt két kocka koordinátáit */
        public int cubeToDetach1X, cubeToDetach1Y, cubeToDetach2X, cubeToDetach2Y;

        #endregion

        #region Properties

        /// <summary>
        /// Lépések számának lekérdezése.
        /// </summary>
        public Int32 GameStepCount { get { return _gameStepCount; } }
        /// <summary>
        /// Első feladat határideje.
        /// </summary>
        public Int32 FirstTaskDeadline { get { return _firstTaskDeadline; } }
        /// <summary>
        /// Második feladat határideje.
        /// </summary>
        public Int32 SecondTaskDeadline { get { return _secondTaskDeadline; } }
        /// <summary>
        /// Első feladatért járó pontszám.
        /// </summary>
        public Int32 FirstTaskPoints { get { return _firstTaskPoints; } }
        /// <summary>
        /// Második feladatért járó pontszám.
        /// </summary>
        public Int32 SecondTaskPoints { get { return _secondTaskPoints; } }
        /// <summary>
        /// Zöld csapat összpontszáma.
        /// </summary>
        public Int32 GreenTeamPoints { get { return _greenTeamPoints; } }
        /// <summary>
        /// Piros csapat összpontszáma.
        /// </summary>
        public Int32 RedTeamPoints { get { return _redTeamPoints; } }
        /// <summary>
        /// Hátramaradt játékidő lekérdezése.
        /// </summary>
        public Int32 RemainingSeconds { get { return _remainingSeconds; } }

        /// <summary>
        /// Tisztításhoz szükséges műveletek számának lekérdezése.
        /// </summary>
        public Int32 CleaningOperetions { get { return _cleaningOperations; } }

        /// <summary>
        /// Csapatak széma lekérdezése
        /// </summary>
        public Int32 Teams { get { return _teams; } }

        /// <summary>
        /// Játéktábla lekérdezése.(játékvezetői)
        /// </summary>
        public RobotokTable Table { set { _table = value; } get { return _table; } }

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

            greenTeamCubeAttachState = 0;
            redTeamCubeAttachState = 0;
            _greenTeamPoints = 0;
            if (_teams == 2)
            {
                _redTeamPoints = 0;
            }
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

            if (_teams == 2)
            {
                _tableRedPlayerOne = new RobotokTable(11, 20);
                _tableRedPlayerTwo = new RobotokTable(11, 20);
            }
            _gameStepCount = 300; // játék kezdeti lépésszáma, folyamatosan csökken, 0-nál játék vége
            // alapból nulla, de majd a GenerateShape-ben ennek lesz valami más értéke
            _firstTaskDeadline = 0;
            _secondTaskDeadline = 0;

            _greenTeamPoints = 0;
            if (_teams == 2)
            {
                _redTeamPoints = 0;
            }
            
            _SyncGreenPlayerOne = false;
            _SyncGreenPlayerTwo = false;
            _SyncRedPlayerOne = false;
            _SyncRedPlayerTwo = false;
            for (int i = 0; i < 11; i++) // játékosok tábláját feltöltjük nem látható mezőkkel, illetve a pálya határával
            {
                for (int j = 0; j < 20; j++)
                {
                    if ((i == 0 || i == 10) && (j >= 0 && j <= 19) || (i >= 0 && i <= 10) && (j == 0 || j == 19)) 
                    {                      
                        _tableGreenPlayerOne.SetValue(i, j, -1, -1);                   
                        _tableGreenPlayerTwo.SetValue(i, j, -1, -1);
                        if (_teams == 2)
                        {                       
                            _tableRedPlayerOne.SetValue(i, j, -1, -1);
                            _tableRedPlayerTwo.SetValue(i, j, -1, -1);           
                        }
                    }
                    else
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
            }

            // Kezdeti értékek generálása a mezőknek
            GenerateFields();
            GenerateWalls();
            GenerateExits();
            GenerateShape();
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
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23) // játék pályán vagyunk-e
                    {
                        if (Math.Abs(i - tempX) + Math.Abs(j - tempY) < tempDistance) // ha igen, akkor megnézzük, hogy benne van a mező a Manhattan távolságban.
                        {
                            if (player == 1)
                            {
                                _greenTeamObservation[i - 3, j - 4] = 1;
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j)); // minden játékosnak külön van egy saját "pálya", amin megjelenítjük a Manhattan távolságot
                                _tableGreenPlayerOne.SetAttachmentValues(i - 3, j - 4, _table.GetAttachmentNorth(i, j), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i, j), _table.GetAttachmentWest(i, j));
                                _tableGreenPlayerOne.SetFaceDirection(i - 3, j - 4, _table.GetFaceNorth(i, j), _table.GetFaceSouth(i, j), _table.GetFaceEast(i, j), _table.GetFaceWest(i, j));
                                _tableGreenPlayerOne.SetInDistance(i - 3, j- 4, true);
                                if (_table.GetFieldValue(i, j) == 8) // Ha a csapattárs benne van a manhattan távolságban, akkor a két játékos nézetét egyesíteni kell 
                                {
                                    toMerge = true;
                                }
                            }

                            if (player == 8)
                            {
                                _greenTeamObservation[i - 3, j - 4] = 8;
                                _tableGreenPlayerTwo.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                _tableGreenPlayerTwo.SetAttachmentValues(i - 3, j - 4, _table.GetAttachmentNorth(i, j), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i, j), _table.GetAttachmentWest(i, j));
                                _tableGreenPlayerTwo.SetFaceDirection(i - 3, j - 4, _table.GetFaceNorth(i, j), _table.GetFaceSouth(i, j), _table.GetFaceEast(i, j), _table.GetFaceWest(i, j));
                                _tableGreenPlayerTwo.SetInDistance(i - 3, j - 4, true);
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
                                    _tableRedPlayerOne.SetAttachmentValues(i - 3, j - 4, _table.GetAttachmentNorth(i, j), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i, j), _table.GetAttachmentWest(i, j));
                                    _tableRedPlayerOne.SetFaceDirection(i - 3, j - 4, _table.GetFaceNorth(i, j), _table.GetFaceSouth(i, j), _table.GetFaceEast(i, j), _table.GetFaceWest(i, j));
                                    _tableRedPlayerOne.SetInDistance(i - 3, j - 4, true);
                                    if (_table.GetFieldValue(i, j) == 9)
                                    {
                                        toMerge = true;
                                    }
                                }

                                if (player == 9)
                                {
                                    _redTeamObservation[i - 3, j - 4] = 9;
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableRedPlayerTwo.SetAttachmentValues(i - 3, j - 4, _table.GetAttachmentNorth(i, j), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i, j), _table.GetAttachmentWest(i, j));
                                    _tableRedPlayerTwo.SetFaceDirection(i - 3, j - 4, _table.GetFaceNorth(i, j), _table.GetFaceSouth(i, j), _table.GetFaceEast(i, j), _table.GetFaceWest(i, j));
                                    _tableRedPlayerTwo.SetInDistance(i-3, j- 4, true);
                                    if (_table.GetFieldValue(i, j) == 2)
                                    {
                                        toMerge = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(player == 1)
                            {
                                _tableGreenPlayerOne.SetInDistance(i - 3, j - 4, false);
                            }
                            if(player == 8)
                            {
                                _tableGreenPlayerTwo.SetInDistance(i - 3, j - 4, false);
                            }
                            if(_teams == 2)
                            {
                                if (player == 2)
                                {
                                    _tableRedPlayerOne.SetInDistance(i - 3, j - 4, false);
                                }
                                if(player == 9)
                                {
                                    _tableRedPlayerTwo.SetInDistance(i - 3, j - 4, false);
                                }
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

                if (_firstTaskDeadline == 0 || _secondTaskDeadline == 0) // megnézzük, hogy valamelyik feladatnak már lejárt a határideje
                {
                    for (int i = 0; i < _table.SizeX; i++)
                    {
                        for (int j = 0; j < _table.SizeY; j++)
                        {
                            if (_table.GetFieldValue(i, j) == _figure1.GetColor() && _firstTaskDeadline == 0)
                            {
                                _table.SetValue(i, j, 7, -1); // figure1 építőkockai törlése
                            }

                            if (_table.GetFieldValue(i, j) == _figure2.GetColor() && _secondTaskDeadline == 0)
                            {
                                _table.SetValue(i, j, 7, -1); // figure2 építőkockai törlése
                            }

                        }
                    }

                    for (int i = 0; i < _table.SizeX; i++) // lehet hogy kockák már voltak összekapcsolva, ezért az összekapcsolást is törölni kell
                    {
                        for (int j = 0; j < _table.SizeY; j++)
                        {
                            if (_table.GetFieldValue(i, j) == 7 && _table.HasAttachments(i, j))
                            {
                                if (_table.GetAttachmentEast(i, j))
                                {
                                    _table.SetAttachmentEast(i, j, false);
                                }

                                if (_table.GetAttachmentNorth(i, j))
                                {
                                    _table.SetAttachmentNorth(i, j, false);
                                }

                                if (_table.GetAttachmentSouth(i, j))
                                {
                                    _table.SetAttachmentSouth(i, j, false);
                                }

                                if (_table.GetAttachmentWest(i, j))
                                {
                                    _table.SetAttachmentWest(i, j, false);
                                }
                            }

                            if (_table.GetFieldValue(i, j) == 1 || _table.GetFieldValue(i, j) == 2 || _table.GetFieldValue(i, j) == 8 || _table.GetFieldValue(i, j) == 9) // az az eset, amikor játékoshoz hozzá voltak kapcsolódva a kockák
                            {
                                if (_table.GetFieldValue(i, j + 1) == 7 && _table.GetAttachmentEast(i, j))
                                {
                                    _table.SetAttachmentEast(i, j, false);
                                }

                                if (_table.GetFieldValue(i, j - 1) == 7 && _table.GetAttachmentWest(i, j))
                                {
                                    _table.SetAttachmentWest(i, j, false);
                                }

                                if (_table.GetFieldValue(i + 1, j) == 7 && _table.GetAttachmentSouth(i, j))
                                {
                                    _table.SetAttachmentSouth(i, j, false);
                                }

                                if (_table.GetFieldValue(i - 1, j) == 7 && _table.GetAttachmentNorth(i, j))
                                {
                                    _table.SetAttachmentNorth(i, j, false);
                                }
                            }
                        }
                    }
                    GenerateShape(); // itt generálunk egy új alakzatot
                }

                if (player == 1)
                {
                    _gameStepCount--; // csökkenti a hátralevő lépések számát, ha az első játékos következik
                    _firstTaskDeadline--;
                    _secondTaskDeadline--;
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
                            if (!_table.HasAttachments(i - 1, j) && (_table.GetFieldValue(i - 1, j) == 0 || _table.GetFieldValue(i - 1, j) == 3 || _table.GetFieldValue(i - 1, j) == 4 || _table.GetFieldValue(i - 1, j) == 5 || _table.GetFieldValue(i - 1, j) == 6 || _table.GetFieldValue(i - 1, j) == 11 || _table.GetFieldValue(i - 1, j) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i - 1, j) != 1)
                                {
                                    _table.SetValue(i - 1, j, _table.GetFieldValue(i - 1, j), _table.GetFieldRemainingCleaningOperations(i - 1, j) - 1);
                                }
                                else
                                {
                                    int cubeValue = _table.GetFieldValue(i - 1, j);
                                    _table.SetValue(i - 1, j, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }

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
                            if (!_table.HasAttachments(i + 1, j) && (_table.GetFieldValue(i + 1, j) == 0 || _table.GetFieldValue(i + 1, j) == 3 || _table.GetFieldValue(i + 1, j) == 4 || _table.GetFieldValue(i + 1, j) == 5 || _table.GetFieldValue(i + 1, j) == 6 || _table.GetFieldValue(i - 1, j) == 11 || _table.GetFieldValue(i - 1, j) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i + 1, j) != 1)
                                {
                                    _table.SetValue(i + 1, j, _table.GetFieldValue(i + 1, j), _table.GetFieldRemainingCleaningOperations(i + 1, j) - 1);
                                }
                                else
                                {
                                    int cubeValue = _table.GetFieldValue(i + 1, j);
                                    _table.SetValue(i + 1, j, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
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
                            if (!_table.HasAttachments(i, j - 1) && (_table.GetFieldValue(i, j - 1) == 0 || _table.GetFieldValue(i, j - 1) == 3 || _table.GetFieldValue(i, j - 1) == 4 || _table.GetFieldValue(i, j - 1) == 5 || _table.GetFieldValue(i, j - 1) == 6 || _table.GetFieldValue(i - 1, j) == 11 || _table.GetFieldValue(i - 1, j) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i, j - 1) != 1)
                                {
                                    _table.SetValue(i, j - 1, _table.GetFieldValue(i, j - 1), _table.GetFieldRemainingCleaningOperations(i, j - 1) - 1);
                                }
                                else
                                {
                                    int cubeValue = _table.GetFieldValue(i, j - 1);
                                    _table.SetValue(i, j - 1, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
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
                            if (!_table.HasAttachments(i, j + 1) && (_table.GetFieldValue(i, j + 1) == 0 || _table.GetFieldValue(i, j + 1) == 3 || _table.GetFieldValue(i, j + 1) == 4 || _table.GetFieldValue(i, j + 1) == 5 || _table.GetFieldValue(i, j + 1) == 6 || _table.GetFieldValue(i - 1, j) == 11 || _table.GetFieldValue(i - 1, j) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i, j + 1) != 1)
                                {
                                    _table.SetValue(i, j + 1, _table.GetFieldValue(i, j + 1), _table.GetFieldRemainingCleaningOperations(i, j + 1) - 1);
                                }
                                else
                                {
                                    int cubeValue = _table.GetFieldValue(i, j + 1);
                                    _table.SetValue(i, j + 1, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
                                }
                                success = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Ha az előző csapattárs kockaösszekapcsolást hajtott végre előbb, növelnünk kell a számlálót (ezzel sikertelen lesz az összekapcsolási kísérlete)
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }
            return success;
        }

        /// <summary>
        /// Forgás logikája
        /// </summary>
        public Boolean Rotate(String direction, int playerNumber)
        {
            // Ha az előző csapattárs kockaösszekapcsolást hajtott végre előbb, növelnünk kell a számlálót (ezzel sikertelen lesz az összekapcsolási kísérlete)
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }

            int playerFieldValue = 0;
            switch (playerNumber)
            {
                case 1:
                    playerFieldValue = 1;
                    break;
                case 2:
                    playerFieldValue = 8;
                    break;
                case 3:
                    playerFieldValue = 2;
                    break;

                case 4:
                    playerFieldValue = 9;
                    break;
            }

            int playerCoordinateX = 0, playerCoordinateY = 0; // 0-val van inicializálva, de a keresés után biztosan helyes értéket kap

            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (_table.GetFieldValue(i, j) == playerFieldValue)
                    {
                        playerCoordinateX = i;
                        playerCoordinateY = j;
                    }
                }
            }

            // Megnézzük, hogy a játékoshoz már van-e valami kapcsolva, ha nem, akkor csak a játékos irányát kell megváltoztatni

            if (!_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentWest(playerCoordinateX, playerCoordinateY))
            {
                if (_table.GetFaceNorth(playerCoordinateX, playerCoordinateY))   // A játékos északra néz 
                {
                    if (direction == "óramutatóval megegyező")
                    {
                        _table.SetFaceEast(playerCoordinateX, playerCoordinateY);
                    }
                    else
                    {
                        _table.SetFaceWest(playerCoordinateX, playerCoordinateY);
                    }
                }
                else if (_table.GetFaceSouth(playerCoordinateX, playerCoordinateY)) // A játékos délre néz 
                {
                    if (direction == "óramutatóval megegyező")
                    {
                        _table.SetFaceWest(playerCoordinateX, playerCoordinateY);
                    }
                    else
                    {
                        _table.SetFaceEast(playerCoordinateX, playerCoordinateY);
                    }
                }
                else if (_table.GetFaceEast(playerCoordinateX, playerCoordinateY)) { // A játékos keletre néz 
                    if (direction == "óramutatóval megegyező")
                    {
                        _table.SetFaceSouth(playerCoordinateX, playerCoordinateY);
                    }
                    else
                    {
                        _table.SetFaceNorth(playerCoordinateX, playerCoordinateY);
                    }
                }
                else                                                                // A játékos nyugatra néz 
                {
                    if (direction == "óramutatóval megegyező")
                    {
                        _table.SetFaceNorth(playerCoordinateX, playerCoordinateY);
                    }
                    else
                    {
                        _table.SetFaceSouth(playerCoordinateX, playerCoordinateY);
                    }
                }
            }

            else   // Ha a játékosra már vannak csatolva blokkok
            {
                // A játékost és a hozzákapcsolt kockákat eltároljuk a régi pozíciókban

                if (_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY))
                {

                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "észak", _table);
                }
                else if (_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "dél", _table);
                }
                else if (_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "kelet", _table);
                }
                else
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "nyugat", _table);

                }

                bool validStep;
                if (direction == "óramutatóval megegyező")     // Iránytól függően forgatunk 
                {            
                    validStep = RotateRight(playerCoordinateX, playerCoordinateY, playerNumber, _table);   
                }
                else
                {
                    validStep = RotateLeft(playerCoordinateX, playerCoordinateY, playerNumber, _table); 
                }


                // Sikerességtől függetlenül a lépés után kitisztítjuk a listákat
                _cubesOldPosition.Clear();
                _cubesNewPosition.Clear();

                if (validStep)    // Ha sikerült a forgatás, akkor a játékos irányát is megváltoztatjuk
                {
                    if (_table.GetFaceNorth(playerCoordinateX, playerCoordinateY))   // A játékos északra néz 
                    {
                        if (direction == "óramutatóval megegyező")
                        {
                            _table.SetFaceEast(playerCoordinateX, playerCoordinateY);
                        }
                        else
                        {
                            _table.SetFaceWest(playerCoordinateX, playerCoordinateY);
                        }
                    }
                    else if (_table.GetFaceSouth(playerCoordinateX, playerCoordinateY)) // A játékos délre néz 
                    {
                        if (direction == "óramutatóval megegyező")
                        {
                            _table.SetFaceWest(playerCoordinateX, playerCoordinateY);
                        }
                        else
                        {
                            _table.SetFaceEast(playerCoordinateX, playerCoordinateY);
                        }
                    }
                    else if (_table.GetFaceEast(playerCoordinateX, playerCoordinateY)) // A játékos keletre néz 
                    {
                        if (direction == "óramutatóval megegyező")
                        {
                            _table.SetFaceSouth(playerCoordinateX, playerCoordinateY);
                        }
                        else
                        {
                            _table.SetFaceNorth(playerCoordinateX, playerCoordinateY);
                        }
                    }
                    else                                                                // A játékos nyugatra néz 
                    {
                        if (direction == "óramutatóval megegyező")
                        {
                            _table.SetFaceNorth(playerCoordinateX, playerCoordinateY);
                        }
                        else
                        {
                            _table.SetFaceSouth(playerCoordinateX, playerCoordinateY);
                        }
                    }
                }

                return validStep;
            }
            return true;
        }

        public void RotatePlayerView(string direction,int playerNumber)
        {
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }

            int playerFieldValue = 0;
            RobotokTable table = TableGreenPlayerOne;
            switch (playerNumber)
            {
                case 1:
                    playerFieldValue = 1;
                    break;
                case 2:
                    playerFieldValue = 8;
                    table = TableGreenPlayerTwo;
                    break;
                case 3:
                    playerFieldValue = 2;
                    table = TableRedPlayerOne;
                    break;

                case 4:
                    playerFieldValue = 9;
                    table = TableRedPlayerTwo;
                    break;
            }

            

            int playerCoordinateX = 0, playerCoordinateY = 0; // 0-val van inicializálva, de a keresés után biztosan helyes értéket kap

            for (int i = 0; i < table.SizeX; i++)
            {
                for (int j = 0; j < table.SizeY; j++)
                {
                    if (table.GetFieldValue(i, j) == playerFieldValue)
                    {
                        playerCoordinateX = i;
                        playerCoordinateY = j;
                    }
                }
            }

            // Megnézzük, hogy a játékoshoz már van-e valami kapcsolva, ha nem, akkor csak a játékos irányát kell megváltoztatni



              // Ha a játékosra már vannak csatolva blokkok
            
                // A játékost és a hozzákapcsolt kockákat eltároljuk a régi pozíciókban

                if (table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY))
                {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "észak", table);
                }
                else if (table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
                {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "dél", table);
                }
                else if (table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
                {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "kelet", table);
                }
                else
                {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "nyugat", table);
                }

                if (direction == "óramutatóval megegyező")     // Iránytól függően forgatunk 
                {
                    RotatePlayerRight(playerCoordinateX, playerCoordinateY, playerNumber, table);
                }
                else
                {
                    RotatePlayerLeft(playerCoordinateX, playerCoordinateY, playerNumber, table);
                }
           
            for(int i=0; i<_cubesNewPosition.Count; i++)
            {
                if (playerNumber == 1)
                {
                    _improvedObservationGreenPlayerOne.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations)); //elmentjük azokat a mezőket, ahonnan forgattunk
                }
                else if (playerNumber == 2)
                {
                    _improvedObservationGreenPlayerTwo.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 3)
                {
                    _improvedObservationRedPlayerOne.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else
                {
                    _improvedObservationRedPlayerTwo.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
            }


            for (int i = 0; i < _cubesNewPosition.Count; i++)
            {
                if (playerNumber == 1)
                {
                    _improvedObservationGreenPlayerOne.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations)); //elmentjük azokat a mezőket, ahová forgattunk
                }
                else if (playerNumber == 2)
                {
                    _improvedObservationGreenPlayerTwo.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 3)
                {
                    _improvedObservationRedPlayerOne.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else
                {
                    _improvedObservationRedPlayerTwo.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
            }

            // Sikerességtől függetlenül a lépés után kitisztítjuk a listákat
            _cubesOldPosition.Clear();
            _cubesNewPosition.Clear();


        }

        void RotatePlayerLeft(int playerCoordinateX, int playerCoordinateY, int playerNumber, RobotokTable table)
        {
            //eltároljuk, hogy a játékos melyik oldalon van kockához csatlakozva
            _cubesNewPosition.Add(new CubeToMove(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].southAttachment, _cubesOldPosition[0].northAttachment, "", _cubesOldPosition[0].remainingCleaningOperations));

            int i = 1;
            // Minden régi mezőnek eltároljuk az elforgatott állapotát 
            while (i < _cubesOldPosition.Count)
            {
                int x = _cubesOldPosition[0].x - (_cubesOldPosition[i].y - _cubesOldPosition[0].y);
                int y = _cubesOldPosition[0].y + (_cubesOldPosition[i].x - _cubesOldPosition[0].x);

                if (x >= 0 && x <= 10 && y >= 0 && y <= 19) 
                {
                    _cubesNewPosition.Add(new CubeToMove(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].southAttachment, _cubesOldPosition[i].northAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                }
                i++;
            }

            // Ha idáig elértünk, akkor minden rendben volt a forgatás során, már csak el kell tárolni a táblán az új kapcsolatokkal együtt
            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false); //kitöröljük az összes régi mező kapcsolatát


                if (i != 0)
                {
                    table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);  //kitöröljük az összes régi mezőt, ami nem a játékos
                }

                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                if (i != 0)
                {
                    table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);   //beállítjuk az új mező értékeket        
                }
                table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);  //beállítjuk az új kapcsolatokat


                i++;
            }
        }

        void RotatePlayerRight(int playerCoordinateX, int playerCoordinateY, int playerNumber, RobotokTable table)
        {
            //eltároljuk, hogy a játékos melyik oldalon van kockához csatlakozva
            _cubesNewPosition.Add(new CubeToMove(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].northAttachment, _cubesOldPosition[0].southAttachment, "", _cubesOldPosition[0].remainingCleaningOperations));

            int i = 1;
            // Minden régi mezőnek eltároljuk az elforgatott állapotát 
            while (i < _cubesOldPosition.Count)
            {
                int x = _cubesOldPosition[0].x - (_cubesOldPosition[0].y - _cubesOldPosition[i].y);
                int y = _cubesOldPosition[0].y + (_cubesOldPosition[0].x - _cubesOldPosition[i].x);

                if (x >= 0 && x <= 10 && y >= 0 && y <= 19)
                {
                    _cubesNewPosition.Add(new CubeToMove(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].northAttachment, _cubesOldPosition[i].southAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                }
                i++;
            }



            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false); //kitöröljük az összes régi mező kapcsolatát


                if (i != 0)
                {
                    table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);  //kitöröljük az összes régi mezőt, ami nem a játékos
                }

                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                if (i != 0)
                {
                    table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);   //beállítjuk az új mező értékeket        
                }
                table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);  //beállítjuk az új kapcsolatokat


                i++;
            }
        }
        


        /// <summary>
        /// Összekapcsolt kockák eltárolása
        /// </summary>

        public void AddCubesToOldList(int playerCoordinateX, int playerCoordinateY, int playerFieldValue, String direction, RobotokTable table)
        {
            _cubesOldPosition.Add(new CubeToMove(playerCoordinateX, playerCoordinateY, playerFieldValue, table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY), table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY), table.GetAttachmentEast(playerCoordinateX, playerCoordinateY), table.GetAttachmentWest(playerCoordinateX, playerCoordinateY), direction, table.GetFieldRemainingCleaningOperations(playerCoordinateX, playerCoordinateY)));

            int i = 0;
            bool contains;
            while (i < _cubesOldPosition.Count)
            {
                if (table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "dél") // Ha a korábbi mezőnek északon van kapcsolata, de korábban a dél irányt kapta meg, akkor a szülőjének délen volt kapcsolata, vagyis már benne van a listában, az && jobb oldali részének csak hatékonysági funkciója van
                {
                    contains = false;
                    for (int j = 0; j < _cubesOldPosition.Count; j++)   // Erre a ciklusra azért van szükség, mert ha már korábban be került egy kocka, de az kapcsolva van a jelenleg vizsgálthoz is, akkor ne rakjuk be mégegyszer
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x - 1 >= 0 && _cubesOldPosition[i].x - 1 < table.SizeX && _cubesOldPosition[i].y >= 0 && _cubesOldPosition[i].y <= table.SizeY)
                        {
                            _cubesOldPosition.Add(new CubeToMove(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, table.GetFieldValue(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentNorth(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), "észak", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y)));
                        }
                    }
                }

                if (table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "észak")
                {
                    contains = false;
                    for (int j = 0; j < _cubesOldPosition.Count; j++)
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x + 1 >= 0 && _cubesOldPosition[i].x + 1 < table.SizeX && _cubesOldPosition[i].y >= 0 && _cubesOldPosition[i].y <= table.SizeY)
                        {
                            _cubesOldPosition.Add(new CubeToMove(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, table.GetFieldValue(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentNorth(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), "dél", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y)));
                        }
                    }
                }

                if (table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "nyugat")
                {
                    contains = false;
                    for (int j = 0; j < _cubesOldPosition.Count; j++)
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x >= 0 && _cubesOldPosition[i].x < table.SizeX && _cubesOldPosition[i].y + 1 >= 0 && _cubesOldPosition[i].y + 1 <= table.SizeY)
                        {
                            _cubesOldPosition.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, table.GetFieldValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), "kelet", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1)));
                        }
                    }
                }

                if (table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "kelet")
                {
                    contains = false;
                    for (int j = 0; j < _cubesOldPosition.Count; j++)
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x >= 0 && _cubesOldPosition[i].x < table.SizeX && _cubesOldPosition[i].y - 1 >= 0 && _cubesOldPosition[i].y - 1 <= table.SizeY)
                        {
                            _cubesOldPosition.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, table.GetFieldValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), "nyugat", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1)));
                        }
                    }

                }
                i++;
            }  
        }

        /// <summary>
        /// Balra forgatás
        /// </summary>
        public Boolean RotateLeft(int playerCoordinateX, int playerCoordinateY, int playerNumber, RobotokTable table)
        {
            //eltároljuk, hogy a játékos melyik oldalon van kockához csatlakozva
            _cubesNewPosition.Add(new CubeToMove(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].southAttachment, _cubesOldPosition[0].northAttachment, "", _cubesOldPosition[0].remainingCleaningOperations));

            int i = 1;
            // Minden régi mezőnek eltároljuk az elforgatott állapotát 
            while (i < _cubesOldPosition.Count)
            {
                int x = _cubesOldPosition[0].x - (_cubesOldPosition[i].y - _cubesOldPosition[0].y);
                int y = _cubesOldPosition[0].y + (_cubesOldPosition[i].x - _cubesOldPosition[0].x);

                //Ha kiindexelnénk a tábláról a forgatás után, akkor az mindenképpen sikertelen
                if (x < 0 || y < 0 || x >= 17 || y >= 28)
                {
                    return false;
                }

                _cubesNewPosition.Add(new CubeToMove(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].southAttachment, _cubesOldPosition[i].northAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));

                i++;
            }

            i = 1;
            // Megnézzük az összes már elforgatott mezőre, hogy a forgatás után rossz helyre kerülne-e
            while (i < _cubesNewPosition.Count)
            {
                if (_table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != 7 && _table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != -2)
                {
                    int j = 1;
                    bool found = false;
                    while (j < _cubesOldPosition.Count)
                    {
                        if (_cubesNewPosition[i].x == _cubesOldPosition[j].x && _cubesNewPosition[i].y == _cubesOldPosition[j].y)
                        {
                            found = true;
                        }
                        j++;
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
                i++;
            }

            // Ha idáig elértünk, akkor minden rendben volt a forgatás során, már csak el kell tárolni a táblán az új kapcsolatokkal együtt
            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                _table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false); //kitöröljük az összes régi mező kapcsolatát
              

                if (i != 0)
                {
                    _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);  //kitöröljük az összes régi mezőt, ami nem a játékos
                }

                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                if (i != 0)
                {
                    _table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);   //beállítjuk az új mező értékeket        
                }
                _table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);  //beállítjuk az új kapcsolatokat
               
                
                i++;
            }

            return true;
        }

        /// <summary>
        /// Jobbra forgatás
        /// </summary>
        public Boolean RotateRight(int playerCoordinateX, int playerCoordinateY, int playerNumber, RobotokTable table)
        {
            //eltároljuk, hogy a játékos melyik oldalon van kockához csatlakozva
            _cubesNewPosition.Add(new CubeToMove(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].northAttachment, _cubesOldPosition[0].southAttachment, "", _cubesOldPosition[0].remainingCleaningOperations));

            int i = 1;
            // Minden régi mezőnek eltároljuk az elforgatott állapotát 
            while (i < _cubesOldPosition.Count)
            {
                int x = _cubesOldPosition[0].x - (_cubesOldPosition[0].y - _cubesOldPosition[i].y);
                int y = _cubesOldPosition[0].y + (_cubesOldPosition[0].x - _cubesOldPosition[i].x);

                //Ha kiindexelnénk a tábláról a forgatás után, akkor az mindenképpen sikertelen
                if (x < 0 || y < 0 || x >= 17 || y >= 28)
                {
                    return false;
                }

                _cubesNewPosition.Add(new CubeToMove(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].northAttachment, _cubesOldPosition[i].southAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));

                i++;
            }

            i = 1;
            // Megnézzük az összes már elforgatott mezőre, hogy a forgatás után rossz helyre kerülne-e
            while (i < _cubesNewPosition.Count)
            {
                //Ha valamelyik kocka nem üres mezőre érkezne a forgatás után, akkor megnézzük, hogy egy korábbi kocka helyére érkezne-e amit elforgattunk, ha nem, akkor a művelet sikertelen 
                if (_table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != 7 && _table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != -2)
                {
                    int j = 1;
                    bool found = false;
                    while (j < _cubesOldPosition.Count)
                    {
                        if (_cubesNewPosition[i].x == _cubesOldPosition[j].x && _cubesNewPosition[i].y == _cubesOldPosition[j].y)
                        {
                            found = true;
                        }
                        j++;
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
                i++;
            }

            // Ha idáig elértünk, akkor minden rendben volt a forgatás során, már csak el kell tárolni a táblán az új kapcsolatokkal együtt
            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                _table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false); //kitöröljük az összes régi mező kapcsolatát


                if (i != 0)
                {
                    _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);  //kitöröljük az összes régi mezőt, ami nem a játékos
                }

                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                if (i != 0)
                {
                    _table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);   //beállítjuk az új mező értékeket        
                }
                _table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);  //beállítjuk az új kapcsolatokat


                i++;
            }

            return true;
        }


        /// <summary>
        /// Lekapcsolás logikája
        /// </summary>
        public Boolean Detach(String direction, int playerNumber)
        {
            // Ha az előző csapattárs kockaösszekapcsolást hajtott végre előbb, növelnünk kell a számlálót (ezzel sikertelen lesz az összekapcsolási kísérlete)
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }
            // Attach műveletnek az inverze, részletesebb leírás ott (itt csak az alakzat letevés van leírva)
            int playerFieldValue = 0;

            switch (playerNumber)
            {
                case 1:
                    playerFieldValue = 1;
                    break;
                case 2:
                    playerFieldValue = 8;
                    break;
                case 3:
                    playerFieldValue = 2;
                    break;
                case 4:
                    playerFieldValue = 9;
                    break;
            }

            int playerCoordinateX = 0, playerCoordinateY = 0;

            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (_table.GetFieldValue(i, j) == playerFieldValue)
                    {
                        playerCoordinateX = i;
                        playerCoordinateY = j;
                    }
                }
            }

            if (direction == "észak")
            {
                // Megnézzük, hogy a játékos alakzattal áll-e a tábla szélén pontszerzés céljából
                if (playerCoordinateX == 3 && _table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY))
                {
                    int result = EvaluateShape("észak");
                    if (result > 0)
                    {
                        if (result == 1) // hozzáadunk pontokat
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _firstTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _firstTaskPoints;
                            }

                            RegenerateShape(result); // újragenerálunk megfelelő alakzatot
                        }
                        else if (result == 2)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _secondTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _secondTaskPoints;
                            }

                            RegenerateShape(result);
                        }

                        
                        _table.SetAttachmentNorth(playerCoordinateX, playerCoordinateY, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (_table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 3 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 4 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 5 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 6 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 11 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 12)
                {
                    _table.SetAttachmentNorth(playerCoordinateX, playerCoordinateY, false);
                    _table.SetAttachmentSouth(playerCoordinateX - 1, playerCoordinateY, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (direction == "dél")
            {
                if (playerCoordinateX == 13 && _table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
                {
                    int result = EvaluateShape("dél");
                    if (result > 0)
                    {
                        if (result == 1)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _firstTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _firstTaskPoints;
                            }

                            RegenerateShape(result);
                        } 
                        else if (result == 2)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _secondTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _secondTaskPoints;
                            }

                            RegenerateShape(result);
                        }

                        _table.SetAttachmentSouth(playerCoordinateX, playerCoordinateY, false);
                        return true;
                    } 
                    else
                    {
                        return false;
                    }
                }
                else if (_table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 3 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 4 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 5 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 6 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 11 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 12)
                {
                    _table.SetAttachmentSouth(playerCoordinateX, playerCoordinateY, false);
                    _table.SetAttachmentNorth(playerCoordinateX + 1, playerCoordinateY, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (direction == "kelet")
            {
                if (playerCoordinateY == 23 && _table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
                {
                    int result = EvaluateShape("kelet");
                    if (result > 0)
                    {
                        if (result == 1)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _firstTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _firstTaskPoints;
                            }

                            RegenerateShape(result);
                        }
                        else if (result == 2)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _secondTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _secondTaskPoints;
                            }

                            RegenerateShape(result);
                        }

                        _table.SetAttachmentEast(playerCoordinateX, playerCoordinateY, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 3 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 4 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 5 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 6 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 11 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 12)
                {
                    _table.SetAttachmentEast(playerCoordinateX, playerCoordinateY, false);
                    _table.SetAttachmentWest(playerCoordinateX, playerCoordinateY + 1, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else
            {
                if (playerCoordinateY == 4 && _table.GetAttachmentWest(playerCoordinateX, playerCoordinateY))
                {
                    int result = EvaluateShape("nyugat");
                    if (result > 0)
                    {
                        if (result == 1)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _firstTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _firstTaskPoints;
                            }
                            RegenerateShape(result);
                        }
                        else if (result == 2)
                        {
                            if (playerNumber == 1 || playerNumber == 2)
                            {
                                _greenTeamPoints += _secondTaskPoints;
                            }
                            else
                            {
                                _redTeamPoints += _secondTaskPoints;
                            }
                            RegenerateShape(result);
                        }

                        _table.SetAttachmentWest(playerCoordinateX, playerCoordinateY, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 3 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 4 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 5 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 6 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 11 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 12)
                {
                    _table.SetAttachmentWest(playerCoordinateX, playerCoordinateY, false);
                    _table.SetAttachmentEast(playerCoordinateX, playerCoordinateY - 1, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Rákapcsolás logikája
        /// </summary>
        public bool Attach(String direction, int playerNumber)
        {
            // Ha az előző csapattárs kockaösszekapcsolást hajtott végre előbb, növelnünk kell a számlálót (ezzel sikertelen lesz az összekapcsolási kísérlete)
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }
            /* Először a kapott játékosszámot át kell alakítani a játékos színértékévé, majd meg kell keresni a pályán, hogy tudjuk
            kihez kell kapcsolni a kockát 
            */
            int playerFieldValue = 0;

            switch (playerNumber)
            {
                case 1:
                    playerFieldValue = 1;
                    break;
                case 2:
                    playerFieldValue = 8;
                    break;
                case 3:
                    playerFieldValue = 2;
                    break;
                case 4:
                    playerFieldValue = 9;
                    break;
            }

            // Megkeressük a játékos koordinátáit a térképen (meghatározásához a színét használjuk)

            int playerCoordinateX = 0, playerCoordinateY = 0; // 0-val van inicializálva, de a keresés után biztosan helyes értéket kap

            for (int i = 4; i < 13; i++)
            {
                for (int j = 5; j < 23; j++)
                {
                    if (_table.GetFieldValue(i, j) == playerFieldValue)
                    {
                        playerCoordinateX = i;
                        playerCoordinateY = j;
                    }
                }
            }

            // Megnézzük, hogy a játékoshoz már van-e valami kapcsolva (ha igen, akkor a művelet sikertelen, hiszen egy játékoshoz nem lehet több irányból több kockát csatolni, csak egy irányból egyet)

            if (_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY) || _table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY) || _table.GetAttachmentEast(playerCoordinateX, playerCoordinateY) || _table.GetAttachmentWest(playerCoordinateX, playerCoordinateY))
            {
                return false;
            }

            // Irányparamétertől függően megpróbáljuk csatlakoztatni a játékost a kockához (ami csak építőkocka lehet)
            if (direction == "észak")
            {
                if (_table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 3 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 4 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 5 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 6 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 11 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 12)
                {
                    // Itt a csatolás kicsit redundáns, hiszen pl. északi csatoláskor a játékos északi csatoló részét is át kell állítani, továbbá annak a déli részét is, amelyik kockához akarunk csatlakozni
                    _table.SetAttachmentNorth(playerCoordinateX, playerCoordinateY, true);
                    _table.SetAttachmentSouth(playerCoordinateX - 1, playerCoordinateY, true);
                    return true; // Sikeres csatolás után igaz értékkel térünk vissza
                }
                else
                {
                    return false; // Sikertelen csatolás esetén hamis értékkel térünk vissza
                }
            }
            else if (direction == "dél")
            {
                if (_table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 3 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 4 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 5 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 6 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 11 || _table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 12)
                {
                    _table.SetAttachmentSouth(playerCoordinateX, playerCoordinateY, true);
                    _table.SetAttachmentNorth(playerCoordinateX + 1, playerCoordinateY, true);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (direction == "kelet")
            {
                if (_table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 3 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 4 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 5 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 6 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 11 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 12)
                {
                    _table.SetAttachmentEast(playerCoordinateX, playerCoordinateY, true);
                    _table.SetAttachmentWest(playerCoordinateX, playerCoordinateY + 1, true);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else // Biztosan a nyugati irány, hiszen a direction változó értéke azelőtt ellenőrzésre kerül, mielőtt meghívodna a metódus
            {
                if (_table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 3 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 4 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 5 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 6 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 11 || _table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 12)
                {
                    _table.SetAttachmentWest(playerCoordinateX, playerCoordinateY, true);
                    _table.SetAttachmentEast(playerCoordinateX, playerCoordinateY - 1, true);
                    return true;
                }
                else
                {
                    return false;
                }
            }

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
        public bool Move(String direction, int playerNumber)
        {
            // Ha az előző csapattárs kockaösszekapcsolást hajtott végre előbb, növelnünk kell a számlálót (ezzel sikertelen lesz az összekapcsolási kísérlete)
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }
            /* Először a kapott játékosszámot át kell alakítani a játékos színértékévé, majd meg kell keresni a pályán, hogy tudjuk
            kit kell léptetni 
            A mozgásnál 2 fő típust fogunk megkülönböztetni, az első, amikor még nincs a játékoshoz csatolva kocka, a másik esetben pedig van
            */
            int playerFieldValue = 0;

            switch (playerNumber)
            {
                case 1:
                    playerFieldValue = 1;
                    break;
                case 2:
                    playerFieldValue = 8;
                    break;
                case 3:
                    playerFieldValue = 2;
                    break;
                case 4:
                    playerFieldValue = 9;
                    break;
            }

            // Megkeressük a játékos koordinátáit a térképen (meghatározásához a színét használjuk)

            int playerCoordinateX = 0, playerCoordinateY = 0; // 0-val van inicializálva, de a keresés után biztosan helyes értéket kap

            for (int i = 0; i < _table.SizeX; i++)
            {
                for (int j = 0; j < _table.SizeY; j++)
                {
                    if (_table.GetFieldValue(i, j) == playerFieldValue)
                    {
                        playerCoordinateX = i;
                        playerCoordinateY = j;
                    }
                }
            }

            // Megnézzük, hogy csak a játékost kell léptetnünk, vagy vele együtt más kockákat is
            //Először az az eset következik, ha a játékoshoz nincs csatolva semmi
            if (!_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentWest(playerCoordinateX, playerCoordinateY))
            {
                if (direction == "észak")
                {
                    if (_table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 7) // Megnézi, hogy üres kockára lép-e
                    {
                        _table.SetValue(playerCoordinateX - 1, playerCoordinateY, playerFieldValue, -1); // új helyre ráléptetjük
                        _table.SetFaceDirection(playerCoordinateX - 1, playerCoordinateY, _table.GetFaceNorth(playerCoordinateX, playerCoordinateY), _table.GetFaceSouth(playerCoordinateX, playerCoordinateY), _table.GetFaceEast(playerCoordinateX, playerCoordinateY), _table.GetFaceWest(playerCoordinateX, playerCoordinateY)); //az új helyre átadjuk hogy merre nézett a játékos
                        _table.SetValue(playerCoordinateX, playerCoordinateY, 7, -1); // régi helyről letöröljük
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY, false, false, false, false); //a régi helyről töröljük hogy merre nézett a játékos
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (direction == "dél")
                {
                    if (_table.GetFieldValue(playerCoordinateX + 1, playerCoordinateY) == 7)
                    {
                        _table.SetValue(playerCoordinateX + 1, playerCoordinateY, playerFieldValue, -1);
                        _table.SetFaceDirection(playerCoordinateX + 1, playerCoordinateY, _table.GetFaceNorth(playerCoordinateX, playerCoordinateY), _table.GetFaceSouth(playerCoordinateX, playerCoordinateY), _table.GetFaceEast(playerCoordinateX, playerCoordinateY), _table.GetFaceWest(playerCoordinateX, playerCoordinateY));
                        _table.SetValue(playerCoordinateX, playerCoordinateY, 7, -1);
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY, false, false, false, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (direction == "kelet")
                {
                    if (_table.GetFieldValue(playerCoordinateX, playerCoordinateY + 1) == 7)
                    {
                        _table.SetValue(playerCoordinateX, playerCoordinateY + 1, playerFieldValue, -1);
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY + 1, _table.GetFaceNorth(playerCoordinateX, playerCoordinateY), _table.GetFaceSouth(playerCoordinateX, playerCoordinateY), _table.GetFaceEast(playerCoordinateX, playerCoordinateY), _table.GetFaceWest(playerCoordinateX, playerCoordinateY));
                        _table.SetValue(playerCoordinateX, playerCoordinateY, 7, -1);
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY, false, false, false, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (_table.GetFieldValue(playerCoordinateX, playerCoordinateY - 1) == 7)
                    {
                        _table.SetValue(playerCoordinateX, playerCoordinateY - 1, playerFieldValue, -1);
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY - 1, _table.GetFaceNorth(playerCoordinateX, playerCoordinateY), _table.GetFaceSouth(playerCoordinateX, playerCoordinateY), _table.GetFaceEast(playerCoordinateX, playerCoordinateY), _table.GetFaceWest(playerCoordinateX, playerCoordinateY));
                        _table.SetValue(playerCoordinateX, playerCoordinateY, 7, -1);
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY, false, false, false, false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else // Az az eset, amikor a játékoshoz van 1 vagy több kocka csatolva
            {
                // A játékost és a hozzákapcsolt kockákat eltároljuk a régi pozíciók listájában

                if (_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "észak", _table);
                }
                else if (_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "dél", _table);
                }
                else if (_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "kelet", _table);
                }
                else
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerFieldValue, "nyugat", _table);
                }

                bool validStep = true;

                // Ellenőrizzük azokat az eseteket, ha a játékos a pálya szélén áll az építménnyel, és ki szeretne lépni a pályáról
                if (direction == "észak")
                {
                    if (playerCoordinateX == 3)
                    {
                        validStep = false;
                    }

                }
                else if (direction == "dél")
                {
                    if (playerCoordinateX == 13)
                    {
                        validStep = false;
                    }
                }
                else if (direction == "kelet")
                {
                    if (playerCoordinateY == 23)
                    {
                        validStep = false;
                    }
                }
                else if (direction == "nyugat")
                {
                    if (playerCoordinateY == 4)
                    {
                        validStep = false;
                    }
                }


                // Készítünk egy másolatot az eredeti játéktábláról, hogy ellenőrzés során ha kiderül, hogy érvénytelen a lépés, ne veszítsünk adatot
                int[,] stepTest = new int[_table.SizeX, _table.SizeY];

                for (int i = 0; i < _table.SizeX; i++)
                {
                    for (int j = 0; j < _table.SizeY; j++)
                    {
                        stepTest[i, j] = _table.GetFieldValue(i, j);
                    }
                }

                // Letöröljük a régi kockapozíciókat a másolt tábláról
                for (int i = 0; i < _cubesOldPosition.Count; i++)
                {
                    stepTest[_cubesOldPosition[i].x, _cubesOldPosition[i].y] = 7;
                }

                // Ellenőrizzük, hogy az adott irányba történő léptetés után a kocka érvényes pozícióra kerülne-e
                for (int i = 0; i < _cubesOldPosition.Count && validStep; i++)
                {
                    if (direction == "észak")
                    {
                        if (_cubesOldPosition[i].x - 1 < 0) // Annak az esetnek a kiszűrése, ha kiindexelnénk a pályáról
                        {
                            validStep = false;
                        }
                        else if (stepTest[_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y] != 7 && stepTest[_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y] != -2)
                        {
                            validStep = false;
                        }
                    }
                    else if (direction == "dél")
                    {
                        if (_cubesOldPosition[i].x + 1 == _table.SizeX)
                        {
                            validStep = false;
                        }
                        else if (stepTest[_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y] != 7 && stepTest[_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y] != -2)
                        {
                            validStep = false;
                        }
                    }
                    else if (direction == "kelet")
                    {
                        if (_cubesOldPosition[i].y + 1 == _table.SizeY)
                        {
                            validStep = false;
                        }
                        else if (stepTest[_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1] != 7 && stepTest[_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1] != -2)
                        {
                            validStep = false;
                        }
                    }
                    else if (direction == "nyugat")
                    {
                        if (_cubesOldPosition[i].y - 1 < 0)
                        {
                            validStep = false;
                        }
                        else if (stepTest[_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1] != 7 && stepTest[_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1] != -2)
                        {
                            validStep = false;
                        }
                    }
                }
                

                if (validStep) // Ha sikeres a lépés ellenőrzése, végrehajtjuk
                {
                    ExecuteSafeSteps(direction);
                }
         /*       for(int i=0; i<_cubesOldPosition.Count; i++)
                {
                    if (playerNumber == 1)
                    {
                        _improvedObservationGreenPlayerOne.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations)); // Elmentjük a régi pozíciókat
                    }
                    else if (playerNumber == 2)
                    {
                        _improvedObservationGreenPlayerTwo.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }
                    else if (playerNumber == 3)
                    {
                        _improvedObservationRedPlayerOne.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }
                    else
                    {
                        _improvedObservationRedPlayerTwo.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }
                }
                for(int i=0; i<_cubesNewPosition.Count; i++)
                {
                    if (playerNumber == 1)
                    {
                        _improvedObservationGreenPlayerOne.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations)); //Elmentjük az új pozíciókat
                    }
                    else if (playerNumber == 2)
                    {
                        _improvedObservationGreenPlayerTwo.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                    }
                    else if (playerNumber == 3)
                    {
                        _improvedObservationRedPlayerOne.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                    }
                    else
                    {
                        _improvedObservationRedPlayerTwo.Add(new CubeToMove(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                    }
                }*/
                // Sikerességtől függetlenül a lépés után kitisztítjuk a listákat (hiszen pl. a játékos kockája minden esetben szerepel a régi pozíciókban)
                _cubesOldPosition.Clear();
                _cubesNewPosition.Clear();

                return validStep;
            }
        }

        void ExecuteSafeSteps(string direction)
        {
            for (int i = 0; i < _cubesOldPosition.Count; i++)
            {
                if (direction == "észak")
                {
                    _cubesNewPosition.Add(new CubeToMove(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (direction == "dél")
                {
                    _cubesNewPosition.Add(new CubeToMove(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (direction == "kelet")
                {
                    _cubesNewPosition.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (direction == "nyugat")
                {
                    _cubesNewPosition.Add(new CubeToMove(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
            }

            // Először letöröljük a tábláról a régi pozíciós kockákat

            for (int i = 0; i < _cubesOldPosition.Count; i++)
            {
                // Az az eset, amikor játékon kívüli kockát törlünk (-2 értékre kell visszaállítani)
                if (_cubesOldPosition[i].x < 3 || _cubesOldPosition[i].x > 13 || _cubesOldPosition[i].y < 4 || _cubesOldPosition[i].y > 23)
                {
                    _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, -2, -1);
                }
                else // Általános eset 7-re (üres kocka) visszaállítva
                {
                    _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);
                }
                _table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false);
                _table.SetFaceDirection(_cubesOldPosition[i].x, _cubesOldPosition[i].x, false, false, false, false);
            }


            // A kockákat újrarajzoljuk a táblán az új pozíciókat tartalmazó lista szerint

            for (int i = 0; i < _cubesNewPosition.Count; i++)
            {
                _table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);
                _table.SetFaceDirection(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _table.GetFaceNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetFaceSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetFaceEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetFaceWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y));
                _table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);
            }
        }

        // Összekapcsolja a paramétereként kapott két kockát
        public bool AttachCubes(string group)
        {
            // Először ellenőrizzük, hogy a két játékos által megadott kockapozíciók egyeznek-e (csoporttól függően)
            if (group == "green")
            {
                if (cube1XPlayer1TeamGreen != cube1XPlayer2TeamGreen || cube1YPlayer1TeamGreen != cube1YPlayer2TeamGreen || cube2XPlayer1TeamGreen != cube2XPlayer2TeamGreen || cube2YPlayer1TeamGreen != cube2YPlayer2TeamGreen)
                {
                    return false;
                }
            }
            else
            {
                if (cube1XPlayer1TeamRed != cube1XPlayer2TeamRed || cube1YPlayer1TeamRed != cube1YPlayer2TeamRed || cube2XPlayer1TeamRed != cube2XPlayer2TeamRed || cube2YPlayer1TeamRed != cube2YPlayer2TeamRed)
                {
                    return false;
                }
            }

            // Következő lépésként ellenőrizzük, hogy a két kocka olyan típusú-e, amit lehet csatolni (mindegy melyik játékoséval nézzük, hiszen ha idáig eljutunk akkor a két játékos által megadott kockák egyenlőek)
            if (group == "green")
            {
                if (!(_table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) == 3 || _table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) == 4 || _table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) == 5 || _table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) == 6 || _table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) == 11 || _table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) == 12) || !(_table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen) == 3 || _table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen) == 4 || _table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen) == 5 || _table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen) == 6 || _table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen) == 11 || _table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen) == 12))
                {
                    return false;
                }
            }
            else
            {
                if (!(_table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) == 3 || _table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) == 4 || _table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) == 5 || _table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) == 6 || _table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) == 11 || _table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) == 12) || !(_table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed) == 3 || _table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed) == 4 || _table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed) == 5 || _table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed) == 6 || _table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed) == 11 || _table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed) == 12))
                {
                    return false;
                }
            }

            // Ellenőriznünk kell azt is, hogy az összekapcsolni kívánt két kocka megegyező színű-e
            if (group == "green")
            {
                if (_table.GetFieldValue(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen) != _table.GetFieldValue(cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen))
                {
                    return false;
                }
            }
            else
            {
                if (_table.GetFieldValue(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed) != _table.GetFieldValue(cube2XPlayer1TeamRed, cube2YPlayer1TeamRed))
                {
                    return false;
                }
            }

            // Következő lépésként meghatározzuk a kapcsolás pozícióját (élszomszédosság helye alapján), majd létrehozzuk azt
            if (group == "green")
            {
                if ((cube1XPlayer1TeamGreen == cube2XPlayer1TeamGreen) && (cube1YPlayer1TeamGreen - 1 == cube2YPlayer1TeamGreen))
                {
                    _table.SetAttachmentWest(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen, true);
                    _table.SetAttachmentEast(cube1XPlayer1TeamGreen, cube2YPlayer1TeamGreen, true);
                    return true;
                }
                if ((cube1XPlayer1TeamGreen == cube2XPlayer1TeamGreen) && (cube1YPlayer1TeamGreen == cube2YPlayer1TeamGreen - 1))
                {
                    _table.SetAttachmentEast(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen, true);
                    _table.SetAttachmentWest(cube1XPlayer1TeamGreen, cube2YPlayer1TeamGreen, true);
                    return true;
                }
                if ((cube1XPlayer1TeamGreen - 1 == cube2XPlayer1TeamGreen) && (cube1YPlayer1TeamGreen == cube2YPlayer1TeamGreen))
                {
                    _table.SetAttachmentSouth(cube2XPlayer1TeamGreen, cube1YPlayer1TeamGreen, true);
                    _table.SetAttachmentNorth(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen, true);
                    return true;
                }
                if ((cube1XPlayer1TeamGreen == cube2XPlayer1TeamGreen - 1) && (cube1YPlayer1TeamGreen == cube2YPlayer1TeamGreen))
                {
                    _table.SetAttachmentNorth(cube2XPlayer1TeamGreen, cube1YPlayer1TeamGreen, true);
                    _table.SetAttachmentSouth(cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen, true);
                    return true;
                }
                else // Ez az az eset, amikor a kiválasztott kockák nem élszomszédosak
                {
                    return false;
                }
            }
            else
            {
                if ((cube1XPlayer1TeamRed == cube2XPlayer1TeamRed) && (cube1YPlayer1TeamRed - 1 == cube2YPlayer1TeamRed))
                {
                    _table.SetAttachmentWest(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed, true);
                    _table.SetAttachmentEast(cube1XPlayer1TeamRed, cube2YPlayer1TeamRed, true);
                    return true;
                }
                if ((cube1XPlayer1TeamRed == cube2XPlayer1TeamRed) && (cube1YPlayer1TeamRed == cube2YPlayer1TeamRed - 1))
                {
                    _table.SetAttachmentEast(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed, true);
                    _table.SetAttachmentWest(cube1XPlayer1TeamRed, cube2YPlayer1TeamRed, true);
                    return true;
                }
                if ((cube1XPlayer1TeamRed - 1 == cube2XPlayer1TeamRed) && (cube1YPlayer1TeamRed == cube2YPlayer1TeamRed))
                {
                    _table.SetAttachmentSouth(cube2XPlayer1TeamRed, cube1YPlayer1TeamRed, true);
                    _table.SetAttachmentNorth(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed, true);
                    return true;
                }
                if ((cube1XPlayer1TeamRed == cube2XPlayer1TeamRed - 1) && (cube1YPlayer1TeamRed == cube2YPlayer1TeamRed))
                {
                    _table.SetAttachmentNorth(cube2XPlayer1TeamRed, cube1YPlayer1TeamRed, true);
                    _table.SetAttachmentSouth(cube1XPlayer1TeamRed, cube1YPlayer1TeamRed, true);
                    return true;
                }
                else // Ez az az eset, amikor a kiválasztott kockák nem élszomszédosak
                {
                    return false;
                }
            }
        }

        public bool DetachCubes(int playerNumber)
        {
            // Ha az előző csapattárs kockaösszekapcsolást hajtott végre előbb, növelnünk kell a számlálót (ezzel sikertelen lesz az összekapcsolási kísérlete)
            if (playerNumber == 1 || playerNumber == 2)
            {
                if (greenTeamCubeAttachState != 0)
                {
                    greenTeamCubeAttachState++;
                }
            }
            else
            {
                if (redTeamCubeAttachState != 0)
                {
                    redTeamCubeAttachState++;
                }
            }
            // Először ellenőrizzük, hogy építőkockákat adtak-e meg a szétválasztáshoz
            if (!(_table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 3 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 4 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 5 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 6 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 11 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 12) || !(_table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 3 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 4 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 5 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 6 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 11 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 12))
            {
                return false;
            }
            // Következő lépésként élszomszédosság helye alapján töröljük a kapcsolatot
            if ((cubeToDetach1X == cubeToDetach2X) && (cubeToDetach1Y - 1 == cubeToDetach2Y))
            {
                // Ellenőrizzük, hogy a szétkapcsolandó helyen van-e összekapcsolás
                if (_table.GetAttachmentWest(cubeToDetach1X, cubeToDetach1Y) && _table.GetAttachmentEast(cubeToDetach1X, cubeToDetach2Y))
                {
                    _table.SetAttachmentWest(cubeToDetach1X, cubeToDetach1Y, false);
                    _table.SetAttachmentEast(cubeToDetach1X, cubeToDetach2Y, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if ((cubeToDetach1X == cubeToDetach2X) && (cubeToDetach1Y == cubeToDetach2Y - 1))
            {
                if (_table.GetAttachmentEast(cubeToDetach1X, cubeToDetach1Y) && _table.GetAttachmentWest(cubeToDetach1X, cubeToDetach2Y))
                {
                    _table.SetAttachmentEast(cubeToDetach1X, cubeToDetach1Y, false);
                    _table.SetAttachmentWest(cubeToDetach1X, cubeToDetach2Y, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if ((cubeToDetach1X - 1 == cubeToDetach2X) && (cubeToDetach1Y == cubeToDetach2Y))
            {
                if (_table.GetAttachmentSouth(cubeToDetach2X, cubeToDetach1Y) && _table.GetAttachmentNorth(cubeToDetach1X, cubeToDetach1Y))
                {
                    _table.SetAttachmentSouth(cubeToDetach2X, cubeToDetach1Y, false);
                    _table.SetAttachmentNorth(cubeToDetach1X, cubeToDetach1Y, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if ((cubeToDetach1X == cubeToDetach2X - 1) && (cubeToDetach1Y == cubeToDetach2Y))
            {
                if (_table.GetAttachmentNorth(cubeToDetach2X, cubeToDetach1Y) && _table.GetAttachmentSouth(cubeToDetach1X, cubeToDetach1Y))
                {
                    _table.SetAttachmentNorth(cubeToDetach2X, cubeToDetach1Y, false);
                    _table.SetAttachmentSouth(cubeToDetach1X, cubeToDetach1Y, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else // Ez az az eset, amikor a kiválasztott kockák nem élszomszédosak
            {
                return false;
            }
        }

        public int EvaluateShape(string direction) // Visszatérési érték: 0 - helytelen alakzat, 1 - 1. alakzat teljesült, 2 - 2. alakzat teljesült
        {
            int result = 0; // A kiértékelés alatt külön változóban tároljuk el a visszatérési értéket

            int figureToEvaluateColor = 0; // Eltároljuk a kivinni kívánt alakzat színét (minden lehetséges alakzat egyedi színű, biztosan helyes értéket kap)

            if (direction == "észak") // Iránytól függően a játékon kívüli területről bepakoljuk a kockákat a kiértékelésre szolgáló listába
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < _table.SizeY; j++)
                    {
                        if (_table.GetFieldValue(i, j) != -2 && _table.GetFieldValue(i, j) != 7)
                        {
                            figureToEvaluateColor = _table.GetFieldValue(i, j); // A kockák listában való eltárolásával egyidejűleg meghatározzuk a kivitt alakzat színét
                            _cubesToEvaluate.Add(new CubeToEvaluate(i, j, _table.GetFieldValue(i, j)));
                        }
                    }
                }
            }
            else if (direction == "dél")
            {
                for (int i = 14; i < 17; i++)
                {
                    for (int j = 0; j < _table.SizeY; j++)
                    {
                        if (_table.GetFieldValue(i, j) != -2 && _table.GetFieldValue(i, j) != 7)
                        {
                            figureToEvaluateColor = _table.GetFieldValue(i, j);
                            _cubesToEvaluate.Add(new CubeToEvaluate(i, j, _table.GetFieldValue(i, j)));
                        }
                    }
                }
            }
            else if (direction == "nyugat")
            {
                for (int i = 0; i < _table.SizeX; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (_table.GetFieldValue(i, j) != -2 && _table.GetFieldValue(i, j) != 7)
                        {
                            figureToEvaluateColor = _table.GetFieldValue(i, j);
                            _cubesToEvaluate.Add(new CubeToEvaluate(i, j, _table.GetFieldValue(i, j)));
                        }
                    }
                }
            }
            else if (direction == "kelet")
            {
                for (int i = 0; i < _table.SizeX; i++)
                {
                    for (int j = 24; j < 28; j++)
                    {
                        if (_table.GetFieldValue(i, j) != -2 && _table.GetFieldValue(i, j) != 7)
                        {
                            figureToEvaluateColor = _table.GetFieldValue(i, j);
                            _cubesToEvaluate.Add(new CubeToEvaluate(i, j, _table.GetFieldValue(i, j)));
                        }
                    }
                }
            }

            // Megkeressük, melyik hirdetőtábláról származik a kivinni kívánt alakzat
            bool foundFigureNumber = false;
            int figureNumber = 0;

            int k = 0;
            while (k < 4 && !foundFigureNumber)
            {
                int l = 0;
                while (l < 4 && !foundFigureNumber)
                {
                    if (_figure1.GetFieldValue(k, l) == figureToEvaluateColor)
                    {
                        figureNumber = 1;
                        foundFigureNumber = true;
                    }
                    else if (_figure2.GetFieldValue(k, l) == figureToEvaluateColor)
                    {
                        figureNumber = 2;
                        foundFigureNumber = true;
                    }
                    l++;
                }
                k++;
            }
            // A megfelelő hirdetőtáblás alakzatból elkészítjük az elforgatott változatokat is
            int[,] _figureMirrorX = new int[4, 4];
            int[,] _figureMirrorY = new int[4, 4];
            int[,] _figureMirrorXY = new int[4, 4];


            // X tengely szerinti tükrözés
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (figureNumber == 1)
                    {
                        _figureMirrorX[i, j] = _figure1.GetFieldValue(i, 3 - j);
                    }
                    else if (figureNumber == 2)
                    {
                        _figureMirrorX[i, j] = _figure2.GetFieldValue(i, 3 - j);
                    }
                }
            }
            // Y tengely szerinti tükrözés
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (figureNumber == 1)
                    {
                        _figureMirrorY[i, j] = _figure1.GetFieldValue(3 - i, j);
                    }
                    else if (figureNumber == 2)
                    {
                        _figureMirrorY[i, j] = _figure2.GetFieldValue(3 - i, j);
                    }
                }
            }
            // X és Y tengely szerinti tükrözés}

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (figureNumber == 1)
                    {
                        _figureMirrorXY[i, j] = _figure1.GetFieldValue(3 - i, 3 - j);
                    }
                    else if (figureNumber == 2)
                    {
                        _figureMirrorXY[i, j] = _figure2.GetFieldValue(3 - i, 3 - j);
                    }
                }
            }


            // A játékos kivitt kockáinak színe alapján kiértékelendő hirdetőtáblán szereplő nem -2 (játékterületen kívüli) kockákat bepakoljuk egy listába
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (figureNumber == 1)
                    {
                        if (_figure1.GetFieldValue(i, j) != -2 && _figure1.GetFieldValue(i, j) != 7)
                        {
                            _figureToEvaluate.Add(new CubeToEvaluate(i, j, _figure1.GetFieldValue(i, j)));
                        }
                        if (_figureMirrorX[i, j] != -2 && _figureMirrorX[i, j] != 7)
                        {
                            _figureToEvaluateMirrorX.Add(new CubeToEvaluate(i, j, _figureMirrorX[i, j]));
                        }
                        if (_figureMirrorY[i, j] != -2 && _figureMirrorY[i, j] != 7)
                        {
                            _figureToEvaluateMirrorY.Add(new CubeToEvaluate(i, j, _figureMirrorY[i, j]));
                        }
                        if (_figureMirrorXY[i, j] != -2 && _figureMirrorXY[i, j] != 7)
                        {
                            _figureToEvaluateMirrorXY.Add(new CubeToEvaluate(i, j, _figureMirrorXY[i, j]));
                        }
                    }
                    else if (figureNumber == 2)
                    {
                        if (_figure2.GetFieldValue(i, j) != -2 && _figure2.GetFieldValue(i, j) != 7)
                        {
                            _figureToEvaluate.Add(new CubeToEvaluate(i, j, _figure2.GetFieldValue(i, j)));
                        }
                        if (_figureMirrorX[i, j] != -2 && _figureMirrorX[i, j] != 7)
                        {
                            _figureToEvaluateMirrorX.Add(new CubeToEvaluate(i, j, _figureMirrorX[i, j]));
                        }
                        if (_figureMirrorY[i, j] != -2 && _figureMirrorY[i, j] != 7)
                        {
                            _figureToEvaluateMirrorY.Add(new CubeToEvaluate(i, j, _figureMirrorY[i, j]));
                        }
                        if (_figureMirrorXY[i, j] != -2 && _figureMirrorXY[i, j] != 7)
                        {
                            _figureToEvaluateMirrorXY.Add(new CubeToEvaluate(i, j, _figureMirrorXY[i, j]));
                        }
                    }
                }
            }

            bool foundFigure = true; // Alapértelmezetten azt állítjuk, hogy megtaláltuk az alakzatot

            if (_cubesToEvaluate.Count != _figureToEvaluate.Count) // Ha nem egyezik meg a kivitt alakzat elemszáma egyik hiretőttáblán szereplő alakzat elemszámával sem, akkor nem kell tovább ellenőriznünk
            {
                foundFigure = false;
            }

            // Megnézzük, hogy a játékon kívüli területről származó alakzat olyan alakú-e, mint ami a hirdetőtáblán van (Itt fontos a sorrend, hogy ugyanolyan sorrendben kerültek be a játéktábláról a kockák, mint a hirdetőtábláról. Mivel ez a tulajonság teljesül, elég megnéznünk, hogy a listában szereplő kockák közötti x,y relatív távolság megegyezik-e
            if (_cubesToEvaluate.Count > 1 && foundFigure == true)
            {
                int cubeToEvaluateFirstX = _cubesToEvaluate[0].x;
                int cubeToEvaluateFirstY = _cubesToEvaluate[0].y;
                _cubesToEvaluate.RemoveAt(0);

                int cubeToEvaluateSecondX = _cubesToEvaluate[0].x;
                int cubeToEvaluateSecondY = _cubesToEvaluate[0].y;
                _cubesToEvaluate.RemoveAt(0);

                int figureToEvaluateFirstX = _figureToEvaluate[0].x;
                int figureToEvaluateFirstY = _figureToEvaluate[0].y;
                _figureToEvaluate.RemoveAt(0);

                int figureToEvaluateSecondX = _figureToEvaluate[0].x;
                int figureToEvaluateSecondY = _figureToEvaluate[0].y;
                _figureToEvaluate.RemoveAt(0);

                int figureToEvaluateFirstXMirrorX = _figureToEvaluateMirrorX[0].x;
                int figureToEvaluateFirstYMirrorX = _figureToEvaluateMirrorX[0].y;
                _figureToEvaluateMirrorX.RemoveAt(0);

                int figureToEvaluateSecondXMirrorX = _figureToEvaluateMirrorX[0].x;
                int figureToEvaluateSecondYMirrorX = _figureToEvaluateMirrorX[0].y;
                _figureToEvaluateMirrorX.RemoveAt(0);

                int figureToEvaluateFirstXMirrorY = _figureToEvaluateMirrorY[0].x;
                int figureToEvaluateFirstYMirrorY = _figureToEvaluateMirrorY[0].y;
                _figureToEvaluateMirrorY.RemoveAt(0);

                int figureToEvaluateSecondXMirrorY = _figureToEvaluateMirrorY[0].x;
                int figureToEvaluateSecondYMirrorY = _figureToEvaluateMirrorY[0].y;
                _figureToEvaluateMirrorY.RemoveAt(0);

                int figureToEvaluateFirstXMirrorXY = _figureToEvaluateMirrorXY[0].x;
                int figureToEvaluateFirstYMirrorXY = _figureToEvaluateMirrorXY[0].y;
                _figureToEvaluateMirrorXY.RemoveAt(0);

                int figureToEvaluateSecondXMirrorXY = _figureToEvaluateMirrorXY[0].x;
                int figureToEvaluateSecondYMirrorXY = _figureToEvaluateMirrorXY[0].y;
                _figureToEvaluateMirrorXY.RemoveAt(0);

                while (_cubesToEvaluate.Count > 0)
                {
                    // Ha valamelyik két koordináta relatív különbsége nem megegyező, az alakzat hibás
                    if (cubeToEvaluateSecondX - cubeToEvaluateFirstX != figureToEvaluateSecondX - figureToEvaluateFirstX || cubeToEvaluateSecondY - cubeToEvaluateFirstY != figureToEvaluateSecondY - figureToEvaluateFirstY)
                    {
                        if (cubeToEvaluateSecondX - cubeToEvaluateFirstX != figureToEvaluateSecondXMirrorX - figureToEvaluateFirstXMirrorX || cubeToEvaluateSecondY - cubeToEvaluateFirstY != figureToEvaluateSecondYMirrorX - figureToEvaluateFirstYMirrorX)
                        {
                            if (cubeToEvaluateSecondX - cubeToEvaluateFirstX != figureToEvaluateSecondXMirrorY - figureToEvaluateFirstXMirrorY || cubeToEvaluateSecondY - cubeToEvaluateFirstY != figureToEvaluateSecondYMirrorY - figureToEvaluateFirstYMirrorY)
                            {
                                if (cubeToEvaluateSecondX - cubeToEvaluateFirstX != figureToEvaluateSecondXMirrorXY - figureToEvaluateFirstXMirrorXY || cubeToEvaluateSecondY - cubeToEvaluateFirstY != figureToEvaluateSecondYMirrorXY - figureToEvaluateFirstYMirrorXY)
                                {
                                    foundFigure = false;
                                }
                            }
                        }
                    }

                    // Az ellenőrzésre szánt kockákat léptetjük eggyel
                    cubeToEvaluateFirstX = cubeToEvaluateSecondX;
                    cubeToEvaluateFirstY = cubeToEvaluateSecondY;

                    cubeToEvaluateSecondX = _cubesToEvaluate[0].x;
                    cubeToEvaluateSecondY = _cubesToEvaluate[0].y;
                    _cubesToEvaluate.RemoveAt(0);

                    figureToEvaluateFirstX = figureToEvaluateSecondX;
                    figureToEvaluateFirstY = figureToEvaluateSecondY;

                    figureToEvaluateSecondX = _figureToEvaluate[0].x;
                    figureToEvaluateSecondY = _figureToEvaluate[0].y;
                    _figureToEvaluate.RemoveAt(0);

                    figureToEvaluateFirstXMirrorX = figureToEvaluateSecondXMirrorX;
                    figureToEvaluateFirstYMirrorX = figureToEvaluateSecondYMirrorX;

                    figureToEvaluateSecondXMirrorX = _figureToEvaluateMirrorX[0].x;
                    figureToEvaluateSecondYMirrorX = _figureToEvaluateMirrorX[0].y;
                    _figureToEvaluateMirrorX.RemoveAt(0);

                    figureToEvaluateFirstXMirrorY = figureToEvaluateSecondXMirrorY;
                    figureToEvaluateFirstYMirrorY = figureToEvaluateSecondYMirrorY;

                    figureToEvaluateSecondXMirrorY = _figureToEvaluateMirrorY[0].x;
                    figureToEvaluateSecondYMirrorY = _figureToEvaluateMirrorY[0].y;
                    _figureToEvaluateMirrorY.RemoveAt(0);

                    figureToEvaluateFirstXMirrorXY = figureToEvaluateSecondXMirrorXY;
                    figureToEvaluateFirstYMirrorXY = figureToEvaluateSecondYMirrorXY;

                    figureToEvaluateSecondXMirrorXY = _figureToEvaluateMirrorXY[0].x;
                    figureToEvaluateSecondYMirrorXY = _figureToEvaluateMirrorXY[0].y;
                    _figureToEvaluateMirrorXY.RemoveAt(0);
                }

                if (foundFigure) // Letöröljük az alakzatot a játéktábláról
                {
                    if (direction == "észak")
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < _table.SizeY; j++)
                            {
                                _table.SetValue(i, j, -2, -1);
                                _table.SetAttachmentNorth(i, j, false);
                                _table.SetAttachmentSouth(i, j, false);
                                _table.SetAttachmentEast(i, j, false);
                                _table.SetAttachmentWest(i, j, false);
                            }
                        }
                    }
                    else if (direction == "dél")
                    {
                        for (int i = 14; i < 17; i++)
                        {
                            for (int j = 0; j < _table.SizeY; j++)
                            {
                                _table.SetValue(i, j, -2, -1);
                                _table.SetAttachmentNorth(i, j, false);
                                _table.SetAttachmentSouth(i, j, false);
                                _table.SetAttachmentEast(i, j, false);
                                _table.SetAttachmentWest(i, j, false);
                            }
                        }
                    }
                    else if (direction == "nyugat")
                    {
                        for (int i = 0; i < _table.SizeX; i++)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                _table.SetValue(i, j, -2, -1);
                                _table.SetAttachmentNorth(i, j, false);
                                _table.SetAttachmentSouth(i, j, false);
                                _table.SetAttachmentEast(i, j, false);
                                _table.SetAttachmentWest(i, j, false);
                            }
                        }
                    }
                    else if (direction == "kelet")
                    {
                        for (int i = 0; i < _table.SizeX; i++)
                        {
                            for (int j = 24; j < 28; j++)
                            {
                                _table.SetValue(i, j, -2, -1);
                                _table.SetAttachmentNorth(i, j, false);
                                _table.SetAttachmentSouth(i, j, false);
                                _table.SetAttachmentEast(i, j, false);
                                _table.SetAttachmentWest(i, j, false);
                            }
                        }
                    }
                    result = figureNumber; // Ha kiértékelődött egy alakzat, és le lettek törölve az építőkockái, akkor a visszatérési érték az aktív alakzat száma lesz
                }
            }

            else if (_cubesToEvaluate.Count == 1) // Speciális eset, ha az alakzat csak egy kockából áll (igazából ilyen alakzatunk jelenleg nincs, a kódban csak a teljesség igénye miatt szerepel)
            {
                if (_figureToEvaluate.Count == _cubesToEvaluate.Count && _cubesToEvaluate[0].value == _figureToEvaluate[0].value) // Ha az 1. alakzat teljesül
                {
                    result = figureNumber;
                }
            }

            _cubesToEvaluate.Clear(); // Mindegyik esetben töröljük a lista tartalmát
            _figureToEvaluate.Clear();
            _figureToEvaluateMirrorX.Clear();
            _figureToEvaluateMirrorY.Clear();
            _figureToEvaluateMirrorXY.Clear();
            return result;
        }

        #endregion

        #region Private game methods
        /// <summary>
        /// Kijáratok generálása.
        /// </summary>
        private void GenerateExits()
        {
            Random coord_i = new Random();
            Random coord_j = new Random();

            for (int walls = 0; walls < 4; walls++)
            {
                if (walls == 0 || walls == 2)
                {
                    int temp_i = coord_i.Next(4, 8);

                    if (walls == 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            _table.SetValue(temp_i + i, 4, 7, -1);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            _table.SetValue(temp_i + i, 23, 7, -1);
                        }
                    }
                }
                else
                {
                    int temp_j = coord_j.Next(5, 18);

                    if (walls == 1)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            _table.SetValue(3, temp_j + j, 7, -1);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            _table.SetValue(13, temp_j + j, 7, -1);
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Akadályok generálása.
        /// </summary>
        private void GenerateWalls()
        {
            Random random = new Random();
            int rnd = random.Next(5, 7);

            Random coordinates_x = new Random();
            Random coordinates_y = new Random();

            int x = coordinates_x.Next(4, 13);
            int y = coordinates_y.Next(5, 23);

            for (int i = 0; i < rnd; ++i)
            {
                while (_table.GetFieldValue(x, y) != 7)
                {
                    x = coordinates_x.Next(4, 13);
                    y = coordinates_y.Next(5, 23);
                }

                _table.SetValue(x, y, 0, _cleaningOperations);
            }
        }
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
                    // Beállítjuk a csatolmányokat (kezdetben egyetlen cellának sincs csatolmánya)
                    _table.SetAttachmentValues(i, j, false, false, false, false);

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
                        _table.SetValue(i, j, 7, -1);   // Alapvetően -1 értéket kapnak a mezők, a tisztítható mezők megkapják az értéküket később
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
                _table.SetValue(redPlayerOne_i, redPlayerOne_j, 2, -1);


                while (_table.GetFieldValue(redPlayerTwo_i, redPlayerTwo_j) != 7)
                {
                    redPlayerTwo_i = random.Next(4, 13);
                    redPlayerTwo_j = random.Next(5, 23);
                }

                _table.SetValue(redPlayerTwo_i, redPlayerTwo_j, 9, -1);
            }
        }
        /// <summary>
        /// Fejlesztett megfigyelés, a mozgatott illetve forgatott, már észlelt kockák megjelenítése a játékosok számára
        /// </summary>
        public void ImprovedObservation(int player)
        {
            List<CubeToMove> list = new List<CubeToMove>();
            switch (player)
            {
                case 1:
                    list = _improvedObservationGreenPlayerOne;
                    break;
                case 2:
                    list = _improvedObservationGreenPlayerTwo;
                    break;
                case 3:
                    list = _improvedObservationRedPlayerOne;
                    break;
                case 4:
                    list = _improvedObservationRedPlayerTwo;
                    break;
            }
           
            for (int i=0; i<list.Count; i++)
            {

                    if (player == 1)
                    {
                        _tableGreenPlayerOne.SetValue(list[i].x, list[i].y, list[i].value, list[i].remainingCleaningOperations);
                        _tableGreenPlayerOne.SetAttachmentValues(list[i].x, list[i].y, list[i].northAttachment, list[i].southAttachment, list[i].eastAttachment, list[i].westAttachment);
                    }
                    else if (player == 2)
                    {
                        _tableGreenPlayerTwo.SetValue(list[i].x, list[i].y, list[i].value, list[i].remainingCleaningOperations);
                        _tableGreenPlayerTwo.SetAttachmentValues(list[i].x, list[i].y, list[i].northAttachment, list[i].southAttachment, list[i].eastAttachment, list[i].westAttachment);
                    }
                    else if (player == 3)
                    {
                        _tableRedPlayerOne.SetValue(list[i].x, list[i].y, list[i].value, list[i].remainingCleaningOperations);
                        _tableRedPlayerOne.SetAttachmentValues(list[i].x, list[i].y, list[i].northAttachment, list[i].southAttachment, list[i].eastAttachment, list[i].westAttachment);
                    }
                    else
                    {
                        _tableRedPlayerTwo.SetValue(list[i].x, list[i].y, list[i].value, list[i].remainingCleaningOperations);
                        _tableRedPlayerTwo.SetAttachmentValues(list[i].x, list[i].y, list[i].northAttachment, list[i].southAttachment, list[i].eastAttachment, list[i].westAttachment);
                    }
                
            }
            
        }
        /// <summary>
        /// ImprovedObservation függvényhez felhasznált Listák törlése
        /// </summary>
        public void ClearImprovedObservationList(int player)
        {
            switch (player)
            {
                case 1:
                    _improvedObservationGreenPlayerOne.Clear();
                    break;
                case 2:
                    _improvedObservationGreenPlayerTwo.Clear();
                    break;
                case 3:
                    _improvedObservationRedPlayerOne.Clear();
                    break;
                case 4:
                    _improvedObservationRedPlayerTwo.Clear();
                    break;
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
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23) // játék pályán vagyunk-e
                    {
                        if (((player == 1 || player == 8) && (_SyncGreenPlayerOne && _SyncGreenPlayerTwo)) || ((player == 2 || player == 9) && (_SyncRedPlayerOne && _SyncRedPlayerTwo))) // Ha mindkét játékos látta már egymást, akkor minden mezőt átadunk
                        {
                            if (player == 1) //frissítjük az egyes játékosok látómezőjét a csapattárs aktuális látómezőjével
                            {
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                _tableGreenPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableGreenPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                _tableGreenPlayerOne.SetFaceDirection(i - 3, j - 4, _tableGreenPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceEast(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceWest(i - 3, j - 4));
                            }
                            if (player == 8)
                            {
                                _tableGreenPlayerTwo.SetValue(i - 3, j - 4, _tableGreenPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                _tableGreenPlayerTwo.SetAttachmentValues(i - 3, j - 4, _tableGreenPlayerOne.GetAttachmentNorth(i - 3, j - 4), _tableGreenPlayerOne.GetAttachmentSouth(i - 3, j - 4), _tableGreenPlayerOne.GetAttachmentEast(i - 3, j - 4), _tableGreenPlayerOne.GetAttachmentWest(i - 3, j - 4));
                                _tableGreenPlayerTwo.SetFaceDirection(i - 3, j - 4, _tableGreenPlayerOne.GetFaceNorth(i - 3, j - 4), _tableGreenPlayerOne.GetFaceSouth(i - 3, j - 4), _tableGreenPlayerOne.GetFaceEast(i - 3, j - 4), _tableGreenPlayerOne.GetFaceWest(i - 3, j - 4));
                            }
                            if (_teams == 2)
                            {
                                if (player == 2)
                                {
                                    _tableRedPlayerOne.SetValue(i - 3, j - 4, _tableRedPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableRedPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableRedPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                    _tableRedPlayerOne.SetFaceDirection(i - 3, j - 4, _tableRedPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableRedPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableRedPlayerTwo.GetFaceEast(i - 3, j - 4), _tableRedPlayerTwo.GetFaceWest(i - 3, j - 4));
                                }

                                if (player == 9)
                                {
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, _tableRedPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableRedPlayerTwo.SetAttachmentValues(i - 3, j - 4, _tableRedPlayerOne.GetAttachmentNorth(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentSouth(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentEast(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentWest(i - 3, j - 4));
                                    _tableRedPlayerTwo.SetFaceDirection(i - 3, j - 4, _tableRedPlayerOne.GetFaceNorth(i - 3, j - 4), _tableRedPlayerOne.GetFaceSouth(i - 3, j - 4), _tableRedPlayerOne.GetFaceEast(i - 3, j - 4), _tableRedPlayerOne.GetFaceWest(i - 3, j - 4));
                                }
                            }
                        }
                        else
                        {

                            if (player == 1) //Ha csak az egyik játékos látta  a másikat, akkor a másik játékos szürke mezőit nem kell átadnunk
                            {
                                if (_tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != 10)
                                {
                                    _tableGreenPlayerOne.SetValue(i - 3, j - 4, _tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableGreenPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableGreenPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                    _tableGreenPlayerOne.SetFaceDirection(i - 3, j - 4, _tableGreenPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceEast(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceWest(i - 3, j - 4));
                                }
                            }
                            if (player == 8)
                            {
                                if (_tableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != 10)
                                {
                                    _tableGreenPlayerTwo.SetValue(i - 3, j - 4, _tableGreenPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableGreenPlayerTwo.SetAttachmentValues(i - 3, j - 4, _tableGreenPlayerOne.GetAttachmentNorth(i - 3, j - 4), _tableGreenPlayerOne.GetAttachmentSouth(i - 3, j - 4), _tableGreenPlayerOne.GetAttachmentEast(i - 3, j - 4), _tableGreenPlayerOne.GetAttachmentWest(i - 3, j - 4));
                                    _tableGreenPlayerTwo.SetFaceDirection(i - 3, j - 4, _tableGreenPlayerOne.GetFaceNorth(i - 3, j - 4), _tableGreenPlayerOne.GetFaceSouth(i - 3, j - 4), _tableGreenPlayerOne.GetFaceEast(i - 3, j - 4), _tableGreenPlayerOne.GetFaceWest(i - 3, j - 4));
                                }
                            }

                            if (_teams == 2)
                            {
                                if (player == 2)
                                {
                                    if (_tableRedPlayerOne.GetFieldValue(i - 3, j - 4) != 10)
                                    {
                                        _tableRedPlayerOne.SetValue(i - 3, j - 4, _tableRedPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                        _tableRedPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableRedPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                        _tableRedPlayerOne.SetFaceDirection(i - 3, j - 4, _tableRedPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableRedPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableRedPlayerTwo.GetFaceEast(i - 3, j - 4), _tableRedPlayerTwo.GetFaceWest(i - 3, j - 4));
                                    }
                                }


                                if (player == 9)
                                {
                                    if (_tableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != 10)
                                    {
                                        _tableRedPlayerTwo.SetValue(i - 3, j - 4, _tableRedPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                        _tableRedPlayerTwo.SetAttachmentValues(i - 3, j - 4, _tableRedPlayerOne.GetAttachmentNorth(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentSouth(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentEast(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentWest(i - 3, j - 4));
                                        _tableRedPlayerTwo.SetFaceDirection(i - 3, j - 4, _tableRedPlayerOne.GetFaceNorth(i - 3, j - 4), _tableRedPlayerOne.GetFaceSouth(i - 3, j - 4), _tableRedPlayerOne.GetFaceEast(i - 3, j - 4), _tableRedPlayerOne.GetFaceWest(i - 3, j - 4));
                                    }
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
                            if (TableGreenPlayerTwo.GetFieldValue(i, j) != 8)
                            {
                                TableGreenPlayerOne.SetValue(i, j, TableGreenPlayerTwo.GetFieldValue(i, j), TableGreenPlayerTwo.GetFieldRemainingCleaningOperations(i, j));
                                TableGreenPlayerOne.SetAttachmentValues(i, j, TableGreenPlayerTwo.GetAttachmentNorth(i, j), TableGreenPlayerTwo.GetAttachmentSouth(i, j), TableGreenPlayerTwo.GetAttachmentEast(i, j), TableGreenPlayerTwo.GetAttachmentWest(i, j));
                                TableGreenPlayerOne.SetFaceDirection(i, j, TableGreenPlayerTwo.GetFaceNorth(i, j), TableGreenPlayerTwo.GetFaceSouth(i, j), TableGreenPlayerTwo.GetFaceEast(i, j), TableGreenPlayerTwo.GetFaceWest(i, j));
                            }
                            else // Ez azért kell, hogy ne jelenjen meg kétszer a játékos az egyesítés után
                            {
                                TableGreenPlayerOne.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i + 3, j + 4));
                                TableGreenPlayerOne.SetAttachmentValues(i, j, _table.GetAttachmentNorth(i + 3, j + 4), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i + 3, j + 4), _table.GetAttachmentWest(i + 3, j + 4));
                                TableGreenPlayerOne.SetFaceDirection(i, j, _table.GetFaceNorth(i + 3, j + 4), _table.GetFaceSouth(i + 3, j + 4), _table.GetFaceEast(i + 3, j + 4), _table.GetFaceWest(i + 3, j + 4));
                            }
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
                            if(TableGreenPlayerOne.GetFieldValue(i, j) != 1) 
                            {
                                TableGreenPlayerTwo.SetValue(i, j, TableGreenPlayerOne.GetFieldValue(i, j), TableGreenPlayerOne.GetFieldRemainingCleaningOperations(i, j));
                                TableGreenPlayerTwo.SetAttachmentValues(i, j, TableGreenPlayerOne.GetAttachmentNorth(i, j), TableGreenPlayerOne.GetAttachmentSouth(i, j), TableGreenPlayerOne.GetAttachmentEast(i, j), TableGreenPlayerOne.GetAttachmentWest(i, j));
                                TableGreenPlayerTwo.SetFaceDirection(i, j, TableGreenPlayerOne.GetFaceNorth(i, j), TableGreenPlayerOne.GetFaceSouth(i, j), TableGreenPlayerOne.GetFaceEast(i, j), TableGreenPlayerOne.GetFaceWest(i, j));
                            }
                            else 
                            {
                                TableGreenPlayerTwo.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i + 3, j + 4));
                                TableGreenPlayerTwo.SetAttachmentValues(i, j, _table.GetAttachmentNorth(i + 3, j + 4), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i + 3, j + 4), _table.GetAttachmentWest(i + 3, j + 4));
                                TableGreenPlayerTwo.SetFaceDirection(i, j, _table.GetFaceNorth(i + 3, j + 4), _table.GetFaceSouth(i + 3, j + 4), _table.GetFaceEast(i + 3, j + 4), _table.GetFaceWest(i + 3, j + 4));
                            }
                            
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
                            if (TableRedPlayerTwo.GetFieldValue(i, j) != 9)
                            {
                                TableRedPlayerOne.SetValue(i, j, TableRedPlayerTwo.GetFieldValue(i, j), TableRedPlayerTwo.GetFieldRemainingCleaningOperations(i, j));
                                TableRedPlayerOne.SetAttachmentValues(i, j, TableRedPlayerTwo.GetAttachmentNorth(i, j), TableRedPlayerTwo.GetAttachmentSouth(i, j), TableRedPlayerTwo.GetAttachmentEast(i, j), TableRedPlayerTwo.GetAttachmentWest(i, j));
                                TableRedPlayerOne.SetFaceDirection(i, j, TableRedPlayerTwo.GetFaceNorth(i, j), TableRedPlayerTwo.GetFaceSouth(i, j), TableRedPlayerTwo.GetFaceEast(i, j), TableRedPlayerTwo.GetFaceWest(i, j));
                            }
                            else
                            {
                                TableRedPlayerOne.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i + 3, j + 4));
                                TableRedPlayerOne.SetAttachmentValues(i, j, _table.GetAttachmentNorth(i + 3, j + 4), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i + 3, j + 4), _table.GetAttachmentWest(i + 3, j + 4));
                                TableRedPlayerOne.SetFaceDirection(i, j, _table.GetFaceNorth(i + 3, j + 4), _table.GetFaceSouth(i + 3, j + 4), _table.GetFaceEast(i + 3, j + 4), _table.GetFaceWest(i + 3, j + 4));
                            }
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
                            if (TableRedPlayerOne.GetFieldValue(i, j) != 2)
                            {
                                TableRedPlayerTwo.SetValue(i, j, TableRedPlayerOne.GetFieldValue(i, j), TableRedPlayerOne.GetFieldRemainingCleaningOperations(i, j));
                                TableRedPlayerTwo.SetAttachmentValues(i, j, TableRedPlayerOne.GetAttachmentNorth(i, j), TableRedPlayerOne.GetAttachmentSouth(i, j), TableRedPlayerOne.GetAttachmentEast(i, j), TableRedPlayerOne.GetAttachmentWest(i, j));
                                TableRedPlayerTwo.SetFaceDirection(i, j, TableRedPlayerOne.GetFaceNorth(i, j), TableRedPlayerOne.GetFaceSouth(i, j), TableRedPlayerOne.GetFaceEast(i, j), TableRedPlayerOne.GetFaceWest(i, j));
                            }
                            else
                            {
                                TableRedPlayerTwo.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i + 3, j + 4));
                                TableRedPlayerTwo.SetAttachmentValues(i, j, _table.GetAttachmentNorth(i + 3, j + 4), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i + 3, j + 4), _table.GetAttachmentWest(i + 3, j + 4));
                                TableRedPlayerTwo.SetFaceDirection(i, j, _table.GetFaceNorth(i + 3, j + 4), _table.GetFaceSouth(i + 3, j + 4), _table.GetFaceEast(i + 3, j + 4), _table.GetFaceWest(i + 3, j + 4));
                            }
                          
                        }
                    }
                }
                _SyncRedPlayerTwo = true;
            }
        }

        /// <summary>
        /// Új kocka generálása
        /// </summary>
        private void GenerateNewCube(int cubeValue)
        {
            Random random_num = new Random();
            int cube_i, cube_j;
            cube_i = random_num.Next(4, 13);
            cube_j = random_num.Next(5, 23);
 

            while (_table.GetFieldValue(cube_i, cube_j) != 7)
            {
                cube_i = random_num.Next(4, 13);
                cube_j = random_num.Next(5, 23);
            }

            _table.SetValue(cube_i, cube_j, cubeValue, _cleaningOperations);
            
        }

        /// <summary>
        /// Új alakzat létrehozása
        /// </summary>
        private void GenerateShape()
        {
            if (_firstTaskDeadline == 0)
            {
                _figure1 = new Shape();
                while(_figure1.GetColor() == _figure2.GetColor())
                {
                    _figure1 = new Shape();
                }

                if (_figure1.Figure == _figure1.Triangle)
                {
                    _firstTaskPoints = 50;
                    _firstTaskDeadline = 60; 
                }
                else if (_figure1.Figure == _figure1.Cube || _figure1.Figure == _figure1.lType || _figure1.Figure == _figure1.Straight)
                {
                    _firstTaskPoints = 65;
                    _firstTaskDeadline = 77;
                }
                else if (_figure1.Figure == _figure1.PiType || _figure1.Figure == _figure1.Rhombus)
                {
                    _firstTaskPoints = 80;
                    _firstTaskDeadline = 100;
                }

                Random random = new Random();

                for (int x = 0; x < _figure1.Figure.GetLength(0); ++x)
                {
                    for (int y = 0; y < _figure1.Figure.GetLength(1); ++y)
                    {
                        if (_figure1.GetFieldValue(x, y) != -2)
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

                for (int i = 0; i < _figure1.Figure.GetLength(0); ++i)
                {
                    for (int j = 0; j < _figure1.Figure.GetLength(1); ++j)
                    {
                        if (_figure1.GetFieldValue(i, j) != -2)
                        {
                            _tableNoticeBoardOne.SetValue(i, j, _figure1.GetFieldValue(i, j), _cleaningOperations);
                        }
                        else
                        {
                            _tableNoticeBoardOne.SetValue(i, j, -2, -1);
                        }
                    }
                }
            } 

            if (_secondTaskDeadline == 0)
            {
                _figure2 = new Shape();

                while (_figure1.GetColor() == _figure2.GetColor())
                {
                    _figure2 = new Shape();
                }

                if (_figure2.Figure == _figure2.Triangle)
                {
                    _secondTaskPoints = 50;
                    _secondTaskDeadline = 60;
                }
                else if (_figure2.Figure == _figure2.Cube || _figure2.Figure == _figure2.lType || _figure2.Figure == _figure2.Straight)
                {
                    _secondTaskPoints = 65;
                    _secondTaskDeadline = 77;
                }
                else if (_figure2.Figure == _figure2.PiType || _figure2.Figure == _figure2.Rhombus)
                {
                    _secondTaskPoints = 80;
                    _secondTaskDeadline = 100;
                }

                Random random = new Random();
                for (int x = 0; x < _figure2.Figure.GetLength(0); ++x)
                {
                    for (int y = 0; y < _figure2.Figure.GetLength(1); ++y)
                    {
                        if (_figure2.GetFieldValue(x, y) != -2)
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

                for (int i = 0; i < _figure2.Figure.GetLength(0); ++i)
                {
                    for (int j = 0; j < _figure2.Figure.GetLength(1); ++j)
                    {
                        if (_figure2.GetFieldValue(i, j) != -2)
                        {
                            _tableNoticeBoardTwo.SetValue(i, j, _figure2.GetFieldValue(i, j), _cleaningOperations);
                        }
                        else
                        {
                            _tableNoticeBoardTwo.SetValue(i, j, -2, -1);
                        }
                    }
                }
            } 
        }

        /// <summary>
        /// Régi alakzat létrehozása
        /// </summary>
        private void RegenerateShape(int num)
        {
            if (num == 1)
            {
                Random random = new Random();

                for (int x = 0; x < _figure1.Figure.GetLength(0); ++x)
                {
                    for (int y = 0; y < _figure1.Figure.GetLength(1); ++y)
                    {
                        if (_figure1.GetFieldValue(x, y) != -2)
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

                for (int i = 0; i < _figure1.Figure.GetLength(0); ++i)
                {
                    for (int j = 0; j < _figure1.Figure.GetLength(1); ++j)
                    {
                        if (_figure1.GetFieldValue(i, j) != -2)
                        {
                            _tableNoticeBoardOne.SetValue(i, j, _figure1.GetFieldValue(i, j), _cleaningOperations);
                        }
                        else
                        {
                            _tableNoticeBoardOne.SetValue(i, j, -2, -1);
                        }
                    }
                }
            } else if (num == 2)
            {
                Random random = new Random();
                for (int x = 0; x < _figure2.Figure.GetLength(0); ++x)
                {
                    for (int y = 0; y < _figure2.Figure.GetLength(1); ++y)
                    {
                        if (_figure2.GetFieldValue(x, y) != -2)
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

                for (int i = 0; i < _figure2.Figure.GetLength(0); ++i)
                {
                    for (int j = 0; j < _figure2.Figure.GetLength(1); ++j)
                    {
                        if (_figure2.GetFieldValue(i, j) != -2)
                        {
                            _tableNoticeBoardTwo.SetValue(i, j, _figure2.GetFieldValue(i, j), _cleaningOperations);
                        }
                        else
                        {
                            _tableNoticeBoardTwo.SetValue(i, j, -2, -1);
                        }
                    }
                }
            }
        }
        #endregion

        #region Private event methods

        #endregion

    }
}
