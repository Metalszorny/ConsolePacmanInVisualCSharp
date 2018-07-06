using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Base class for Ghost.
    /// </summary>
    public class Ghost
    {
        #region Fields

        // The search algorithm.
        private WavefrontSearch wavefrontSearch;

        // The position field of the Ghost class.
        private Position position;

        // The charUnder field of the Ghost class.
        private char charUnder;

        // The previous position of the Ghost class.
        private Position previous;

        // The previous charUnder field of the Ghost class.
        private char previousCharUnder;

        // The in wait field of the Ghost class.
        private bool inWait;

        // The random.
        private Random random;

        // The previous random movement direction.
        private MoveDirections previousRandomDirection;

        // The directions ghost can move.
        public enum MoveDirections
        {
            Up,
            Down,
            Left,
            Right,
            Stay
        }

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Position Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the charUnder.
        /// </summary>
        /// <value>
        /// The charUnder.
        /// </value>
        public char CharUnder
        {
            get { return charUnder; }
            set { charUnder = value; }
        }

        /// <summary>
        /// Gets or sets the previous.
        /// </summary>
        /// <value>
        /// The previous.
        /// </value>
        public Position Previous
        {
            get { return previous; }
            set { previous = value; }
        }

        /// <summary>
        /// Gets or sets the previousCharUnder.
        /// </summary>
        /// <value>
        /// The previousCharUnder.
        /// </value>
        public char PreviousCharUnder
        {
            get { return previousCharUnder; }
            set { previousCharUnder = value; }
        }

        /// <summary>
        /// Gets or sets the inWait.
        /// </summary>
        /// <value>
        /// The inWait.
        /// </value>
        public bool InWait
        {
            get { return inWait; }
            set { inWait = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Ghost"/> class.
        /// </summary>
        public Ghost()
        {
            PreSetValues();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ghost"/> class.
        /// </summary>
        /// <param name="position">The input value for the position field.</param>
        public Ghost(Position position)
        {
            Position = position;

            PreSetValues();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Draws the Ghost.
        /// </summary>
        /// <returns>The image of the Ghost object.</returns>
        public char Draw()
        {
            try
            {
                return 'n';
            }
            catch
            {
                return ' ';
            }
        }

        /// <summary>
        /// Presets the values.
        /// </summary>
        private void PreSetValues()
        {
            try
            {
                random = new Random();
                previousRandomDirection = MoveDirections.Stay;
                PreviousCharUnder = ' ';
            }
            catch
            { }
        }

        /// <summary>
        /// Moves the Ghost.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="pacman">The pacman.</param>
        /// <param name="ghostHunt">The ghostHunt.</param>
        /// <param name="ghostSpawn">The ghostSpawn.</param>
        /// <param name="justDied">The justDied.</param>
        /// <param name="refreshDisplay">The refreshDisplay.</param>
        /// <param name="mapWidth">The mapWidth.</param>
        /// <param name="mapHeight">The mapHeight.</param>
        public void Move(ref char[,] map, ref Pacman pacman, ref bool ghostHunt, Position ghostSpawn, ref bool justDied, 
            ref bool refreshDisplay, int mapWidth, int mapHeight)
        {
            try
            {
                // The ghost is on the move.
                if (!InWait)
                {
                    // The ghost is not hunted.
                    if (!ghostHunt)
                    {
                        int choise = random.Next(1, 101);

                        // Mainly hunt the pacman.
                        if (choise <= 80)
                        {
                            // Hunt for the pacman.
                            Hunt(ref map, mapWidth, mapHeight, pacman);
                        }
                        // But occasionly wander off.
                        else
                        {
                            // Wander off randomly.
                            Wander(ref map, ref refreshDisplay, mapWidth, mapHeight);
                        }
                    }
                    // The ghost is hunted.
                    else
                    {
                        // Flee from the pacman.
                        Flee(ref map, mapWidth, mapHeight, pacman);
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Flees from pacman.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapWidth">The width of the map.</param>
        /// <param name="mapHeight">The height of the map.</param>
        /// <param name="pacman">The pacman.</param>
        private void Flee(ref char[,] map, int mapWidth, int mapHeight, Pacman pacman)
        {
            try
            {
                // Initialize the search.
                wavefrontSearch = new WavefrontSearch(map, mapWidth, mapHeight, pacman, this);

                // Get the new position.
                Position nextPosition = wavefrontSearch.Flee();

                // The new position is valid.
                if (ValidateDirection(ConvertPositionToDirection(nextPosition), ref map, mapWidth, mapHeight))
                {
                    // Reposition the ghost based on the new position.
                    Reposition(nextPosition, ref map);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Hunts pacman.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapWidth">The width of the map.</param>
        /// <param name="mapHeight">The height of the map.</param>
        /// <param name="pacman">The pacman.</param>
        private void Hunt(ref char[,] map, int mapWidth, int mapHeight, Pacman pacman)
        {
            try
            {
                // Initialize the search.
                wavefrontSearch = new WavefrontSearch(map, mapWidth, mapHeight, pacman, this);

                // Get the new position.
                Position nextPosition = wavefrontSearch.Hunt();

                // The new position is valid.
                if (ValidateDirection(ConvertPositionToDirection(nextPosition), ref map, mapWidth, mapHeight))
                {
                    // Reposition the ghost based on the new position.
                    Reposition(nextPosition, ref map);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Converts the position to direction.
        /// </summary>
        /// <param name="newPosition">The position.</param>
        /// <returns>The appropriate direction.</returns>
        private MoveDirections ConvertPositionToDirection(Position newPosition)
        {
            try
            {
                // The new position is left to the old position.
                if (newPosition.X + 1 == Position.X && newPosition.Y == Position.Y)
                {
                    return MoveDirections.Left;
                }
                // The new position is right to the old position.
                else if (newPosition.X - 1 == Position.X && newPosition.Y == Position.Y)
                {
                    return MoveDirections.Right;
                }
                // The new position is higher to the old position.
                else if (newPosition.X == Position.X && newPosition.Y + 1 == Position.Y)
                {
                    return MoveDirections.Up;
                }
                // The new position is lower to the old position.
                else if (newPosition.X == Position.X && newPosition.Y - 1 == Position.Y)
                {
                    return MoveDirections.Down;
                }

                return MoveDirections.Stay;
            }
            catch
            {
                return MoveDirections.Stay;
            }
        }

        /// <summary>
        /// Random generates a direction.
        /// </summary>
        /// <returns>The direction the ghost will take.</returns>
        private MoveDirections RandomGenerateDirection()
        {
            try
            {
                // Random generate a number.
                int direction = random.Next(1, 101);
                MoveDirections returnValue = MoveDirections.Stay;

                // Assign the random generated number to a direction.
                if ((direction >= 1) && (direction <= 25))
                {
                    returnValue = MoveDirections.Down;
                }
                else if ((direction >= 26) && (direction <= 50))
                {
                    returnValue = MoveDirections.Left;
                }
                else if ((direction >= 51) && (direction <= 75))
                {
                    returnValue = MoveDirections.Right;
                }
                else if ((direction >= 76) && (direction <= 100))
                {
                    returnValue = MoveDirections.Up;
                }

                return returnValue;
            }
            catch
            {
                return MoveDirections.Stay;
            }
        }

        /// <summary>
        /// Validates the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        private bool ValidateDirection(MoveDirections direction, ref char[,] map, int mapWidth, int mapHeight)
        {
            try
            {
                #region Down

                // Move the ghost down.
                if (direction == MoveDirections.Down)
                {
                    // The down movement can be checked.
                    if (Position.X < mapHeight - 1)
                    {
                        // The down movement can be made.
                        if (map[Position.X + 1, Position.Y] != '#' && map[Position.X + 1, Position.Y] != '-' && map[Position.X + 1, Position.Y] != 'n' &&
                            map[Position.X + 1, Position.Y] != 'O')
                        {
                            return true;
                        }

                        return false;
                    }
                    // The down movement can't be checked.
                    else
                    {
                        // The down movement will put the ghost to the opposit of the map.
                        if (Position.X + 1 > mapHeight - 1)
                        {
                            // The down movement can be made.
                            if (map[0, Position.Y] != '#' && map[0, Position.Y] != '-' && map[0, Position.Y] != 'n' && map[0, Position.Y] != 'O')
                            {
                                return true;
                            }

                            return false;
                        }

                        return false;
                    }
                }

                #endregion Down

                #region Left

                // Move the ghost left.
                else if (direction == MoveDirections.Left)
                {
                    // The left movement can be checked.
                    if (Position.Y > 0)
                    {
                        // The left movement can be made.
                        if (map[Position.X, Position.Y - 1] != '#' && map[Position.X, Position.Y - 1] != '-' && map[Position.X, Position.Y - 1] != 'n' &&
                            map[Position.X, Position.Y - 1] != 'O')
                        {
                            return true;
                        }

                        return false;
                    }
                    // The left movement can't be checked.
                    else
                    {
                        // The left movement will put the ghost to the opposit of the map.
                        if (Position.Y - 1 <= 0)
                        {
                            // The left movement can be made.
                            if (map[Position.X, mapWidth - 1] != '#' && map[Position.X, mapWidth - 1] != '-' && map[Position.X, mapWidth - 1] != 'n' && 
                                map[Position.X, mapWidth - 1] != 'O')
                            {
                                return true;
                            }

                            return false;
                        }

                        return false;
                    }
                }

                #endregion Left

                #region Right

                // Move the ghost right.
                else if (direction == MoveDirections.Right)
                {
                    // The right movement can be checked.
                    if (Position.Y < mapWidth - 1)
                    {
                        // The right movement can be made.
                        if (map[Position.X, Position.Y + 1] != '#' && map[Position.X, Position.Y + 1] != '-' && map[Position.X, Position.Y + 1] != 'n' &&
                            map[Position.X, Position.Y + 1] != 'O')
                        {
                            return true;
                        }

                        return false;
                    }
                    // The right movement can't be checked.
                    else
                    {
                        // The right movement will put the ghost to the opposit of the map.
                        if (Position.Y + 1 > mapWidth - 1)
                        {
                            // The right movement can be made.
                            if (map[Position.X, 0] != '#' && map[Position.X, 0] != '-' && map[Position.X, 0] != 'n' && map[Position.X, 0] != 'O')
                            {
                                return true;
                            }

                            return false;
                        }

                        return false;
                    }
                }

                #endregion Right

                #region Up

                // Move the ghost up.
                else if (direction == MoveDirections.Up)
                {
                    // The up movement can be checked.
                    if (Position.X > 0)
                    {
                        // The up movement can be made.
                        if (map[Position.X - 1, Position.Y] != '#' && map[Position.X - 1, Position.Y] != '-' && map[Position.X - 1, Position.Y] != 'n' &&
                            map[Position.X - 1, Position.Y] != 'O')
                        {
                            return true;
                        }

                        return false;
                    }
                    // The up movement can't be checked.
                    else
                    {
                        // The up movement will put the ghost to the opposit of the map.
                        if (Position.X - 1 <= 0)
                        {
                            // The up movement can be made.
                            if (map[mapHeight - 1, Position.Y] != '#' && map[mapHeight - 1, Position.Y] != '-' && map[mapHeight - 1, Position.Y] != 'n' && 
                                map[mapHeight - 1, Position.Y] != 'O')
                            {
                                return true;
                            }

                            return false;
                        }

                        return false;
                    }
                }

                #endregion Up

                return false;
            }
            catch
            {
                direction = MoveDirections.Stay;
                return true;
            }
        }

        /// <summary>
        /// Repositions the ghost.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <param name="map">The map.</param>
        public void Reposition(Position newPosition, ref char[,] map)
        {
            try
            {
                // Set the previous values.
                Previous = Position;
                PreviousCharUnder = CharUnder;
                map[Previous.X, Previous.Y] = PreviousCharUnder;

                // Set the current values.
                Position = newPosition;
                CharUnder = map[Position.X, Position.Y];
                map[Position.X, Position.Y] = Draw();
            }
            catch
            { }
        }

        /// <summary>
        /// Wander in a random direction.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="refreshDisplay">The refreshDisplay.</param>
        private void Wander(ref char[,] map, ref bool refreshDisplay, int mapWidth, int mapHeight)
        {
            try
            {
                #region SetDirection

                MoveDirections direction = MoveDirections.Stay;

                // 25% chance of random direction or first random move.
                if (random.Next(1, 101) > 75 || previousRandomDirection == MoveDirections.Stay)
                {
                    // Repeat the random direction assigment until a valid is found.
                    do
                    {
                        direction = RandomGenerateDirection();
                    } while (!ValidateDirection(direction, ref map, mapWidth, mapHeight));
                }
                // Continue the previous random direction, this will make the movement less random.
                else
                {
                    // The previous direction can't be continued.
                    if (!ValidateDirection(previousRandomDirection, ref map, mapWidth, mapHeight))
                    {
                        // Repeat the random direction assigment until a valid is found.
                        do
                        {
                            direction = RandomGenerateDirection();
                        } while (!ValidateDirection(direction, ref map, mapWidth, mapHeight));
                    }
                    // The previous direction can be continued.
                    else
                    {
                        direction = previousRandomDirection;
                    }
                }

                #endregion SetDirection

                // Set previous values.
                refreshDisplay = true;
                Previous = Position;
                PreviousCharUnder = CharUnder;
                map[Previous.X, Previous.Y] = PreviousCharUnder;

                #region ChangePosition

                // Change the position of the ghost.
                switch (direction)
                {
                    case MoveDirections.Up:
                        position.X -= 1;
                        if (Position.X < 0)
                        {
                            position.X = mapHeight - 1;
                        }
                        previousRandomDirection = MoveDirections.Up;
                        break;
                    case MoveDirections.Down:
                        position.X += 1;
                        if (Position.X > mapHeight - 1)
                        {
                            position.X = 0;
                        }
                        previousRandomDirection = MoveDirections.Down;
                        break;
                    case MoveDirections.Left:
                        position.Y -= 1;
                        if (Position.Y < 0)
                        {
                            position.Y = mapWidth - 1;
                        }
                        previousRandomDirection = MoveDirections.Left;
                        break;
                    case MoveDirections.Right:
                        position.Y += 1;
                        if (Position.Y > mapWidth - 1)
                        {
                            position.Y = 0;
                        }
                        previousRandomDirection = MoveDirections.Right;
                        break;
                }

                #endregion ChangePosition

                // Set the current values.
                CharUnder = map[Position.X, Position.Y];
                map[Position.X, Position.Y] = Draw();
            }
            catch
            { }
        }

        #endregion Methods
    }
}
