using ELTE.Robotok.Persistence;

namespace ELTE.Robotok.Model
{
    public class RobotokGameModel
    {
        #region Difficulty constants

        /// <summary>
        /// Manhattan-távolságok beállítására szolgáló konstansok (játék nehézségétől függ)
        /// </summary>
        private Int32 _manhattanDistanceEasy = 6;
        private Int32 _manhattanDistanceMedium = 5;
        private Int32 _manhattanDistanceHard = 4;

        #endregion

        #region Fields

        private RobotokTable _table; // teljes játéktábla, a játékvezető láthatja opcionálisan
        private RobotokTable _tableGreenPlayerOne, _tableGreenPlayerTwo; // zöld csapatban a játékosok táblái
        private RobotokTable _tableRedPlayerOne, _tableRedPlayerTwo; // piros csapatban a játékosok táblái
        private RobotokTable _tableNoticeBoardOne, _tableNoticeBoardTwo; // alakzatok hirdetőtáblái (megjelenítés céljából, shape osztályban pedig a háttérlogika)
        private GameDifficulty _gameDifficulty; // játék nehézsége (felhasználói döntéstől függ)
        private Int32 _gameStepCount; // hátralevő lépések száma a játék végéig
        private Int32 _remainingSeconds; // hátralevő másodpercek száma a következő lépésig
        private Int32 _cleaningOperations; // játék nehézségétől függő tisztítási műveletek száma
        private Shape _figure1, _figure2; // alakzatok eltárolása (háttérlogikához használt)
        private Int32 _teams; // csapatok száma (felhasználó állítja be játék kezdetekor)
        private Int32[,] _greenTeamObservation, _redTeamObservation; // játékosnézetek összekapcsolása után találkozáskor eltárolja a közösen ismert kockák adatait
        private Boolean _syncGreenPlayerOne, _syncGreenPlayerTwo; // eltárolja, hogy a zöld csapatban az adott játékos látta-e a csapattársát, vagy sem
        private Boolean _syncRedPlayerOne, _syncRedPlayerTwo; // eltárolja, hogy a piros csapatban _syncRedPlayerOneaz adott játékos látta-e a csapattársát, vagy sem
        private List<Cube> _cubesOldPosition = new List<Cube>(); // kockával történő lépés végrehajtásakor a régi helyek eltárolása
        private List<Cube> _cubesNewPosition = new List<Cube>(); // kockával történő lépés végrehajtásakor az új helyek eltárolása
        private List<CubeToEvaluate> _cubesToEvaluate = new List<CubeToEvaluate>(); // ebbe a listába pakoljuk be a játék határán kívüli kockákat, amit ki szeretnénk értékelni, hogy helyes alakzat-e
        private List<CubeToEvaluate> _figureToEvaluate = new List<CubeToEvaluate>(); // ebbe a listába pakoljuk be a hirdetőtáblán szereplő alakzat építőkockáit (külön-külön lista az egyes tükrözésekhez)
        private List<CubeToEvaluate> _figureToEvaluateMirrorX = new List<CubeToEvaluate>(); // kiértékeléskor eltároljuk a hirdetőtáblán szereplő alakzat X tengely szerinti tükrözött mását
        private List<CubeToEvaluate> _figureToEvaluateMirrorY = new List<CubeToEvaluate>(); // kiértékeléskor eltároljuk a hirdetőtáblán szereplő alakzat Y tengely szerinti tükrözött mását
        private List<CubeToEvaluate> _figureToEvaluateMirrorXY = new List<CubeToEvaluate>(); // kiértékeléskor eltároljuk a hirdetőtáblán szereplő alakzat X és Y tengely szerinti tükrözött mását
        private Int32 _firstTaskDeadline, _secondTaskDeadline; // feladatok határideje
        private Int32 _firstTaskPoints, _secondTaskPoints; // feladatokért járó pont
        private Int32 _greenTeamPoints, _redTeamPoints; // csapatok pontszáma
        private List<Cube> _observationGreenPlayerOne = new List<Cube>(); // zöld csapat első játékosához kapcsolt alakzat kockáinak eltárolása
        private List<Cube> _observationGreenPlayerTwo = new List<Cube>(); // zöld csapat második játékosához kapcsolt alakzat kockáinak eltárolása
        private List<Cube> _observationRedPlayerOne = new List<Cube>(); // piros csapat első játékosához kapcsolt alakzat kockáinak eltárolása
        private List<Cube> _observationRedPlayerTwo = new List<Cube>(); // piros csapat második játékosához kapcsolt alakzat kockáinak eltárolása
        private List<Cube> _outOfTableGreenPlayerOne = new List<Cube>(); // zöld csapat első játékosához kapcsolt, művelet elvégzése után pályán kívülre eső alakzat kockáinak eltárolása
        private List<Cube> _outOfTableGreenPlayerTwo = new List<Cube>(); // zöld csapat második játékosához kapcsolt, művelet elvégzése után pályán kívülre eső alakzat kockáinak eltárolása
        private List<Cube> _outOfTableRedPlayerOne = new List<Cube>(); // piros csapat első játékosához kapcsolt, művelet elvégzése után pályán kívülre eső alakzat kockáinak eltárolása
        private List<Cube> _outOfTableRedPlayerTwo = new List<Cube>(); // piros csapat második játékosához kapcsolt, művelet elvégzése után pályán kívülre eső alakzat kockáinak eltárolása

        /* Eltároljuk minden játékosról, hogy milyen műveletet végzet legutoljára sikerességtől függetlenül
        0 - még nem végzett műveletet (alapállapot játék elején), 1 - várakozás, 2 - mozgás, 3 - forgás, 4 - kocka csatolása robothoz, 5 - kocka lecsatolása robotról, 6 - kocka-kocka összekapcsolás, 7 - kocka-kocka szétválasztás, 8 - tisztítás 
        */
        public Int32 lastOperationTypePlayer1TeamGreen = 0, lastOperationTypePlayer2TeamGreen = 0, lastOperationTypePlayer1TeamRed = 0, lastOperationTypePlayer2TeamRed = 0;

        /* Eltároljuk mindkét játékos által megadott összekapcsolni kívánt kockák x és y koordinátáját mindkét csapat esetében*/
        public Int32 cube1XPlayer1TeamGreen, cube1YPlayer1TeamGreen, cube2XPlayer1TeamGreen, cube2YPlayer1TeamGreen, cube1XPlayer2TeamGreen, cube1YPlayer2TeamGreen, cube2XPlayer2TeamGreen, cube2YPlayer2TeamGreen, cube1XPlayer1TeamRed, cube1YPlayer1TeamRed, cube2XPlayer1TeamRed, cube2YPlayer1TeamRed, cube1XPlayer2TeamRed, cube1YPlayer2TeamRed, cube2XPlayer2TeamRed, cube2YPlayer2TeamRed;

        /* Annak eltárolása, hogy hol tart az összekapcsolás művelet csapatonként, 0 - nincs kezdeményezve, 1 - az első csapattárs megadta a koordinátákat, 2 - a második csapattárs megadta a koordinátákat, >2 - a második csapattárs más műveletet kezdeményezett (így érvénytelen lesz az összekapcsolás)  */
        public Int32 greenTeamCubeAttachState, redTeamCubeAttachState;

        /* Eltároljuk a szétválasztani kívánt két kocka koordinátáit */
        public Int32 cubeToDetach1X, cubeToDetach1Y, cubeToDetach2X, cubeToDetach2Y;

        #endregion

        #region Properties

        /// <summary>
        /// Lépések számának lekérdezése
        /// </summary>
        public Int32 GameStepCount { get { return _gameStepCount; } }

        /// <summary>
        /// Első feladat határideje
        /// </summary>
        public Int32 FirstTaskDeadline { get { return _firstTaskDeadline; } }

        /// <summary>
        /// Második feladat határideje
        /// </summary>
        public Int32 SecondTaskDeadline { get { return _secondTaskDeadline; } }
        /// <summary>
        /// Első feladatért járó pontszám
        /// </summary>
        public Int32 FirstTaskPoints { get { return _firstTaskPoints; } }

        /// <summary>
        /// Második feladatért járó pontszám
        /// </summary>
        public Int32 SecondTaskPoints { get { return _secondTaskPoints; } }

        /// <summary>
        /// Zöld csapat összpontszáma
        /// </summary>
        public Int32 GreenTeamPoints { get { return _greenTeamPoints; } }

        /// <summary>
        /// Piros csapat összpontszáma
        /// </summary>
        public Int32 RedTeamPoints { get { return _redTeamPoints; } }

        /// <summary>
        /// Hátramaradt játékidő lekérdezése
        /// </summary>
        public Int32 RemainingSeconds { get { return _remainingSeconds; } }

        /// <summary>
        /// Tisztításhoz szükséges műveletek számának lekérdezése
        /// </summary>
        public Int32 CleaningOperetions { get { return _cleaningOperations; } }

        /// <summary>
        /// Csapatok számának lekérdezése
        /// </summary>
        public Int32 Teams { get { return _teams; } }

        /// <summary>
        /// Játékvezetői tábla lekérdezése
        /// </summary>
        public RobotokTable Table { set { _table = value; } get { return _table; } }

        /// <summary>
        /// Játéktábla lekérdezése (zöld csapataban az 1. játékosé)
        /// </summary>
        public RobotokTable TableGreenPlayerOne { get { return _tableGreenPlayerOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (zöld csapatban a 2. játékosé)
        /// </summary>
        public RobotokTable TableGreenPlayerTwo { get { return _tableGreenPlayerTwo; } }

        /// <summary>
        /// Játéktábla lekérdezése (piros csapataban az 1. játékosé)
        /// </summary>
        public RobotokTable TableRedPlayerOne { get { return _tableRedPlayerOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (piros csapatban a 2. játékosé)
        /// </summary>
        public RobotokTable TableRedPlayerTwo { get { return _tableRedPlayerTwo; } }

        /// <summary>
        /// Játéktábla lekérdezése (első hirdetőtábla)
        /// </summary>
        public RobotokTable TableNoticeBoardOne { get { return _tableNoticeBoardOne; } }

        /// <summary>
        /// Játéktábla lekérdezése (második hirdetőtábla)
        /// </summary>
        public RobotokTable TableNoticeBoardTwo { get { return _tableNoticeBoardTwo; } }

        /// <summary>
        /// Játék végének lekérdezése
        /// </summary>
        public Boolean IsGameOver { get { return (_gameStepCount == 0); } }

        /// <summary>
        /// Játék nehézségének lekérdezése, vagy beállítása
        /// </summary>
        public GameDifficulty GameDifficulty { get { return _gameDifficulty; } set { _gameDifficulty = value; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Modelt létrehozó konstruktor játéknehézség és csapatok száma alapján
        /// </summary>
        /// <param name="selectedDifficulty">Játékos által kiválasztott nehézség</param>
        /// <param name="teams">Játékos által kiválasztott csapatok száma</param>
        public RobotokGameModel(Int32 selectedDifficulty, Int32 teams)
        {
            // Piros játékostáblák esetén a null értékek soha nem fordulhatnak elő, a kódban mindig le vannak kezelve
            _tableRedPlayerOne = null!;
            _tableRedPlayerTwo = null!;

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

            _figure1 = new Shape(); // első feladat alakzatának kigenerálása
            _figure2 = new Shape(); // második feladat alakzatának kigenerálása

            _tableNoticeBoardOne = new RobotokTable(3, 3); // első hirdetőtábla inicializálása
            _tableNoticeBoardTwo = new RobotokTable(3, 3); // második hirdetőtábla inicializálása

            greenTeamCubeAttachState = 0;
            redTeamCubeAttachState = 0;

            _greenTeamPoints = 0;

            if (_teams == 2)
            {
                _redTeamPoints = 0;
            }

            // Táblák inicializálása
            _table = new RobotokTable(17, 28);
            _tableGreenPlayerOne = new RobotokTable(11, 20);
            _tableGreenPlayerTwo = new RobotokTable(11, 20);
            _greenTeamObservation = new Int32[11, 20];
            _redTeamObservation = new Int32[11, 20];

            // Több csapat esetén a piros csapat tábláit is kell inicializálni
            if (_teams == 2)
            {
                _tableRedPlayerOne = new RobotokTable(11, 20);
                _tableRedPlayerTwo = new RobotokTable(11, 20);
            }

            _syncGreenPlayerOne = false;
            _syncGreenPlayerTwo = false;
            _syncRedPlayerOne = false;
            _syncRedPlayerTwo = false;

            for (Int32 i = 0; i < _tableGreenPlayerOne.SizeX; i++) // játékosok tábláját feltöltjük nem látható mezőkkel, illetve a pálya határával
            {
                for (Int32 j = 0; j < _tableGreenPlayerOne.SizeY; j++)
                {
                    if ((i == 0 || i == 10) && (j >= 0 && j <= 19) || (i >= 0 && i <= 10) && (j == 0 || j == 19)) // játékhatárok kigenerálása a játékosok nézetén
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
                        _greenTeamObservation[i, j] = 0; // ahol nem voltak még a játékosok, ott 0-val inicializáljuk a mező láthatóságát (0 - egyik játékos sem látta a mezőt, 1 - a zöld csapatnak megfelelő 1-es játékos látta a mezőt, 8 - a zöld csapatnak megfelelő 2-es játékos látta a mezőt)
                        _tableGreenPlayerOne.SetValue(i, j, 10, _cleaningOperations);
                        _tableGreenPlayerTwo.SetValue(i, j, 10, _cleaningOperations);
                        if (_teams == 2)
                        {
                            _redTeamObservation[i, j] = 0; // (0 - egyik játékos sem látta a mezőt, 2 - a piros csapatnak megfelelő 1-es játékos látta a mezőt, 9 - a piros csapatnak megfelelő 2-es játékos látta a mezőt)
                            _tableRedPlayerOne.SetValue(i, j, 10, _cleaningOperations);
                            _tableRedPlayerTwo.SetValue(i, j, 10, _cleaningOperations);
                        }
                    }
                }
            }

            _gameStepCount = 300;

            GenerateFields();
            GenerateObstacles();
            GenerateExits();
            GenerateShapes();
        }

        #endregion

        #region Public game methods

        /// <summary>
        /// Kiszámolja játékosonként a Manhattan-távolságot majd az így kiszámolt tartományban frissíti az adott játékosok nézeteit
        /// </summary>
        /// <param name="difficulty">Játék nehézsége</param>
        /// <param name="player">Játékos azonosítója</param>
        public void ManhattanDistance(Int32 difficulty, Int32 player)
        {
            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(player, _table);

            Int32 maxDistance; // adott konstans szerinti maximális távolság, amelyen belül frissítjük a nézetet

            switch (difficulty)
            {
                case 1:
                    maxDistance = _manhattanDistanceEasy;
                    break;
                case 2:
                    maxDistance = _manhattanDistanceMedium;
                    break;
                case 3:
                    maxDistance = _manhattanDistanceHard;
                    break;
                default:
                    maxDistance = 0;
                    break;
            }

            Boolean toMerge = false; // Ha egy játékos a csapattársának a Manhattan-távolságában van, akkor egyesítik nézetüket

            for (Int32 i = 0; i < _table.SizeX; i++)
            {
                for (Int32 j = 0; j < _table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23) // játék pályán vagyunk-e
                    {
                        if (Math.Abs(i - playerCoordinateX) + Math.Abs(j - playerCoordinateY) < maxDistance) // ha igen, akkor megnézzük, hogy benne van-e a mező a Manhattan-távolságban
                        {
                            if (player == 1)
                            {
                                _greenTeamObservation[i - 3, j - 4] = 1; // beállítjuk, hogy az adott játékos látta az aktív mezőt (a koordináták el vannak tolva, hogy a játékos nézeten is a megfelelő helyen legyenek beállítva az értékek)
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _table.GetFieldValue(i, j), _table.GetFieldRemainingCleaningOperations(i, j));
                                _tableGreenPlayerOne.SetAttachmentValues(i - 3, j - 4, _table.GetAttachmentNorth(i, j), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i, j), _table.GetAttachmentWest(i, j));
                                _tableGreenPlayerOne.SetFaceDirection(i - 3, j - 4, _table.GetFaceNorth(i, j), _table.GetFaceSouth(i, j), _table.GetFaceEast(i, j), _table.GetFaceWest(i, j));
                                _tableGreenPlayerOne.SetInDistance(i - 3, j - 4, true); // a játékos táblán beállítja, hogy az aktív mező Manhattan-távolságban van-e
                                if (_table.GetFieldValue(i, j) == 8) // ha a csapattárs benne van a Manhattan-távolságban, akkor a két játékos nézetét egyesíteni kell 
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
                        else // ha nincs benne a Manhattan-távolságban az aktív mező
                        {
                            if (player == 1)
                            {
                                _tableGreenPlayerOne.SetInDistance(i - 3, j - 4, false);
                            }
                            if (player == 8)
                            {
                                _tableGreenPlayerTwo.SetInDistance(i - 3, j - 4, false);
                            }
                            if (_teams == 2)
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
            if (toMerge) // ha volt Manhattan-távolságon belül csapattárs
            {
                if (!(player == 1 && _syncGreenPlayerOne) || !(player == 8 && _syncGreenPlayerTwo) || !(player == 2 && _syncRedPlayerOne) || !(player == 9 && _syncRedPlayerTwo))
                {
                    Merge(player);
                }
            }

            switch (player) // ha az egyik játékos már látta a másikat, akkor frissítjük a másik csapattárs területét a jelenleg észlelt területtel
            {
                case 1:
                    if (_syncGreenPlayerTwo)
                    {
                        Observation(8);
                    }
                    break;
                case 8:

                    if (_syncGreenPlayerOne)
                    {
                        Observation(1);
                    }
                    break;
                case 2:
                    if (_syncRedPlayerOne)
                    {
                        Observation(9);
                    }
                    break;
                case 9:
                    if (_syncRedPlayerTwo)
                    {
                        Observation(2);
                    }
                    break;
            }

        }

        /// <summary>
        /// Játékidő léptetése
        /// </summary>
        /// <param name="player">Játékos azonosítója</param>
        public void AdvanceTime(Int32 player)
        {
            if (IsGameOver)
            {
                return;
            }

            _remainingSeconds--;

            if (_remainingSeconds == -1) // ha lejárt a lépések közötti idő, újraindítjuk a visszaszámlálást, majd végrehajtjuk a megadott játékműveletet
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

                if (_firstTaskDeadline == 0 || _secondTaskDeadline == 0) // megnézzük, hogy valamelyik feladatnak már lejárt-e a határideje
                {
                    for (Int32 i = 0; i < _table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < _table.SizeY; j++)
                        {
                            if (_table.GetFieldValue(i, j) == _figure1.GetColor() && _firstTaskDeadline == 0)
                            {
                                _table.SetValue(i, j, 7, -1); // első feladathoz tartozó alakzat építőkockáinak törlése
                            }

                            if (_table.GetFieldValue(i, j) == _figure2.GetColor() && _secondTaskDeadline == 0)
                            {
                                _table.SetValue(i, j, 7, -1); // második feladathoz tartozó alakzat építőkockáinak törlése
                            }

                        }
                    }

                    for (Int32 i = 0; i < _table.SizeX; i++) // ha az alakzat törlése után maradtak üres csatlakozási helyek, akkor azokat is törölni kell
                    {
                        for (Int32 j = 0; j < _table.SizeY; j++)
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

                            if (_table.GetFieldValue(i, j) == 1 || _table.GetFieldValue(i, j) == 2 || _table.GetFieldValue(i, j) == 8 || _table.GetFieldValue(i, j) == 9) // a robot és az építmény első kockája közötti csatolás törlése
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
                    GenerateShapes(); // generálunk egy új alakzatot
                }

                if (player == 1) // minden új kör elején csökkenti a határidőket, lépések számát
                {
                    _gameStepCount--;
                    _firstTaskDeadline--;
                    _secondTaskDeadline--;
                }
            }
        }

        /// <summary>
        /// Tisztítás logikája
        /// </summary>
        /// <param name="direction">Játékos által megadott tisztítási irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean Clear(String direction, Int32 playerNumber)
        {
            updateTeamCubeAttachStates(playerNumber);

            if (direction == "észak") // iránytól függően tisztítást végez az adott kockán
            {
                for (Int32 i = 4; i < _table.SizeX - 4; i++)
                {
                    for (Int32 j = 5; j < _table.SizeY - 5; j++)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (!_table.HasAttachments(i - 1, j) && (_table.GetFieldValue(i - 1, j) == 0 || _table.GetFieldValue(i - 1, j) == 3 || _table.GetFieldValue(i - 1, j) == 4 || _table.GetFieldValue(i - 1, j) == 5 || _table.GetFieldValue(i - 1, j) == 6 || _table.GetFieldValue(i - 1, j) == 11 || _table.GetFieldValue(i - 1, j) == 12)) // megnézi, hogy adott irányban még nem jött létre csatolmány, továbbá megnézi, hogy a csatolni kívánt kocka építőkocka
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i - 1, j) != 1) // ha még lehet a kockát tisztítani, tovább csökkentjük a tisztítások értékét
                                {
                                    _table.SetValue(i - 1, j, _table.GetFieldValue(i - 1, j), _table.GetFieldRemainingCleaningOperations(i - 1, j) - 1);
                                }
                                else // amennyiben a tisztítási műveletek száma eléri a 0-t, töröljük az aktív mezőt
                                {
                                    Int32 cubeValue = _table.GetFieldValue(i - 1, j);
                                    _table.SetValue(i - 1, j, 7, -1);

                                    if (cubeValue != 0) // ha építőkockát töröltünk, generálunk helyette újat a pályán, hogy továbbra is össze lehessen építeni az alakzatot
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            else if (direction == "dél")
            {
                for (Int32 i = _table.SizeX - 5; i > 3; i--)
                {
                    for (Int32 j = _table.SizeY - 6; j > 4; j--)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (!_table.HasAttachments(i + 1, j) && (_table.GetFieldValue(i + 1, j) == 0 || _table.GetFieldValue(i + 1, j) == 3 || _table.GetFieldValue(i + 1, j) == 4 || _table.GetFieldValue(i + 1, j) == 5 || _table.GetFieldValue(i + 1, j) == 6 || _table.GetFieldValue(i + 1, j) == 11 || _table.GetFieldValue(i + 1, j) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i + 1, j) != 1)
                                {
                                    _table.SetValue(i + 1, j, _table.GetFieldValue(i + 1, j), _table.GetFieldRemainingCleaningOperations(i + 1, j) - 1);
                                }
                                else
                                {
                                    Int32 cubeValue = _table.GetFieldValue(i + 1, j);
                                    _table.SetValue(i + 1, j, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            else if (direction == "nyugat")
            {
                for (Int32 i = 4; i < _table.SizeX - 4; i++)
                {
                    for (Int32 j = 5; j < _table.SizeY - 5; j++)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (!_table.HasAttachments(i, j - 1) && (_table.GetFieldValue(i, j - 1) == 0 || _table.GetFieldValue(i, j - 1) == 3 || _table.GetFieldValue(i, j - 1) == 4 || _table.GetFieldValue(i, j - 1) == 5 || _table.GetFieldValue(i, j - 1) == 6 || _table.GetFieldValue(i, j - 1) == 11 || _table.GetFieldValue(i, j - 1) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i, j - 1) != 1)
                                {
                                    _table.SetValue(i, j - 1, _table.GetFieldValue(i, j - 1), _table.GetFieldRemainingCleaningOperations(i, j - 1) - 1);
                                }
                                else
                                {
                                    Int32 cubeValue = _table.GetFieldValue(i, j - 1);
                                    _table.SetValue(i, j - 1, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                for (Int32 i = _table.SizeX - 5; i > 3; i--)
                {
                    for (Int32 j = _table.SizeY - 6; j > 4; j--)
                    {
                        if (_table.GetFieldValue(i, j) == playerNumber)
                        {
                            if (!_table.HasAttachments(i, j + 1) && (_table.GetFieldValue(i, j + 1) == 0 || _table.GetFieldValue(i, j + 1) == 3 || _table.GetFieldValue(i, j + 1) == 4 || _table.GetFieldValue(i, j + 1) == 5 || _table.GetFieldValue(i, j + 1) == 6 || _table.GetFieldValue(i, j + 1) == 11 || _table.GetFieldValue(i, j + 1) == 12))
                            {
                                if (_table.GetFieldRemainingCleaningOperations(i, j + 1) != 1)
                                {
                                    _table.SetValue(i, j + 1, _table.GetFieldValue(i, j + 1), _table.GetFieldRemainingCleaningOperations(i, j + 1) - 1);
                                }
                                else
                                {
                                    Int32 cubeValue = _table.GetFieldValue(i, j + 1);
                                    _table.SetValue(i, j + 1, 7, -1);
                                    if (cubeValue != 0)
                                    {
                                        GenerateNewCube(cubeValue);
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Forgás logikája
        /// </summary>
        /// <param name="direction">Játékos által megadott tisztítási irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean Rotate(String direction, Int32 playerNumber)
        {
            updateTeamCubeAttachStates(playerNumber);

            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(playerNumber, _table);

            if (!_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentWest(playerCoordinateX, playerCoordinateY)) // megnézzük, hogy a játékoshoz már van-e valami kapcsolva, ha nem, akkor csak a játékos irányát kell megváltoztatni
            {
                if (_table.GetFaceNorth(playerCoordinateX, playerCoordinateY))
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
                else if (_table.GetFaceSouth(playerCoordinateX, playerCoordinateY))
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
                else if (_table.GetFaceEast(playerCoordinateX, playerCoordinateY))
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
                else
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
            else // ha a játékosra már vannak csatolva blokkok
            {
                if (_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "észak", _table); // a játékost és a hozzákapcsolt kockákat eltároljuk a régi pozíciókban
                }
                else if (_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "dél", _table);
                }
                else if (_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "kelet", _table);
                }
                else
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "nyugat", _table);
                }

                Boolean validStep; // több kocka forgatása esetén külön eltároljuk a sikerességet

                if (direction == "óramutatóval megegyező")
                {            
                    validStep = RotateRight(playerCoordinateX, playerCoordinateY);   
                }
                else
                {
                    validStep = RotateLeft(playerCoordinateX, playerCoordinateY); 
                }

                _cubesOldPosition.Clear(); // lépés sikerességétől függetlenül visszaállítjuk a listákat az eredeti állapotukba 
                _cubesNewPosition.Clear();

                if (validStep) // a játékos tájolását a kockák forgatásától függetlenül külön meg kell változtatni
                {
                    if (_table.GetFaceNorth(playerCoordinateX, playerCoordinateY))
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
                    else if (_table.GetFaceSouth(playerCoordinateX, playerCoordinateY))
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
                    else if (_table.GetFaceEast(playerCoordinateX, playerCoordinateY))
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
                    else
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

        /// <summary>
        /// Forgás logikája a játékosok nézetein
        /// </summary>
        /// <param name="direction">Játékos által megadott irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        public void RotatePlayerView(String direction, Int32 playerNumber)
        {
            RobotokTable table = null!;
            List<Cube> outOfTableList = null!;

            switch (playerNumber)
            {
                case 1:
                    table = TableGreenPlayerOne;
                    outOfTableList = new List<Cube>(_outOfTableGreenPlayerOne);
                    break;
                case 8:
                    table = TableGreenPlayerTwo;
                    outOfTableList = new List<Cube>(_outOfTableGreenPlayerTwo);
                    break;
                case 2:
                    table = TableRedPlayerOne;
                    outOfTableList = new List<Cube>(_outOfTableRedPlayerOne);
                    break;
                case 9:
                    table = TableRedPlayerTwo;
                    outOfTableList = new List<Cube>(_outOfTableRedPlayerTwo);
                    break;
            }

            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(playerNumber, table);

            if (table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY))
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "észak", table);
            }
            else if (table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "dél", table);
            }
            else if (table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "kelet", table);
            }
            else
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "nyugat", table);
            }

            if (direction == "óramutatóval megegyező")
            {
                RotatePlayerRight(playerCoordinateX, playerCoordinateY, playerNumber, table, outOfTableList);
            }
            else
            {
                RotatePlayerLeft(playerCoordinateX, playerCoordinateY, playerNumber, table, outOfTableList);
            }
           
            for (Int32 i = 0; i < _cubesOldPosition.Count; i++) // játékosonként külön listákba bekapolja a forgatás előtti és utáni állapotokat
            {
                if (playerNumber == 1)
                {
                    _observationGreenPlayerOne.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 8)
                {
                    _observationGreenPlayerTwo.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 2)
                {
                    _observationRedPlayerOne.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 9)
                {
                    _observationRedPlayerTwo.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
            }

            for (Int32 i = 0; i < _cubesNewPosition.Count; i++)
            {
                if (playerNumber == 1)
                {
                    _observationGreenPlayerOne.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 2)
                {
                    _observationGreenPlayerTwo.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 3)
                {
                    _observationRedPlayerOne.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else
                {
                    _observationRedPlayerTwo.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
            }

            _cubesOldPosition.Clear(); // a lépés után visszaálítjuk a listákat az eredeti állapotukba
            _cubesNewPosition.Clear();
        }

        /// <summary>
        /// Lekapcsolás logikája
        /// </summary>
        /// <param name="direction">Játékos által megadott irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean Detach(String direction, Int32 playerNumber)
        {
            updateTeamCubeAttachStates(playerNumber);

            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(playerNumber, _table);

            if (direction == "észak")
            {
                if (playerCoordinateX == 3 && _table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY)) // játékos irányától függően megnézzük, hogy a játékterület szélén áll-e és szeretne-e kockát lecsatolni (ez az az eset, amikor alakzatkiértékelést hajtunk végre)
                {
                    Int32 result = EvaluateShape(direction);

                    if (result > 0)
                    {
                        EvaluateResult(result, playerNumber);

                        _table.SetAttachmentNorth(playerCoordinateX, playerCoordinateY, false); // az alakzat és a játékos közötti csatolást külön el kell távolítanunk

                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (_table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 3 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 4 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 5 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 6 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 11 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 12) // ellenőrzi, hogy a lecsatolni kívánt kocka építőkocka-e
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
                    Int32 result = EvaluateShape("dél");
                    if (result > 0)
                    {
                        EvaluateResult(result, playerNumber);

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
                    Int32 result = EvaluateShape("kelet");
                    if (result > 0)
                    {
                        EvaluateResult(result, playerNumber);

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
                    Int32 result = EvaluateShape("nyugat");
                    if (result > 0)
                    {
                        EvaluateResult(result, playerNumber);

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
        /// <param name="direction">Játékos által megadott irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean Attach(String direction, Int32 playerNumber)
        {
            updateTeamCubeAttachStates(playerNumber);

            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(playerNumber, _table);

            if (_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY) || _table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY) || _table.GetAttachmentEast(playerCoordinateX, playerCoordinateY) || _table.GetAttachmentWest(playerCoordinateX, playerCoordinateY)) // megnézzük, hogy a játékoshoz már van-e valami kapcsolva (ha igen, akkor a művelet sikertelen, hiszen egy játékoshoz nem lehet több irányból több kockát csatolni, csak egy irányból egyet)
            {
                return false;
            }

            if (direction == "észak") // irányparamétertől függően megpróbáljuk csatlakoztatni a játékost a kockához(ami csak építőkocka lehet)
            {
                if (_table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 3 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 4 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 5 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 6 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 11 || _table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 12)
                {
                    _table.SetAttachmentNorth(playerCoordinateX, playerCoordinateY, true);
                    _table.SetAttachmentSouth(playerCoordinateX - 1, playerCoordinateY, true);
                    return true;
                }
                else
                {
                    return false;
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
            else
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
            _remainingSeconds = 1; // segítségével felgyorsítjuk a következő művelet bekövetkezését
        }

        /// <summary>
        /// Lépés logikája
        /// </summary>
        /// <param name="direction">Játékos által megadott irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean Move(String direction, Int32 playerNumber)
        {
            updateTeamCubeAttachStates(playerNumber);

            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(playerNumber, _table);

            if (!_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY) && !_table.GetAttachmentWest(playerCoordinateX, playerCoordinateY)) // megnézzük, hogy csak a játékost kell-e léptetnünk, vagy vele együtt más kockákat is (először az az eset következik, ha a játékoshoz nincs csatolva semmi)
            {
                if (direction == "észak")
                {
                    if (_table.GetFieldValue(playerCoordinateX - 1, playerCoordinateY) == 7) // megnézi, hogy üres kockára lép-e
                    {
                        _table.SetValue(playerCoordinateX - 1, playerCoordinateY, playerNumber, -1); // új helyre léptetjük a megadott irány szerint
                        _table.SetFaceDirection(playerCoordinateX - 1, playerCoordinateY, _table.GetFaceNorth(playerCoordinateX, playerCoordinateY), _table.GetFaceSouth(playerCoordinateX, playerCoordinateY), _table.GetFaceEast(playerCoordinateX, playerCoordinateY), _table.GetFaceWest(playerCoordinateX, playerCoordinateY)); // az új helyre átadjuk, hogy merre nézett a játékos
                        _table.SetValue(playerCoordinateX, playerCoordinateY, 7, -1); // régi helyről letöröljük a játékost
                        _table.SetFaceDirection(playerCoordinateX, playerCoordinateY, false, false, false, false); // a régi helyről töröljük hogy merre nézett a játékos
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
                        _table.SetValue(playerCoordinateX + 1, playerCoordinateY, playerNumber, -1);
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
                        _table.SetValue(playerCoordinateX, playerCoordinateY + 1, playerNumber, -1);
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
                        _table.SetValue(playerCoordinateX, playerCoordinateY - 1, playerNumber, -1);
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
            else // az az eset, amikor a játékoshoz van egy vagy több kocka van csatolva
            {
                if (_table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY)) // a játékost és a hozzákapcsolt kockákat eltároljuk a régi pozíciók listájában
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "észak", _table);
                }
                else if (_table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "dél", _table);
                }
                else if (_table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "kelet", _table);
                }
                else
                {
                    AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "nyugat", _table);
                }

                Boolean validStep = true;

                if (direction == "észak") // ellenőrizzük azokat az eseteket, ha a játékos a pálya szélén áll az építménnyel, és ki szeretne lépni a pályáról
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

                Int32[,] stepTest = new Int32[_table.SizeX, _table.SizeY]; // készítünk egy másolatot az eredeti játéktábláról, hogy ellenőrzés során ha kiderül, hogy érvénytelen a lépés, ne veszítsünk adatot

                for (Int32 i = 0; i < _table.SizeX; i++)
                {
                    for (Int32 j = 0; j < _table.SizeY; j++)
                    {
                        stepTest[i, j] = _table.GetFieldValue(i, j);
                    }
                }

                for (Int32 i = 0; i < _cubesOldPosition.Count; i++) // letöröljük a régi kockapozíciókat a másolt tábláról
                {
                    stepTest[_cubesOldPosition[i].x, _cubesOldPosition[i].y] = 7;
                }

                for (Int32 i = 0; i < _cubesOldPosition.Count && validStep; i++) // ellenőrizzük, hogy az adott irányba történő léptetés után a kocka érvényes pozícióra kerülne-e
                {
                    if (direction == "észak")
                    {
                        if (_cubesOldPosition[i].x - 1 < 0) // annak az esetnek a kiszűrése, ha kiindexelnénk a pályáról
                        {
                            validStep = false;
                        }
                        else if (stepTest[_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y] != 7 && stepTest[_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y] != -2) // abban az esetben, ha az aktív kocka olyan pozícióra kerülne, ami nem üres kocka sem pedig játékon kívüli terület, érvénytelen a lépés
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

                if (validStep)
                {
                    ExecuteSafeSteps(direction);
                }

                _cubesOldPosition.Clear(); // sikerességtől függetlenül a lépés után visszaállítjuk a listákat az eredeti állapotukba
                _cubesNewPosition.Clear();

                return validStep;
            }
        }

        /// <summary>
        /// Lépés logikája a játékos nézetén
        /// </summary>
        /// <param name="direction">Játékos által megadott irány</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        public void MovePlayerView(String direction, Int32 playerNumber)
        {
            RobotokTable table = null!;
            List<Cube> outOfTableList = null!;

            switch (playerNumber)
            {
                case 1:
                    table = TableGreenPlayerOne;
                    outOfTableList = new List<Cube>(_outOfTableGreenPlayerOne);
                    break;
                case 8:
                    table = TableGreenPlayerTwo;
                    outOfTableList = new List<Cube>(_outOfTableGreenPlayerTwo);
                    break;
                case 2:
                    table = TableRedPlayerOne;
                    outOfTableList = new List<Cube>(_outOfTableRedPlayerOne);
                    break;
                case 9:
                    table = TableRedPlayerTwo;
                    outOfTableList = new List<Cube>(_outOfTableRedPlayerTwo);
                    break;
            }

            (Int32 playerCoordinateX, Int32 playerCoordinateY) = getActivePlayerCoordinates(playerNumber, table);

            if (table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY)) // a játékost és a hozzákapcsolt kockákat eltároljuk a régi pozíciók listájában
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "észak", table);
            }
            else if (table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY))
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "dél", table);
            }
            else if (table.GetAttachmentEast(playerCoordinateX, playerCoordinateY))
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "kelet", table);
            }
            else
            {
                AddCubesToOldList(playerCoordinateX, playerCoordinateY, playerNumber, "nyugat", table);
            }

            ExecutePlayerSteps(direction, table, outOfTableList, playerNumber);

            for (Int32 i = 0; i < _cubesOldPosition.Count; i++)
            {
                if (playerNumber == 1)
                {
                    _observationGreenPlayerOne.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations)); // elmentjük azokat a mezőket, ahonnan forgattunk
                }
                else if (playerNumber == 8)
                {
                    _observationGreenPlayerTwo.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 2)
                {
                    _observationRedPlayerOne.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 9)
                {
                    _observationRedPlayerTwo.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, false, false, false, false, _cubesOldPosition[i].direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
            }

            for (Int32 i = 0; i < _cubesNewPosition.Count; i++)
            {
                if (playerNumber == 1)
                {
                    _observationGreenPlayerOne.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 8)
                {
                    _observationGreenPlayerTwo.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 2)
                {
                    _observationRedPlayerOne.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
                else if (playerNumber == 9)
                {
                    _observationRedPlayerTwo.Add(new Cube(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment, _cubesNewPosition[i].direction, _cubesNewPosition[i].remainingCleaningOperations));
                }
            }

            _cubesOldPosition.Clear();
            _cubesNewPosition.Clear();
        }

        /// <summary>
        /// Lépés logikája a játékos nézetén
        /// </summary>
        /// <param name="group">Kockák csatolását kezdeményező csapat</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean AttachCubes(String group)
        {
            if (group == "green") // először ellenőrizzük, hogy a két játékos által megadott kockapozíciók egyeznek-e (csoporttól függően)
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

            if (group == "green") // következő lépésként ellenőrizzük, hogy a két kocka olyan típusú-e, amit lehet csatolni (mindegy melyik játékoséval nézzük, hiszen ha idáig eljutunk akkor a két játékos által megadott kockák egyenlőek)
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

            if (group == "green") // ellenőriznünk kell azt is, hogy az összekapcsolni kívánt két kocka megegyező színű-e
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

            if (group == "green") // következő lépésként meghatározzuk a kapcsolás pozícióját (élszomszédosság helye alapján), majd létrehozzuk azt
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
                else // ez az az eset, amikor a kiválasztott kockák nem élszomszédosak
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
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Szétkapcsolás logikája
        /// </summary>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <returns>A művelet sikeressége</returns>
        public Boolean DetachCubes(Int32 playerNumber)
        {
            updateTeamCubeAttachStates(playerNumber);

            if (!(_table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 3 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 4 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 5 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 6 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 11 || _table.GetFieldValue(cubeToDetach1X, cubeToDetach1Y) == 12) || !(_table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 3 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 4 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 5 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 6 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 11 || _table.GetFieldValue(cubeToDetach2X, cubeToDetach2Y) == 12)) // először ellenőrizzük, hogy építőkockákat adtak-e meg a szétválasztáshoz
            {
                return false;
            }

            if ((cubeToDetach1X == cubeToDetach2X) && (cubeToDetach1Y - 1 == cubeToDetach2Y)) // következő lépésként élszomszédosság helye alapján töröljük a kapcsolatot
            {
                if (_table.GetAttachmentWest(cubeToDetach1X, cubeToDetach1Y) && _table.GetAttachmentEast(cubeToDetach1X, cubeToDetach2Y)) // ellenőrizzük, hogy a szétkapcsolandó helyen van-e összekapcsolás
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
            else // az az eset, amikor a kiválasztott kockák nem élszomszédosak
            {
                return false;
            }
        }

        /// <summary>
        /// Fejlesztett megfigyelés, a mozgatott illetve forgatott, már észlelt kockák megjelenítése a játékosok számára
        /// </summary>
        /// <param name="player">Játékos azonosítója</param>
        public void ImprovedObservation(Int32 player)
        {
            List<Cube> listOfAttachedCubes = new List<Cube>();

            switch (player)
            {
                case 1:
                    listOfAttachedCubes = _observationGreenPlayerOne;
                    break;
                case 8:
                    listOfAttachedCubes = _observationGreenPlayerTwo;
                    break;
                case 2:
                    listOfAttachedCubes = _observationRedPlayerOne;
                    break;
                case 9:
                    listOfAttachedCubes = _observationRedPlayerTwo;
                    break;
            }

            for (Int32 i = 0; i < listOfAttachedCubes.Count; i++)
            {
                if (player == 1)
                {
                    _tableGreenPlayerOne.SetValue(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].value, listOfAttachedCubes[i].remainingCleaningOperations);
                    _tableGreenPlayerOne.SetAttachmentValues(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].northAttachment, listOfAttachedCubes[i].southAttachment, listOfAttachedCubes[i].eastAttachment, listOfAttachedCubes[i].westAttachment);
                }
                else if (player == 8)
                {
                    _tableGreenPlayerTwo.SetValue(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].value, listOfAttachedCubes[i].remainingCleaningOperations);
                    _tableGreenPlayerTwo.SetAttachmentValues(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].northAttachment, listOfAttachedCubes[i].southAttachment, listOfAttachedCubes[i].eastAttachment, listOfAttachedCubes[i].westAttachment);
                }
                else if (player == 2)
                {
                    _tableRedPlayerOne.SetValue(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].value, listOfAttachedCubes[i].remainingCleaningOperations);
                    _tableRedPlayerOne.SetAttachmentValues(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].northAttachment, listOfAttachedCubes[i].southAttachment, listOfAttachedCubes[i].eastAttachment, listOfAttachedCubes[i].westAttachment);
                }
                else if (player == 9)
                {
                    _tableRedPlayerTwo.SetValue(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].value, listOfAttachedCubes[i].remainingCleaningOperations);
                    _tableRedPlayerTwo.SetAttachmentValues(listOfAttachedCubes[i].x, listOfAttachedCubes[i].y, listOfAttachedCubes[i].northAttachment, listOfAttachedCubes[i].southAttachment, listOfAttachedCubes[i].eastAttachment, listOfAttachedCubes[i].westAttachment);
                }
            }
        }

        #endregion

        #region Private game methods

        /// <summary>
        /// Kijáratok generálása
        /// </summary>
        private void GenerateExits()
        {
            Random randomCoordinate = new Random();

            Int32 exitStartPosition;

            for (Int32 exit = 0; exit < 4; exit++) // minden irányban generálunk egy kijáratot
            {
                if (exit == 0 || exit == 2)
                {
                    exitStartPosition = randomCoordinate.Next(4, 8);

                    if (exit == 0)
                    {
                        for (Int32 i = 0; i < 4; i++)
                        {
                            _table.SetValue(exitStartPosition + i, 4, 7, -1); // a határon a kijárat kockáit átalakítjük üres kockává
                        }
                    }
                    else
                    {
                        for (Int32 i = 0; i < 4; i++)
                        {
                            _table.SetValue(exitStartPosition + i, 23, 7, -1);
                        }
                    }
                }
                else
                {
                    exitStartPosition = randomCoordinate.Next(5, 18);

                    if (exit == 1)
                    {
                        for (Int32 j = 0; j < 4; j++)
                        {
                            _table.SetValue(3, exitStartPosition + j, 7, -1);
                        }
                    }
                    else
                    {
                        for (Int32 j = 0; j < 4; j++)
                        {
                            _table.SetValue(13, exitStartPosition + j, 7, -1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Akadályok generálása
        /// </summary>
        private void GenerateObstacles()
        {
            Random random = new Random();

            Int32 obstacleCount = random.Next(5, 7);

            Int32 coordinateX = random.Next(4, 13);
            Int32 coordinateY = random.Next(5, 23);

            for (Int32 i = 0; i < obstacleCount; i++)
            {
                while (_table.GetFieldValue(coordinateX, coordinateY) != 7) // addig generáljuk az új koordinátákat, amíg üres helyet nem találunk
                {
                    coordinateX = random.Next(4, 13);
                    coordinateY = random.Next(5, 23);
                }

                _table.SetValue(coordinateX, coordinateY, 0, _cleaningOperations);
            }
        }

        /// <summary>
        /// Mezők generálása
        /// </summary>
        private void GenerateFields()
        {
            Random random = new Random();

            for (Int32 i = 0; i < _table.SizeX; i++)
            {
                for (Int32 j = 0; j < _table.SizeY; j++)
                {
                    _table.SetAttachmentValues(i, j, false, false, false, false); // beállítjuk a csatolmányokat (kezdetben egyetlen cellának sincs csatolmánya)

                    if (i < 3 || i > 13 || j < 4 || j > 23) // azoknak a mezőknek az esete, amelyek a pálya határán túl helyezkednek el
                    {
                        _table.SetValue(i, j, -2, -1); // a pálya határán túli mezők törhetetlenek, ezért -1 a tisztító műveletük
                    }
                    else if (i == 3 || i == 13 || j == 4 || j == 23) // a határt képező mezők esete
                    {
                        _table.SetValue(i, j, -1, -1);
                    }
                    else // a többi mező kitöltése (általánosan, játékosok, akadályok, kijáratok mezői még nincsenek)
                    {
                        _table.SetValue(i, j, 7, -1);
                    }
                }
            }

            Int32 greenPlayerOneI, greenPlayerOneJ, greenPlayerTwoI, greenPlayerTwoJ; // játékosok mezőinek kigenerálása
            greenPlayerOneI = random.Next(4, 13);
            greenPlayerOneJ = random.Next(5, 23);
            greenPlayerTwoI = random.Next(4, 13);
            greenPlayerTwoJ = random.Next(5, 23);

            while (greenPlayerOneI == greenPlayerTwoI && greenPlayerTwoJ == greenPlayerOneJ)
            {
                greenPlayerOneI = random.Next(4, 13);
                greenPlayerOneJ = random.Next(5, 23);
            }

            _table.SetValue(greenPlayerOneI, greenPlayerOneJ, 1, -1);
            _table.SetValue(greenPlayerTwoI, greenPlayerTwoJ, 8, -1);

            if (_teams == 2)
            {
                Int32 redPlayerOneI, redPlayerOneJ, redPlayerTwoI, redPlayerTwoJ;
                redPlayerOneI = random.Next(4, 13);
                redPlayerOneJ = random.Next(5, 23);
                redPlayerTwoI = random.Next(4, 13);
                redPlayerTwoJ = random.Next(5, 23);

                while (_table.GetFieldValue(redPlayerOneI, redPlayerOneJ) != 7)
                {
                    redPlayerOneI = random.Next(4, 13);
                    redPlayerOneJ = random.Next(5, 23);
                }
                _table.SetValue(redPlayerOneI, redPlayerOneJ, 2, -1);

                while (_table.GetFieldValue(redPlayerTwoI, redPlayerTwoJ) != 7)
                {
                    redPlayerTwoI = random.Next(4, 13);
                    redPlayerTwoJ = random.Next(5, 23);
                }

                _table.SetValue(redPlayerTwoI, redPlayerTwoJ, 9, -1);
            }
        }

        /// <summary>
        /// ImprovedObservation függvényhez felhasznált Listák törlése
        /// </summary>
        /// <param name="player">Játékos azonosítója</param>
        public void ClearImprovedObservationList(Int32 player)
        {
            switch (player)
            {
                case 1:
                    _observationGreenPlayerOne.Clear();
                    break;
                case 8:
                    _observationGreenPlayerTwo.Clear();
                    break;
                case 2:
                    _observationRedPlayerOne.Clear();
                    break;
                case 9:
                    _observationRedPlayerTwo.Clear();
                    break;
            }
        }

        /// <summary>
        /// Miután a játékosok nézetét egyesítettük, tájékoztatjuk őket folyamatosan egymás megfigyeléseiről
        /// </summary>
        /// <param name="player">Játékos azonosítója</param>
        private void Observation(Int32 player)
        {
            for (Int32 i = 0; i < _table.SizeX; i++)
            {
                for (Int32 j = 0; j < _table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23) // játék pályán vagyunk-e
                    {
                        if (((player == 1 || player == 8) && (_syncGreenPlayerOne && _syncGreenPlayerTwo)) || ((player == 2 || player == 9) && (_syncRedPlayerOne && _syncRedPlayerTwo))) // ha mindkét játékos ugyanabban a csapatban látta már egymást, akkor minden mezőt átadunk
                        {
                            if (player == 1) // frissítjük az egyes játékosok látómezőjét a csapattárs aktuális látómezőjével
                            {
                                _tableGreenPlayerOne.SetValue(i - 3, j - 4, _tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                _tableGreenPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableGreenPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                _tableGreenPlayerOne.SetFaceDirection(i - 3, j - 4, _tableGreenPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceEast(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceWest(i - 3, j - 4));
                            }
                            else if (player == 8)
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
                                else if (player == 9)
                                {
                                    _tableRedPlayerTwo.SetValue(i - 3, j - 4, _tableRedPlayerOne.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableRedPlayerTwo.SetAttachmentValues(i - 3, j - 4, _tableRedPlayerOne.GetAttachmentNorth(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentSouth(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentEast(i - 3, j - 4), _tableRedPlayerOne.GetAttachmentWest(i - 3, j - 4));
                                    _tableRedPlayerTwo.SetFaceDirection(i - 3, j - 4, _tableRedPlayerOne.GetFaceNorth(i - 3, j - 4), _tableRedPlayerOne.GetFaceSouth(i - 3, j - 4), _tableRedPlayerOne.GetFaceEast(i - 3, j - 4), _tableRedPlayerOne.GetFaceWest(i - 3, j - 4));
                                }
                            }
                        }
                        else
                        {
                            if (player == 1) // ha csak az egyik játékos látta a másikat, akkor a másik játékos fel nem fedezett mezőit nem kell átadnunk
                            {
                                if (_tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != 10  && _tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != -1)
                                {
                                    _tableGreenPlayerOne.SetValue(i - 3, j - 4, _tableGreenPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                    _tableGreenPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableGreenPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableGreenPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                    _tableGreenPlayerOne.SetFaceDirection(i - 3, j - 4, _tableGreenPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceEast(i - 3, j - 4), _tableGreenPlayerTwo.GetFaceWest(i - 3, j - 4));
                                }
                            }
                            else if (player == 8)
                            {
                                if (_tableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != 10 && _tableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != -1)
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
                                    if (_tableRedPlayerOne.GetFieldValue(i - 3, j - 4) != 10 && _tableRedPlayerOne.GetFieldValue(i - 3, j - 4) != -1)
                                    {
                                        _tableRedPlayerOne.SetValue(i - 3, j - 4, _tableRedPlayerTwo.GetFieldValue(i - 3, j - 4), _table.GetFieldRemainingCleaningOperations(i, j));
                                        _tableRedPlayerOne.SetAttachmentValues(i - 3, j - 4, _tableRedPlayerTwo.GetAttachmentNorth(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentSouth(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentEast(i - 3, j - 4), _tableRedPlayerTwo.GetAttachmentWest(i - 3, j - 4));
                                        _tableRedPlayerOne.SetFaceDirection(i - 3, j - 4, _tableRedPlayerTwo.GetFaceNorth(i - 3, j - 4), _tableRedPlayerTwo.GetFaceSouth(i - 3, j - 4), _tableRedPlayerTwo.GetFaceEast(i - 3, j - 4), _tableRedPlayerTwo.GetFaceWest(i - 3, j - 4));
                                    }
                                }
                                else if (player == 9)
                                {
                                    if (_tableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != 10 && _tableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != -1)
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

        /// <summary>
        /// Első találkozás során a pálya frissítése
        /// </summary>
        /// <param name="player">Játékos azonosítója</param>
        private void Merge(Int32 player)
        {
            if (player == 1) // megnézzük, hogy melyik játékos észlelte a másikat, és az ő területét frissítjük
            {
                for (Int32 i = 0; i < 11; i++)
                {
                    for (Int32 j = 0; j < 20; j++)
                    {
                        if (_greenTeamObservation[i, j] == 8)
                        {
                            if (TableGreenPlayerTwo.GetFieldValue(i, j) != 8)
                            {
                                TableGreenPlayerOne.SetValue(i, j, TableGreenPlayerTwo.GetFieldValue(i, j), TableGreenPlayerTwo.GetFieldRemainingCleaningOperations(i, j));
                                TableGreenPlayerOne.SetAttachmentValues(i, j, TableGreenPlayerTwo.GetAttachmentNorth(i, j), TableGreenPlayerTwo.GetAttachmentSouth(i, j), TableGreenPlayerTwo.GetAttachmentEast(i, j), TableGreenPlayerTwo.GetAttachmentWest(i, j));
                                TableGreenPlayerOne.SetFaceDirection(i, j, TableGreenPlayerTwo.GetFaceNorth(i, j), TableGreenPlayerTwo.GetFaceSouth(i, j), TableGreenPlayerTwo.GetFaceEast(i, j), TableGreenPlayerTwo.GetFaceWest(i, j));
                            }
                            else // ez azért kell, hogy ne jelenjen meg kétszer a frissen észlelt játékos az egyesítés után
                            {
                                TableGreenPlayerOne.SetValue(i, j, _table.GetFieldValue(i + 3, j + 4), _table.GetFieldRemainingCleaningOperations(i + 3, j + 4));
                                TableGreenPlayerOne.SetAttachmentValues(i, j, _table.GetAttachmentNorth(i + 3, j + 4), _table.GetAttachmentSouth(i, j), _table.GetAttachmentEast(i + 3, j + 4), _table.GetAttachmentWest(i + 3, j + 4));
                                TableGreenPlayerOne.SetFaceDirection(i, j, _table.GetFaceNorth(i + 3, j + 4), _table.GetFaceSouth(i + 3, j + 4), _table.GetFaceEast(i + 3, j + 4), _table.GetFaceWest(i + 3, j + 4));
                            }
                        }
                    }
                }

                _syncGreenPlayerOne = true;  // innentől kezdve folyamatosan szinkronizálják majd a térképeket
            }
            else if (player == 8)
            {
                for (Int32 i = 0; i < 11; i++)
                {
                    for (Int32 j = 0; j < 20; j++)
                    {
                        if (_greenTeamObservation[i, j] == 1)
                        {
                            if (TableGreenPlayerOne.GetFieldValue(i, j) != 1) 
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

                _syncGreenPlayerTwo = true;
            }
            else if (player == 2)
            {
                for (Int32 i = 0; i < 11; i++)
                {
                    for (Int32 j = 0; j < 20; j++)
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

                _syncRedPlayerOne = true;
            }
            else
            {
                for (Int32 i = 0; i < 11; i++)
                {
                    for (Int32 j = 0; j < 20; j++)
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

                _syncRedPlayerTwo = true;
            }
        }

        /// <summary>
        /// Új kocka generálása tisztítás után
        /// </summary>
        /// <param name="cubeValue">Kocka típusa, amelyből szeretnénk újat generálni</param>
        private void GenerateNewCube(Int32 cubeValue)
        {
            Random random = new Random();
            Int32 newCubeCoordinateX, newCubeCoordinateY;
            newCubeCoordinateX = random.Next(4, 13);
            newCubeCoordinateY = random.Next(5, 23);

            while (_table.GetFieldValue(newCubeCoordinateX, newCubeCoordinateY) != 7)
            {
                newCubeCoordinateX = random.Next(4, 13);
                newCubeCoordinateY = random.Next(5, 23);
            }

            _table.SetValue(newCubeCoordinateX, newCubeCoordinateY, cubeValue, _cleaningOperations);  
        }

        /// <summary>
        /// Új alakzat létrehozása
        /// </summary>
        private void GenerateShapes()
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

                FirstTaskInitialization();
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

                SecondTaskInitialization();
            } 
        }

        /// <summary>
        /// Alakzat újragenerálása
        /// </summary>
        /// <param name="taskNumber">Feladat azonosítója</param>
        private void RegenerateShape(Int32 taskNumber)
        {
            if (taskNumber == 1)
            {
                FirstTaskInitialization();
            }
            else if (taskNumber == 2)
            {
                SecondTaskInitialization();
            }
        }

        /// <summary>
        /// Alakzattal történő balra forgás a játékos nézeten
        /// </summary
        /// <param name="playerCoordinateX">Játékos X koordinátája</param>
        /// <param name="playerCoordinateY">Játékos Y koordinátája</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <param name="table">Tábla azonosítója</param>
        /// <param name="outOfTableList">Pályán kívülre eső kockák listája</param>
        private void RotatePlayerLeft(Int32 playerCoordinateX, Int32 playerCoordinateY, Int32 playerNumber, RobotokTable table, List<Cube> outOfTableList)
        {
            _cubesNewPosition.Add(new Cube(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].southAttachment, _cubesOldPosition[0].northAttachment, "", _cubesOldPosition[0].remainingCleaningOperations)); // felvesszük elsőként a játékos kockáját a listába

            List<Cube> edgeCaseCubes = new List<Cube>(); // létrehozunk egy listát azoknak a kockáknak, amelyeknek az állapotára forgáskor külön szabályok vonatkoznak

            Int32 i = 0;

            while (i < outOfTableList.Count)
            {
                Int32 x = _cubesOldPosition[0].x - (outOfTableList[i].y - _cubesOldPosition[0].y);
                Int32 y = _cubesOldPosition[0].y + (outOfTableList[i].x - _cubesOldPosition[0].x);

                if (x >= 0 && x <= 10 && y >= 0 && y <= 19) // ha a kocka forgatás után a táblára esik, akkor eltároljuk
                {
                    _cubesNewPosition.Add(new Cube(x, y, outOfTableList[i].value, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].southAttachment, outOfTableList[i].northAttachment, "", outOfTableList[i].remainingCleaningOperations));
                }
                else // ha a kocka forgatás után szintén pályán kívülre esne, akkor belerakjuk egy külön listába
                {
                    edgeCaseCubes.Add(new Cube(x, y, outOfTableList[i].value, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].southAttachment, outOfTableList[i].northAttachment, "", outOfTableList[i].remainingCleaningOperations));
                }
                i++;
            }
            outOfTableList.Clear();
            outOfTableList = new List<Cube>(edgeCaseCubes);

            i = 1;
            while (i < _cubesOldPosition.Count) // minden régi mezőnek eltároljuk az elforgatott állapotát 
            {
                Int32 x = _cubesOldPosition[0].x - (_cubesOldPosition[i].y - _cubesOldPosition[0].y);
                Int32 y = _cubesOldPosition[0].y + (_cubesOldPosition[i].x - _cubesOldPosition[0].x);

                if (x >= 0 && x <= 10 && y >= 0 && y <= 19)
                {
                    if (_cubesOldPosition[i].value == 10)
                    {
                        _cubesNewPosition.Add(new Cube(x, y, _table.GetFieldValue(x + 3, y + 4), _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].southAttachment, _cubesOldPosition[i].northAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                    }
                    else
                    {
                        _cubesNewPosition.Add(new Cube(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].southAttachment, _cubesOldPosition[i].northAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                    }

                }
                else
                {
                    outOfTableList.Add(new Cube(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].southAttachment, _cubesOldPosition[i].northAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                }
                i++;
            }

            i = 0;
            while (i < _cubesOldPosition.Count) // ha idáig elértünk, akkor minden rendben volt a forgatás során, már csak el kell tárolni a kockákat a táblán az új kapcsolatokkal együtt
            {
                table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false); // kitöröljük az összes régi mező kapcsolatát

                if (i != 0)
                {
                    table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);  // kitöröljük az összes régi mezőt, ami nem a játékos
                }
                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                if (i != 0)
                {
                    table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations); // beállítjuk az új mező értékeket        
                }
                table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);  // beállítjuk az új kapcsolatokat
                i++;
            }

            switch (playerNumber)
            {
                case 1:
                    _outOfTableGreenPlayerOne = new List<Cube>(outOfTableList);
                    break;
                case 8:
                    _outOfTableGreenPlayerTwo = new List<Cube>(outOfTableList);
                    break;
                case 2:
                    _outOfTableRedPlayerOne = new List<Cube>(outOfTableList);
                    break;
                case 9:
                    _outOfTableRedPlayerTwo = new List<Cube>(outOfTableList);
                    break;
            }
        }

        /// <summary>
        /// Alakzattal történő jobbra forgás a játékos nézeten
        /// </summary
        /// <param name="playerCoordinateX">Játékos X koordinátája</param>
        /// <param name="playerCoordinateY">Játékos Y koordinátája</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <param name="table">Tábla azonosítója</param>
        /// <param name="outOfTableList">Pályán kívülre eső kockák listája</param>
        private void RotatePlayerRight(Int32 playerCoordinateX, Int32 playerCoordinateY, Int32 playerNumber, RobotokTable table, List<Cube> outOfTableList)
        {
            _cubesNewPosition.Add(new Cube(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].northAttachment, _cubesOldPosition[0].southAttachment, "", _cubesOldPosition[0].remainingCleaningOperations));

            List<Cube> edgeCaseCubes = new List<Cube>();

            Int32 i = 0;
            while (i < outOfTableList.Count)
            {
                Int32 x = _cubesOldPosition[0].x - (_cubesOldPosition[0].y - outOfTableList[i].y);
                Int32 y = _cubesOldPosition[0].y + (_cubesOldPosition[0].x - outOfTableList[i].x);
                if (x >= 0 && x <= 10 && y >= 0 && y <= 19)
                {
                    _cubesNewPosition.Add(new Cube(x, y, outOfTableList[i].value, outOfTableList[i].westAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, "", outOfTableList[i].remainingCleaningOperations));
                }
                else
                {
                    edgeCaseCubes.Add(new Cube(x, y, outOfTableList[i].value, outOfTableList[i].westAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, "", outOfTableList[i].remainingCleaningOperations));
                }
                i++;
            }
            outOfTableList.Clear();
            outOfTableList = new List<Cube>(edgeCaseCubes);

            i = 1;
            while (i < _cubesOldPosition.Count)
            {
                Int32 x = _cubesOldPosition[0].x - (_cubesOldPosition[0].y - _cubesOldPosition[i].y);
                Int32 y = _cubesOldPosition[0].y + (_cubesOldPosition[0].x - _cubesOldPosition[i].x);

                if (x >= 0 && x <= 10 && y >= 0 && y <= 19)
                {
                    if (_cubesOldPosition[i].value == 10)
                    {
                        _cubesNewPosition.Add(new Cube(x, y, _table.GetFieldValue(x + 3, y + 4), _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].northAttachment, _cubesOldPosition[i].southAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                    }
                    else
                    {
                        _cubesNewPosition.Add(new Cube(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].northAttachment, _cubesOldPosition[i].southAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                    }
                }
                else
                {
                    outOfTableList.Add(new Cube(x, y, _cubesOldPosition[i].value, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].northAttachment, _cubesOldPosition[i].southAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));
                }
                i++;
            }

            i = 0;
            while (i < _cubesOldPosition.Count)
            {
                table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false);


                if (i != 0)
                {
                    table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);
                }

                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                if (i != 0)
                {
                    table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);
                }
                table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);  //beállítjuk az új kapcsolatokat


                i++;
            }

            switch (playerNumber)
            {
                case 1:
                    _outOfTableGreenPlayerOne = new List<Cube>(outOfTableList);
                    break;
                case 8:
                    _outOfTableGreenPlayerTwo = new List<Cube>(outOfTableList);
                    break;
                case 2:
                    _outOfTableRedPlayerOne = new List<Cube>(outOfTableList);
                    break;

                case 9:
                    _outOfTableRedPlayerTwo = new List<Cube>(outOfTableList);
                    break;
            }
        }
        /// <summary>
        /// Alakzat összes elemének eltárolása
        /// </summary>
        /// <param name="playerCoordinateX">Játékos X koordinátája</param>
        /// <param name="playerCoordinateY">Játékos Y koordinátája</param>
        /// <param name="playerNumber">Játékos azonosítója</param>
        /// <param name="direction">Játékoshoz kapcsolt kocka iránya</param>
        /// <param name="table">A hívó függvény számára megfelelő adatokat tartalmazó tábla</param>
        private void AddCubesToOldList(Int32 playerCoordinateX, Int32 playerCoordinateY, Int32 playerNumber, String direction, RobotokTable table)
        {
            _cubesOldPosition.Add(new Cube(playerCoordinateX, playerCoordinateY, playerNumber, table.GetAttachmentNorth(playerCoordinateX, playerCoordinateY), table.GetAttachmentSouth(playerCoordinateX, playerCoordinateY), table.GetAttachmentEast(playerCoordinateX, playerCoordinateY), table.GetAttachmentWest(playerCoordinateX, playerCoordinateY), direction, table.GetFieldRemainingCleaningOperations(playerCoordinateX, playerCoordinateY))); // játékos eltárolára a listában

            Boolean contains; // a lista tartalmazza-e az adott mezőt
            Int32 i = 0;
            while (i < _cubesOldPosition.Count)
            {
                if (table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "dél") // ha a korábbi mezőnek északon van kapcsolata, de korábban a dél irányt kapta meg, akkor a szülőjének délen volt kapcsolata, vagyis már benne van a listában (az && jobb oldali részének csak hatékonysági funkciója van)
                {
                    contains = false;
                    for (Int32 j = 0; j < _cubesOldPosition.Count; j++) // erre a ciklusra azért van szükség, mert ha már korábban bekerült egy kocka, de az kapcsolva van a jelenleg vizsgálthoz is, akkor ne rakjuk be mégegyszer
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x - 1 >= 0 && _cubesOldPosition[i].x - 1 < table.SizeX && _cubesOldPosition[i].y >= 0 && _cubesOldPosition[i].y < table.SizeY)
                        {
                            _cubesOldPosition.Add(new Cube(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, table.GetFieldValue(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentNorth(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y), "észak", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y)));
                        }
                    }
                }

                if (table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "észak")
                {
                    contains = false;
                    for (Int32 j = 0; j < _cubesOldPosition.Count; j++)
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x + 1 >= 0 && _cubesOldPosition[i].x + 1 < table.SizeX && _cubesOldPosition[i].y >= 0 && _cubesOldPosition[i].y < table.SizeY)
                        {
                            _cubesOldPosition.Add(new Cube(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, table.GetFieldValue(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentNorth(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y), "dél", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y)));
                        }
                    }
                }

                if (table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "nyugat")
                {
                    contains = false;
                    for (Int32 j = 0; j < _cubesOldPosition.Count; j++)
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x >= 0 && _cubesOldPosition[i].x < table.SizeX && _cubesOldPosition[i].y + 1 >= 0 && _cubesOldPosition[i].y + 1 < table.SizeY)
                        {
                            _cubesOldPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, table.GetFieldValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1), "kelet", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1)));
                        }
                    }
                }

                if (table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y) && _cubesOldPosition[i].direction != "kelet")
                {
                    contains = false;
                    for (Int32 j = 0; j < _cubesOldPosition.Count; j++)
                    {
                        if (i != j && _cubesOldPosition[j].x == _cubesOldPosition[i].x && _cubesOldPosition[j].y == _cubesOldPosition[i].y)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (_cubesOldPosition[i].x >= 0 && _cubesOldPosition[i].x < table.SizeX && _cubesOldPosition[i].y - 1 >= 0 && _cubesOldPosition[i].y - 1 < table.SizeY)
                        {
                            _cubesOldPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, table.GetFieldValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1), "nyugat", table.GetFieldRemainingCleaningOperations(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1)));
                        }
                    }

                }
                i++;
            }
        }

        /// <summary>
        /// Balra forgatás a játékpályán
        /// </summary>
        /// <param name="playerCoordinateX">Játékos X koordinátája</param>
        /// <param name="playerCoordinateY">Játékos Y koordinátája</param>
        private Boolean RotateLeft(Int32 playerCoordinateX, Int32 playerCoordinateY)
        {
            _cubesNewPosition.Add(new Cube(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].southAttachment, _cubesOldPosition[0].northAttachment, "", _cubesOldPosition[0].remainingCleaningOperations)); // felvesszük a játékost az új irányokkal

            Int32 i = 1;
            while (i < _cubesOldPosition.Count) // a játékoson kívül minden mezőnek eltároljuk az elforgatott változatát
            {
                Int32 newCoordinateX = _cubesOldPosition[0].x - (_cubesOldPosition[i].y - _cubesOldPosition[0].y);
                Int32 newCoordinateY = _cubesOldPosition[0].y + (_cubesOldPosition[i].x - _cubesOldPosition[0].x);

                if (newCoordinateX < 0 || newCoordinateY < 0 || newCoordinateX >= 17 || newCoordinateY >= 28) // ha kiindexelnénk a tábláról a forgatás után, akkor a művelet mindenképpen sikertelen
                {
                    return false;
                }

                _cubesNewPosition.Add(new Cube(newCoordinateX, newCoordinateY, _cubesOldPosition[i].value, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].southAttachment, _cubesOldPosition[i].northAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));

                i++;
            }

            i = 1;
            while (i < _cubesNewPosition.Count) // megnézzük az összes már elforgatott mezőre, hogy a forgatás után üres mezőre érkeznek-e
            {
                if (_table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != 7 && _table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != -2)
                {
                    Int32 j = 1;
                    Boolean found = false;
                    while (j < _cubesOldPosition.Count)
                    {
                        if (_cubesNewPosition[i].x == _cubesOldPosition[j].x && _cubesNewPosition[i].y == _cubesOldPosition[j].y)
                        {
                            found = true; // ha forgás után olyan pozícióra érkezik az új kocka, ami benne van a régi kockák pozíciólistájában, akkor attól még lehet helyes a művelet
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

            i = 0;
            while (i < _cubesNewPosition.Count) // ha idáig elértünk, akkor minden rendben volt a forgatás során, már csak el kell tárolni a táblán az új kapcsolatokkal együtt a kockákat
            {
                _table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false); // kitöröljük az összes régi mező kapcsolatát

                if (i > 0) // a játékos kockáját figyelmen kívül hagyjuk
                {
                    if (_cubesOldPosition[i].x < 3 || _cubesOldPosition[i].x > 13 || _cubesOldPosition[i].y < 4 || _cubesOldPosition[i].y > 23)
                    {
                        _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, -2, -1);  // a pályán kívüli mezőket láthatatlanra állítjuk
                    }
                    else
                    {
                        _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);  // kitöröljük az összes régi mezőt, ami nem a játékos
                    }
                }

                i++;
            }

            i = 0;
            while (i < _cubesNewPosition.Count) // az új pozíciók felrajzolása
            {
                if (i != 0)
                {
                    _table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations); // beállítjuk az új mező értékét   
                }

                _table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);

                i++;
            }

            return true;
        }

        /// <summary>
        /// Jobbra forgatás a játékpályán
        /// </summary>
        /// <param name="playerCoordinateX">Játékos X koordinátája</param>
        /// <param name="playerCoordinateY">Játékos Y koordinátája</param>
        private Boolean RotateRight(Int32 playerCoordinateX, Int32 playerCoordinateY)
        {
            _cubesNewPosition.Add(new Cube(playerCoordinateX, playerCoordinateY, _cubesOldPosition[0].value, _cubesOldPosition[0].westAttachment, _cubesOldPosition[0].eastAttachment, _cubesOldPosition[0].northAttachment, _cubesOldPosition[0].southAttachment, "", _cubesOldPosition[0].remainingCleaningOperations));

            Int32 i = 1;
            while (i < _cubesOldPosition.Count)
            {
                Int32 newCoordinateX = _cubesOldPosition[0].x - (_cubesOldPosition[0].y - _cubesOldPosition[i].y);
                Int32 newCoordinateY = _cubesOldPosition[0].y + (_cubesOldPosition[0].x - _cubesOldPosition[i].x);

                if (newCoordinateX < 0 || newCoordinateY < 0 || newCoordinateX >= 17 || newCoordinateY >= 28)
                {
                    return false;
                }

                _cubesNewPosition.Add(new Cube(newCoordinateX, newCoordinateY, _cubesOldPosition[i].value, _cubesOldPosition[i].westAttachment, _cubesOldPosition[i].eastAttachment, _cubesOldPosition[i].northAttachment, _cubesOldPosition[i].southAttachment, "", _cubesOldPosition[i].remainingCleaningOperations));

                i++;
            }

            i = 1;

            while (i < _cubesNewPosition.Count)
            { 
                if (_table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != 7 && _table.GetFieldValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y) != -2)
                {
                    Int32 j = 1;
                    Boolean found = false;
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

            i = 0;
            while (i < _cubesNewPosition.Count)
            {
                _table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false);


                if (i != 0)
                {
                    if (_cubesOldPosition[i].x < 3 || _cubesOldPosition[i].x > 13 || _cubesOldPosition[i].y < 4 || _cubesOldPosition[i].y > 23)
                    {
                        _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, -2, -1);
                    }
                    else
                    {
                        _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);
                    }
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
        /// Játékos alakzattal való lépésének végrehajtása
        /// </summary>
        /// <param name="direction">Játékos X koordinátája</param>
        /// <param name="table">Játékos Y koordinátája</param>
        /// /// <param name="outOfTableList">Játékos X koordinátája</param>
        /// <param name="playerNumber">Játékos Y koordinátája</param>
        private void ExecutePlayerSteps(String direction, RobotokTable table, List<Cube> outOfTableList, Int32 playerNumber)
        {
            List<Cube> edgeCaseCubes = new List<Cube>(); //létrehozunk egy segédlistát

            for (Int32 i = 0; i < outOfTableList.Count; i++)
            {
                if (direction == "észak")
                {
                    if (outOfTableList[i].x - 1 >= 0 && outOfTableList[i].x - 1 <= 10 && outOfTableList[i].y >= 0 && outOfTableList[i].y <= 19)
                    {
                        _cubesNewPosition.Add(new Cube(outOfTableList[i].x - 1, outOfTableList[i].y, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }
                    else
                    {
                        edgeCaseCubes.Add(new Cube(outOfTableList[i].x - 1, outOfTableList[i].y, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }
                }
                else if (direction == "dél")
                {
                    if (outOfTableList[i].x + 1 >= 0 && outOfTableList[i].x + 1 <= 10 && outOfTableList[i].y >= 0 && outOfTableList[i].y <= 19)
                    {
                        _cubesNewPosition.Add(new Cube(outOfTableList[i].x + 1, outOfTableList[i].y, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }
                    else
                    {
                        edgeCaseCubes.Add(new Cube(outOfTableList[i].x + 1, outOfTableList[i].y, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }
                }
                else if (direction == "kelet")
                {
                    if (outOfTableList[i].x >= 0 && outOfTableList[i].x <= 10 && outOfTableList[i].y + 1 >= 0 && outOfTableList[i].y + 1 <= 19)
                    {
                        _cubesNewPosition.Add(new Cube(outOfTableList[i].x, outOfTableList[i].y + 1, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }
                    else
                    {
                        edgeCaseCubes.Add(new Cube(outOfTableList[i].x, outOfTableList[i].y + 1, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }

                }
                else if (direction == "nyugat")
                {
                    if (outOfTableList[i].x >= 0 && outOfTableList[i].x <= 10 && outOfTableList[i].y - 1 >= 0 && outOfTableList[i].y - 1 <= 19)
                    {
                        _cubesNewPosition.Add(new Cube(outOfTableList[i].x, outOfTableList[i].y - 1, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }
                    else
                    {
                        edgeCaseCubes.Add(new Cube(outOfTableList[i].x, outOfTableList[i].y - 1, outOfTableList[i].value, outOfTableList[i].northAttachment, outOfTableList[i].southAttachment, outOfTableList[i].eastAttachment, outOfTableList[i].westAttachment, outOfTableList[i].direction, outOfTableList[i].remainingCleaningOperations));
                    }

                }
                i++;
            }
            outOfTableList.Clear();
            outOfTableList = new List<Cube>(edgeCaseCubes);

            for (Int32 i = 0; i < _cubesOldPosition.Count; i++)
            {
                if (direction == "észak")
                {
                    if (_cubesOldPosition[i].x - 1 >= 0 && _cubesOldPosition[i].x - 1 <= 10 && _cubesOldPosition[i].y >= 0 && _cubesOldPosition[i].y <= 19)
                    {
                        if (_cubesOldPosition[i].value == 10)
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, _table.GetFieldValue(_cubesOldPosition[i].x - 1 + 3, _cubesOldPosition[i].y + 4), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }
                        else
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }

                    }
                    else
                    {
                        outOfTableList.Add(new Cube(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }
                }
                else if (direction == "dél")
                {
                    if (_cubesOldPosition[i].x + 1 >= 0 && _cubesOldPosition[i].x + 1 <= 10 && _cubesOldPosition[i].y >= 0 && _cubesOldPosition[i].y <= 19)
                    {
                        if (_cubesOldPosition[i].value == 10)
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, _table.GetFieldValue(_cubesOldPosition[i].x + 1 + 3, _cubesOldPosition[i].y + 4), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }
                        else
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }

                    }
                    else
                    {
                        outOfTableList.Add(new Cube(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }
                }
                else if (direction == "kelet")
                {
                    if (_cubesOldPosition[i].x >= 0 && _cubesOldPosition[i].x <= 10 && _cubesOldPosition[i].y + 1 >= 0 && _cubesOldPosition[i].y + 1 <= 19)
                    {
                        if (_cubesOldPosition[i].value == 10)
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, _table.GetFieldValue(_cubesOldPosition[i].x + 3, _cubesOldPosition[i].y + 1 + 4), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }
                        else
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }
                    }
                    else
                    {
                        outOfTableList.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }

                }
                else if (direction == "nyugat")
                {
                    if (_cubesOldPosition[i].x >= 0 && _cubesOldPosition[i].x <= 10 && _cubesOldPosition[i].y - 1 >= 0 && _cubesOldPosition[i].y - 1 <= 19)
                    {
                        if (_cubesOldPosition[i].value == 10)
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, _table.GetFieldValue(_cubesOldPosition[i].x + 3, _cubesOldPosition[i].y - 1 + 4), table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }
                        else
                        {
                            _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                        }
                    }
                    else
                    {
                        outOfTableList.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, _cubesOldPosition[i].value, table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                    }

                }
            }

            switch (playerNumber)
            {
                case 1:
                    _outOfTableGreenPlayerOne = new List<Cube>(outOfTableList);
                    break;
                case 8:
                    _outOfTableGreenPlayerTwo = new List<Cube>(outOfTableList);
                    break;
                case 2:
                    _outOfTableRedPlayerOne = new List<Cube>(outOfTableList);
                    break;
                case 9:
                    _outOfTableRedPlayerTwo = new List<Cube>(outOfTableList);
                    break;
            }

            // Először letöröljük a tábláról a régi pozíciós kockákat

            for (Int32 i = 0; i < _cubesOldPosition.Count; i++)
            {
                table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);
                table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false);
            }


            // A kockákat újrarajzoljuk a táblán az új pozíciókat tartalmazó lista szerint

            for (Int32 i = 0; i < _cubesNewPosition.Count; i++)
            {
                table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);
                table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);
            }
        }

        /// <summary>
        /// Ellenőrzött lépések végrehajtása a játékterületen
        /// </summary>
        /// <param name="direction">Lépések iránya</param>
        private void ExecuteSafeSteps(String direction)
        {
            for (Int32 i = 0; i < _cubesOldPosition.Count; i++) // bekapoljuk a mezők új koordinátáit egy külön listába
            {
                if (direction == "észak")
                {
                    _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x - 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (direction == "dél")
                {
                    _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x + 1, _cubesOldPosition[i].y, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (direction == "kelet")
                {
                    _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y + 1, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
                else if (direction == "nyugat")
                {
                    _cubesNewPosition.Add(new Cube(_cubesOldPosition[i].x, _cubesOldPosition[i].y - 1, _cubesOldPosition[i].value, _table.GetAttachmentNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetAttachmentWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y), direction, _cubesOldPosition[i].remainingCleaningOperations));
                }
            }

            for (Int32 i = 0; i < _cubesOldPosition.Count; i++) // először letöröljük a tábláról a régi pozíciós kockákat
            {
                if (_cubesOldPosition[i].x < 3 || _cubesOldPosition[i].x > 13 || _cubesOldPosition[i].y < 4 || _cubesOldPosition[i].y > 23) // az az eset, amikor játékon kívüli kockát törlünk (-2 értékre kell visszaállítani)
                {
                    _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, -2, -1);
                }
                else // általános eset 7-re (üres kocka) visszaállítva
                {
                    _table.SetValue(_cubesOldPosition[i].x, _cubesOldPosition[i].y, 7, -1);
                }

                _table.SetAttachmentValues(_cubesOldPosition[i].x, _cubesOldPosition[i].y, false, false, false, false);
                _table.SetFaceDirection(_cubesOldPosition[i].x, _cubesOldPosition[i].x, false, false, false, false);
            }

            for (Int32 i = 0; i < _cubesNewPosition.Count; i++) // a kockákat újrarajzoljuk a táblán az új pozíciókat tartalmazó lista szerint
            {
                _table.SetValue(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].value, _cubesNewPosition[i].remainingCleaningOperations);
                _table.SetFaceDirection(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _table.GetFaceNorth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetFaceSouth(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetFaceEast(_cubesOldPosition[i].x, _cubesOldPosition[i].y), _table.GetFaceWest(_cubesOldPosition[i].x, _cubesOldPosition[i].y));
                _table.SetAttachmentValues(_cubesNewPosition[i].x, _cubesNewPosition[i].y, _cubesNewPosition[i].northAttachment, _cubesNewPosition[i].southAttachment, _cubesNewPosition[i].eastAttachment, _cubesNewPosition[i].westAttachment);
            }
        }

        /// <summary>
        /// Pályáról kivitt alakzat kiértékelése
        /// </summary>
        /// <param name="direction">Alakzatkivitel iránya</param>
        /// <returns>A művelet sikeressége (0 - helytelen alakzat, 1 - 1. alakzat teljesült, 2 - 2. alakzat teljesült)</returns>
        private Int32 EvaluateShape(String direction)
        {
            Int32 result = 0; // a kiértékelés alatt külön változóban tároljuk el a visszatérési értéket

            Int32 figureToEvaluateColor = 0; // eltároljuk a kivinni kívánt alakzat színét (minden lehetséges alakzat egyedi színű, biztosan helyes értéket kap)

            if (direction == "észak") // Iránytól függően a játékon kívüli területről bepakoljuk a kockákat a kiértékelésre szolgáló listába
            {
                for (Int32 i = 0; i < 3; i++)
                {
                    for (Int32 j = 0; j < _table.SizeY; j++)
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
                for (Int32 i = 14; i < 17; i++)
                {
                    for (Int32 j = 0; j < _table.SizeY; j++)
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
                for (Int32 i = 0; i < _table.SizeX; i++)
                {
                    for (Int32 j = 0; j < 4; j++)
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
                for (Int32 i = 0; i < _table.SizeX; i++)
                {
                    for (Int32 j = 24; j < 28; j++)
                    {
                        if (_table.GetFieldValue(i, j) != -2 && _table.GetFieldValue(i, j) != 7)
                        {
                            figureToEvaluateColor = _table.GetFieldValue(i, j);
                            _cubesToEvaluate.Add(new CubeToEvaluate(i, j, _table.GetFieldValue(i, j)));
                        }
                    }
                }
            }

            Boolean foundFigureNumber = false;
            Int32 figureNumber = 0; // megkeressük, melyik hirdetőtábláról származik a kivinni kívánt alakzat

            Int32 k = 0;
            while (k < 3 && !foundFigureNumber)
            {
                Int32 l = 0;
                while (l < 3 && !foundFigureNumber)
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

            Int32[,] _figureMirrorX = new Int32[3, 3]; // a felismert hirdetőtábla alakzatából elkészítjük az elforgatott változatokat is
            Int32[,] _figureMirrorY = new Int32[3, 3];
            Int32[,] _figureMirrorXY = new Int32[3, 3];


            // X tengely szerinti tükrözés
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    if (figureNumber == 1)
                    {
                        _figureMirrorX[i, j] = _figure1.GetFieldValue(i, 2 - j);
                    }
                    else if (figureNumber == 2)
                    {
                        _figureMirrorX[i, j] = _figure2.GetFieldValue(i, 2 - j);
                    }
                }
            }

            // Y tengely szerinti tükrözés
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    if (figureNumber == 1)
                    {
                        _figureMirrorY[i, j] = _figure1.GetFieldValue(2 - i, j);
                    }
                    else if (figureNumber == 2)
                    {
                        _figureMirrorY[i, j] = _figure2.GetFieldValue(2 - i, j);
                    }
                }
            }

            // X és Y tengely szerinti tükrözés
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
                {
                    if (figureNumber == 1)
                    {
                        _figureMirrorXY[i, j] = _figure1.GetFieldValue(2 - i, 2 - j);
                    }
                    else if (figureNumber == 2)
                    {
                        _figureMirrorXY[i, j] = _figure2.GetFieldValue(2 - i, 2 - j);
                    }
                }
            }

            // A játékos kivitt kockáinak színe alapján kiértékelendő hirdetőtáblán szereplő nem -2 (játékterületen kívüli) kockákat bepakoljuk egy listába
            for (Int32 i = 0; i < 3; i++)
            {
                for (Int32 j = 0; j < 3; j++)
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

            Boolean foundFigure = true; // alapértelmezetten azt állítjuk, hogy megtaláltuk az alakzatot

            if (_cubesToEvaluate.Count != _figureToEvaluate.Count) // ha nem egyezik meg a kivitt alakzat elemszáma egyik hiretőttáblán szereplő alakzat elemszámával sem, akkor nem kell tovább ellenőriznünk
            {
                foundFigure = false;
            }

            // Megnézzük, hogy a játékon kívüli területről származó alakzat olyan alakú-e, mint ami a hirdetőtáblán van (Itt fontos a sorrend, hogy ugyanolyan sorrendben kerültek be a játéktábláról a kockák, mint a hirdetőtábláról. Mivel ez a tulajonság teljesül, elég megnéznünk, hogy a listában szereplő kockák közötti x,y relatív távolság megegyezik-e)
            if (_cubesToEvaluate.Count > 1 && foundFigure == true)
            {
                Int32 cubeToEvaluateFirstX = _cubesToEvaluate[0].x;
                Int32 cubeToEvaluateFirstY = _cubesToEvaluate[0].y;
                _cubesToEvaluate.RemoveAt(0);

                Int32 cubeToEvaluateSecondX = _cubesToEvaluate[0].x;
                Int32 cubeToEvaluateSecondY = _cubesToEvaluate[0].y;
                _cubesToEvaluate.RemoveAt(0);

                Int32 figureToEvaluateFirstX = _figureToEvaluate[0].x;
                Int32 figureToEvaluateFirstY = _figureToEvaluate[0].y;
                _figureToEvaluate.RemoveAt(0);

                Int32 figureToEvaluateSecondX = _figureToEvaluate[0].x;
                Int32 figureToEvaluateSecondY = _figureToEvaluate[0].y;
                _figureToEvaluate.RemoveAt(0);

                Int32 figureToEvaluateFirstXMirrorX = _figureToEvaluateMirrorX[0].x;
                Int32 figureToEvaluateFirstYMirrorX = _figureToEvaluateMirrorX[0].y;
                _figureToEvaluateMirrorX.RemoveAt(0);

                Int32 figureToEvaluateSecondXMirrorX = _figureToEvaluateMirrorX[0].x;
                Int32 figureToEvaluateSecondYMirrorX = _figureToEvaluateMirrorX[0].y;
                _figureToEvaluateMirrorX.RemoveAt(0);

                Int32 figureToEvaluateFirstXMirrorY = _figureToEvaluateMirrorY[0].x;
                Int32 figureToEvaluateFirstYMirrorY = _figureToEvaluateMirrorY[0].y;
                _figureToEvaluateMirrorY.RemoveAt(0);

                Int32 figureToEvaluateSecondXMirrorY = _figureToEvaluateMirrorY[0].x;
                Int32 figureToEvaluateSecondYMirrorY = _figureToEvaluateMirrorY[0].y;
                _figureToEvaluateMirrorY.RemoveAt(0);

                Int32 figureToEvaluateFirstXMirrorXY = _figureToEvaluateMirrorXY[0].x;
                Int32 figureToEvaluateFirstYMirrorXY = _figureToEvaluateMirrorXY[0].y;
                _figureToEvaluateMirrorXY.RemoveAt(0);

                Int32 figureToEvaluateSecondXMirrorXY = _figureToEvaluateMirrorXY[0].x;
                Int32 figureToEvaluateSecondYMirrorXY = _figureToEvaluateMirrorXY[0].y;
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

                if (foundFigure) // letöröljük az alakzatot a játéktábláról
                {
                    if (direction == "észak")
                    {
                        for (Int32 i = 0; i < 3; i++)
                        {
                            for (Int32 j = 0; j < _table.SizeY; j++)
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
                        for (Int32 i = 14; i < 17; i++)
                        {
                            for (Int32 j = 0; j < _table.SizeY; j++)
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
                        for (Int32 i = 0; i < _table.SizeX; i++)
                        {
                            for (Int32 j = 0; j < 4; j++)
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
                        for (Int32 i = 0; i < _table.SizeX; i++)
                        {
                            for (Int32 j = 24; j < 28; j++)
                            {
                                _table.SetValue(i, j, -2, -1);
                                _table.SetAttachmentNorth(i, j, false);
                                _table.SetAttachmentSouth(i, j, false);
                                _table.SetAttachmentEast(i, j, false);
                                _table.SetAttachmentWest(i, j, false);
                            }
                        }
                    }
                    result = figureNumber; // ha kiértékelődött egy alakzat, és le lettek törölve az építőkockái, akkor a visszatérési érték az aktív alakzat száma lesz
                }
            }

            else if (_cubesToEvaluate.Count == 1) // speciális eset, ha az alakzat csak egy kockából áll (igazából ilyen alakzatunk jelenleg nincs, a kódban csak a teljesség igénye miatt szerepel)
            {
                if (_figureToEvaluate.Count == _cubesToEvaluate.Count && _cubesToEvaluate[0].value == _figureToEvaluate[0].value)
                {
                    result = figureNumber;
                }
            }

            _cubesToEvaluate.Clear(); // alakzat kiértékelésének eredményétől függetlenül töröljük a lista tartalmát
            _figureToEvaluate.Clear();
            _figureToEvaluateMirrorX.Clear();
            _figureToEvaluateMirrorY.Clear();
            _figureToEvaluateMirrorXY.Clear();
            return result;
        }

        /// <summary>
        /// Visszaadja a játékos koordinátáit a táblán
        /// </summary>
        /// <param name="playerNumber">Játékos egyedi azonosítója</param>
        /// <param name="table">Tábla, amelyen szeretnénk megkeresni a játékos azonosítóját</param>
        private (Int32, Int32) getActivePlayerCoordinates(Int32 playerNumber, RobotokTable table)
        {
            Int32 playerCoordinateX = 0, playerCoordinateY = 0;

            for (Int32 i = 0; i < table.SizeX; i++)
            {
                for (Int32 j = 0; j < table.SizeY; j++)
                {
                    if (table.GetFieldValue(i, j) == playerNumber)
                    {
                        playerCoordinateX = i;
                        playerCoordinateY = j;
                    }
                }
            }

            return (playerCoordinateX, playerCoordinateY);
        }

        /// <summary>
        /// Paraméterül kapott csapat kockaösszekapcsolási állapotának aktualizálása
        /// </summary>
        /// <param name="playerNumber">Játékos egyedi azonosítója</param>
        private void updateTeamCubeAttachStates (int playerNumber)
        {
            if (playerNumber == 1 || playerNumber == 8)
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
        }

        /// <summary>
        /// Pályáról kivitt alakzat eredményének kiértékelése
        /// </summary>
        /// <param name="result">Teljesített alakzat sorszáma</param>
        /// <param name="playerNumber">Játékos egyedi azonosítója</param>
        private void EvaluateResult(Int32 result, Int32 playerNumber)
        {
            if (result == 1)
            {
                if (playerNumber == 1 || playerNumber == 8)
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
                if (playerNumber == 1 || playerNumber == 8)
                {
                    _greenTeamPoints += _secondTaskPoints;
                }
                else
                {
                    _redTeamPoints += _secondTaskPoints;
                }

                RegenerateShape(result);
            }
        }

        /// <summary>
        /// Első alakzat kigenerálása, elemeinek szétszórása a pályán, hirdetőtáblán való megjelenítése
        /// </summary>
        private void FirstTaskInitialization()
        {
            Random random = new Random();

            for (Int32 x = 0; x < _figure1.Figure.GetLength(0); ++x)
            {
                for (Int32 y = 0; y < _figure1.Figure.GetLength(1); ++y)
                {
                    if (_figure1.GetFieldValue(x, y) != -2) // amennyiben az ábrán alakzatelemhez érünk, akkor az annak megfeleltetett építőkockát el kell helyeznünk a pályán
                    {
                        Int32 figureCoordinateX = random.Next(4, 13);
                        Int32 figureCoordinateY = random.Next(5, 23);

                        while (_table.GetFieldValue(figureCoordinateX, figureCoordinateX) != 7)
                        {
                            figureCoordinateX = random.Next(4, 13);
                            figureCoordinateY = random.Next(5, 23);
                        }

                        _table.SetValue(figureCoordinateX, figureCoordinateY, _figure1.GetColor(), _cleaningOperations);
                    }

                }
            }

            for (Int32 i = 0; i < _figure1.Figure.GetLength(0); ++i) // elhelyezzük a hirdetőtáblán is az új alakzatot
            {
                for (Int32 j = 0; j < _figure1.Figure.GetLength(1); ++j)
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

        /// <summary>
        /// Második alakzat kigenerálása, elemeinek szétszórása a pályán, hirdetőtáblán való megjelenítése
        /// </summary>
        private void SecondTaskInitialization()
        {
            Random random = new Random();

            for (Int32 x = 0; x < _figure2.Figure.GetLength(0); ++x)
            {
                for (Int32 y = 0; y < _figure2.Figure.GetLength(1); ++y)
                {
                    if (_figure2.GetFieldValue(x, y) != -2)
                    {
                        Int32 figureCoordinateX = random.Next(4, 13);
                        Int32 figureCoordinateY = random.Next(5, 23);
                        while (_table.GetFieldValue(figureCoordinateX, figureCoordinateY) != 7)
                        {
                            figureCoordinateX = random.Next(4, 13);
                            figureCoordinateY = random.Next(5, 23);
                        }

                        _table.SetValue(figureCoordinateX, figureCoordinateY, _figure2.GetColor(), _cleaningOperations);
                    }

                }
            }

            for (Int32 i = 0; i < _figure2.Figure.GetLength(0); ++i)
            {
                for (Int32 j = 0; j < _figure2.Figure.GetLength(1); ++j)
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

        #endregion

    }
}
