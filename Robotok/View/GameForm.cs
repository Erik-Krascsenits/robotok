using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;

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
        private string _successText; // a végrehajtott művelet sikeressége
        private bool _operationDone; // végzett-e valamilyen műveletet a játékos
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
            GenerateTables(activePlayer);

        }

        #endregion

        #region Game event handlers

        #endregion

        #region Grid event handlers

        #endregion

        #region Menu event handlers

        #endregion

        #region Private methods
        // Lejebb sokszor található lesz i - 3, illetve j - 4. Ez azért van, mert maga a játékpálya sokkal nagyobb mint a játékosak "játékpályai", ezért hogy ne legyen semmilyen túlindexelés vagy valami hasonló, ezt így oldottam meg
        // Létrehozza a gombokat, amiből a játékosnézet és a hirdektőtáblák felépülnek
        private void GenerateTables(int active) 
        {
            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);
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
                        _buttonGridPlayer[i - 3, j - 4].Enabled = false; // kikapcsolt állapot
                        _buttonGridPlayer[i - 3, j - 4].Visible = true;
                        _buttonGridPlayer[i - 3, j - 4].FlatStyle = FlatStyle.Flat; // lapított stípus
                        if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i-3, j-4) == -1) // minden mezőnek megadjuk a színét
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
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
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                        }
                        else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 10) // Manhattan távolságon belül nem látható mezők
                        {
                            _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                        }
                        Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                        // felvesszük az ablakra a gombot
                        successfulText.Text = "Nincs művelet!";
                    }
                }
                
            }
            
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

                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, active);
                    //
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
                if(GameMenuForm.instance._model.Clear(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres tisztítás!";
                }
                else
                {
                    _successText = "Sikertelen tisztítás!";
                }
            }
            _operationDone = true;
            DisableButtons();
        }

        // Csökkenti a körök számát, a másik játékos következik
        public void waitButton_Click(object sender, EventArgs e)
        {
            _successText = "Sikeres várakozás!";
            GameMenuForm.instance._model.Wait();
            stepsLeftValueText.Text = GameMenuForm.instance._model.GameStepCount.ToString();
            _operationDone = true;
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
                
                if(GameMenuForm.instance._model.Move(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres mozgás!";
                }
                else
                {
                    _successText = "Sikertelen mozgás!";
                } 
            }
            _operationDone = true;
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
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);
                for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                    {
                        if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                        {
                            if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == -1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
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
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
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
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 8);
                for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                    {
                        if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                        {
                            if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == -1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
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
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
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
                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, 2);

                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == -1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
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
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
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
                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, 9);

                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == -1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
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
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
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

        #endregion
    }
}
