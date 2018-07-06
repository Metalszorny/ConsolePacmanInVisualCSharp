using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsolePacman
{
    /// <summary>
    /// Interacion logic for XmlAccess.
    /// </summary>
    public class XmlAccess
    {
        #region Fields

        // The path of the xml file.
        private string xmlPath;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the xmlPath.
        /// </summary>
        /// <value>
        /// The xmlPath.
        /// </value>
        public string XmlPath
        {
            get { return xmlPath; }
            set { xmlPath = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlAccess"/> class.
        /// </summary>
        /// <param name="path">The path of the xml file.</param>
        public XmlAccess(string path)
        {
            XmlPath = path;

            if (!FileExists())
            {
                Create();
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates the file.
        /// </summary>
        private void Create()
        {
            try
            {
                XElement nameElement = new XElement("Name", "Test");
                XElement scoreElement = new XElement("Score", "0");
                XAttribute placeAttribute = new XAttribute("Place", 1);
                XElement newElements = new XElement("Highscore", placeAttribute, nameElement, scoreElement);
                XElement newPerson = new XElement("Highscores", newElements);
                XDocument newDocument = new XDocument(newPerson);
                newDocument.Save(XmlPath);
            }
            catch
            { }
        }

        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <returns>True if the file exists, false if not.</returns>
        private bool FileExists()
        {
            try
            {
                if (File.Exists(XmlPath))
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
        /// Lists the items from the highscores xml file.
        /// </summary>
        /// <returns>The list of highscore items or null.</returns>
        public List<Highscore> LoadScores()
        {
            try
            {
                // Open the highscores xml file.
                XDocument highscoresFromXml = XDocument.Load(XmlPath);
                var readDataFromXml = highscoresFromXml.Descendants("Highscore");

                // Get the items from the highscores xml file.
                List<Highscore> returnValue = new List<Highscore>();
                var fromXml = from x in readDataFromXml
                              select x;

                // Store the items in a Highscore list.
                foreach (var oneElement in fromXml)
                {
                    returnValue.Add(new Highscore((int)oneElement.Attribute("Place"), (string)oneElement.Element("Name"), (int)oneElement.Element("Score")));
                }

                // Sort the highscores in a descending order.
                returnValue.Sort((x, y) => y.Score.CompareTo(x.Score));

                return returnValue;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Saves the items to the highscores xml file.
        /// </summary>
        /// <param name="scores">The list of highscore items to save.</param>
        /// <returns>The outcome of the method.</returns>
        public bool SaveScores(List<Highscore> scores)
        {
            try
            {
                // The input is not null, there is at least one item and the file exists.
                if (scores != null && scores.Count > 0 && FileExists())
                {
                    // Sort the highscores in a descending order.
                    scores.Sort((x, y) => y.Score.CompareTo(x.Score));

                    // Open the highscores xml file.
                    XDocument highscoresFromXml = XDocument.Load(XmlPath);
                    var readDataFromXml = highscoresFromXml.Descendants("Highscore");

                    // Get the items from the highscores xml file.
                    var fromXml = from x in readDataFromXml
                                  select x;

                    // Update the existing values in the highscores xml file.
                    int i = 0;
                    foreach (var oneElement in fromXml)
                    {
                        oneElement.Element("Name").Value = scores[i].Name;
                        oneElement.Element("Score").Value = scores[i].Score.ToString();

                        i++;
                    }

                    // Create new items in the xml file if the input list is greater the number of records in the xml file.
                    if (scores.Count > i)
                    {
                        // Check each highscore.
                        for (int j = i; j < scores.Count; j++)
                        {
                            // Create the new highscore item.
                            XElement nameElement = new XElement("Name", scores[j].Name);
                            XElement scoreElement = new XElement("Score", scores[j].Score);
                            XAttribute placeAttribute = new XAttribute("Place", (j + 1).ToString());
                            XElement newElements = new XElement("Highscore", placeAttribute, nameElement, scoreElement);

                            // Add new item to the highscores xml file.
                            highscoresFromXml.Element("Highscores").Add(newElements);
                        }
                    }

                    // Update the values in the highscores xml file.
                    highscoresFromXml.Save(XmlPath);

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion Methods
    }
}
