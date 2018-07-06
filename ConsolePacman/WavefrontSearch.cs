using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Interaction logic for WavefrontSearch.
    /// </summary>
    public class WavefrontSearch
    {
        #region Fields

        // The copy of the map.
        private int[,] copyMap;

        // The original map.
        private char[,] originalMap;

        // The width of tha map.
        private int mapWidth;

        // The height of the map.
        private int mapHeight;

        // The value of the wavefront.
        private int wavefrontValue;

        // The position of the currently search point.
        private Position currentPosition;

        // The position of the pacman.
        private Position pacmanPosition;

        // The position of the ghost.
        private Position ghostPosition;

        // The value of the empty items.
        private int emptyValue;

        // The value of the obstacle items.
        private int obstacleValue;

        // The value of the pacman item.
        private int pacmanValue;

        // The value of the ghost item.
        private int ghostValue;

        // The count of cells.
        private int cellCount;

        // The zone where the ghost is.
        private int ghostFleeZone;

        // The zone where the pacman is.
        private int pacmanFleeZone;

        // The position of the escape point.
        private Position escapePoint;

        // The value of the escape point item.
        private int escapeValue;

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WavefrontSearch"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapWidth">The mapWidth.</param>
        /// <param name="mapHeight">The mapHeight.</param>
        /// <param name="pacman">The pacman.</param>
        /// <param name="ghost">The ghost.</param>
        public WavefrontSearch(char[,] map, int mapWidth, int mapHeight, Pacman pacman, Ghost ghost)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            originalMap = new char[mapHeight, mapWidth];
            originalMap = map;
            copyMap = new int[mapHeight, mapWidth];
            pacmanPosition = pacman.Position;
            ghostPosition = ghost.Position;
            emptyValue = -1;
            obstacleValue = -9;
            pacmanValue = -8;
            ghostValue = -7;
            escapeValue = -6;
            cellCount = 0;
            wavefrontValue = 1;
        }

        #endregion Constructors

        #region Methods

        #region Hunt

        /// <summary>
        /// Makes the ghost go closer to the pacman.
        /// </summary>
        /// <returns>The next position.</returns>
        public Position Hunt()
        {
            try
            {
                return HuntSearch();
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        /// <summary>
        /// Searches from the pacman to the ghost for the way between them.
        /// </summary>
        /// <returns>The next position.</returns>
        private Position HuntSearch()
        {
            try
            {
                // The wavefront reached the ghost (start position).
                bool reached = false;

                // Set the search position to the pacman (finish position).
                currentPosition = pacmanPosition;

                // The start and finish positions are valid.
                if ((pacmanPosition.X >= 0) && (pacmanPosition.Y >= 0) && (ghostPosition.X >= 0) && (ghostPosition.Y >= 0))
                {
                    #region FillCopyMap

                    // Add the start and the finish values.
                    copyMap[pacmanPosition.X, pacmanPosition.Y] = pacmanValue;
                    copyMap[ghostPosition.X, ghostPosition.Y] = ghostValue;

                    // Check each row of the map.
                    for (int i = 0; i < mapHeight; i++)
                    {
                        // Check each column of the map.
                        for (int j = 0; j < mapWidth; j++)
                        {
                            // The cell is a obstacle item.
                            if (originalMap[i, j] == '#' || originalMap[i, j] == '-')
                            {
                                // Add the obstacle values.
                                copyMap[i, j] = obstacleValue;
                            }
                            // The cell is a space item.
                            else if (originalMap[i, j] != 'O' && (originalMap[i, j] != 'n' && i != ghostPosition.X && j != ghostPosition.Y))
                            {
                                // Add the space values.
                                copyMap[i, j] = emptyValue;
                            }
                        }
                    }

                    #endregion FillCopyMap

                    cellCount = 0;

                    #region SearchForTheWay

                    // Make waves from the pacman (finish) to the ghost (start) and check if the way between them exists.
                    do
                    {
                        // Make a wave and check how many cells were modified.
                        cellCount = HuntWave();

                        // The rows around the ghost.
                        for (int i = Math.Max(0, ghostPosition.X - 1); i < Math.Min(ghostPosition.X + 1, mapHeight - 1); i++)
                        {
                            // The columns around the ghost.
                            for (int j = Math.Max(0, ghostPosition.Y - 1); j < Math.Min(ghostPosition.Y + 1, mapWidth - 1); j++)
                            {
                                // The wavefront reached the ghost.
                                if (copyMap[i, j] > 0 && 
                                    ((i == ghostPosition.X - 1 && j == ghostPosition.Y) || /* Left */
                                    (i == ghostPosition.X + 1 && j == ghostPosition.Y) || /* Right */
                                    (i == ghostPosition.X && j == ghostPosition.Y - 1) || /* Up */
                                    (i == ghostPosition.X && j == ghostPosition.Y + 1))) /* Down */
                                {
                                    reached = true;
                                    break;
                                }
                            }

                            // Exit the loop if the way is found.
                            if (reached)
                            {
                                break;
                            }
                        }
                    } while (!reached && cellCount > 0);

                    #endregion SearchForTheWay

                    // The way couldn't be found.
                    if (!reached)
                    {
                        return new Position(-1, -1);
                    }
                    // The way was found.
                    else
                    {
                        // Take a step closer to the pacman.
                        return HuntStep();
                    }
                }
                // The start and finish positions are not valid.
                else
                {
                    return new Position(-1, -1);
                }
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        /// <summary>
        /// Search for the minimum wavefront value arond the ghost, this will be the ghost's next position.
        /// </summary>
        /// <returns>The next position.</returns>
        private Position HuntStep()
        {
            try
            {
                Position nextPosition = new Position(-1, -1);
                int currentMinValue = 100000;

                // The rows around the ghost.
                for (int i = 0; i < 3; i++)
                {
                    // The columns around the ghost.
                    for (int j = 0; j < 3; j++)
                    {
                        // The cell around the ghost is a wavefront value and it's smaller than the current minimum value.
                        if (currentMinValue > copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] &&
                            copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] != obstacleValue &&
                            copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] != emptyValue &&
                            copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] != ghostValue && 
                            ((i == ghostPosition.X - 1 && j == ghostPosition.Y) || /* Left */
                            (i == ghostPosition.X + 1 && j == ghostPosition.Y) || /* Right */
                            (i == ghostPosition.X && j == ghostPosition.Y - 1) || /* Up */
                            (i == ghostPosition.X && j == ghostPosition.Y + 1))) /* Down */
                        {
                            // Make the current minimum value the found cell value.
                            currentMinValue = copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j];
                        }
                    }
                }

                return nextPosition;
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        /// <summary>
        /// Fills the map with the wavefront values until the ghost (start position) is reached.
        /// </summary>
        /// <returns>The count of the modified cell.</returns>
        private int HuntWave()
        {
            try
            {
                int cell = 0;

                // The first modification coordinate is on the pacman (finish): the wave has not been started yet.
                if ((currentPosition.X == pacmanPosition.X) && (currentPosition.Y == pacmanPosition.Y))
                {
                    // Start the wave.
                    return HuntFirstWave();
                }
                // The wave has been started.
                else
                {
                    // Continue the wave.
                    cell = 0;
                    wavefrontValue++;

                    // The rows of the map.
                    for (int i = 0; i < mapHeight; i++)
                    {
                        // The columns of the map.
                        for (int j = 0; j < mapWidth; j++)
                        {
                            // Searches for an egsisting wavefront value to continue the wavefront.
                            if (copyMap[i, j] != emptyValue && copyMap[i, j] != obstacleValue && copyMap[i, j] != pacmanValue && copyMap[i, j] == wavefrontValue - 1)
                            {
                                // Sets the found wavefront value's position to a position to search around it.
                                currentPosition = new Position(i, j);

                                // The rows around the found wavefront value.
                                for (int k = 0; k < 3; k++)
                                {
                                    // The columns around the found wavefront value.
                                    for (int l = 0; l < 3; l++)
                                    {
                                        // The cell is an empty value.
                                        if (copyMap[currentPosition.X - 1 + l, currentPosition.Y - 1 + k] == emptyValue)
                                        {
                                            // The wavefront value can be written in it.
                                            cell++;
                                            copyMap[currentPosition.X - 1 + l, currentPosition.Y - 1 + k] = wavefrontValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return cell;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Makes the first wave around the pacman.
        /// </summary>
        /// <returns>The count of the modified cell.</returns>
        private int HuntFirstWave()
        {
            try
            {
                // Goes around the pacman (finish position) and writes the first wavefront values around it in the empty cells.
                int cell = 0;

                // The rows around the pacman.
                for (int i = 0; i < 3; i++)
                {
                    // The columns around the pacman.
                    for (int j = 0; j < 3; j++)
                    {
                        // The cell is an empty value.
                        if (copyMap[pacmanPosition.X - 1 + i, pacmanPosition.Y - 1 + j] == emptyValue)
                        {
                            // Add the wavefront value.
                            copyMap[pacmanPosition.X - 1 + i, pacmanPosition.Y - 1 + j] = wavefrontValue;
                            cell++;

                            // Sets the search position from the pacman.
                            currentPosition = new Position(0, 0);
                        }
                    }
                }

                wavefrontValue = 1;

                return cell;
            }
            catch
            {
                return 0;
            }
        }

        #endregion Hunt

        #region Flee

        /// <summary>
        /// Makes the ghost go further from the pacman.
        /// </summary>
        /// <returns>The new position.</returns>
        public Position Flee()
        {
            try
            {
                ghostFleeZone = 0;
                pacmanFleeZone = 0;
                escapePoint.X = -1;
                escapePoint.Y = -1;

                #region SetZones

                if (ghostPosition.X <= 1 * mapHeight / 3)
                {
                    if (ghostPosition.Y <= 1 * mapWidth / 3)
                    {
                        ghostFleeZone = 1;
                    }
                    else if (ghostPosition.Y >= 1 * mapWidth / 3 && ghostPosition.Y <= 2 * mapWidth / 3)
                    {
                        ghostFleeZone = 2;
                    }
                    else if (ghostPosition.Y >= 2 * mapWidth / 3)
                    {
                        ghostFleeZone = 3;
                    }
                }
                else if (ghostPosition.X >= 1 * mapHeight / 3 && ghostPosition.X <= 2 * mapHeight / 3)
                {
                    if (ghostPosition.Y <= 1 * mapWidth / 3)
                    {
                        ghostFleeZone = 4;
                    }
                    else if (ghostPosition.Y >= 1 * mapWidth / 3 && ghostPosition.Y <= 2 * mapWidth / 3)
                    {
                        ghostFleeZone = 5;
                    }
                    else if (ghostPosition.Y >= 2 * mapWidth / 3)
                    {
                        ghostFleeZone = 6;
                    }
                }
                else if (ghostPosition.X >= 2 * mapHeight / 3)
                {
                    if (ghostPosition.Y <= 1 * mapWidth / 3)
                    {
                        ghostFleeZone = 7;
                    }
                    else if (ghostPosition.Y >= 1 * mapWidth / 3 && ghostPosition.Y <= 2 * mapWidth / 3)
                    {
                        ghostFleeZone = 8;
                    }
                    else if (ghostPosition.Y >= 2 * mapWidth / 3)
                    {
                        ghostFleeZone = 9;
                    }
                }

                if (pacmanPosition.X <= 1 * mapHeight / 3)
                {
                    if (pacmanPosition.Y <= 1 * mapWidth / 3)
                    {
                        pacmanFleeZone = 1;
                    }
                    else if (pacmanPosition.Y >= 1 * mapWidth / 3 && pacmanPosition.Y <= 2 * mapWidth / 3)
                    {
                        pacmanFleeZone = 2;
                    }
                    else if (pacmanPosition.Y >= 2 * mapWidth / 3)
                    {
                        pacmanFleeZone = 3;
                    }
                }
                else if (pacmanPosition.X >= 1 * mapHeight / 3 && pacmanPosition.X <= 2 * mapHeight / 3)
                {
                    if (pacmanPosition.Y <= 1 * mapWidth / 3)
                    {
                        pacmanFleeZone = 4;
                    }
                    else if (pacmanPosition.Y >= 1 * mapWidth / 3 && pacmanPosition.Y <= 2 * mapWidth / 3)
                    {
                        pacmanFleeZone = 5;
                    }
                    else if (pacmanPosition.Y >= 2 * mapWidth / 3)
                    {
                        pacmanFleeZone = 6;
                    }
                }
                else if (pacmanPosition.X >= 2 * mapHeight / 3)
                {
                    if (pacmanPosition.Y <= 1 * mapWidth / 3)
                    {
                        pacmanFleeZone = 7;
                    }
                    else if (pacmanPosition.Y >= 1 * mapWidth / 3 && pacmanPosition.Y <= 2 * mapWidth / 3)
                    {
                        pacmanFleeZone = 8;
                    }
                    else if (pacmanPosition.Y >= 2 * mapWidth / 3)
                    {
                        pacmanFleeZone = 9;
                    }
                }

                #endregion SetZones

                #region SetEscapePoint

                switch (pacmanFleeZone)
                {
                    #region TopLeft

                    case 1:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 2:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 3:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 4:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 5:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 6:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 7:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 8:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 9:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                        }
                        break;

                    #endregion TopLeft

                    #region TopCenter

                    case 2:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 2:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 3:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 4:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 5:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 6:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 7:
                                if (ghostPosition.X != 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 8:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 9:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                        }
                        break;

                    #endregion TopCenter

                    #region TopRight

                    case 3:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 2:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 3:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 4:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 5:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 6:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 7:
                                if (ghostPosition.X != 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 8:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 9:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                        }
                        break;

                    #endregion TopRight

                    #region MiddleLeft

                    case 4:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 2:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 3:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 4:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 5:
                                if (pacmanPosition.Y >= ghostPosition.Y)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.Y <= ghostPosition.Y)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 6:
                                if (pacmanPosition.Y >= ghostPosition.Y)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.Y <= ghostPosition.Y)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 7:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 8:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 9:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                        }
                        break;

                    #endregion MiddleLeft

                    #region MiddleCenter

                    case 5:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                if (ghostPosition.X != 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 2:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                break;
                            case 3:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 4:
                                if (pacmanPosition.Y >= ghostPosition.Y)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.Y <= ghostPosition.Y)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 5:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 6:
                                if (pacmanPosition.Y >= ghostPosition.Y)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.Y <= ghostPosition.Y)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 7:
                                if (ghostPosition.X != 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 8:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 9:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                        }
                        break;

                    #endregion MiddleCenter

                    #region MiddleRight

                    case 6:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                if (ghostPosition.X != 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 2:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 3:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 4:
                                if (pacmanPosition.Y >= ghostPosition.Y)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.Y <= ghostPosition.Y)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 5:
                                if (pacmanPosition.Y >= ghostPosition.Y)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.Y <= ghostPosition.Y)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                break;
                            case 6:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 7:
                                if (ghostPosition.X != 2 && ghostPosition.Y != mapWidth - 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = mapWidth - 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 8:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                            case 9:
                                escapePoint.X = 2;
                                escapePoint.Y = mapWidth - 2;
                                break;
                        }
                        break;

                    #endregion MiddleRight

                    #region BottomLeft

                    case 7:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 2:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 3:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 4:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 5:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 6:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 7:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 8:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 9:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                        }
                        break;

                    #endregion BottomLeft

                    #region BottomCenter

                    case 8:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                if (ghostPosition.X != 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 2:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                break;
                            case 3:
                                if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 4:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 5:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    escapePoint.X = mapHeight - 2;
                                    escapePoint.Y = 2;
                                }
                                break;
                            case 6:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                            case 7:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 8:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                break;
                            case 9:
                                escapePoint.X = mapHeight - 2;
                                escapePoint.Y = 2;
                                break;
                        }
                        break;

                    #endregion BottomCenter

                    #region BottomRight

                    case 9:
                        switch (ghostFleeZone)
                        {
                            case 1:
                                if (ghostPosition.X != 2 && ghostPosition.Y != 2)
                                {
                                    escapePoint.X = 2;
                                    escapePoint.Y = 2;
                                }
                                else
                                {
                                    return new Position(-1, -1);
                                }
                                break;
                            case 2:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 3:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 4:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 5:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 6:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 7:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 8:
                                escapePoint.X = 2;
                                escapePoint.Y = 2;
                                break;
                            case 9:
                                if (pacmanPosition.X >= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        escapePoint.X = 2;
                                        escapePoint.Y = mapWidth - 2;
                                    }
                                }
                                else if (pacmanPosition.X <= ghostPosition.X)
                                {
                                    if (pacmanPosition.Y >= ghostPosition.Y)
                                    {
                                        escapePoint.X = mapHeight - 2;
                                        escapePoint.Y = 2;
                                    }
                                    else if (pacmanPosition.Y <= ghostPosition.Y)
                                    {
                                        if (ghostPosition.X != mapHeight - 2 && ghostPosition.Y != mapWidth - 2)
                                        {
                                            escapePoint.X = mapHeight - 2;
                                            escapePoint.Y = mapWidth - 2;
                                        }
                                        else
                                        {
                                            return new Position(-1, -1);
                                        }
                                    }
                                }
                                break;
                        }
                        break;

                    #endregion BottomRight
                }

                #endregion SetEscapePoint

                return FleeSearch();
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        /// <summary>
        /// Searches from the escape point to the ghost for the way between them.
        /// </summary>
        /// <returns>The new position.</returns>
        private Position FleeSearch()
        {
            try
            {
                // The wavefront reached the ghost (start position).
                bool reached = false;

                // Set the search position to the pacman (finish position).
                currentPosition = pacmanPosition;

                // The start and finish positions are valid.
                if ((pacmanPosition.X >= 0) && (pacmanPosition.Y >= 0) && (ghostPosition.X >= 0) && (ghostPosition.Y >= 0))
                {
                    #region FillCopyMap

                    // Add the start and the finish values.
                    copyMap[pacmanPosition.X, pacmanPosition.Y] = pacmanValue;
                    copyMap[ghostPosition.X, ghostPosition.Y] = ghostValue;
                    copyMap[escapePoint.X, escapePoint.Y] = escapeValue;

                    // Check each row of the map.
                    for (int i = 0; i < mapHeight; i++)
                    {
                        // Check each column of the map.
                        for (int j = 0; j < mapWidth; j++)
                        {
                            // The cell is a obstacle item.
                            if (originalMap[i, j] == '#' || originalMap[i, j] == '-')
                            {
                                // Add the obstacle values.
                                copyMap[i, j] = obstacleValue;
                            }
                            // The cell is a space item.
                            else if (originalMap[i, j] != 'O' && (originalMap[i, j] != 'n' && i != ghostPosition.X && j != ghostPosition.Y) && 
                                (i != escapePoint.X && j != escapePoint.Y))
                            {
                                // Add the space values.
                                copyMap[i, j] = emptyValue;
                            }
                        }
                    }

                    #endregion FillCopyMap

                    cellCount = 0;

                    #region SearchForTheWay

                    // Make waves from the escape point to the ghost (start) and check if the way between them exists.
                    do
                    {
                        // Make a wave and check how many cells were modified.
                        cellCount = FleeWave();

                        // The rows around the ghost.
                        for (int i = Math.Max(0, ghostPosition.X - 1); i < Math.Min(ghostPosition.X + 1, mapHeight - 1); i++)
                        {
                            // The columns around the ghost.
                            for (int j = Math.Max(0, ghostPosition.Y - 1); j < Math.Min(ghostPosition.Y + 1, mapWidth - 1); j++)
                            {
                                // The wavefront reached the ghost.
                                if (copyMap[i, j] > 0 &&
                                    ((i == ghostPosition.X - 1 && j == ghostPosition.Y) || /* Left */
                                    (i == ghostPosition.X + 1 && j == ghostPosition.Y) || /* Right */
                                    (i == ghostPosition.X && j == ghostPosition.Y - 1) || /* Up */
                                    (i == ghostPosition.X && j == ghostPosition.Y + 1))) /* Down */
                                {
                                    reached = true;
                                    break;
                                }
                            }

                            // Exit the loop if the way is found.
                            if (reached)
                            {
                                break;
                            }
                        }
                    } while (!reached && cellCount > 0);

                    #endregion SearchForTheWay

                    // The way couldn't be found.
                    if (!reached)
                    {
                        return new Position(-1, -1);
                    }
                    // The way was found.
                    else
                    {
                        // Take a step closer to the pacman.
                        return FleeStep();
                    }
                }
                // The start and finish positions are not valid.
                else
                {
                    return new Position(-1, -1);
                }
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        /// <summary>
        /// Search for the minimum wavefront value arond the ghost, this will be the ghost's next position.
        /// </summary>
        /// <returns>The next position.</returns>
        private Position FleeStep()
        {
            try
            {
                Position nextPosition = new Position(-1, -1);
                int currentMinValue = 100000;

                // The rows around the ghost.
                for (int i = 0; i < 3; i++)
                {
                    // The columns around the ghost.
                    for (int j = 0; j < 3; j++)
                    {
                        // The cell around the ghost is a wavefront value and it's smaller than the current minimum value.
                        if (currentMinValue > copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] &&
                            copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] != obstacleValue &&
                            copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] != emptyValue &&
                            copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j] != ghostValue &&
                            ((i == ghostPosition.X - 1 && j == ghostPosition.Y) || /* Left */
                            (i == ghostPosition.X + 1 && j == ghostPosition.Y) || /* Right */
                            (i == ghostPosition.X && j == ghostPosition.Y - 1) || /* Up */
                            (i == ghostPosition.X && j == ghostPosition.Y + 1))) /* Down */
                        {
                            // Make the current minimum value the found cell value.
                            currentMinValue = copyMap[ghostPosition.X - 1 + i, ghostPosition.Y - 1 + j];
                        }
                    }
                }

                return nextPosition;
            }
            catch
            {
                return new Position(-1, -1);
            }
        }

        /// <summary>
        /// Fills the map with the wavefront values until the ghost (start position) is reached.
        /// </summary>
        /// <returns>The count of the modified cell.</returns>
        private int FleeWave()
        {
            try
            {
                int cell = 0;

                // The first modification coordinate is on the escape point: the wave has not been started yet.
                if ((currentPosition.X == escapePoint.X) && (currentPosition.Y == escapePoint.Y))
                {
                    // Start the wave.
                    return FleeFirstWave();
                }
                // The wave has been started.
                else
                {
                    // Continue the wave.
                    cell = 0;
                    wavefrontValue++;

                    // The rows of the map.
                    for (int i = 0; i < mapHeight; i++)
                    {
                        // The columns of the map.
                        for (int j = 0; j < mapWidth; j++)
                        {
                            // Searches for an egsisting wavefront value to continue the wavefront.
                            if (copyMap[i, j] != emptyValue && copyMap[i, j] != obstacleValue && copyMap[i, j] != pacmanValue && copyMap[i, j] == wavefrontValue - 1)
                            {
                                // Sets the found wavefront value's position to a position to search around it.
                                currentPosition = new Position(i, j);

                                // The rows around the found wavefront value.
                                for (int k = 0; k < 3; k++)
                                {
                                    // The columns around the found wavefront value.
                                    for (int l = 0; l < 3; l++)
                                    {
                                        // The cell is an empty value.
                                        if (copyMap[currentPosition.X - 1 + l, currentPosition.Y - 1 + k] == emptyValue)
                                        {
                                            // The wavefront value can be written in it.
                                            cell++;
                                            copyMap[currentPosition.X - 1 + l, currentPosition.Y - 1 + k] = wavefrontValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return cell;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Makes the first wave around the escape point.
        /// </summary>
        /// <returns>The count of the modified cell.</returns>
        private int FleeFirstWave()
        {
            try
            {
                // Goes around the escape point and writes the first wavefront values around it in the empty cells.
                int cell = 0;

                // The rows around the escape point.
                for (int i = 0; i < 3; i++)
                {
                    // The columns around the escape point.
                    for (int j = 0; j < 3; j++)
                    {
                        // The cell is an empty value.
                        if (copyMap[escapePoint.X - 1 + i, escapePoint.Y - 1 + j] == emptyValue)
                        {
                            // Add the wavefront value.
                            copyMap[escapePoint.X - 1 + i, escapePoint.Y - 1 + j] = wavefrontValue;
                            cell++;

                            // Sets the search position from the pacman.
                            currentPosition = new Position(0, 0);
                        }
                    }
                }

                wavefrontValue = 1;

                return cell;
            }
            catch
            {
                return 0;
            }
        }

        #endregion Flee
        
        #endregion Methods
    }
}
