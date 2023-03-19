using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;

namespace ELTE.Robotok.View
{
    public partial class GameForm : Form
    {
        #region Fields

        private IRobotokDataAccess _dataAccess = null!; // adatelérés
        private RobotokGameModel _model = null!; // játékmodell
        private Button[,] _buttonGrid = null!; // gombrács (játékvezetői)
        private Button[,] _buttonGridPlayerOne = null!; // gombrács (1. játékos)
        private Button[,] _buttonGridPlayerTwo = null!; // gombrács (2. játékos)
        private Button[,] _buttonGridNoticeBoardOne = null!; // gombrács (hirdetőtábla 1)
        private Button[,] _buttonGridNoticeBoardTwo = null!; // gombrács (hirdetőtábla 2)
        private bool isRefereeModeVisible = false; // játékvezetői mód pályájának láthatósága

        #endregion

        #region Constructor

        /// <summary>
        /// Játékablak példányosítása.
        /// </summary>

        public GameForm()
        {
            InitializeComponent();

            //modell létrehozása
            _model = new RobotokGameModel(_dataAccess);

            // játéktáblák inicializálása
            GenerateTables();

        }

        #endregion

        #region Game event handlers

        #endregion

        #region Grid event handlers

        #endregion

        #region Menu event handlers

        #endregion

        #region Timer event handlers

        #endregion

        #region Private methods

        private void GenerateTables()
        {
            // Játékvezetői tábla
            _buttonGrid = new Button[_model.Table.SizeX, _model.Table.SizeY];
            for (Int32 i = 0; i < _model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < _model.Table.SizeY; j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Location = new Point(315 + 19 * j, 55 + 19 * i); // elhelyezkedés
                    _buttonGrid[i, j].Size = new Size(19, 19); // méret
                    _buttonGrid[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGrid[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGrid[i, j].Visible = false; // megjelenítés alapértelmezett kikapcsolása
                    //_buttonGrid[i, j].TabIndex = 100 + i * _model.Table.Size + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGrid[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    //_buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezelő hozzárendelése minden gombhoz
                    // ezek a sorok ki vannak kommentezve, azért hagytam benne, mert nemsokára implementálni kell, és így nem kell keresni

                    Controls.Add(_buttonGrid[i, j]);
                    // felvesszük az ablakra a gombot
                }
            }

            // 1. játékos táblája
            _buttonGridPlayerOne = new Button[_model.TablePlayerOne.SizeX, _model.TablePlayerOne.SizeY];
            for (Int32 i = 0; i < _model.TablePlayerOne.SizeX; i++)
            {
                for (Int32 j = 0; j < _model.TablePlayerOne.SizeY; j++)
                {
                    _buttonGridPlayerOne[i, j] = new Button();
                    _buttonGridPlayerOne[i, j].Location = new Point(15 + 25 * j, 115 + 25 * i); // elhelyezkedés
                    _buttonGridPlayerOne[i, j].Size = new Size(25, 25); // méret
                    _buttonGridPlayerOne[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridPlayerOne[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridPlayerOne[i, j].Visible = true;
                    //_buttonGrid[i, j].TabIndex = 100 + i * _model.Table.Size + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGridPlayerOne[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    //_buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezelő hozzárendelése minden gombhoz

                    Controls.Add(_buttonGridPlayerOne[i, j]);
                    //SetDoubleBuffer(_buttonGridPlayerOne[i, j], true);
                    // felvesszük az ablakra a gombot
                }
            }

            // 2. játékos táblája
            _buttonGridPlayerTwo = new Button[_model.TablePlayerTwo.SizeX, _model.TablePlayerTwo.SizeY];
            for (Int32 i = 0; i < _model.TablePlayerTwo.SizeX; i++)
            {
                for (Int32 j = 0; j < _model.TablePlayerTwo.SizeY; j++)
                {
                    _buttonGridPlayerTwo[i, j] = new Button();
                    _buttonGridPlayerTwo[i, j].Location = new Point(600 + 25 * j, 115 + 25 * i); // elhelyezkedés
                    _buttonGridPlayerTwo[i, j].Size = new Size(25, 25); // méret
                    _buttonGridPlayerTwo[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridPlayerTwo[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridPlayerTwo[i, j].Visible = true;
                    //_buttonGrid[i, j].TabIndex = 100 + i * _model.Table.Size + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGridPlayerTwo[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    //_buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezelő hozzárendelése minden gombhoz

                    Controls.Add(_buttonGridPlayerTwo[i, j]);
                    //SetDoubleBuffer(_buttonGridPlayerTwo[i, j], true);
                    // felvesszük az ablakra a gombot
                }
            }
            
            // Hirdetőtábla 1
            _buttonGridNoticeBoardOne = new Button[_model.TableNoticeBoardOne.SizeX, _model.TableNoticeBoardOne.SizeY];
            for (Int32 i = 0; i < _model.TableNoticeBoardOne.SizeX; i++)
            {
                for (Int32 j = 0; j < _model.TableNoticeBoardOne.SizeY; j++)
                {
                    _buttonGridNoticeBoardOne[i, j] = new Button();
                    _buttonGridNoticeBoardOne[i, j].Location = new Point(215 + 25 * j, 450 + 25 * i); // elhelyezkedés
                    _buttonGridNoticeBoardOne[i, j].Size = new Size(25, 25); // méret
                    _buttonGridNoticeBoardOne[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridNoticeBoardOne[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridNoticeBoardOne[i, j].Visible = true;
                    //_buttonGrid[i, j].TabIndex = 100 + i * _model.Table.Size + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGridNoticeBoardOne[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    //_buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezelő hozzárendelése minden gombhoz

                    Controls.Add(_buttonGridNoticeBoardOne[i, j]);
                    //SetDoubleBuffer(_buttonGridNoticeBoardOne[i, j], true);
                    // felvesszük az ablakra a gombot
                }
            }

            // Hirdetőtábla 2
            _buttonGridNoticeBoardTwo = new Button[_model.TableNoticeBoardTwo.SizeX, _model.TableNoticeBoardTwo.SizeY];
            for (Int32 i = 0; i < _model.TableNoticeBoardTwo.SizeX; i++)
            {
                for (Int32 j = 0; j < _model.TableNoticeBoardTwo.SizeY; j++)
                {
                    _buttonGridNoticeBoardTwo[i, j] = new Button();
                    _buttonGridNoticeBoardTwo[i, j].Location = new Point(365 + 25 * j, 450 + 25 * i); // elhelyezkedés
                    _buttonGridNoticeBoardTwo[i, j].Size = new Size(25, 25); // méret
                    _buttonGridNoticeBoardTwo[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridNoticeBoardTwo[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridNoticeBoardTwo[i, j].Visible = true;
                    //_buttonGrid[i, j].TabIndex = 100 + i * _model.Table.Size + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGridNoticeBoardTwo[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    //_buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezelő hozzárendelése minden gombhoz

                    Controls.Add(_buttonGridNoticeBoardTwo[i, j]);
                    //SetDoubleBuffer(_buttonGridNoticeBoardTwo[i, j], true);
                    // felvesszük az ablakra a gombot
                }
            }
        }

        /*
        Eltűnteti vagy megjeleníti a játékvezetői mód mátrixát attól függően, hogy mi az 
        isReferreModeVisible változó értéke, majd pedig meghívja a repositioning függvényt,
        ami a Formban található többi elem pozíciójának megfelelő igazításáért felel
        */
        private void refereeModeButton_Click(object sender, EventArgs e)
        {
            if (isRefereeModeVisible)
            {
                refereeModeButton.Text = "Játékvezető mód: bekapcsolás";
                for (Int32 i = 0; i < _model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < _model.Table.SizeY; j++)
                    {
                        _buttonGrid[i, j].Visible = false;
                    }
                }
                Repositioning();
                isRefereeModeVisible = false;
            }
            else
            {
                refereeModeButton.Text = "Játékvezető mód: kikapcsolás";
                for (Int32 i = 0; i < _model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < _model.Table.SizeY; j++)
                    {
                        _buttonGrid[i, j].Visible = true;
                    }
                }
                Repositioning();
                isRefereeModeVisible = true;
            }
        }

        private void Repositioning()
        {
            if (isRefereeModeVisible)
            {
                // Különböző vezérlőelemek áthelyezése

                this.Size = new Size(this.Width, this.Height - 280);
                playerOneViewText.Location = new Point(playerOneViewText.Location.X, playerOneViewText.Location.Y - 325);
                playerTwoViewText.Location = new Point(playerTwoViewText.Location.X, playerTwoViewText.Location.Y - 325);
                noticeBoardText.Location = new Point(noticeBoardText.Location.X, noticeBoardText.Location.Y - 290);
                availableOperationsText.Location = new Point(availableOperationsText.Location.X, availableOperationsText.Location.Y - 275);
                operationParametersText.Location = new Point(operationParametersText.Location.X, operationParametersText.Location.Y - 265);
                waitButton.Location = new Point(waitButton.Location.X, waitButton.Location.Y - 275);
                moveButton.Location = new Point(moveButton.Location.X, moveButton.Location.Y - 275);
                turnButton.Location = new Point(turnButton.Location.X, turnButton.Location.Y - 275);
                attachButton.Location = new Point(attachButton.Location.X, attachButton.Location.Y - 275);
                detachButton.Location = new Point(detachButton.Location.X, detachButton.Location.Y - 275);
                attachCubesButton.Location = new Point(attachCubesButton.Location.X, attachCubesButton.Location.Y - 275);
                detachCubesButton.Location = new Point(detachCubesButton.Location.X, detachCubesButton.Location.Y - 275);
                clearButton.Location = new Point(clearButton.Location.X, clearButton.Location.Y - 275);
                operationParameter.Location = new Point(operationParameter.Location.X, operationParameter.Location.Y - 265);
                coordinate1.Location = new Point(coordinate1.Location.X, coordinate1.Location.Y - 265);
                coordinate2.Location = new Point(coordinate2.Location.X, coordinate2.Location.Y - 265);
                communicationWindowText.Location = new Point(communicationWindowText.Location.X - 155, communicationWindowText.Location.Y - 120);
                communicationWindow.Location = new Point(communicationWindow.Location.X - 230, communicationWindow.Location.Y - 120);
                communicationWindow.Size = new Size(552, 210);

                // Gombrácsok áthelyezése összevont egymásba ágyazott ciklussal

                for (Int32 i = 0; i < _model.TablePlayerOne.SizeX; i++)
                {
                    for (Int32 j = 0; j < _model.TablePlayerOne.SizeY; j++)
                    {
                        _buttonGridPlayerOne[i, j].Location = new Point(15 + 25 * j, 115 + 25 * i);
                        _buttonGridPlayerTwo[i, j].Location = new Point(600 + 25 * j, 115 + 25 * i);

                        // A kisebb hirdetőtábláknál feltételt alkalmazunk

                        if (i < _model.TableNoticeBoardOne.SizeX && j < _model.TableNoticeBoardOne.SizeY)
                        {
                            _buttonGridNoticeBoardOne[i, j].Location = new Point(215 + 25 * j, 450 + 25 * i);
                            _buttonGridNoticeBoardTwo[i, j].Location = new Point(365 + 25 * j, 450 + 25 * i);
                        }
                    }
                }
            }

            else
            {
                // Különböző vezérlőelemek áthelyezése

                this.Size = new Size(this.Width, this.Height + 280);
                playerOneViewText.Location = new Point(playerOneViewText.Location.X, playerOneViewText.Location.Y + 325);
                playerTwoViewText.Location = new Point(playerTwoViewText.Location.X, playerTwoViewText.Location.Y + 325);
                noticeBoardText.Location = new Point(noticeBoardText.Location.X, noticeBoardText.Location.Y + 290);
                availableOperationsText.Location = new Point(availableOperationsText.Location.X, availableOperationsText.Location.Y + 275);
                operationParametersText.Location = new Point(operationParametersText.Location.X, operationParametersText.Location.Y + 265);
                waitButton.Location = new Point(waitButton.Location.X, waitButton.Location.Y + 275);
                moveButton.Location = new Point(moveButton.Location.X, moveButton.Location.Y + 275);
                turnButton.Location = new Point(turnButton.Location.X, turnButton.Location.Y + 275);
                attachButton.Location = new Point(attachButton.Location.X, attachButton.Location.Y + 275);
                detachButton.Location = new Point(detachButton.Location.X, detachButton.Location.Y + 275);
                attachCubesButton.Location = new Point(attachCubesButton.Location.X, attachCubesButton.Location.Y + 275);
                detachCubesButton.Location = new Point(detachCubesButton.Location.X, detachCubesButton.Location.Y + 275);
                clearButton.Location = new Point(clearButton.Location.X, clearButton.Location.Y + 275);
                operationParameter.Location = new Point(operationParameter.Location.X, operationParameter.Location.Y + 265);
                coordinate1.Location = new Point(coordinate1.Location.X, coordinate1.Location.Y + 265);
                coordinate2.Location = new Point(coordinate2.Location.X, coordinate2.Location.Y + 265);
                communicationWindowText.Location = new Point(communicationWindowText.Location.X + 155, communicationWindowText.Location.Y + 120);
                communicationWindow.Location = new Point(communicationWindow.Location.X + 230, communicationWindow.Location.Y + 120);
                communicationWindow.Size = new Size(360, 210);

                // Gombrácsok áthelyezése összevont egymásba ágyazott ciklussal

                for (Int32 i = 0; i < _model.TablePlayerOne.SizeX; i++)
                {
                    for (Int32 j = 0; j < _model.TablePlayerOne.SizeY; j++)
                    {
                        _buttonGridPlayerOne[i, j].Location = new Point(15 + 25 * j, 320 + 115 + 25 * i);
                        _buttonGridPlayerTwo[i, j].Location = new Point(600 + 25 * j, 320 + 115 + 25 * i);

                        // A kisebb hirdetőtábláknál feltételt alkalmazunk

                        if (i < _model.TableNoticeBoardOne.SizeX && j < _model.TableNoticeBoardOne.SizeY)
                        {
                            _buttonGridNoticeBoardOne[i, j].Location = new Point(215 + 25 * j, 740 + 25 * i);
                            _buttonGridNoticeBoardTwo[i, j].Location = new Point(365 + 25 * j, 740 + 25 * i);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
