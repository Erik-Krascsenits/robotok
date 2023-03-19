using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELTE.Robotok.Persistence
{
    /// <summary>
    /// Robotok adatelérés kivétel típusa.
    /// </summary>
    public class RobotokDataException : Exception
    {
        /// <summary>
        /// Robotok adatelérés kivétel példányosítása.
        /// </summary>
        public RobotokDataException() { }
    }
}
