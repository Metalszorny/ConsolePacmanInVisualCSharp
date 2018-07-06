using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Base class for Position.
    /// </summary>
    public struct Position
    {
        #region Fields

        // The x field of the Position class.
        private int x;

        // The y field of the Position class.
        private int y;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="x">The input value for the x field.</param>
        /// <param name="y">The input value for the y field.</param>
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion Constructors

        #region Methods

        #endregion Methods
    }
}
