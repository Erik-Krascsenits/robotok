namespace ELTE.Robotok.Model
{
    /// <summary>
    /// Játékban szereplő alakzatok típusa
    /// </summary>
    public class Shape
    {
        #region Fields

        private Int32[,] _shape = null!; // maga az alakzat
        private Int32[,] _cube = new Int32[3, 3] { // kocka típusú alakzat
            {-2, -2, -2},
            {-2, 3, 3},
            {-2, 3, 3},
        };
        private Int32[,] _triangle = new Int32[3, 3] { // háromszög típusú alakzat
            {-2, -2, -2},
            {-2, -2, 4},
            {-2, 4, 4},
        };
        private Int32[,] _straight = new Int32[3, 3] { // egyenes típusú alakzat
            {-2, -2, -2},
            {-2, -2, -2},
            {5, 5, 5}
        };
        private Int32[,] _lType = new Int32[3, 3] { // l típusú alakzat
            { -2, -2, -2},
            { 6, -2, -2},
            { 6, 6, 6}
        };
        private Int32[,] _rhombus = new Int32[3, 3] { // rombusz típusú alakzat
            { -2, 11, -2},
            { 11, 11, 11},
            { -2, 11, -2}
        };
        private Int32[,] _bridge = new Int32[3, 3] { // híd típusú alakzat
            {-2, -2, -2},
            {12, 12, 12},
            {12, -2, 12}
        };

        #endregion

        #region Properties

        /// <summary>
        /// Alakzat lekérdezése
        /// </summary>
        public Int32[,] Figure { get { return _shape; } }

        /// <summary>
        /// Kocka típusú alakzat lekérdezése
        /// </summary>
        public Int32[,] Cube { get { return _cube; } }

        /// <summary>
        /// Háromszög típusú alakzat lekérdezése
        /// </summary>
        public Int32[,] Triangle { get { return _triangle; } }

        /// <summary>
        /// Egyenes típusú alakzat lekérdezése
        /// </summary>
        public Int32[,] Straight { get { return _straight; } }

        /// <summary>
        /// Háromszög típusú alakzat lekérdezése
        /// </summary>
        public Int32[,] lType { get { return _lType; } }

        /// <summary>
        /// Rhombusz típusú alakzat lekérdezése
        /// </summary>
        public Int32[,] Rhombus { get { return _rhombus; } }

        /// <summary>
        /// Híd típusú alakzat lekérdezése
        /// </summary>
        public Int32[,] Bridge { get { return _bridge; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Véletlen alakzatot létrehozó konstruktor
        /// </summary>
        public Shape() 
        {
            GenerateShape();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Új alakzat létrehozása
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
                    _shape = _bridge;
                    break;
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Mező értékének lekérdezése
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke.</returns>
        public Int32 GetFieldValue(Int32 x, Int32 y)
        {
            return _shape[x, y];
        }

        /// <summary>
        /// Alakzat színének a lekérdezése
        /// </summary>
        public Int32 GetColor()
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
