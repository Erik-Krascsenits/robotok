using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Robotok.Persistence
{
    /// <summary>
    /// Robotok játéktábla típusa.
    /// </summary>
    public class RobotokTable
    {
        // Field struktúra, elterálja a mezőről az értékét, a szükséges tisztítási műveletek számát (törhetetlen esetén -1), és a kapcsolatokat a különböző irányokba
        struct Field
        {
            public Int32 _fieldValue;
            public Int32 _remainingCleaningOperations;
            public bool _attachmentOnTop, _attachmentOnBottom, _attachmentOnLeft, _attachmentOnRight;
        }

        #region Fields

        private Field[,] _fields; // Field típusú mezők tömbje

        #endregion

        #region Properties

        /// <summary>
        /// Játéktábla szélességének lekérdezése.
        /// </summary>
        public Int32 SizeX { get { return _fields.GetLength(0); } }

        /// <summary>
        /// Játéktábla magasságának lekérdezése.
        /// </summary>
        public Int32 SizeY { get { return _fields.GetLength(1); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Robotok játéktábla példányosítása.
        /// </summary>
        /// <param name="tableSizeX">Játéktábla szélessége.</param>
        /// /// <param name="tableSizeY">Játéktábla magassága.</param>
        /// <param name="regionSize">Ház mérete.</param>
        public RobotokTable(Int32 tableSizeX, Int32 tableSizeY)
        {
            _fields = new Field[tableSizeX, tableSizeY];
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Mező értékének beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="value">Érték.</param>
        /// <param name="cleaningOperations"Tisztító műveletek száma.</param>
        public void SetValue(Int32 x, Int32 y, Int32 value, Int32 cleaningOperations)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._fieldValue = value;
            _fields[x, y]._remainingCleaningOperations = cleaningOperations;
        }

        /// <summary>
        /// Mező értékének lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public Int32 GetFieldValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._fieldValue;
        }

        #endregion

        #region Private methods

        #endregion
    }
}
