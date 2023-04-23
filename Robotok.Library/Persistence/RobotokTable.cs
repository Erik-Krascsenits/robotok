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
        // Field struktúra, eltárolja a mezőről az értékét, a szükséges tisztítási műveletek számát (törhetetlen esetén -1), és a kapcsolatokat a különböző irányokba
        struct Field
        {
            public Int32 _fieldValue;
            public Int32 _remainingCleaningOperations;
            public bool _attachmentNorth, _attachmentSouth, _attachmentEast, _attachmentWest;
            public bool _faceNorth, _faceSouth, _faceEast, _faceWest;
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

            if ((value == 1 || value == 2 || value == 8 || value == 9) && (!_fields[x, y]._faceNorth && !_fields[x, y]._faceSouth && !_fields[x, y]._faceEast && !_fields[x, y]._faceWest))
            {
                SetFaceSouth(x, y);

            }
        }

        /// <summary>
        /// Mező csatolási értékeinek beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="north">Érték.</param>
        /// /// <param name="south">Érték.</param>
        /// /// <param name="east">Érték.</param>
        /// /// <param name="west">Érték.</param>
        public void SetAttachmentValues(Int32 x, Int32 y, bool north, bool south, bool east, bool west)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._attachmentNorth = north;
            _fields[x, y]._attachmentSouth = south;
            _fields[x, y]._attachmentEast = east;
            _fields[x, y]._attachmentWest = west;
        }

        /// <summary>
        /// Mező északi csatolási értékének beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="north">Érték.</param>
        public void SetAttachmentNorth(Int32 x, Int32 y, bool north)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._attachmentNorth = north;
        }

        /// <summary>
        /// Mező déli csatolási értékének beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="south">Érték.</param>
        public void SetAttachmentSouth(Int32 x, Int32 y, bool south)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._attachmentSouth = south;
        }

        /// <summary>
        /// Mező keleti csatolási értékének beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="east">Érték.</param>
        public void SetAttachmentEast(Int32 x, Int32 y, bool east)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._attachmentEast = east;
        }

        /// <summary>
        /// Mező nyugati csatolási értékének beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="west">Érték.</param>
        public void SetAttachmentWest(Int32 x, Int32 y, bool west)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._attachmentWest = west;
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
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range: " + x);
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range: " + y);
            }

            return _fields[x, y]._fieldValue;
        }

        /// <summary>
        /// Tisztításhoz szükséges lépésel száma.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public Int32 GetFieldRemainingCleaningOperations(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._remainingCleaningOperations;
        }

        /// <summary>
        /// Visszaadja egy mező északi csatolási részéről, hogy van-e valami hozzá csatolva
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetAttachmentNorth(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._attachmentNorth;
        }

        /// <summary>
        /// Visszaadja egy mező déli csatolási részéről, hogy van-e valami hozzá csatolva
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetAttachmentSouth(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._attachmentSouth;
        }

        /// <summary>
        /// Visszaadja egy mező keleti csatolási részéről, hogy van-e valami hozzá csatolva
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetAttachmentEast(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._attachmentEast;
        }

        /// <summary>
        /// Visszaadja egy mező keleti csatolási részéről, hogy van-e valami hozzá csatolva
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetAttachmentWest(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._attachmentWest;
        }

        /// <summary>
        /// Északra nézés beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        public void SetFaceNorth(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._faceNorth = true;
            _fields[x, y]._faceSouth = false;
            _fields[x, y]._faceEast = false;
            _fields[x, y]._faceWest = false;
        }

        /// <summary>
        /// Délre nézés beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        public void SetFaceSouth(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._faceNorth = false;
            _fields[x, y]._faceSouth = true;
            _fields[x, y]._faceEast = false;
            _fields[x, y]._faceWest = false;
            
        }

        /// <summary>
        /// Keletre nézés beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        public void SetFaceEast(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._faceNorth = false;
            _fields[x, y]._faceSouth = false;
            _fields[x, y]._faceEast = true;
            _fields[x, y]._faceWest = false;
        }

        /// <summary>
        /// Nyugatra nézés beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        public void SetFaceWest(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._faceNorth = false;
            _fields[x, y]._faceSouth = false;
            _fields[x, y]._faceEast = false;
            _fields[x, y]._faceWest = true;
        }

        /// <summary>
        /// Visszaadja, hogy északra néz-e a játékos (bármilyen más mező esetén false)
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetFaceNorth(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._faceNorth;
        }

        /// <summary>
        /// Visszaadja, hogy délre néz-e a játékos (bármilyen más mező esetén false)
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetFaceSouth(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._faceSouth;
        }

        /// <summary>
        /// Visszaadja, hogy keletre néz-e a játékos (bármilyen más mező esetén false)
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetFaceEast(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._faceEast;
        }

        /// <summary>
        /// Visszaadja, hogy nyugatra néz-e a játékos (bármilyen más mező esetén false)
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public bool GetFaceWest(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._faceWest;
        }

        /// <summary>
        /// Mező irányának beállítása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <param name="north">Érték.</param>
        /// <param name="south">Érték.</param>
        /// <param name="east">Érték.</param>
        /// <param name="west">Érték.</param>
        public void SetFaceDirection(Int32 x, Int32 y, Boolean north, Boolean south, Boolean east, Boolean west)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._faceNorth = north;
            _fields[x, y]._faceSouth = south;
            _fields[x, y]._faceEast = east;
            _fields[x, y]._faceWest = west;
        }

        #endregion

        #region Private methods

        #endregion
    }
}
