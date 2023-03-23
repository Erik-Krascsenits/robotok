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
        struct Field
        {
            private Int32 _fieldValue;
            private Int32 _remainingCleaningOperations;
            bool _attachmentOnTop, _attachmentOnBottom, _attachmentOnLeft, _attachmentOnRight;
        }
        #region Fields

        private Field[,] _fields; // mezők

        #endregion

        #region Properties

        /// <summary>
        /// Játéktábla szélességének lekérdezése.
        /// </summary>
        public Int32 SizeX { get { return _fields.GetLength(1); } }

        /// <summary>
        /// Játéktábla magasságának lekérdezése.
        /// </summary>
        public Int32 SizeY { get { return _fields.GetLength(0); } }

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
            _fields= new Field[tableSizeX, tableSizeY];
        }

        #endregion

        #region Public methods

        #endregion

        #region Private methods

        #endregion
    }
}
