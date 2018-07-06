using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Base class for Highscore.
    /// </summary>
    public class Highscore
    {
        #region Fields

        // The place field of the Highscore class.
        private int place;

        // The name field of the Highscore class.
        private string name;

        // The score field of the Highscore class.
        private int score;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>
        /// The place.
        /// </value>
        public int Place
        {
            get { return place; }
            set { place = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Highscore"/> class.
        /// </summary>
        public Highscore()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Highscore"/> class.
        /// </summary>
        /// <param name="place">The input value for the place field.</param>
        /// <param name="name">The input value for the name field.</param>
        /// <param name="score">The input value for the score field.</param>
        public Highscore(int place, string name, int score)
        {
            Place = place;
            Name = name;
            Score = score;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Returns the name and the score.
        /// </summary>
        /// <returns>The overriden tostring.</returns>
        public override string ToString()
        {
            try
            {
                return string.Format("Place: {0}, Name: {1}, Score: {2}", Place, Name, Score);
            }
            catch
            {
                return null;
            }
        }

        #endregion Methods
    }
}
