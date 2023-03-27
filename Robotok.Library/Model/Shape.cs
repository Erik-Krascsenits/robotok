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
        private Int32 x;
        private Int32 y;
        private Int32[,] _shape;
        private Int32[,] _cube = new Int32[2, 2] {
            {3, 3},
            {3, 3}
        };

        private Int32[,] _triangle = new Int32[2, 2] {
            {0, 4},
            {4, 4}
        };

        private Int32[,] _straight = new Int32[4, 4] {
            {0, 0, 0, 0},
            {6, 6, 6, 6},
            {0, 0, 0, 0},
            {0, 0, 0, 0}
        };

        private Int32[,] _lType = new Int32[4, 3] {
            {7, 0, 0},
            {7, 7, 7},
            {0, 0, 0},
            {0, 0, 0 }
        };

        private Int32[,] _rhombus = new Int32[3, 3] {
            {0, 3, 0},
            {3, 3, 3},
            {0, 3, 0},
        };

        private Int32[,] _piType = new Int32[4, 4] {
            {0, 0, 0, 0},
            {4, 4, 4, 4},
            {4, 0, 0, 4},
            {0, 0, 0, 0}
        };

        #endregion

        #region Properties

        public Int32 X { get { return x; } }
        public Int32 Y { get { return y; } }
        public Int32[,] Figure { get { return _shape; } }

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
            switch (temp.Next(1, 7))
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
        /// <summary>
        /// Alakzat eltűntetése.
        /// </summary>
        private void Destroy()
        {
            for (Int32 i = 0; i < x; ++i)
            {
                for (Int32 j = 0; j < y; ++j)
                {
                    _shape[x, y] = 8;
                }
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
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= this.y)
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _shape[x, y];
        }
        #endregion
    }
}
