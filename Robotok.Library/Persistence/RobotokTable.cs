namespace ELTE.Robotok.Persistence
{
    /// <summary>
    /// Robotok játéktáblájának típusa
    /// </summary>
    public class RobotokTable
    {
        /// <summary>
        /// Mezőknek az értékeit, tisztitási műveletek számát, illetve a kapcsolatokat tároló struktúra
        /// </summary>
        struct Field
        {
            public Int32 _fieldValue;
            public Int32 _remainingCleaningOperations;
            public Boolean _attachmentNorth, _attachmentSouth, _attachmentEast, _attachmentWest;
            public Boolean _faceNorth, _faceSouth, _faceEast, _faceWest;
            public Boolean _inDistance;
        }

        #region Fields

        private Field[,] _fields;

        #endregion

        #region Properties

        /// <summary>
        /// Játéktábla szélességének lekérdezése
        /// </summary>
        public Int32 SizeX { get { return _fields.GetLength(0); } }

        /// <summary>
        /// Játéktábla magasságának lekérdezése
        /// </summary>
        public Int32 SizeY { get { return _fields.GetLength(1); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Robotok játéktábla példányosítása
        /// </summary>
        /// <param name="tableSizeX">Játéktábla szélessége.</param>
        /// <param name="tableSizeY">Játéktábla magassága.</param>
        public RobotokTable(Int32 tableSizeX, Int32 tableSizeY)
        {
            _fields = new Field[tableSizeX, tableSizeY];
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Mező értékének beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="value">Érték</param>
        /// <param name="cleaningOperations">Tisztító műveletek száma</param>
        public void SetValue(Int32 x, Int32 y, Int32 value, Int32 cleaningOperations)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range: " + x.ToString());
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range: " + y.ToString());
            }

            _fields[x, y]._fieldValue = value;
            _fields[x, y]._remainingCleaningOperations = cleaningOperations;

            if ((value == 1 || value == 2 || value == 8 || value == 9) && (!_fields[x, y]._faceNorth && !_fields[x, y]._faceSouth && !_fields[x, y]._faceEast && !_fields[x, y]._faceWest))
            {
                SetFaceSouth(x, y);
            }
        }

        /// <summary>
        /// Mező csatolási értékeinek beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="north">Érték</param>
        /// /// <param name="south">Érték</param>
        /// /// <param name="east">Érték</param>
        /// /// <param name="west">Érték</param>
        public void SetAttachmentValues(Int32 x, Int32 y, Boolean north, Boolean south, Boolean east, Boolean west)
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
        /// Mező északi csatolási értékének beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="north">Érték</param>
        public void SetAttachmentNorth(Int32 x, Int32 y, Boolean north)
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
        /// Mező déli csatolási értékének beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="south">Érték</param>
        public void SetAttachmentSouth(Int32 x, Int32 y, Boolean south)
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
        /// Mező keleti csatolási értékének beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="east">Érték</param>
        public void SetAttachmentEast(Int32 x, Int32 y, Boolean east)
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
        /// Mező nyugati csatolási értékének beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="west">Érték</param>
        public void SetAttachmentWest(Int32 x, Int32 y, Boolean west)
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
        /// Mező értékének lekérdezése
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
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
        /// Tisztításhoz szükséges lépések száma
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>Az adott kockának hátralévő tisztítási lépések száma</returns>
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
        /// Visszaadja, hogy a mezőhöz van-e csatolva valami
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns> A mező értéke </returns>
        public Boolean HasAttachments(Int32 x, Int32 y)
        {
            if(!_fields[x, y]._attachmentNorth && !_fields[x, y]._attachmentSouth && !_fields[x, y]._attachmentEast && !_fields[x, y]._attachmentWest)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Visszaadja egy mező északi csatolási részéről, hogy van-e valami hozzá csatolva
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező érték</returns>
        public Boolean GetAttachmentNorth(Int32 x, Int32 y)
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetAttachmentSouth(Int32 x, Int32 y)
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetAttachmentEast(Int32 x, Int32 y)
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetAttachmentWest(Int32 x, Int32 y)
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
        /// Északra nézés beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
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
        /// Délre nézés beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
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
        /// Keletre nézés beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
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
        /// Nyugatra nézés beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetFaceNorth(Int32 x, Int32 y)
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetFaceSouth(Int32 x, Int32 y)
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetFaceEast(Int32 x, Int32 y)
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
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <returns>A mező értéke</returns>
        public Boolean GetFaceWest(Int32 x, Int32 y)
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
        /// Mező irányának beállítása
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="north">Érték</param>
        /// <param name="south">Érték</param>
        /// <param name="east">Érték</param>
        /// <param name="west">Érték</param>
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

        /// <summary>
        /// Beállítja hogy Manhattan távolságban van a mező a játékoshoz képest
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        /// <param name="inDistance">Benne van-e a Manhattan távolságban a kocka</param>
        public void SetInDistance(Int32 x, Int32 y, Boolean inDistance)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            _fields[x, y]._inDistance = inDistance;
        }

        /// <summary>
        /// Lekérdezzük egy kockáról hogy Manhattan távolságban van-e
        /// </summary>
        /// <param name="x">Vízszintes koordináta</param>
        /// <param name="y">Függőleges koordináta</param>
        public Boolean GetInDistance(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fields.GetLength(0))
            {
                throw new ArgumentOutOfRangeException(nameof(x), "The X coordinate is out of range.");
            }
            if (y < 0 || y >= _fields.GetLength(1))
            {
                throw new ArgumentOutOfRangeException(nameof(y), "The Y coordinate is out of range.");
            }

            return _fields[x, y]._inDistance;
        }

        #endregion
    }
}
