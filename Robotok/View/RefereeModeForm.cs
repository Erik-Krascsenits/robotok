using ELTE.Robotok.View;
using ELTE.Robotok.Model;
using Robotok.WinForms.Properties;

namespace Robotok.WinForms.View
{
    public partial class RefereeModeForm : Form
    {
        #region Fields

        private Button[,] _buttonGrid = null!; // gombrács (játékvezetői)
        private Panel[,] _verticalPanels = null!; // függőleges panelek (kapcsolódások megjelenítésére)
        private Panel[,] _horizontalPanels = null!; // vízszintes panelek

        #endregion

        #region Constructor

        /// <summary>
        /// Játékvezetői ablak példányosítása
        /// </summary>
        public RefereeModeForm()
        {
            InitializeComponent();
            GenerateRefereeTable();
            GenerateRefereeAttachments();
        }

        #endregion

        #region Closing events
        
        /// <summary>
        /// Játékvezetői nézet bezárása
        /// </summary>
        private void RefereeModeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("A játékvezetői mód bezárásával a nézet végleg elérhetetlenné válik. Folytatja?", "Figyelmeztetés", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Játékvezetői tábla kigenerálása 
        /// </summary>
        private void GenerateRefereeTable()
        {
            _buttonGrid = new Button[GameMenuForm.instance._model.Table.SizeX, GameMenuForm.instance._model.Table.SizeY];

            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Location = new Point(Convert.ToInt32(115 * GetScalingFactor()) + Convert.ToInt32(22 * GetScalingFactor()) * j, Convert.ToInt32(85 * GetScalingFactor()) + Convert.ToInt32(22 * GetScalingFactor()) * i); // elhelyezkedés
                    _buttonGrid[i, j].Size = new Size(Convert.ToInt32(22 * GetScalingFactor()), Convert.ToInt32(22 * GetScalingFactor())); // méret
                    _buttonGrid[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); 
                    _buttonGrid[i, j].Enabled = false;
                    _buttonGrid[i, j].FlatStyle = FlatStyle.Flat; 
                    _buttonGrid[i, j].BackgroundImageLayout = ImageLayout.Stretch; // kép mezőhöz méretezése

                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == -2)
                    {
                        _buttonGrid[i, j].BackColor = Color.Gray;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == -1)
                    {
                        _buttonGrid[i, j].BackColor = Color.Black;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 0)
                    {
                        _buttonGrid[i, j].BackColor = Color.Brown;
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 1)
                    {
                        _buttonGrid[i, j].BackColor = Color.Green;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 2)
                    {
                        _buttonGrid[i, j].BackColor = Color.Red;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 3)
                    {
                        _buttonGrid[i, j].BackColor = Color.Yellow;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 4)
                    {
                        _buttonGrid[i, j].BackColor = Color.Orange;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 5)
                    {
                        _buttonGrid[i, j].BackColor = Color.Blue;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 6)
                    {
                        _buttonGrid[i, j].BackColor = Color.Violet;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 11)
                    {
                        _buttonGrid[i, j].BackColor = Color.Purple;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 12)
                    {
                        _buttonGrid[i, j].BackColor = Color.Aquamarine;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 7)
                    {
                        _buttonGrid[i, j].BackColor = Color.White;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 8)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkGreen;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 9)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkRed;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                    }

                    Controls.Add(_buttonGrid[i, j]); // felvesszük az ablakra a gombot
                }
            }
        }

        /// <summary>
        /// Panelek kigenerálása a játékvezetői nézeten (a kockaösszekapcsolások megjelenítésére) 
        /// </summary>
        private void GenerateRefereeAttachments()
        {
            _verticalPanels = new Panel[GameMenuForm.instance._model.Table.SizeX, GameMenuForm.instance._model.Table.SizeY];
            _horizontalPanels = new Panel[GameMenuForm.instance._model.Table.SizeX, GameMenuForm.instance._model.Table.SizeY];

            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++) // a kockák pozíciójához viszonyítva két kocka határán jelenítjük meg a kapcsolatokat
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    _verticalPanels[i, j] = new Panel();
                    _verticalPanels[i, j].Location = new Point(Convert.ToInt32(113 * GetScalingFactor()) + Convert.ToInt32(22 * GetScalingFactor()) * j, Convert.ToInt32(85 * GetScalingFactor()) + Convert.ToInt32(22 * GetScalingFactor()) * i); // elhelyezés két kocka határára
                    _verticalPanels[i, j].Size = new Size(Convert.ToInt32(5 * GetScalingFactor()), Convert.ToInt32(22 * GetScalingFactor())); 
                    _verticalPanels[i, j].BackColor = Color.Red; 
                    _verticalPanels[i, j].Visible = false;

                    _horizontalPanels[i, j] = new Panel();
                    _horizontalPanels[i, j].Location = new Point(Convert.ToInt32(115 * GetScalingFactor()) + Convert.ToInt32(22 * GetScalingFactor()) * j, Convert.ToInt32(83 * GetScalingFactor()) + Convert.ToInt32(22 * GetScalingFactor()) * i);
                    _horizontalPanels[i, j].Size = new Size(Convert.ToInt32(22 * GetScalingFactor()), Convert.ToInt32(5 * GetScalingFactor()));
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
        /// Játékvezetői tábla frissítése
        /// </summary>
        public void RefreshRefereeView()
        {
            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    RefreshRefereeCleaningOperationImage(i, j);
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == -2)
                    {
                        _buttonGrid[i, j].BackColor = Color.Gray;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == -1)
                    {
                        _buttonGrid[i, j].BackColor = Color.Black;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 0)
                    {
                        _buttonGrid[i, j].BackColor = Color.Brown;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 1)
                    {
                        _buttonGrid[i, j].BackColor = Color.Green;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                        RotateRefereeImage(i, j);
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 2)
                    {
                        _buttonGrid[i, j].BackColor = Color.Red;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                        RotateRefereeImage(i, j);
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 3)
                    {
                        _buttonGrid[i, j].BackColor = Color.Yellow;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 4)
                    {
                        _buttonGrid[i, j].BackColor = Color.Orange;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 5)
                    {
                        _buttonGrid[i, j].BackColor = Color.Blue;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 6)
                    {
                        _buttonGrid[i, j].BackColor = Color.Violet;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 11)
                    {
                        _buttonGrid[i, j].BackColor = Color.Purple;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 12)
                    {
                        _buttonGrid[i, j].BackColor = Color.Aquamarine;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 7)
                    {
                        _buttonGrid[i, j].BackColor = Color.White;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 8)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkGreen;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                        RotateRefereeImage(i, j);
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 9)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkRed;
                        _buttonGrid[i, j].BackgroundImage = Resources.robot;
                        RotateRefereeImage(i, j);
                    }
      
                    if (GameMenuForm.instance._model.Table.GetAttachmentEast(i, j))  // kapcsolatok megjelenítésének frissítése
                    {
                        _verticalPanels[i, j + 1].Visible = true;
                    }
                    
                    if (GameMenuForm.instance._model.Table.GetAttachmentWest(i, j))
                    {
                        _verticalPanels[i, j].Visible = true;
                    }
                    else
                    {
                        _verticalPanels[i, j].Visible = false;
                    }

                    if (GameMenuForm.instance._model.Table.GetAttachmentNorth(i, j))
                    {
                        _horizontalPanels[i, j].Visible = true;
                    }
                    else
                    {
                        _horizontalPanels[i, j].Visible = false;
                    }

                    if (GameMenuForm.instance._model.Table.GetAttachmentSouth(i, j))
                    {
                        _horizontalPanels[i + 1, j].Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Robotok képeinek forgatása a játékvezetői nézeten
        /// </summary>
        public void RotateRefereeImage(Int32 i, Int32 j)
        {
            if (GameMenuForm.instance._model.Table.GetFaceNorth(i, j)) // megnézzük, hogy a robot melyik irányba néz, és ez alapján forgatjuk a robot képét         
            {
                _buttonGrid[i , j].BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipX);    
            }
            else if (GameMenuForm.instance._model.Table.GetFaceWest(i, j))
            {
                _buttonGrid[i ,j].BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipXY);
            }
            else if (GameMenuForm.instance._model.Table.GetFaceEast(i, j))
            {
                _buttonGrid[i, j].BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipXY);
            }
        }

        /// <summary>
        /// Hátralévő tisztítási műveletek képének frissítése
        /// </summary>
        public void RefreshRefereeCleaningOperationImage(Int32 i, Int32 j)
        {
            if(GameMenuForm.instance._model.GameDifficulty  == GameDifficulty.Easy)
            {
                if(GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(i, j) == 1)
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.crack3;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall_cracked_3;
                    }
                }
                else
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = null;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall;
                    }
                }
            }
            else if(GameMenuForm.instance._model.GameDifficulty == GameDifficulty.Medium)
            {
                if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(i, j) == 2)
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = Properties.Resources.crack2;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall_cracked_2;
                    }
                }
                else if(GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(i, j) == 1)
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = Properties.Resources.crack3;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall_cracked_3;
                    }
                }
                else
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = null;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall;
                    }
                }
            }
            else
            {
                if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(i, j) == 3)
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = Properties.Resources.crack1;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall_cracked_1;
                    }
                }
                else if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(i, j) == 2)
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = Properties.Resources.crack2;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall_cracked_2;
                    }
                }
                else if (GameMenuForm.instance._model.Table.GetFieldRemainingCleaningOperations(i, j) == 1)
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = Properties.Resources.crack3;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall_cracked_3;
                    }
                }
                else
                {
                    if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) != 0)
                    {
                        _buttonGrid[i, j].BackgroundImage = null;
                    }
                    else
                    {
                        _buttonGrid[i, j].BackgroundImage = Resources.brick_wall;
                    }
                }
            }
        }
        #endregion
    }
}