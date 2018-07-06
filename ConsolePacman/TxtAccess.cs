using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Interaction logic for TxtAccess.
    /// </summary>
    public class TxtAccess
    {
        #region Fields

        // The path of the txt file.
        private string txtPath;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the txtPath.
        /// </summary>
        /// <value>
        /// The txtPath.
        /// </value>
        public string TxtPath
        {
            get { return txtPath; }
            set { txtPath = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TxtAccess"/> class.
        /// </summary>
        /// <param name="path">The path of the xml file.</param>
        public TxtAccess(string path)
        {
            TxtPath = path;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <returns>True if the file exists, false if not.</returns>
        public bool FileExists()
        {
            try
            {
                // The file exists.
                if (File.Exists(TxtPath))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads the map.
        /// </summary>
        /// <param name="mapWidth">The width of the map.</param>
        /// <param name="mapHeight">The height of the map.</param>
        /// <returns>The map or null.</returns>
        public char[,] LoadMap(int mapWidth, int mapHeight)
        {
            try
            {
                // The file exists.
                if (FileExists())
                {
                    char[,] returnValue = new char[mapHeight, mapWidth];

                    // Open the connection.
                    var reader = new StreamReader(TxtPath);
                    string line;
                    int i = 0;

                    // Read a line.
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Read the characters from the line.
                        for (int j = 0; j < mapWidth; j++)
                        {
                            // Add the characters to the map.
                            returnValue[i, j] = line[j];
                        }

                        i++;
                    }

                    // Close the connection.
                    reader.Close();

                    return returnValue;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion Methods
    }
}
