using ELTE.Robotok.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public RefereeModeForm()
        {
            InitializeComponent();
            GenerateRefereeTable();
            GenerateRefereeAttachments();
        }

        #endregion

        #region Private methods

        private void GenerateRefereeTable()
        {
            // Játékvezetői tábla
            _buttonGrid = new Button[GameMenuForm.instance._model.Table.SizeX, GameMenuForm.instance._model.Table.SizeY];
            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Location = new Point(115 + 22 * j, 85 + 22 * i); // elhelyezkedés
                    _buttonGrid[i, j].Size = new Size(22, 22); // méret
                    _buttonGrid[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // betűtípus
                    _buttonGrid[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGrid[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus

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
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 2)
                    {
                        _buttonGrid[i, j].BackColor = Color.Red;
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
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 7)
                    {
                        _buttonGrid[i, j].BackColor = Color.White;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 8)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkGreen;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 9)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkRed;
                    }


                    Controls.Add(_buttonGrid[i, j]);
                    // felvesszük az ablakra a gombot
                }
            }
        }

        /*Panelek kigenerálására szolgáló függvény. A panelek célja, hogyha egy játékos csatlakozik egy kockához, jelenjen meg egy
        vastag vonal a kapcsolódott két kocka között. Azért kellett ezt az alternatív megoldást alkalmazni, mivel a Windows Forms
        nem támogatja egy gomb határainak egyenkénti módosítását. Az összes panelt kigenerálju előre, kapcsolódáskor csak a láthatóságát
        állítjuk át igazra.
        */
        private void GenerateRefereeAttachments()
        {

            _verticalPanels = new Panel[GameMenuForm.instance._model.Table.SizeX, GameMenuForm.instance._model.Table.SizeY];
            _horizontalPanels = new Panel[GameMenuForm.instance._model.Table.SizeX, GameMenuForm.instance._model.Table.SizeY];

            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
                    _verticalPanels[i, j] = new Panel();
                    _verticalPanels[i, j].Location = new Point(113 + 22 * j, 85 + 22 * i); // elhelyezkedés
                    _verticalPanels[i, j].Size = new Size(5, 22); // méret
                    _verticalPanels[i, j].BackColor = Color.Black; // debuggolás miatt piros, hogy a határoknál ne olvadjon bele a fekete színbe, később át lehet írni
                    _verticalPanels[i, j].Visible = false;

                    _horizontalPanels[i, j] = new Panel();
                    _horizontalPanels[i, j].Location = new Point(115 + 22 * j, 83 + 22 * i);
                    _horizontalPanels[i, j].Size = new Size(22, 5);
                    _horizontalPanels[i, j].BackColor = Color.Black;
                    _horizontalPanels[i, j].Visible = false;

                    Controls.Add(_verticalPanels[i, j]); // Felvesszük őket az ablakra
                    Controls.Add(_horizontalPanels[i, j]);

                    _verticalPanels[i, j].BringToFront(); // Előtérbe kell hozni a paneleket, hogy a pálya gombjai ne takarják el
                    _horizontalPanels[i, j].BringToFront();

                }
            }
        }
        #endregion

        #region Public methods
        public void RefreshRefereeView()
        {
            for (Int32 i = 0; i < GameMenuForm.instance._model.Table.SizeX; i++)
            {
                for (Int32 j = 0; j < GameMenuForm.instance._model.Table.SizeY; j++)
                {
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
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 2)
                    {
                        _buttonGrid[i, j].BackColor = Color.Red;
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
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 7)
                    {
                        _buttonGrid[i, j].BackColor = Color.White;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 8)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkGreen;
                    }
                    else if (GameMenuForm.instance._model.Table.GetFieldValue(i, j) == 9)
                    {
                        _buttonGrid[i, j].BackColor = Color.DarkRed;
                    }

                    // Kapcsolatok megjelenítésének frissítése
                    if (GameMenuForm.instance._model.Table.GetAttachmentEast(i, j))
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
        #endregion
    }
}