using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;
using Robotok.WinForms.Properties; 

namespace ELTE.Robotok.View
{
    public partial class GameForm : Form
    {
        #region Fields

        private Button[,] _buttonGridPlayer = null!; // gombrács a játékos nézetének megjelenítésére
        private Button[,] _buttonGridNoticeBoardOne = null!; // gombrács (hirdetőtábla 1)
        private Button[,] _buttonGridNoticeBoardTwo = null!; // gombrács (hirdetőtábla 2)
        private int _difficulty; // játék nehézsége
        private int _teams = 0; // csapatok száma
        private int _activePlayer; // játékos azonosítója
        private string? _successText; // a végrehajtott művelet sikeressége
        private bool _operationDone; // végzett-e valamilyen műveletet a játékos
        private int _activeCoordinateBox = 1; // 1 - első összekapcsolandó kocka koorinátáinak doboza 2 - második összekapcsolandó kocka koordinátáinak doboza
        #endregion

        #region Constructor

        /// <summary>
        /// Játékablak példányosítása.
        /// </summary>

        public GameForm(int difficulty, int teams, int activePlayer) // ez a három paraméter alapján majd meg fognak változni a nézetek
        {
            InitializeComponent();
            _difficulty = difficulty;
            _teams = teams;
            _activePlayer = activePlayer;
            _operationDone = false;
            // Játéktáblák inicializálása
            GenerateTables();

        }

        #endregion

        #region Game event handlers

        #endregion

        #region Grid event handlers

        private void ButtonGrid_Click(object sender, EventArgs e)
        {
            Button senderButton = sender as Button;
            if (_activeCoordinateBox == 1)
            {
                coordinate1.Text = senderButton.Name;
                _activeCoordinateBox = 2;
            }
            else
            {
                coordinate2.Text = senderButton.Name;
                _activeCoordinateBox = 1;
            }
        }

        #endregion

        #region Menu event handlers

        #endregion

        #region Private methods
        // Lejebb sokszor található lesz i - 3, illetve j - 4. Ez azért van, mert maga a játékpálya sokkal nagyobb mint a játékosak "játékpályai", ezért hogy ne legyen semmilyen túlindexelés vagy valami hasonló, ezt így oldottam meg
        // Létrehozza a gombokat, amiből a játékosnézet és a hirdektőtáblák felépülnek
        private void GenerateTables() 
        {
            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);
            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 8);
            if (_teams == 2)
            {
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 2);
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 9);
            }
            // Játékos táblája
            _buttonGridPlayer = new Button[GameMenuForm.instance._model.TableGreenPlayerOne.SizeX, GameMenuForm.instance._model.TableGreenPlayerOne.SizeY]; // Mindegy hogy melyik játékos tábláját használjuk, mindegyik ugyanaz a méretű
            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                    {
                        _buttonGridPlayer[i - 3, j - 4] = new Button();
                        _buttonGridPlayer[i - 3, j - 4].Location = new Point(60 + 25 * (j-4), 85 + 25 * (i-3)); // elhelyezkedés
                        _buttonGridPlayer[i - 3, j - 4].Size = new Size(25, 25); // méret
                        _buttonGridPlayer[i - 3, j - 4].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                        _buttonGridPlayer[i - 3, j - 4].Enabled = true;
                        _buttonGridPlayer[i - 3, j - 4].Visible = true;
                        _buttonGridPlayer[i - 3, j - 4].FlatStyle = FlatStyle.Flat; // lapított stípus
                        _buttonGridPlayer[i - 3, j - 4].BackgroundImageLayout = ImageLayout.Stretch; // Kép mezőhöz méretezése
                        _buttonGridPlayer[i - 3, j - 4].Name = i.ToString() + "," + j.ToString();
                        _buttonGridPlayer[i - 3, j - 4].Click += ButtonGrid_Click;

                        if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i-3, j-4) == -1) // minden mezőnek megadjuk a színét
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 0)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 3)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 4)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 5)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 6)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 8)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 10) // Manhattan távolságon belül nem látható mezők
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                        }
                        Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                        // felvesszük az ablakra a gombot
                        successfulText.Text = "Nincs művelet!";
                        nextRoundValueText.Text = "Zöld csapat 2. játékos";
                    }
                }
            }

            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);

            // Hirdetőtábla 1
            _buttonGridNoticeBoardOne = new Button[GameMenuForm.instance._model.TableNoticeBoardOne.SizeX, GameMenuForm.instance._model.TableNoticeBoardOne.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.TableNoticeBoardOne.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TableNoticeBoardOne.SizeY; j++)
                {
                    _buttonGridNoticeBoardOne[i, j] = new Button();
                    _buttonGridNoticeBoardOne[i, j].Location = new Point(215 + 25 * j, 400 + 25 * i); // elhelyezkedés
                    _buttonGridNoticeBoardOne[i, j].Size = new Size(25, 25); // méret
                    _buttonGridNoticeBoardOne[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridNoticeBoardOne[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridNoticeBoardOne[i, j].Visible = true;
                    _buttonGridNoticeBoardOne[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 3)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Yellow;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 4)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Orange;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 5)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Blue;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 6)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Violet;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 7)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.White;
                    }

                    Controls.Add(_buttonGridNoticeBoardOne[i, j]);
                    // felvesszük az ablakra a gombot
                }
            }

            // Hirdetőtábla 2
            _buttonGridNoticeBoardTwo = new Button[GameMenuForm.instance._model.TableNoticeBoardTwo.SizeX, GameMenuForm.instance._model.TableNoticeBoardTwo.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.TableNoticeBoardTwo.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TableNoticeBoardTwo.SizeY; j++)
                {
                    _buttonGridNoticeBoardTwo[i, j] = new Button();
                    _buttonGridNoticeBoardTwo[i, j].Location = new Point(365 + 25 * j, 400 + 25 * i); // elhelyezkedés
                    _buttonGridNoticeBoardTwo[i, j].Size = new Size(25, 25); // méret
                    _buttonGridNoticeBoardTwo[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridNoticeBoardTwo[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridNoticeBoardTwo[i, j].Visible = true;
                    _buttonGridNoticeBoardTwo[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 3) // szín
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Yellow;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 4)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Orange;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 5)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Blue;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 6)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Violet;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 7)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.White;
                    }
                    Controls.Add(_buttonGridNoticeBoardTwo[i, j]);
                    // felvesszük az ablakra a gombot
                }
            }
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("A játékosnézet ablak bezárásával a játék megszakad, folytatja?", "Figyelmeztetés", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    GameMenuForm.instance.DisposeAllForms();
                }
            }
        }
        #endregion

        #region Public methods
        // Elküldi az irány paraméterét a játékos számával a modelnek, és végrehajtja a tisztítást
        private void clearButton_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                if (GameMenuForm.instance._model.Clear(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres tisztítás!";
                }
                else
                {
                    _successText = "Sikertelen tisztítás!";
                }
            }
            GameMenuForm.instance._model.Wait();
            _operationDone = true;
            if (_activePlayer == 1)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer1TeamGreen = 8;
            }
            else if (_activePlayer == 2)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer2TeamGreen = 8;
            }
            DisableButtons();
        }

        // Csökkenti a körök számát, a másik játékos következik
        public void waitButton_Click(object sender, EventArgs e)
        {
            _successText = "Sikeres várakozás!";
            GameMenuForm.instance._model.Wait();
            stepsLeftValueText.Text = GameMenuForm.instance._model.GameStepCount.ToString();
            _operationDone = true;
            if (_activePlayer == 1)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer1TeamGreen = 1;
            }
            else if (_activePlayer == 2)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer2TeamGreen = 1;
            }
            DisableButtons();
        }
        public void dettachButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {

                if (GameMenuForm.instance._model.Dettach(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres lekapcsolódás!";
                }
                else
                {
                    _successText = "Sikertelen lekapcsolódás!";
                }
            }
            GameMenuForm.instance._model.Wait();
            _operationDone = true;
            if (_activePlayer == 1)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer1TeamGreen = 5;
            }
            else if (_activePlayer == 2)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer2TeamGreen = 5;
            }
            DisableButtons();
        }
        public void attachButton_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                if (GameMenuForm.instance._model.Attach(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres kapcsolódás!";
                }
                else
                {
                    _successText = "Sikertelen kapcsolódás!";
                }
            }
            GameMenuForm.instance._model.Wait();
            _operationDone = true;
            if (_activePlayer == 1)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer1TeamGreen = 4;
            }
            else if (_activePlayer == 2)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer2TeamGreen = 4;
            }
            DisableButtons();
        }

        // Elküldi az irány paraméterét a játékos számával a modelnek, és végrehajtja a mozgást (egyelőre kapcsolódás nélkül)
        public void moveButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                
                if (GameMenuForm.instance._model.Move(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres mozgás!";
                }
                else
                {
                    _successText = "Sikertelen mozgás!";
                }
            }
            GameMenuForm.instance._model.Wait();
            _operationDone = true;
            DisableButtons();
        }

        // Elküldi az irány paraméterét a játékos számával a modelnek, és végrehajtja a forgást
        public void turnButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "óramutatóval megegyező") && operationParameter.Text != "óramutatóval ellenkező"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                if (GameMenuForm.instance._model.Rotate(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres forgás!";
                }          
                else
                {
                    _successText = "Sikertelen forgás!";
                }
            }
            GameMenuForm.instance._model.Wait();
            _operationDone = true;
            if (_activePlayer == 1)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer1TeamGreen = 2;
            }
            else if (_activePlayer == 2)
            {
                GameMenuForm.instance._model.lastOperationTypePlayer2TeamGreen = 2;
            }
            DisableButtons();
        }

        //Lejebb egy nagy függvény van. Igazából jobb lene, hogy ha lenne valami eszköz a tömörítéséhez, de szerintem ez itt lehetetlen, hoszen minden jétákosnak van egy saját táblája, és így külön mindegyiket kell frissíteni.
        public void RefreshTable(int active)
        {
            _activePlayer = active;
            if (_operationDone)
            {
                successfulText.Text = _successText;
            }
            else
            {
                successfulText.Text = "Nincs művelet!";
            }
            _operationDone = false;

            if (active == 1) //első játékos zöld csapat esetén
            {
                nextRoundValueText.Text = "Zöld csapat 2. játékos";
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);
                for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                    {
                        if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;
                            if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == -1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 0)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 3)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 4)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 5)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 6)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 8)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 10) // nem látható mezők
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                            }
                            Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                        }
                    }
                }
            }
            if (active == 2) //másik játékos zöld csapat esetén
            {
                if (_teams == 1)
                {
                    nextRoundValueText.Text = "Zöld csapat 1. játékos";
                }
                else
                {
                    nextRoundValueText.Text = "Piros csapat 1. játékos";
                }
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 8);
                for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                    {
                        if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;
                            if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == -1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 0)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 3)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 4)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 5)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 6)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 8)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 10) // nem látható mezők
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                            }
                            Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                        }
                    }
                }
            }
            if (_teams == 2)
            {
                if (active == 3) //első játékos piros csapat esetén
                {
                    nextRoundValueText.Text = "Piros csapat 2. játékos";
                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, 2);

                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;
                                if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == -1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 0)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 3)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 4)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 5)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 6)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 8)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 10) // nem látható mezők
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                }
                                Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                            }
                        }
                    }
                } 
                else if (active == 4) //másik játékos piros csapat esetén
                {
                    nextRoundValueText.Text = "Zöld csapat 1. játékos";
                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, 9);

                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;
                                if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == -1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 0)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 3)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 4)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 5)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 6)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 8)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 10) // nem látható mezők
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                }
                                Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                            }
                        }
                    }
                }
            }
            // Letöröljük a szöveget a koordináta dobozairól
            coordinate1.Text = "";
            coordinate2.Text = "";
            // Visszaállítjuk, hogy először az első koordinátadoboz kerüljön kitöltésre
            _activeCoordinateBox = 1;
        }

        public void GameForm_Load(object sender, EventArgs e)
        {

        }
        
        // Robotok képeinek forgatása 
        public void RotateImage(int i, int j)
        {
            if (GameMenuForm.instance._model.Table.GetFaceNorth(i, j))            // Megnézzük, hogy a robot melyik irányba néz          
            {
                _buttonGridPlayer[i - 3, j - 4].BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipX);    // Abba az irányba forgatjuk a képet, amerre a robot néz
            }
            else if (GameMenuForm.instance._model.Table.GetFaceWest(i, j))
            {
                _buttonGridPlayer[i - 3, j - 4].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipXY);
            }
            else if (GameMenuForm.instance._model.Table.GetFaceEast(i, j))
            {
                _buttonGridPlayer[i - 3, j - 4].BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
            }
        }

        // Letiltja a műveletek használatát
        public void DisableButtons()
        {
            waitButton.Enabled = false;
            moveButton.Enabled = false;
            turnButton.Enabled = false;
            attachButton.Enabled = false;
            detachButton.Enabled = false;
            clearButton.Enabled = false;
            attachCubesButton.Enabled = false;
            detachCubesButton.Enabled = false;
        }

        // Engedélyezi a műveletek használatát
        public void EnableButtons()
        {
            waitButton.Enabled = true;
            moveButton.Enabled = true;
            turnButton.Enabled = true;
            attachButton.Enabled = true;
            detachButton.Enabled = true;
            clearButton.Enabled = true;
            attachCubesButton.Enabled = true;
            detachCubesButton.Enabled = true;
        }

        private void attachCubesButton_Click(object sender, EventArgs e)
        {
            if (_activePlayer == 1 || _activePlayer == 2)
            {
                GameMenuForm.instance._model.greenTeamCubeAttachState++;
            }
            else
            {
                GameMenuForm.instance._model.redTeamCubeAttachState++;
            }

            if (coordinate1.Text != "" && coordinate2.Text != "")
            {
                if (_activePlayer == 1 || _activePlayer == 2)
                {
                    // Csapattól függően külön kezeljük a kockaösszekapcsolást
                    if (GameMenuForm.instance._model.greenTeamCubeAttachState == 1)
                    {
                        // Amikor az első játékos kezdeményezi az összekapcsolást, elmentjük a megadott koordinátáit
                        GameMenuForm.instance._model.cube1XPlayer1TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube1YPlayer1TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.cube2XPlayer1TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube2YPlayer1TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[1]);
                        _successText = "Sikeres részművelet!";
                    }
                    else if (GameMenuForm.instance._model.greenTeamCubeAttachState == 2)
                    {
                        // Amikor a második játékos kezdeményezi az összekapcsolást, elmentjuk a megadott koordinátáit, és meghívjuk az összekapcsolás műveletet
                        GameMenuForm.instance._model.cube1XPlayer2TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube1YPlayer2TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.cube2XPlayer2TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube2YPlayer2TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[1]);

                        if (GameMenuForm.instance._model.AttachCubes("green") == true)
                        {
                            _successText = "Sikeres összekapcsolás!";
                        }
                        else
                        {
                            _successText = "Sikertelen összekapcsolás!";
                        }
                    }
                    else if (GameMenuForm.instance._model.greenTeamCubeAttachState > 2)
                    {
                        _successText = "Sikertelen összekapcsolás!";
                        GameMenuForm.instance._model.greenTeamCubeAttachState = 0;
                    }
                }
                else
                {
                    if (GameMenuForm.instance._model.redTeamCubeAttachState == 1)
                    {
                        GameMenuForm.instance._model.cube1XPlayer1TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube1YPlayer1TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.cube2XPlayer1TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube2YPlayer1TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[1]);
                        _successText = "Sikeres részművelet!";
                    }
                    else if (GameMenuForm.instance._model.redTeamCubeAttachState == 2)
                    {
                        GameMenuForm.instance._model.cube1XPlayer2TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube1YPlayer2TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.cube2XPlayer2TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.cube2YPlayer2TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[1]);

                        if (GameMenuForm.instance._model.AttachCubes("red") == true)
                        {
                            _successText = "Sikeres összekapcsolás!";
                        }
                        else
                        {
                            _successText = "Sikertelen összekapcsolás!";
                        }
                    }
                    else if (GameMenuForm.instance._model.redTeamCubeAttachState > 2)
                    {
                        _successText = "Sikertelen összekapcsolás!";
                        GameMenuForm.instance._model.redTeamCubeAttachState = 0;
                    }
                }
            }
            else
            {
                _successText = "Sikertelen összekapcsolás!";
            }
            _operationDone = true;
            GameMenuForm.instance._model.Wait(); 
            DisableButtons();
        }

        private void detachCubesButton_Click(object sender, EventArgs e)
        {
            if (coordinate1.Text != "" && coordinate2.Text != "")
            {
                // Bekérjük a szétkapcsolandó kockák koordinátáit, és meghívjuk rájuk a szétkapcsolás műveletet
                GameMenuForm.instance._model.cubeToDetach1X = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                GameMenuForm.instance._model.cubeToDetach1Y = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                GameMenuForm.instance._model.cubeToDetach2X = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                GameMenuForm.instance._model.cubeToDetach2Y = Convert.ToInt32(coordinate2.Text.Split(',')[1]);
                if (GameMenuForm.instance._model.DetachCubes(_activePlayer) == true)
                {
                    _successText = "Sikeres szétkapcsolás!";
                }
                else
                {
                    _successText = "Sikertelen szétkapcsolás!";
                }
            }
            else
            {
                _successText = "Sikertelen szétkapcsolás!";
            }
            _operationDone = true;
            GameMenuForm.instance._model.Wait(); 
            DisableButtons();
        }

        #endregion

    }
}
