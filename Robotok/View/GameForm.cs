using ELTE.Robotok.Model;
using ELTE.Robotok.Persistence;
using Robotok.WinForms.Properties;

namespace ELTE.Robotok.View
{
    public partial class GameForm : Form
    {
        #region Fields

        private Button[,] _buttonGridPlayer = null!; // játékosnézet gombjai
        private Button[,] _buttonGridNoticeBoardOne = null!; // első hirdetőtábla gombjai
        private Button[,] _buttonGridNoticeBoardTwo = null!; // második hirdetőtábla gombjai
        private Int32 _difficulty; // játék nehézsége
        private Int32 _teams; // csapatok száma
        private Int32 _activePlayer; // játékos azonosítója
        private string? _successText; // a végrehajtott művelet sikeressége
        private Boolean _operationDone; // végzett-e valamilyen műveletet a játékos
        private Int32 _activeCoordinateBox; // 1 - első összekapcsolandó kocka koorinátáinak doboza, 2 - második összekapcsolandó kocka koordinátáinak doboza
        private Panel[,] _verticalPanels = null!; // függőleges panelek (kockák közötti kapcsolódások megjelenítésére)
        private Panel[,] _horizontalPanels = null!; // vízszintes panelek (kockák közötti kapcsolódások megjelenítésére)

        #endregion

        #region Constructor

        /// <summary>
        /// Játékablak példányosítása
        /// </summary>
        /// <param name="difficulty">Játék nehézsége</param>
        /// <param name="teams">Csapatok száma</param>
        /// <param name="activePlayer">Soron következő játékos</param>
        public GameForm(Int32 difficulty, Int32 teams, Int32 activePlayer)
        {
            InitializeComponent();
            _difficulty = difficulty;
            _teams = teams;
            _activeCoordinateBox = 1;
            if (GameMenuForm.instance._model.Teams != 2)
            {
                this.redGroupPointsValueText.Text = "-";
            }
            else
            {
                this.redGroupPointsValueText.Text = GameMenuForm.instance._model.RedTeamPoints.ToString();
            }
            _activePlayer = activePlayer;
            _operationDone = false;
            // Játéktáblák inicializálása
            GenerateTables();
            GenerateAttachments();
            InitializeTexts();
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Játékoznézet gombjainak eseménykezelője (kockaösszekapcsolás és -szétválasztásnál használt)
        /// </summary>
        private void ButtonGrid_Click(object? sender, EventArgs e)
        {
            if (sender is Button senderButton)
            {
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
        }

        /// <summary>
        /// Eseménykezelő az ablak bezárására (összes játékablakra vonatkozik)
        /// </summary>
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
                    GameMenuForm.instance.DisposeAllForms(); // bármelyik játékosablak bezárásakor az összes többi is bezáródik, és visszakerülünk a főmenübe
                }
            }
        }

        /// <summary>
        /// Tisztítás gomb eseménykezelője
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
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
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            _operationDone = true;
            DisableButtons();
        }

        /// <summary>
        /// Várakozás gomb eseménykezelője
        /// </summary>
        private void WaitButton_Click(object sender, EventArgs e)
        {
            _successText = "Sikeres várakozás!";
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            stepsLeftValueText.Text = GameMenuForm.instance._model.GameStepCount.ToString();
            _operationDone = true;
            DisableButtons();
        }

        /// <summary>
        /// Kapcsolódás gomb eseménykezelője
        /// </summary>
        private void AttachButton_Click(object sender, EventArgs e)
        {
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
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            _operationDone = true;
            DisableButtons();
        }

        /// <summary>
        /// Lekapcsolódás gomb eseménykezelője
        /// </summary>
        private void DettachButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                if (GameMenuForm.instance._model.Detach(operationParameter.Text, _activePlayer))
                {
                    _successText = "Sikeres lekapcsolódás!";
                }
                else
                {
                    _successText = "Sikertelen lekapcsolódás!";
                }
            }
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            _operationDone = true;
            DisableButtons();
        }

        /// <summary>
        /// Mozgás gomb eseménykezelője
        /// </summary>
        private void MoveButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                Int32 moveResult = GameMenuForm.instance._model.Move(operationParameter.Text, _activePlayer);

                if (moveResult == 1)
                {
                    GameMenuForm.instance._model.MovePlayerView(operationParameter.Text, _activePlayer);
                    _successText = "Sikeres mozgás!";
                }
                else if (moveResult == 2)
                {
                    _successText = "Sikeres részművelet!";
                }
                else if (moveResult == 3)
                {
                    GameMenuForm.instance._model.MovePlayerView(operationParameter.Text, _activePlayer);
                    _successText = "Sikeres karavánmozgás!";
                }
                else
                {
                    _successText = "Sikertelen mozgás!";
                }
            }
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            _operationDone = true;
            DisableButtons();
        }

        /// <summary>
        /// Forgás gomb eseménykezelője
        /// </summary>
        private void TurnButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) || ((operationParameter.Text != "óramutatóval megegyező") && operationParameter.Text != "óramutatóval ellenkező"))
            {
                _successText = "Hibás paraméter!";
            }
            else
            {
                if (GameMenuForm.instance._model.Rotate(operationParameter.Text, _activePlayer))
                {
                    GameMenuForm.instance._model.RotatePlayerView(operationParameter.Text, _activePlayer);
                    _successText = "Sikeres forgás!";
                }
                else
                {
                    _successText = "Sikertelen forgás!";
                }
            }
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            _operationDone = true;
            DisableButtons();
        }

        /// <summary>
        /// Összekapcsolás gomb eseménykezelője
        /// </summary>
        private void AttachCubesButton_Click(object sender, EventArgs e)
        {
            if (_activePlayer == 1 || _activePlayer == 8)
            {
                GameMenuForm.instance._model.GreenTeamCubeAttachState++;
            }
            else
            {
                GameMenuForm.instance._model.RedTeamCubeAttachState++;
            }

            if (coordinate1.Text != "" && coordinate2.Text != "")
            {
                if (_activePlayer == 1 || _activePlayer == 8)
                {
                    // csapattól függően külön kezeljük a kockaösszekapcsolást
                    if (GameMenuForm.instance._model.GreenTeamCubeAttachState == 1)
                    {
                        // amikor az első játékos kezdeményezi az összekapcsolást, elmentjük a megadott koordinátáit
                        GameMenuForm.instance._model.Cube1XPlayer1TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube1YPlayer1TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.Cube2XPlayer1TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube2YPlayer1TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[1]);
                        _successText = "Sikeres részművelet!";
                    }
                    else if (GameMenuForm.instance._model.GreenTeamCubeAttachState == 2)
                    {
                        // amikor a második játékos kezdeményezi az összekapcsolást, elmentjük a megadott koordinátáit, és meghívjuk az összekapcsolás műveletet
                        GameMenuForm.instance._model.Cube1XPlayer2TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube1YPlayer2TeamGreen = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.Cube2XPlayer2TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube2YPlayer2TeamGreen = Convert.ToInt32(coordinate2.Text.Split(',')[1]);

                        if (GameMenuForm.instance._model.AttachCubes("green") == true)
                        {
                            _successText = "Sikeres összekapcsolás!";
                            GameMenuForm.instance._model.GreenTeamCubeAttachState = 0;
                        }
                        else
                        {
                            _successText = "Sikertelen összekapcsolás!";
                            GameMenuForm.instance._model.GreenTeamCubeAttachState = 0;
                        }
                    }
                    else if (GameMenuForm.instance._model.GreenTeamCubeAttachState > 2)
                    {
                        _successText = "Sikertelen összekapcsolás!";
                        GameMenuForm.instance._model.GreenTeamCubeAttachState = 0;
                    }
                }
                else
                {
                    if (GameMenuForm.instance._model.RedTeamCubeAttachState == 1)
                    {
                        GameMenuForm.instance._model.Cube1XPlayer1TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube1YPlayer1TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.Cube2XPlayer1TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube2YPlayer1TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[1]);
                        _successText = "Sikeres részművelet!";
                    }
                    else if (GameMenuForm.instance._model.RedTeamCubeAttachState == 2)
                    {
                        GameMenuForm.instance._model.Cube1XPlayer2TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube1YPlayer2TeamRed = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                        GameMenuForm.instance._model.Cube2XPlayer2TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                        GameMenuForm.instance._model.Cube2YPlayer2TeamRed = Convert.ToInt32(coordinate2.Text.Split(',')[1]);

                        if (GameMenuForm.instance._model.AttachCubes("red") == true)
                        {
                            _successText = "Sikeres összekapcsolás!";
                            GameMenuForm.instance._model.RedTeamCubeAttachState = 0;
                        }
                        else
                        {
                            _successText = "Sikertelen összekapcsolás!";
                            GameMenuForm.instance._model.RedTeamCubeAttachState = 0;
                        }
                    }
                    else if (GameMenuForm.instance._model.RedTeamCubeAttachState > 2)
                    {
                        _successText = "Sikertelen összekapcsolás!";
                        GameMenuForm.instance._model.RedTeamCubeAttachState = 0;
                    }
                }
            }
            else
            {
                _successText = "Sikertelen összekapcsolás!";
            }
            _operationDone = true;
            GameMenuForm.instance._model.Wait(_activePlayer, 0);
            DisableButtons();
        }

        /// <summary>
        /// Szétkapcsolás gomb eseménykezelője
        /// </summary>
        private void DetachCubesButton_Click(object sender, EventArgs e)
        {
            if (coordinate1.Text != "" && coordinate2.Text != "")
            {
                // bekérjük a szétkapcsolandó kockák koordinátáit, és meghívjuk rájuk a szétkapcsolás műveletet
                GameMenuForm.instance._model.CubeToDetach1X = Convert.ToInt32(coordinate1.Text.Split(',')[0]);
                GameMenuForm.instance._model.CubeToDetach1Y = Convert.ToInt32(coordinate1.Text.Split(',')[1]);
                GameMenuForm.instance._model.CubeToDetach2X = Convert.ToInt32(coordinate2.Text.Split(',')[0]);
                GameMenuForm.instance._model.CubeToDetach2Y = Convert.ToInt32(coordinate2.Text.Split(',')[1]);
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
            GameMenuForm.instance._model.Wait(_activePlayer, 1);
            DisableButtons();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Játéktáblák kigenerálása (játékos táblája, hirdetőtáblák)
        /// </summary>
        private void GenerateTables()
        {
            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);
            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 8);

            if (_teams == 2)
            {
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 2);
                GameMenuForm.instance._model.ManhattanDistance(_difficulty, 9);
            }

            // Játékosok táblájának létrehozása
            _buttonGridPlayer = new Button[GameMenuForm.instance._model.TableGreenPlayerOne.SizeX, GameMenuForm.instance._model.TableGreenPlayerOne.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    if (i >= 3 && i <= 13 && j >= 4 && j <= 23) // a játékosnézeteken csak a játékterület belső része van megjelenítve
                    {
                        _buttonGridPlayer[i - 3, j - 4] = new Button();
                        _buttonGridPlayer[i - 3, j - 4].Location = new Point(Convert.ToInt32(60 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * (j - 4), Convert.ToInt32(85 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * (i - 3)); // elhelyezkedés
                        _buttonGridPlayer[i - 3, j - 4].Size = new Size(Convert.ToInt32(25 * GetScalingFactor()), Convert.ToInt32(25 * GetScalingFactor())); // méret
                        _buttonGridPlayer[i - 3, j - 4].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);
                        _buttonGridPlayer[i - 3, j - 4].Enabled = true;
                        _buttonGridPlayer[i - 3, j - 4].Visible = true;
                        _buttonGridPlayer[i - 3, j - 4].FlatStyle = FlatStyle.Flat;
                        _buttonGridPlayer[i - 3, j - 4].BackgroundImageLayout = ImageLayout.Stretch; // kép mezőhöz méretezése
                        _buttonGridPlayer[i - 3, j - 4].Name = i.ToString() + "," + j.ToString();
                        _buttonGridPlayer[i - 3, j - 4].Click += ButtonGrid_Click;
                        if (_activePlayer == 1)
                        {
                            if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == -1) // minden mezőnek megadjuk a színét
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
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 11)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 12)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
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
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 10) // Manhattan-távolságon belül nem látható mezők beállítása
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                            }
                            if (!GameMenuForm.instance._model.TableGreenPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != 10) // Manhattan-távolságon kívüli mezők beállítása
                            {
                                Color color = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, color.R, color.G, color.B);
                            }
                            if (GameMenuForm.instance._model.TableGreenPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 7) // Manhattan-távolságon belüli mezők beállítása
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.Green.R, Color.Green.G, Color.Green.B);
                            }
                        }
                        else if (_activePlayer == 8)
                        {
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
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
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
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 11)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 12)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 8)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 10) 
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                            }
                            if (!GameMenuForm.instance._model.TableGreenPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != 10) 
                            {
                                Color color = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, color.R, color.G, color.B);
                            }
                            if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 7) 
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.DarkGreen.R, Color.DarkGreen.G, Color.DarkGreen.B);
                            }
                        }
                        if (_teams == 2)
                        {
                            if (_activePlayer == 2)
                            {
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
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
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
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 11)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 12)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 8)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 10)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                }
                                if (!GameMenuForm.instance._model.TableRedPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) != 10) 
                                {
                                    Color color = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, color.R, color.G, color.B);
                                }
                                if (GameMenuForm.instance._model.TableRedPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 7) 
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.Red.R, Color.Red.G, Color.Red.B);
                                }
                            }
                            else if (_activePlayer == 9)
                            {
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
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
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
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 11)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 12)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 8)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 10)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                }
                                if (!GameMenuForm.instance._model.TableRedPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != 10) 
                                {
                                    Color color = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, color.R, color.G, color.B);
                                }
                                if (GameMenuForm.instance._model.TableRedPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 7) 
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.DarkRed.R, Color.DarkRed.G, Color.DarkRed.B);
                                }
                            }
                        }

                        Controls.Add(_buttonGridPlayer[i - 3, j - 4]); 
                    }
                }
            }

            GameMenuForm.instance._model.ManhattanDistance(_difficulty, 1);

            // Első hirdetőtábla
            _buttonGridNoticeBoardOne = new Button[GameMenuForm.instance._model.TableNoticeBoardOne.SizeX, GameMenuForm.instance._model.TableNoticeBoardOne.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.TableNoticeBoardOne.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TableNoticeBoardOne.SizeY; j++)
                {
                    _buttonGridNoticeBoardOne[i, j] = new Button();
                    _buttonGridNoticeBoardOne[i, j].Location = new Point(Convert.ToInt32(215 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * j, Convert.ToInt32(400 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * i);
                    _buttonGridNoticeBoardOne[i, j].Size = new Size(Convert.ToInt32(25 * GetScalingFactor()), Convert.ToInt32(25 * GetScalingFactor()));
                    _buttonGridNoticeBoardOne[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);
                    _buttonGridNoticeBoardOne[i, j].Enabled = false;
                    _buttonGridNoticeBoardOne[i, j].Visible = true;
                    _buttonGridNoticeBoardOne[i, j].FlatStyle = FlatStyle.Flat;
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
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 11)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Purple;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 12)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Aquamarine;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == -2)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.White;
                    }

                    Controls.Add(_buttonGridNoticeBoardOne[i, j]); // megjelenítjük az ablakon a gombot
                }
            }

            // Második hirdetőtábla
            _buttonGridNoticeBoardTwo = new Button[GameMenuForm.instance._model.TableNoticeBoardTwo.SizeX, GameMenuForm.instance._model.TableNoticeBoardTwo.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.TableNoticeBoardTwo.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TableNoticeBoardTwo.SizeY; j++)
                {
                    _buttonGridNoticeBoardTwo[i, j] = new Button();
                    _buttonGridNoticeBoardTwo[i, j].Location = new Point(Convert.ToInt32(365 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * j, Convert.ToInt32(400 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * i);
                    _buttonGridNoticeBoardTwo[i, j].Size = new Size(Convert.ToInt32(25 * GetScalingFactor()), Convert.ToInt32(25 * GetScalingFactor()));
                    _buttonGridNoticeBoardTwo[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);
                    _buttonGridNoticeBoardTwo[i, j].Enabled = false;
                    _buttonGridNoticeBoardTwo[i, j].Visible = true;
                    _buttonGridNoticeBoardTwo[i, j].FlatStyle = FlatStyle.Flat;
                    if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 3)
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
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 11)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Purple;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 12)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Aquamarine;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == -2)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.White;
                    }

                    Controls.Add(_buttonGridNoticeBoardTwo[i, j]);
                }
            }
        }

        /// <summary>
        /// Panelek kigenerálása (a kockaösszekapcsolások megjelenítésére)
        /// </summary>
        private void GenerateAttachments()
        {
            _verticalPanels = new Panel[GameMenuForm.instance._model.TableGreenPlayerOne.SizeX + 1, GameMenuForm.instance._model.TableGreenPlayerOne.SizeY + 1];
            _horizontalPanels = new Panel[GameMenuForm.instance._model.TableGreenPlayerOne.SizeX + 1, GameMenuForm.instance._model.TableGreenPlayerOne.SizeY + 1];

            for (Int32 i = 0; i <= GameMenuForm.instance._model.TableGreenPlayerOne.SizeX; i++) // a kockák pozíciójához viszonyítva két kocka határán jelenítjük meg a kapcsolatokat
            {
                for (Int32 j = 0; j <= GameMenuForm.instance._model.TableGreenPlayerOne.SizeY; j++)
                {
                    _verticalPanels[i, j] = new Panel();
                    _verticalPanels[i, j].Location = new Point(Convert.ToInt32(63 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * j - 4, Convert.ToInt32(88 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * i - 3); // elhelyezés két kocka határára
                    _verticalPanels[i, j].Size = new Size(Convert.ToInt32(5 * GetScalingFactor()), Convert.ToInt32(25 * GetScalingFactor()));
                    _verticalPanels[i, j].BackColor = Color.Red;
                    _verticalPanels[i, j].Visible = false;

                    _horizontalPanels[i, j] = new Panel();
                    _horizontalPanels[i, j].Location = new Point(Convert.ToInt32(64 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * j - 4, Convert.ToInt32(87 * GetScalingFactor()) + Convert.ToInt32(25 * GetScalingFactor()) * i - 3);
                    _horizontalPanels[i, j].Size = new Size(Convert.ToInt32(25 * GetScalingFactor()), Convert.ToInt32(5 * GetScalingFactor()));
                    _horizontalPanels[i, j].BackColor = Color.Red;
                    _horizontalPanels[i, j].Visible = false;

                    Controls.Add(_verticalPanels[i, j]); // felvesszük őket az ablakra
                    Controls.Add(_horizontalPanels[i, j]);

                    _verticalPanels[i, j].BringToFront(); // előtérbe kell hozni a paneleket, hogy a pálya gombjai ne takarják el
                    _horizontalPanels[i, j].BringToFront();
                }
            }
        }

        /// <summary>
        /// Műveletek letiltása
        /// </summary>
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

        /// <summary>
        /// Műveletek engedélyezése
        /// </summary>
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

        /// <summary>
        /// Csatolások frissítése
        /// </summary>
        /// <param name="coordinateX">Aktív mező X koordinátája</param>
        /// <param name="coordinateY">Aktív mező Y koordinátája</param>
        /// <param name="table">Tábla, amelyen keressük a csatolást</param>
        private void RefreshAttachments(Int32 coordinateX, Int32 coordinateY, RobotokTable table)
        {
            if (table.GetAttachmentEast(coordinateX, coordinateY))
            {
                _verticalPanels[coordinateX, coordinateY + 1].Visible = true;
            }

            if (table.GetAttachmentWest(coordinateX, coordinateY))
            {
                _verticalPanels[coordinateX, coordinateY].Visible = true;
            }

            if (table.GetAttachmentNorth(coordinateX, coordinateY))
            {
                _horizontalPanels[coordinateX, coordinateY].Visible = true;
            }

            if (table.GetAttachmentSouth(coordinateX, coordinateY))
            {
                _horizontalPanels[coordinateX + 1, coordinateY].Visible = true;
            }
        }

        /// <summary>
        /// Csatolások törlése
        /// </summary>
        /// <param name="coordinateX">Aktív mező X koordinátája</param>
        /// <param name="coordinateY">Aktív mező Y koordinátája</param>
        /// <param name="table">Tábla, amelyen keressük a csatolást</param>
        private void ClearAttachments(Int32 coordinateX, Int32 coordinateY, RobotokTable table)
        {
            if (!table.GetAttachmentEast(coordinateX, coordinateY))
            {
                _verticalPanels[coordinateX, coordinateY + 1].Visible = false;
            }

            if (!table.GetAttachmentWest(coordinateX, coordinateY))
            {
                _verticalPanels[coordinateX, coordinateY].Visible = false;
            }

            if (!table.GetAttachmentNorth(coordinateX, coordinateY))
            {
                _horizontalPanels[coordinateX, coordinateY].Visible = false;
            }

            if (!table.GetAttachmentSouth(coordinateX, coordinateY))
            {
                _horizontalPanels[coordinateX + 1, coordinateY].Visible = false;
            }
        }

        /// <summary>
        /// Robot képének forgatása
        /// </summary>
        /// <param name="coordinateX">Aktív mező X koordinátája</param>
        /// <param name="coordinateY">Aktív mező Y koordinátája</param>
        /// <param name="table">Tábla, amelyen keressük a csatolást</param>
        private void RotateImage(Int32 coordinateX, Int32 coordinateY, RobotokTable table)
        {
            if (table.GetFaceNorth(coordinateX, coordinateY)) // megnézzük, hogy a robot melyik irányba néz, és ez alapján forgatjuk a robot képét        
            {
                _buttonGridPlayer[coordinateX, coordinateY].BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipX);
            }
            else if (table.GetFaceWest(coordinateX, coordinateY))
            {
                _buttonGridPlayer[coordinateX, coordinateY].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipXY);
            }
            else if (table.GetFaceEast(coordinateX, coordinateY))
            {
                _buttonGridPlayer[coordinateX, coordinateY].BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
            }
        }

        /// <summary>
        /// Hátralévő tisztítási műveletek képének frissítése
        /// </summary>
        /// <param name="coordinateX">Aktív mező X koordinátája</param>
        /// <param name="coordinateY">Aktív mező Y koordinátája</param>
        private void RefreshCleaningOperationImage(Int32 coordinateX, Int32 coordinateY)
        {
            if (GameMenuForm.instance._model.GameDifficulty == GameDifficulty.Easy)
            {
                if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(coordinateX, coordinateY) == 1)
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = Resources.crack3;
                }
                else
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = null;
                }
            }
            else if (GameMenuForm.instance._model.GameDifficulty == GameDifficulty.Medium)
            {
                if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(coordinateX, coordinateY) == 2)
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = Resources.crack2;
                }
                else if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(coordinateX, coordinateY) == 1)
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = Resources.crack3;
                }
                else
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = null;
                }
            }
            else
            {
                if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(coordinateX, coordinateY) == 3)
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = Resources.crack1;
                }
                else if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(coordinateX, coordinateY) == 2)
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = Resources.crack2;
                }
                else if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(coordinateX, coordinateY) == 1)
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = Resources.crack3;
                }
                else
                {
                    _buttonGridPlayer[coordinateX - 3, coordinateY - 4].BackgroundImage = null;
                }
            }
        }

        /// <summary>
        /// Játéknézet megnyitásakor inicializálja a szövegeket
        /// </summary>
        private void InitializeTexts()
        {
            taskOneDeadlineValueText.Text = GameMenuForm.instance._model.FirstTaskDeadline.ToString() + " lépés van hátra";
            taskOnePointsValueText.Text = GameMenuForm.instance._model.FirstTaskPoints.ToString();

            taskTwoDeadlineValueText.Text = GameMenuForm.instance._model.SecondTaskDeadline.ToString() + " lépés van hátra";
            taskTwoPointsValueText.Text = GameMenuForm.instance._model.SecondTaskPoints.ToString();

            greenGroupPointsValueText.Text = GameMenuForm.instance._model.GreenTeamPoints.ToString();
        }

        /// <summary>
        /// Kiszámít egy relatív értéket, hogy milyen arányú felbontásváltozás történt az 1920*1080 125%-os nagyítású nézethez képest
        /// </summary>
        /// <returns>Relatív felbontáskülönbség</returns>
        private float GetScalingFactor()
        {
            using (Graphics graphics = CreateGraphics())
            {
                return graphics.DpiX / 120f;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Játékostábla frissítése
        /// </summary>
        /// <param name="active">Aktív játékos</param>
        public void RefreshTable(Int32 active)
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
                            ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                        }
                    }
                }
                for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                    {
                        if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                        {
                            RefreshAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;

                            if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == -1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 0)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 3)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 4)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 5)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 6)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 11)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 12)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 8)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 10) // nem látható mezők
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerOne);
                            }

                            if (!GameMenuForm.instance._model.TableGreenPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) != 10)
                            {
                                Color color = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, color.R, color.G, color.B);
                            }

                            if (GameMenuForm.instance._model.TableGreenPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.Green.R, Color.Green.G, Color.Green.B);
                            }

                            Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                        }
                    }
                }
            }
            if (active == 8) // második játékos zöld csapat esetén
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
                            ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                        }
                    }
                }
                for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                {
                    for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                    {
                        if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                        {
                            RefreshAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                            _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;

                            if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == -1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 0)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 1)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 3)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 4)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 5)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 6)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 11)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 12)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                                RefreshCleaningOperationImage(i, j);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 8)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                            }
                            else if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 10)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableGreenPlayerTwo);
                            }

                            if (!GameMenuForm.instance._model.TableGreenPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) != 10)
                            {
                                Color col = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, col.R, col.G, col.B);
                            }

                            if (GameMenuForm.instance._model.TableGreenPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableGreenPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                            {
                                _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.DarkGreen.R, Color.DarkGreen.G, Color.DarkGreen.B);
                            }

                            Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                        }
                    }
                }
            }
            if (_teams == 2)
            {
                if (active == 2) // első játékos piros csapat esetén
                {
                    nextRoundValueText.Text = "Piros csapat 2. játékos";
                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, 2);

                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);
                            }
                        }
                    }
                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                RefreshAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);

                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;
                                if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == -1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 0)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 3)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 4)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 5)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 6)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 11)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 12)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 8)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 10)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                    ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerOne);
                                }
                                if (!GameMenuForm.instance._model.TableRedPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) != 10)
                                {
                                    Color col = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, col.R, col.G, col.B);
                                }
                                if (GameMenuForm.instance._model.TableRedPlayerOne.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerOne.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.Red.R, Color.Red.G, Color.Red.B);
                                }

                                Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                            }
                        }
                    }
                }
                else if (active == 9) // második játékos piros csapat esetén
                {
                    nextRoundValueText.Text = "Zöld csapat 1. játékos";
                    GameMenuForm.instance._model.ManhattanDistance(_difficulty, 9);

                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                            }
                        }
                    }
                    for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
                    {
                        for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                        {
                            if (i >= 3 && i <= 13 && j >= 4 && j <= 23)
                            {
                                RefreshAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                                _buttonGridPlayer[i - 3, j - 4].BackgroundImage = null;

                                if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == -1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Black;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 0)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Brown;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 1)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Green;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 2)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Red;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 3)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Yellow;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 4)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Orange;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 5)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Blue;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 6)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Violet;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 11)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Purple;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 12)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.Aquamarine;
                                    RefreshCleaningOperationImage(i, j);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.White;
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 8)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGreen;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 9)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkRed;
                                    _buttonGridPlayer[i - 3, j - 4].BackgroundImage = Resources.robot;
                                    RotateImage(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                                }
                                else if (GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 10)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.DarkGray;
                                    ClearAttachments(i - 3, j - 4, GameMenuForm.instance._model.TableRedPlayerTwo);
                                }
                                if (!GameMenuForm.instance._model.TableRedPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != -1 && GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) != 10)
                                {
                                    Color col = _buttonGridPlayer[i - 3, j - 4].BackColor;
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(150, col.R, col.G, col.B);
                                }
                                if (GameMenuForm.instance._model.TableRedPlayerTwo.GetInDistance(i - 3, j - 4) && GameMenuForm.instance._model.TableRedPlayerTwo.GetFieldValue(i - 3, j - 4) == 7)
                                {
                                    _buttonGridPlayer[i - 3, j - 4].BackColor = Color.FromArgb(70, Color.DarkRed.R, Color.DarkRed.G, Color.DarkRed.B);
                                }

                                Controls.Add(_buttonGridPlayer[i - 3, j - 4]);
                            }
                        }
                    }
                }
            }

            coordinate1.Text = ""; // töröljük a szöveget a koordináták dobozairól
            coordinate2.Text = "";

            greenGroupPointsValueText.Text = GameMenuForm.instance._model.GreenTeamPoints.ToString(); // játék pontszáma

            if (_teams == 2)
            {
                redGroupPointsValueText.Text = GameMenuForm.instance._model.RedTeamPoints.ToString();
            }

            _activeCoordinateBox = 1; // visszaállítjuk, hogy először az első koordinátadoboz kerüljön kitöltésre

            for (Int32 i = 0; i < GameMenuForm.instance._model.TableNoticeBoardOne.SizeX; i++) // frissítjük a hirdetőtáblákat is
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TableNoticeBoardOne.SizeY; j++)
                {
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
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 11)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Purple;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == 12)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.Aquamarine;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardOne.GetFieldValue(i, j) == -2)
                    {
                        _buttonGridNoticeBoardOne[i, j].BackColor = Color.White;
                    }
                }
            }

            for (Int32 i = 0; i < GameMenuForm.instance._model.TableNoticeBoardTwo.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TableNoticeBoardTwo.SizeY; j++)
                {
                    if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 3)
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
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 11)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Purple;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == 12)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.Aquamarine;
                    }
                    else if (GameMenuForm.instance._model.TableNoticeBoardTwo.GetFieldValue(i, j) == -2)
                    {
                        _buttonGridNoticeBoardTwo[i, j].BackColor = Color.White;
                    }
                }
            }
            if (_teams == 1) // elmenti az előző körben észlelt kocka mozgatásokat
            {
                if (active == 1)
                {
                    GameMenuForm.instance._model.ImprovedObservation(2);
                    GameMenuForm.instance._model.ClearImprovedObservationList(2);
                }
                else
                {
                    GameMenuForm.instance._model.ImprovedObservation(1);
                    GameMenuForm.instance._model.ClearImprovedObservationList(1);
                }
            }
            else
            {
                if (active == 1)
                {
                    GameMenuForm.instance._model.ImprovedObservation(2);
                    GameMenuForm.instance._model.ClearImprovedObservationList(2);
                }
                else if (active == 2)
                {
                    GameMenuForm.instance._model.ImprovedObservation(1);
                    GameMenuForm.instance._model.ClearImprovedObservationList(1);
                }
                else if (active == 3)
                {
                    GameMenuForm.instance._model.ImprovedObservation(4);
                    GameMenuForm.instance._model.ClearImprovedObservationList(4);
                }
                else
                {
                    GameMenuForm.instance._model.ImprovedObservation(3);
                    GameMenuForm.instance._model.ClearImprovedObservationList(3);
                }
            }
        }

        #endregion
    }
}
