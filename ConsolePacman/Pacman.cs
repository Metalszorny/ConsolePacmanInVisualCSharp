using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Base class for Pacman.
    /// </summary>
    public class Pacman
    {
        #region Fields

        // The position field of the Pacman class.
        private Position position;

        // The life field of the Pacman class.
        private int life;

        // The score field of the Pacman class.
        private int score;

        // The previous position of the Pacman class.
        private Position previous;

        // The move direction of the pacman class.
        private MoveDirections moveDirection;

        // The directions pacman can move.
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
        /// Gets or sets the moveDirection.
        /// </summary>
        /// <value>
        /// The moveDirection.
        /// </value>
        public MoveDirections MoveDirection
        {
            get { return moveDirection; }
            set { moveDirection = value; }
        }

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
        /// Gets or sets the life.
        /// </summary>
        /// <value>
        /// The life.
        /// </value>
        public int Life
        {
            get { return life; }
            set { life = value; }
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

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Pacman"/> class.
        /// </summary>
        public Pacman()
        {
            PreSetValues();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pacman"/> class.
        /// </summary>
        /// <param name="position">The input value for the position field.</param>
        /// <param name="life">The input value for the life field.</param>
        /// <param name="score">The input value for the score field.</param>
        public Pacman(Position position, int life, int score)
        {
            Position = position;
            Life = life;
            Score = score;

            PreSetValues();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Draws the Pacman.
        /// </summary>
        /// <returns>The image of the Pacman object.</returns>
        public char Draw()
        {
            try
            {
                return 'O';
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
                MoveDirection = MoveDirections.Stay;
            }
            catch
            { }
        }

        /// <summary>
        /// Repositions the pacman.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <param name="map">The map.</param>
        public void Reposition(Position newPosition, ref char[,] map)
        {
            try
            {
                // Set the previous values.
                Previous = Position;
                map[Previous.X, Previous.Y] = ' ';

                // Set the current values.
                Position = newPosition;
                map[Position.X, Position.Y] = Draw();
            }
            catch
            { }
        }

        /// <summary>
        /// Validates the next step of the pacman.
        /// </summary>
        /// <param name="positionX">The x position.</param>
        /// <param name="positionY">The y position.</param>
        /// <param name="map">The map.</param>
        /// <param name="mapWidth">The width of the map.</param>
        /// <param name="mapHeight">The height of the map.</param>
        /// <param name="collectables">The collectables.</param>
        /// <param name="refreshDisplay">The display refresh.</param>
        /// <param name="ghostHunt">The hunt for ghosts.</param>
        /// <param name="ghostHuntCounter">The counter for ghost hunter.</param>
        /// <param name="ghostScoreMultiplier">The ghost score multiplier.</param>
        /// <param name="justDied">The pacman just died.</param>
        /// <param name="ghostDied">The ghost died.</param>
        /// <param name="pacmanSpawn">The spawn point of the pacman.</param>
        /// <param name="ghosts">The ghost list.</param>
        private void ValidateNextStep(int positionX, int positionY, ref char[,] map, int mapWidth, int mapHeight, ref int collectables, ref bool refreshDisplay, 
            ref bool ghostHunt, ref int ghostHuntCounter, ref int ghostScoreMultiplier, ref bool justDied, ref Position ghostDied, Position pacmanSpawn, 
            ref List<Ghost> ghosts)
        {
            try
            {
                char place = map[positionX, positionY];

                // Handle the next step based on the next item.
                switch (place)
                {
                    case '.':
                        Score += 10;
                        collectables -= 1;

                        Reposition(new Position(positionX, positionY), ref map);
                        refreshDisplay = true;
                        break;
                    case '*':
                        Score += 100;
                        ghostHunt = true;
                        ghostHuntCounter = 0;
                        ghostScoreMultiplier = 1;

                        Reposition(new Position(positionX, positionY), ref map);
                        refreshDisplay = true;
                        break;
                    case ' ':
                        Reposition(new Position(positionX, positionY), ref map);
                        refreshDisplay = true;
                        break;
                    case 'n':
                        // The ghost can be eaten.
                        if (ghostHunt)
                        {
                            Score += ghostScoreMultiplier * 500;
                            ghostScoreMultiplier++;
                            ghostDied.X = positionX;
                            ghostDied.Y = positionY;

                            // Check each ghost.
                            foreach (var ghost in ghosts)
                            {
                                // The ghost is the one, that is next to the pacman.
                                if (ghost.Position.X == positionX && ghost.Position.Y == positionY)
                                {
                                    // Handle the item under the ghost.
                                    switch (ghost.CharUnder)
                                    {
                                        case '.':
                                            Score += 10;
                                            collectables -= 1;

                                            ghost.CharUnder = ' ';
                                            break;
                                        case '*':
                                            Score += 100;
                                            ghostHunt = true;
                                            ghostHuntCounter = 0;

                                            ghost.CharUnder = ' ';
                                            break;
                                        case ' ':
                                        case 'n':
                                        case '#':
                                        case '-':
                                        case 'O':
                                            break;
                                    }
                                }
                            }

                            Reposition(new Position(positionX, positionY), ref map);
                            refreshDisplay = true;
                        }
                        // The ghost can't be eaten.
                        else
                        {
                            Life -= 1;
                            justDied = true;

                            Reposition(new Position(pacmanSpawn.X, pacmanSpawn.Y), ref map);
                            refreshDisplay = true;
                        }
                        break;
                    case '#':
                    case '-':
                    case 'O':
                        break;
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Moves the Pacman based on the direction.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="collectables">The collectables.</param>
        /// <param name="ghostHunt">The ghostHunt.</param>
        /// <param name="justDied">The justDied.</param>
        /// <param name="ghostDied">The ghostDied.</param>
        /// <param name="pacmanSpawn">The pacmanSpawn.</param>
        /// <param name="refreshDisplay">The refreshDisplay.</param>
        /// <param name="ghostScoreMultiplier">The ghostScoreMultiplier.</param>
        /// <param name="mapWidth">The mapWidth.</param>
        /// <param name="mapHeight">The mapHeight.</param>
        /// <param name="ghostHuntCounter">The ghostHuntCounter.</param>
        /// <param name="ghosts">The ghosts.</param>
        public void Move(ref char[,] map, ref int collectables, ref bool ghostHunt, ref bool justDied, ref Position ghostDied, 
            Position pacmanSpawn, ref bool refreshDisplay, ref int ghostScoreMultiplier, int mapWidth, int mapHeight, ref int ghostHuntCounter, 
            ref List<Ghost> ghosts)
        {
            try
            {
                #region Left

                // Move the pacman left.
                if (MoveDirection == MoveDirections.Left)
                {
                    // At tunnel.
                    if (Position.Y == 0)
                    {
                        ValidateNextStep(Position.X, mapWidth - 1, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                    // Not at tunnel.
                    else
                    {
                        ValidateNextStep(Position.X, Position.Y - 1, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                }

                #endregion Left

                #region Right

                // Move the pacman right.
                else if (MoveDirection == MoveDirections.Right)
                {
                    // Is at tunnel?
                    if (Position.Y == mapWidth - 1)
                    {
                        ValidateNextStep(Position.X, 0, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                    // Not at tunnel.
                    else
                    {
                        ValidateNextStep(Position.X, Position.Y + 1, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                }

                #endregion Right

                #region Up

                // Move the pacman up.
                else if (MoveDirection == MoveDirections.Up)
                {
                    // At tunnel.
                    if (Position.X == 0)
                    {
                        ValidateNextStep(mapHeight - 1, Position.Y, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                    // Not at tunnel.
                    else
                    {
                        ValidateNextStep(Position.X - 1, Position.Y, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                }

                #endregion Up

                #region Down

                // Move the pacman down.
                else if (MoveDirection == MoveDirections.Down)
                {
                    // At tunnel.
                    if (Position.X == mapHeight - 1)
                    {
                        ValidateNextStep(0, Position.Y, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                    // Not at tunnel.
                    else
                    {
                        ValidateNextStep(Position.X + 1, Position.Y, ref map, mapWidth, mapHeight, ref collectables, ref refreshDisplay, ref ghostHunt,
                            ref ghostHuntCounter, ref ghostScoreMultiplier, ref justDied, ref ghostDied, pacmanSpawn, ref ghosts);
                    }
                }

                #endregion Down
            }
            catch
            { }
        }

        #endregion Methods
    }
}
