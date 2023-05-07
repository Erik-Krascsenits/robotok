using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotok.Library.Model
{
    public class Shape
    {
        #region Fields
        private Int32 x; // a táblán vízszintes elhelyezkedése
        private Int32 y; // a táblán függőleges elhelyezkedése
        private Int32[,] _shape = null!; // maga az alakzat
        private Int32[,] _cube = new Int32[4, 4] { // egyszerűség kedvéért, most minden alakzat 4x4 méretű mátrixban lesz, de jövőben ez megváltoztathatjuk
            {-2, -2, -2, -2},
            {-2, -2, -2, -2},
            {-2, 3, 3, -2},
            {-2, 3, 3, -2}
        };

        private Int32[,] _triangle = new Int32[4, 4] {
            {-2, -2, -2, -2},
            {-2, -2, -2, -2},
            {-2, -2, 4, -2},
            {-2, 4, 4, -2}
        };

        private Int32[,] _straight = new Int32[4, 4] {
            {-2, -2, -2, -2},
            {-2, -2, -2, -2},
            {-2, -2, -2, -2},
            {5, 5, 5, 5}
        };

        private Int32[,] _lType = new Int32[4, 4] {
            {-2, -2, -2, -2},
            {-2, -2, -2, -2},
            {-2, 6, -2, -2},
            {-2, 6, 6, 6}
        };

        private Int32[,] _rhombus = new Int32[4, 4] {
            {-2, -2, -2, -2},
            {-2, -2, 11, -2},
            {-2, 11, 11, 11},
            {-2, -2, 11, -2}
        };

        private Int32[,] _piType = new Int32[4, 4] {
            {-2, -2, -2, -2},
            {-2, -2, -2, -2},
            {12, 12, 12, 12},
            {12, -2, -2, 12}
        };

        #endregion

        #region Properties

        public Int32 X { get { return x; } }
        public Int32 Y { get { return y; } }
        public Int32[,] Figure { get { return _shape; } }
        public Int32[,] Cube { get { return _cube; } }
        public Int32[,] Triangle { get { return _triangle; } }
        public Int32[,] Straight { get { return _straight; } }
        public Int32[,] lType { get { return _lType; } }
        public Int32[,] Rhombus { get { return _rhombus; } }
        public Int32[,] PiType { get { return _piType; } }



        #endregion

        #region Constructor
        public Shape() 
        {
            GenerateShape();
            x = _shape.GetLength(0);
            y = _shape.GetLength(1);
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Új alakzat létrehozása.
        /// </summary>
        private void GenerateShape()
        {
            Random temp = new Random();
            switch (temp.Next(1, 6))
            {
                case 1:
                    _shape = _cube;
                    break;
                case 2:
                    _shape = _triangle;
                    break;
                case 3:
                    _shape = _straight;
                    break;
                case 4:
                    _shape = _lType;
                    break;
                case 5:
                    _shape = _rhombus;
                    break;
                case 6:
                    _shape = _piType;
                    break;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Mező értékének lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public Int32 GetFieldValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= this.x)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range: " + x.ToString());
            }
            if (y < 0 || y >= this.y)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range: " + y.ToString());
            }

            return _shape[x, y];
        }
        /// <summary>
        /// Mező értékének beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="value">Érték.</param>
        public void SetValue(Int32 x, Int32 y, Int32 value)
        {
            if (x < 0 || x >= _shape.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _shape.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _shape[x, y] = value;
        }

        /// <summary>
        /// Szín lekérdezése.
        /// </summary>
        public int GetColor()
        {
            if (this._shape == _cube)
            {
                return 3;
            }

            if (this._shape == _triangle)
            {
                return 4;
            }

            if (this._shape == _straight)
            {
                return 5;
            }

            if (this._shape == _lType)
            {
                return 6;
            }

            if (this._shape == _rhombus)
            {
                return 11;
            }

            return 12;

        }
        #endregion
    }
}
