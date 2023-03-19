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
        #region Fields

        private Int32[,] _fieldValues; // mezőértékek

        #endregion

        #region Properties

        /// <summary>
        /// Játéktábla szélességének lekérdezése.
        /// </summary>
        public Int32 SizeX { get { return _fieldValues.GetLength(1); } }

        /// <summary>
        /// Játéktábla magasságának lekérdezése.
        /// </summary>
        public Int32 SizeY { get { return _fieldValues.GetLength(0); } }

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
            _fieldValues = new Int32[tableSizeX, tableSizeY];
        }

        #endregion

        #region Public methods

        #endregion

        #region Private methods

        #endregion
    }
}
