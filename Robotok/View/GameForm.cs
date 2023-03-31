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

        #endregion

        #region Constructor

        /// <summary>
        /// Játékablak példányosítása.
        /// </summary>

        public GameForm()
        {
            InitializeComponent();

            // Játéktáblák inicializálása
            GenerateTables();

        }

        #endregion

        #region Game event handlers

        #endregion

        #region Grid event handlers

        #endregion

        #region Menu event handlers

        #endregion

        #region Private methods

        // Létrehozza a gombokat, amiből a játékosnézet és a hirdektőtáblák felépülnek
        private void GenerateTables()
        {
            // Játékos táblája
            _buttonGridPlayer = new Button[GameMenuForm.instance._model.TablePlayerOne.SizeX, GameMenuForm.instance._model.TablePlayerOne.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.TablePlayerOne.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.TablePlayerOne.SizeY; j++)
                {
                    _buttonGridPlayer[i, j] = new Button();
                    _buttonGridPlayer[i, j].Location = new Point(60 + 25 * j, 85 + 25 * i); // elhelyezkedés
                    _buttonGridPlayer[i, j].Size = new Size(25, 25); // méret
                    _buttonGridPlayer[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGridPlayer[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGridPlayer[i, j].Visible = true;
                    _buttonGridPlayer[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus

                    Controls.Add(_buttonGridPlayer[i, j]);
                    // felvesszük az ablakra a gombot
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

                    Controls.Add(_buttonGridNoticeBoardTwo[i, j]);
                    // felvesszük az ablakra a gombot
                }
            }
        }


        // Csökkenti a körök számát, a másik játékos következik
        public void waitButton_Click(object sender, EventArgs e)
        {
            GameMenuForm.instance._model.Wait();
            stepsLeftValueText.Text = GameMenuForm.instance._model.GameStepCount.ToString();
        }

        // Elküldi az irány paraméterét a játékos számával a modelnek, és végrehajtja a mozgást (egyelőre kapcsolódás nélkül)
        public void moveButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(operationParameter.Text) && (operationParameter.Text != "észak") && operationParameter.Text != "dél" && operationParameter.Text != "kelet" && operationParameter.Text != "nyugat")
            {
                // hibaüzenet megjelenítésének helye


            }
            else
            {
                if (int.Parse(stepsLeftValueText.Text) % 2 == 0) 
                {
                    GameMenuForm.instance._model.Move(operationParameter.Text, 1);
                }
                else
                {
                    GameMenuForm.instance._model.Move(operationParameter.Text, 2);
                }
                DisableButtons();
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
