using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Robotok.Persistence
{
    /// <summary>
    /// Robotok fájl kezelő felülete.
    /// </summary>
    public interface IRobotokDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játéktábla.</returns>
        Task<RobotokTable> LoadAsync(String path);

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        Task SaveAsync(String path, RobotokTable table);
    }
}