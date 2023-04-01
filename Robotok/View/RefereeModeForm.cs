using ELTE.Robotok.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        #endregion

        #region Constructor
        public RefereeModeForm()
        {
            InitializeComponent();
            GenerateRefereeTable();
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
                    _buttonGrid[i, j].Location = new Point(115 + 19 * j, 85 + 19 * i); // elhelyezkedés
                    _buttonGrid[i, j].Size = new Size(19, 19); // méret
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
                }
            }
        }
        #endregion
    }
}