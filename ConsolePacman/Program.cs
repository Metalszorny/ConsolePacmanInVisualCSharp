using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsolePacman
{
    /// <summary>
    /// Interaction logic for Program.
    /// </summary>
    public class Program
    {
        #region Fields

        #region Map

        // The map txt access.
        private static TxtAccess txtAccess = new TxtAccess(@"..\..\Resources\map.txt");

        // The map's width value.
        private const int mapWidth = 28;

        // The map's height value.
        private const int mapHeight = 23;

        // The map.
        public static char[,] map = new char[mapHeight, mapWidth];

        // Draw the map in color.
        private static bool coloredMap = true;

        #endregion Map

        #region Highscore

        // The limit of the highscores.
        private const int scoreLimit = 20;

        // The highscores file path.
        private static XmlAccess xmlAccess = new XmlAccess(@"..\..\Resources\highscores.xml");

        // The list of highscores.
        private static List<Highscore> highscoresList = new List<Highscore>();

        #endregion Highscore

        #region Character

        // The pacman.
        private static Pacman pacman;

        // The list of ghosts.
        private static List<Ghost> ghosts = new List<Ghost>(4);

        #endregion Character

        #region CharacterFunction

        // The pacman's spawn point.
        public static Position pacmanSpawn = new Position(17, 14);

        // Where did the ghost has been eaten.
        public static Position ghostDied = new Position(mapWidth + 1, mapHeight + 1);

        // The ghost's spawn point.
        private static Position ghostSpawn = new Position(7, 14);

        // The time delay to start each ghost.
        private static int startGhostCounter = 0;

        // The time delay to move each ghost.
        private static int moveGhostCounter = 0;

        // The user's life.
        private const int lifeCount = 3;

        // The player's score.
        private const int playerScore = 0;

        #endregion CharacterFunction

        #region MapFunction

        // The random.
        private static Random rnd = new Random();

        // Indicates that the game is in session or not.
        private static bool gameFinished = false;

        // Indicates that the user's score should be checked or not.
        private static bool checkHighscores = false;

        // The status of the game.
        private static GameStages gameStatus = GameStages.Menu;

        // The number of collectables to finish the map.
        public static int collectables = 240;

        // Indicates that the ghosthunt is on or not.
        public static bool ghostHunt = false;

        // Multiplies the score to get after a ghost is eaten in a row.
        public static int ghostScoreMultiplier = 1;

        // The "timer" for the ghost hunt.
        private static int ghostHuntCounter = 0;

        // The time limit of the ghost hunt.
        private const int ghostHuntTime = 1000000;

        // The time limit of the ghost start.
        private const int startGhotTime = 500000;

        // The time limit of the ghost move.
        private const int moveGhostTime = 10000;

        // Indicates that the play died or not.
        public static bool justDied = false;

        // The game needs to preset.
        private static bool gamePreset = true;

        // Refresh the display.
        public static bool refreshDisplay = false;

        // Refresh the ghost's spawn area.
        private static bool refreshGhostSpawn = false;

        // The enum of game stages.
        private enum GameStages
        {
            Menu,
            Highsores,
            Game,
            Exit
        }

        #endregion MapFunction

        #endregion Fields

        #region Methods

        /// <summary>
        /// The main method of the Program.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        static void Main(string[] args)
        {
            try
            {
                #region AddCharacters

                // The pacman.
                pacman = new Pacman(pacmanSpawn, lifeCount, playerScore);

                // The ghost: Shadow a.k.a. Blinky.
                ghosts.Add(new Ghost(ghostSpawn));

                // The ghost: Speedy a.k.a. Pinky.
                ghosts.Add(new Ghost(new Position(10, 12)));

                // The ghost: Bashful a.k.a. Inky.
                ghosts.Add(new Ghost(new Position(10, 13)));

                // The ghost: Pokey a.k.a. Clyde.
                ghosts.Add(new Ghost(new Position(10, 14)));

                #endregion AddCharacters

                while (!gameFinished)
                {
                    #region Menu

                    // The menu stage.
                    if (gameStatus == GameStages.Menu)
                    {
                        #region RefreshDisplay

                        DisplayMenu();

                        #endregion RefreshDisplay

                        // Repeat the key read until a valid key is pushed.
                        while (gameStatus == GameStages.Menu)
                        {
                            #region HandleInput

                            // A key was pushed.
                            if (Console.KeyAvailable)
                            {
                                // Handle the key input.
                                switch (Console.ReadKey().Key)
                                {
                                    case ConsoleKey.D1:
                                    case ConsoleKey.NumPad1:
                                        gameStatus = GameStages.Game;
                                        break;
                                    case ConsoleKey.D2:
                                    case ConsoleKey.NumPad2:
                                        gameStatus = GameStages.Highsores;
                                        break;
                                    case ConsoleKey.D3:
                                    case ConsoleKey.NumPad3:
                                        gameStatus = GameStages.Exit;
                                        break;
                                }

                                // Delete the read key input.
                                DeleteKeyInput();
                            }

                            #endregion HandleInput
                        }
                    }

                    #endregion Menu

                    #region Game

                    // The game stage.
                    else if (gameStatus == GameStages.Game)
                    {
                        #region PreSet

                        pacman.Position = pacmanSpawn;
                        ghosts[0].Position = ghostSpawn;
                        ghosts[1].Position = new Position(10, 12);
                        ghosts[2].Position = new Position(10, 13);
                        ghosts[3].Position = new Position(10, 14);

                        // Pre-set values.
                        if (gamePreset)
                        {
                            try
                            {
                                // Read and store the map.
                                if (txtAccess.FileExists())
                                {
                                    // Load the map.
                                    map = txtAccess.LoadMap(mapWidth, mapHeight);

                                    // Put the pacman and the ghosts on the map.
                                    map[pacman.Position.X, pacman.Position.Y] = pacman.Draw();

                                    for (int j = 0; j < ghosts.Count; j++)
                                    {
                                        map[ghosts[j].Position.X, ghosts[j].Position.Y] = ghosts[j].Draw();
                                    }
                                }
                                // 
                                else
                                {
                                    Console.WriteLine("Map not found.");
                                    Console.ReadLine();
                                    gameFinished = true;
                                }
                            }
                            catch
                            { }

                            ghostHunt = false;
                            collectables = 240;
                            pacman.Life = lifeCount;
                            pacman.Score = playerScore;
                            pacman.Position = pacmanSpawn;
                            gamePreset = false;
                            checkHighscores = false;
                            pacman.Previous = new Position(mapHeight + 1, mapWidth + 1);
                            ghosts[0].Previous = new Position(mapHeight + 1, mapWidth + 1);
                            ghosts[1].Previous = new Position(mapHeight + 1, mapWidth + 1);
                            ghosts[2].Previous = new Position(mapHeight + 1, mapWidth + 1);
                            ghosts[3].Previous = new Position(mapHeight + 1, mapWidth + 1);
                            ghosts[0].CharUnder = ' ';
                            ghosts[1].CharUnder = ' ';
                            ghosts[2].CharUnder = ' ';
                            ghosts[3].CharUnder = ' ';
                            ghosts[0].InWait = true;
                            ghosts[1].InWait = true;
                            ghosts[2].InWait = true;
                            ghosts[3].InWait = true;
                            ghostScoreMultiplier = 1;
                            refreshGhostSpawn = false;
                        }

                        #endregion PreSet

                        #region RefreshDisplay

                        DisplayMap();

                        #endregion RefreshDisplay

                        // Repeat while the game is active.
                        while (gameStatus == GameStages.Game)
                        {
                            #region HandleInput

                            // A key was pushed.
                            if (Console.KeyAvailable)
                            {
                                // Handle the key input.
                                switch (Console.ReadKey().Key)
                                {
                                    case ConsoleKey.W:
                                    case ConsoleKey.UpArrow:
                                        pacman.MoveDirection = Pacman.MoveDirections.Up;
                                        break;
                                    case ConsoleKey.A:
                                    case ConsoleKey.LeftArrow:
                                        pacman.MoveDirection = Pacman.MoveDirections.Left;
                                        break;
                                    case ConsoleKey.D:
                                    case ConsoleKey.RightArrow:
                                        pacman.MoveDirection = Pacman.MoveDirections.Right;
                                        break;
                                    case ConsoleKey.S:
                                    case ConsoleKey.DownArrow:
                                        pacman.MoveDirection = Pacman.MoveDirections.Down;
                                        break;
                                    case ConsoleKey.Escape:
                                        gameStatus = GameStages.Menu;
                                        checkHighscores = false;
                                        gamePreset = true;
                                        break;
                                }

                                // Delete the read key input.
                                DeleteKeyInput();
                            }

                            #endregion HandleInput

                            #region HandleChanges

                            // Only handle the changes in the game stage.
                            if (gameStatus == GameStages.Game)
                            {
                                #region MovePacman

                                // The pacman moves.
                                if (pacman.MoveDirection != Pacman.MoveDirections.Stay)
                                {
                                    // Move the pacman.
                                    pacman.Move(ref map, ref collectables, ref ghostHunt, ref justDied, ref ghostDied, pacmanSpawn,
                                        ref refreshDisplay, ref ghostScoreMultiplier, mapWidth, mapHeight, ref ghostHuntCounter, ref ghosts);
                                }

                                #endregion MovePacman

                                #region ZeroCollectables

                                // The collectables reached zero, the game is over.
                                if (collectables == 0)
                                {
                                    checkHighscores = true;
                                    gameStatus = GameStages.Menu;
                                    gamePreset = true;
                                    break;
                                }

                                #endregion ZeroCollectables

                                #region PacmanDied

                                // Pacman died, set the characters to the default position.
                                if (justDied)
                                {
                                    RefreshChar(pacman.Previous, ' ');
                                    RefreshChar(pacman.Position, pacman.Draw());

                                    ghosts[0].Reposition(ghostSpawn, ref map);
                                    RefreshChar(ghosts[0].Previous, ghosts[0].PreviousCharUnder);
                                    RefreshChar(ghosts[0].Position, ghosts[0].Draw());
                                    ghosts[0].InWait = true;

                                    ghosts[1].Reposition(new Position(10, 12), ref map);
                                    RefreshChar(ghosts[1].Previous, ghosts[1].PreviousCharUnder);
                                    RefreshChar(ghosts[1].Position, ghosts[1].Draw());
                                    ghosts[1].InWait = true;

                                    ghosts[2].Reposition(new Position(10, 13), ref map);
                                    RefreshChar(ghosts[2].Previous, ghosts[2].PreviousCharUnder);
                                    RefreshChar(ghosts[2].Position, ghosts[2].Draw());
                                    ghosts[2].InWait = true;

                                    ghosts[3].Reposition(new Position(10, 14), ref map);
                                    RefreshChar(ghosts[3].Previous, ghosts[3].PreviousCharUnder);
                                    RefreshChar(ghosts[3].Position, ghosts[3].Draw());
                                    ghosts[3].InWait = true;

                                    refreshDisplay = true;
                                    refreshGhostSpawn = true;
                                    justDied = false;
                                }

                                #endregion PacmanDied

                                #region GhostHuntCounter

                                // The ghost hunt is on.
                                if (ghostHunt)
                                {
                                    ghostHuntCounter += 1;

                                    // 
                                    if (ghostHuntCounter == ghostHuntTime)
                                    {
                                        ghostHuntCounter = 0;
                                        ghostHunt = false;
                                    }

                                    // The displayed timer will change in every second.
                                    if (ghostHuntCounter % (ghostHuntTime / 10) == 0)
                                    {
                                        // Refresh is needed.
                                        refreshDisplay = true;
                                    }
                                }
                                // The ghost hunt is off.
                                else
                                {
                                    // The multiplier is not the default value.
                                    if (ghostScoreMultiplier > 1)
                                    {
                                        // Set the multiplier to the default value.
                                        ghostScoreMultiplier = 1;
                                    }
                                }

                                #endregion GhostHuntCounter

                                #region GhostEaten

                                // The position of the eaten ghost is not out of the map.
                                if (ghostDied.X != mapHeight + 1 && ghostDied.Y != mapWidth + 1)
                                {
                                    // Check each ghost.
                                    for (int j = 0; j < ghosts.Count; j++)
                                    {
                                        // The position of the ghost maches the position of the eaten ghost.
                                        if (ghosts[j].Position.X == ghostDied.X && ghosts[j].Position.Y == ghostDied.Y)
                                        {
                                            // Set the needed values.
                                            ghosts[j].InWait = true;

                                            // The first standby position is not empty.
                                            if (map[10, 12] == 'n')
                                            {
                                                // The second standby position is not empty.
                                                if (map[10, 13] == 'n')
                                                {
                                                    // The third standby position is not empty.
                                                    if (map[10, 14] == 'n')
                                                    {
                                                        // Put the ghost on the fourth standby position.
                                                        ghosts[j].Reposition(new Position(10, 15), ref map);
                                                        refreshGhostSpawn = true;
                                                    }
                                                    // The third standby position is empty.
                                                    else
                                                    {
                                                        // Put the ghost on the third standby position.
                                                        ghosts[j].Reposition(new Position(10, 14), ref map);
                                                        refreshGhostSpawn = true;
                                                    }
                                                }
                                                // The second standby position is empty.
                                                else
                                                {
                                                    // Put the ghost on the second standby position.
                                                    ghosts[j].Reposition(new Position(10, 13), ref map);
                                                    refreshGhostSpawn = true;
                                                }
                                            }
                                            // The first standby position is empty.
                                            else
                                            {
                                                // Put the ghost on the first standby position.
                                                ghosts[j].Reposition(new Position(10, 12), ref map);
                                                refreshGhostSpawn = true;
                                            }

                                            // Set the position of the eaten ghost outside the map.
                                            ghostDied.X = mapWidth + 1;
                                            ghostDied.Y = mapHeight + 1;

                                            // Refresh is needed.
                                            refreshDisplay = true;
                                            break;
                                        }
                                    }
                                }

                                #endregion GhostEaten

                                #region MoveGhosts

                                int standbyCounter = 0;

                                // Check each ghost.
                                for (int i = 0; i < ghosts.Count; i++)
                                {
                                    // The ghost is in wait.
                                    if (ghosts[i].InWait)
                                    {
                                        // Increase counter.
                                        standbyCounter++;
                                    }
                                }

                                #region StartGhost

                                // There is at leat one ghost on standby, it needs to be started.
                                if (standbyCounter > 0)
                                {
                                    startGhostCounter++;

                                    // 
                                    if (startGhostCounter == startGhotTime)
                                    {
                                        // Check each ghost.
                                        for (int i = 0; i < ghosts.Count; i++)
                                        {
                                            // The ghost is in wait.
                                            if (ghosts[i].InWait)
                                            {
                                                // The ghost is on the ghost spawn position.
                                                if (ghosts[i].Position.X == ghostSpawn.X && ghosts[i].Position.Y == ghostSpawn.Y)
                                                {
                                                    // Start the ghost.
                                                    ghosts[i].InWait = false;

                                                    // Set the counter to zero to start again.
                                                    startGhostCounter = 0;
                                                    standbyCounter--;
                                                    break;
                                                }
                                                // The ghost isn't on the ghost spawn position.
                                                else
                                                {
                                                    // The ghost spawn point isn't empty.
                                                    if (map[ghostSpawn.X, ghostSpawn.Y] != ' ')
                                                    {
                                                        // Delay the start a bit.
                                                        startGhostCounter--;
                                                        break;
                                                    }
                                                    // The ghost spawn point is empty.
                                                    else
                                                    {
                                                        // Start the ghost.
                                                        ghosts[i].InWait = false;

                                                        // Set the ghost to the start position.
                                                        ghosts[i].Reposition(ghostSpawn, ref map);
                                                        RefreshChar(ghosts[i].Previous, ghosts[i].PreviousCharUnder);

                                                        // Set the counter to zero to start again.
                                                        startGhostCounter = 0;
                                                        standbyCounter--;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                #endregion StartGhost

                                #region MoveGhost

                                // Not all of the ghosts are in wait, it needs to move.
                                if (standbyCounter < 4)
                                {
                                    moveGhostCounter++;

                                    // 
                                    if (moveGhostCounter == moveGhostTime)
                                    {
                                        // Check each ghost.
                                        for (int i = 0; i < ghosts.Count; i++)
                                        {
                                            // The ghost is not in wait.
                                            if (!ghosts[i].InWait)
                                            {
                                                // Move the ghost.
                                                ghosts[i].Move(ref map, ref pacman, ref ghostHunt, ghostSpawn, ref justDied, ref refreshDisplay, mapWidth, mapHeight);
                                            }
                                        }

                                        moveGhostCounter = 0;
                                    }
                                }

                                #endregion MoveGhost

                                #endregion MoveGhosts

                                #region PacmanDied

                                // Pacman died, set the characters to the default position.
                                if (justDied)
                                {
                                    RefreshChar(pacman.Previous, ' ');
                                    RefreshChar(pacman.Position, pacman.Draw());

                                    ghosts[0].Reposition(ghostSpawn, ref map);
                                    RefreshChar(ghosts[0].Previous, ghosts[0].PreviousCharUnder);
                                    RefreshChar(ghosts[0].Position, ghosts[0].Draw());
                                    ghosts[0].InWait = true;

                                    ghosts[1].Reposition(new Position(10, 12), ref map);
                                    RefreshChar(ghosts[1].Previous, ghosts[1].PreviousCharUnder);
                                    RefreshChar(ghosts[1].Position, ghosts[1].Draw());
                                    ghosts[1].InWait = true;

                                    ghosts[2].Reposition(new Position(10, 13), ref map);
                                    RefreshChar(ghosts[2].Previous, ghosts[2].PreviousCharUnder);
                                    RefreshChar(ghosts[2].Position, ghosts[2].Draw());
                                    ghosts[2].InWait = true;

                                    ghosts[3].Reposition(new Position(10, 14), ref map);
                                    RefreshChar(ghosts[3].Previous, ghosts[3].PreviousCharUnder);
                                    RefreshChar(ghosts[3].Position, ghosts[3].Draw());
                                    ghosts[3].InWait = true;

                                    refreshDisplay = true;
                                    refreshGhostSpawn = true;
                                    justDied = false;
                                }

                                #endregion PacmanDied

                                #region ZeroLife

                                // The life reached zero, game over.
                                if (pacman.Life < 1)
                                {
                                    //checkHighscores = true;
                                    gameStatus = GameStages.Menu;
                                    gamePreset = true;
                                    break;
                                }

                                #endregion ZeroLife
                            }

                            #endregion HandleChanges

                            #region GameRefresh

                            // Stall the movement of the pacman for the rest of the time.
                            pacman.MoveDirection = Pacman.MoveDirections.Stay;

                            // The display needs to be refreshed.
                            if (refreshDisplay)
                            {
                                #region Text

                                #region Life

                                // Display the life.
                                Console.SetCursorPosition(6, 23);
                                Console.Write(pacman.Life);
                                Console.SetCursorPosition(0, 24);

                                #endregion Life

                                #region Score

                                // Display the player's score.
                                if (pacman.Score / 10000 > 0)
                                {
                                    Console.SetCursorPosition(16, 23);
                                }
                                else if (pacman.Score / 1000 > 0)
                                {
                                    Console.SetCursorPosition(16, 23);
                                    Console.Write("0");
                                    Console.SetCursorPosition(17, 23);
                                }
                                else if (pacman.Score / 100 > 0)
                                {
                                    Console.SetCursorPosition(16, 23);
                                    Console.Write("00");
                                    Console.SetCursorPosition(18, 23);
                                }
                                else if (pacman.Score / 10 > 0)
                                {
                                    Console.SetCursorPosition(16, 23);
                                    Console.Write("000");
                                    Console.SetCursorPosition(19, 23);
                                }
                                else
                                {
                                    Console.SetCursorPosition(16, 23);
                                    Console.Write("0000");
                                    Console.SetCursorPosition(20, 23);
                                }

                                Console.Write(pacman.Score);
                                Console.SetCursorPosition(0, 24);

                                #endregion Score

                                #region Collectables

                                // Display the number of collectables.
                                if (collectables / 100 > 0)
                                {
                                    Console.SetCursorPosition(37, 23);
                                }
                                else if (collectables / 10 > 0)
                                {
                                    Console.SetCursorPosition(37, 23);
                                    Console.Write("0");
                                    Console.SetCursorPosition(38, 23);
                                }
                                else
                                {
                                    Console.SetCursorPosition(38, 23);
                                    Console.Write("0");
                                    Console.SetCursorPosition(39, 23);
                                }

                                Console.Write(collectables);
                                Console.SetCursorPosition(0, 24);

                                #endregion Collectables

                                #region GhostHunt

                                // Display the ghost hunt time.
                                if ((10 - (ghostHuntCounter / (ghostHuntTime / 10))) / 10 > 0 && ghostHunt)
                                {
                                    Console.SetCursorPosition(54, 23);
                                }
                                else
                                {
                                    Console.SetCursorPosition(54, 23);
                                    Console.Write("0");
                                    Console.SetCursorPosition(55, 23);
                                }

                                Console.Write(ghostHuntCounter > 0 ? 10 - (ghostHuntCounter / (ghostHuntTime / 10)) : 0);
                                Console.SetCursorPosition(0, 24);

                                #endregion GhostHunt

                                #endregion Text

                                #region Character

                                #region Pacman

                                // The previous position of the pacman is not outside of the map.
                                if (pacman.Previous.X != mapHeight + 1 && pacman.Previous.Y != mapWidth + 1)
                                {
                                    Console.SetCursorPosition(pacman.Previous.Y, pacman.Previous.X);
                                    Console.Write(" ");
                                }

                                Console.SetCursorPosition(pacman.Position.Y, pacman.Position.X);

                                // Display pacman in color.
                                if (coloredMap)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(pacman.Draw());
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                }
                                // Display pacman in black and white.
                                else
                                {
                                    Console.Write(pacman.Draw());
                                }

                                Console.SetCursorPosition(0, 24);

                                #endregion Pacman

                                #region Ghost

                                // Check each ghost.
                                for (int i = 0; i < ghosts.Count; i++)
                                {
                                    // The ghost is not in wait.
                                    if (!ghosts[i].InWait)
                                    {
                                        Console.SetCursorPosition(ghosts[i].Previous.Y, ghosts[i].Previous.X);

                                        // Display the ghost in color.
                                        if (coloredMap)
                                        {
                                            // Set the color based on the item.
                                            switch (ghosts[i].PreviousCharUnder)
                                            {
                                                case '#':
                                                case '-':
                                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                                    break;
                                                case '.':
                                                case '*':
                                                    Console.ForegroundColor = ConsoleColor.White;
                                                    break;
                                            }

                                            Console.Write(ghosts[i].PreviousCharUnder);
                                            Console.SetCursorPosition(ghosts[i].Position.Y, ghosts[i].Position.X);

                                            // The hunt for the ghosts isn't on.
                                            if (!ghostHunt)
                                            {
                                                // Set the color based on the ghost.
                                                switch (i.ToString())
                                                {
                                                    case "0":
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        break;
                                                    case "1":
                                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                                        break;
                                                    case "2":
                                                        Console.ForegroundColor = ConsoleColor.Blue;
                                                        break;
                                                    case "3":
                                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                                        break;
                                                }
                                            }
                                            // The hunt for the ghosts is on.
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.Gray;
                                            }

                                            Console.Write(ghosts[i].Draw());
                                            Console.ForegroundColor = ConsoleColor.Gray;
                                        }
                                        // Display the ghost in black and white.
                                        else
                                        {
                                            Console.Write(ghosts[i].PreviousCharUnder);
                                            Console.SetCursorPosition(ghosts[i].Position.Y, ghosts[i].Position.X);
                                            Console.Write(ghosts[i].Draw());
                                        }
                                    }
                                }

                                Console.SetCursorPosition(0, 24);

                                // Refresh the ghost spawn area.
                                if (refreshGhostSpawn)
                                {
                                    // Check each ghost.
                                    for (int i = 0; i < ghosts.Count; i++)
                                    {
                                        // The ghost is in the ghost spawn area.
                                        if (ghosts[i].Position.X == 10 && ghosts[i].Position.Y >= 12 && ghosts[i].Position.Y <= 15)
                                        {
                                            Console.SetCursorPosition(ghosts[i].Position.Y, ghosts[i].Position.X);

                                            // Display the ghost in color.
                                            if (coloredMap)
                                            {
                                                // Set the color based on the ghost.
                                                switch (i.ToString())
                                                {
                                                    case "0":
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        break;
                                                    case "1":
                                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                                        break;
                                                    case "2":
                                                        Console.ForegroundColor = ConsoleColor.Blue;
                                                        break;
                                                    case "3":
                                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                                        break;
                                                }

                                                Console.Write(ghosts[i].Draw());
                                                Console.ForegroundColor = ConsoleColor.Gray;
                                            }
                                            // Display the ghost in black and white.
                                            else
                                            {
                                                Console.Write(ghosts[i].Draw());
                                            }
                                        }
                                    }

                                    Console.SetCursorPosition(0, 24);
                                    refreshGhostSpawn = false;
                                }

                                #endregion Ghost

                                #endregion Character

                                refreshDisplay = false;
                            }

                            #endregion GameRefresh
                        }

                        // Check the player's score if it's in the highscores.
                        if (checkHighscores)
                        {
                            #region RefreshDisplay

                            StoreHighscore();

                            #endregion RefreshDisplay

                            gamePreset = true;
                        }
                    }

                    #endregion Game

                    #region Highscores

                    // The highscore stage.
                    else if (gameStatus == GameStages.Highsores)
                    {
                        #region RefreshDisplay

                        DisplayHighscores();

                        #endregion RefreshDisplay

                        // Repeat the key read until the escape is pushed.
                        while (gameStatus == GameStages.Highsores)
                        {
                            #region HandleInput

                            // A key was pushed.
                            if (Console.KeyAvailable)
                            {
                                // Handle the key input.
                                if (Console.ReadKey().Key == ConsoleKey.Escape)
                                {
                                    gameStatus = GameStages.Menu;
                                }

                                // Delete the read key input.
                                DeleteKeyInput();
                            }

                            #endregion HandleInput
                        }
                    }

                    #endregion Highscores

                    #region Exit

                    // The exit stage.
                    else if (gameStatus == GameStages.Exit)
                    {
                        // End the program.
                        gameFinished = true;
                    }

                    #endregion Exit
                }
            }
            catch
            { }
        }

        #region Functions

        /// <summary>
        /// Overwrites the given position with the given character.
        /// </summary>
        /// <param name="position">The position to write to.</param>
        /// <param name="character">The character to write.</param>
        private static void RefreshChar(Position position, char character)
        {
            try
            {
                Console.SetCursorPosition(position.Y, position.X);

                // Display the ghost in color.
                if (coloredMap)
                {
                    // Handles the characters.
                    switch (character)
                    {
                        case '#':
                        case '-':
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            break;
                        case '.':
                        case '*':
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case 'O':
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case 'n':
                            if (position.X == ghosts[0].Position.X && position.Y == ghosts[0].Position.Y)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            else if (position.X == ghosts[1].Position.X && position.Y == ghosts[1].Position.Y)
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                            }
                            else if (position.X == ghosts[2].Position.X && position.Y == ghosts[2].Position.Y)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                            }
                            else if (position.X == ghosts[3].Position.X && position.Y == ghosts[3].Position.Y)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                            }
                            break;
                        case ' ':
                            break;
                    }

                    Console.Write(character);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                // Display the ghost in black and white.
                else
                {
                    Console.Write(character);
                }

                Console.SetCursorPosition(0, 24);
            }
            catch
            { }
        }

        /// <summary>
        /// Deletes the read key input.
        /// </summary>
        private static void DeleteKeyInput()
        {
            try
            {
                Console.CursorLeft -= 1;
                Console.Write(" ");
                Console.CursorLeft -= 1;
            }
            catch
            { }
        }

        /// <summary>
        /// Displays the menu.
        /// </summary>
        private static void DisplayMenu()
        {
            try
            {
                // Clear the console.
                Console.Clear();

                // Display the menu.
                Console.WriteLine("Press the number of the option.");
                Console.WriteLine();
                Console.WriteLine("1. Start Game");
                Console.WriteLine();
                Console.WriteLine("2. Highscores");
                Console.WriteLine();
                Console.WriteLine("3. Exit");
                Console.WriteLine();
            }
            catch
            { }
        }

        /// <summary>
        /// Displays the highscores.
        /// </summary>
        private static void DisplayHighscores()
        {
            try
            {
                // Clear the console.
                Console.Clear();

                // Display the highscores.
                Console.WriteLine("Highscores.");
                Console.WriteLine();

                try
                {
                    // Get the highscores from the file.
                    highscoresList = xmlAccess.LoadScores();

                    // There is at leat one highsore in the list.
                    if (highscoresList != null && highscoresList.Count > 0)
                    {
                        // Sort the highscores in the list in a descending order.
                        highscoresList.Sort((x, y) => y.Score.CompareTo(x.Score));

                        // Check each highscore.
                        foreach (var oneItem in highscoresList)
                        {
                            // Show the highscore.
                            Console.WriteLine(oneItem.ToString());
                        }
                    }
                }
                catch
                { }

                // Display the return message.
                Console.WriteLine();
                Console.WriteLine("Press Esc to return to menu.");
            }
            catch
            { }
        }

        /// <summary>
        /// Displays the map.
        /// </summary>
        private static void DisplayMap()
        {
            try
            {
                // Clear the console.
                Console.Clear();

                // The rows of the map.
                for (int i = 0; i < mapHeight; i++)
                {
                    // The columns of the map.
                    for (int j = 0; j < mapWidth; j++)
                    {
                        // Show colored map.
                        if (coloredMap)
                        {
                            // Set the color based on the map item.
                            switch (map[i, j])
                            {
                                case '#':
                                case '-':
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                    break;
                                case '.':
                                case '*':
                                    Console.ForegroundColor = ConsoleColor.White;
                                    break;
                                case 'O':
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case 'n':
                                    if (i == ghosts[0].Position.X && j == ghosts[0].Position.Y)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                    }
                                    else if (i == ghosts[1].Position.X && j == ghosts[1].Position.Y)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Magenta;
                                    }
                                    else if (i == ghosts[2].Position.X && j == ghosts[2].Position.Y)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Blue;
                                    }
                                    else if (i == ghosts[3].Position.X && j == ghosts[3].Position.Y)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    }
                                    break;
                                case ' ':
                                    break;
                            }

                            // Show the item and reset the color.
                            Console.Write(map[i, j]);
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        // Show black and white map.
                        else
                        {
                            Console.Write(map[i, j]);
                        }
                    }

                    // Brake the line.
                    Console.WriteLine();
                }

                // Show the information line.
                Console.WriteLine("Life: {0:0}, Score: {1:00000}, Collectables: {2:000}, Ghost Hunt: {3:00}, Exit: Press Esc.",
                    pacman.Life, pacman.Score, collectables, 0);
            }
            catch
            { }
        }

        /// <summary>
        /// Stores a new highscore.
        /// </summary>
        private static void StoreHighscore()
        {
            try
            {
                // Clear the console.
                Console.Clear();

                List<Highscore> scoreList = new List<Highscore>();

                try
                {
                    // Load the scores.
                    scoreList = xmlAccess.LoadScores();

                    // The scores are valid.
                    if (scoreList != null && scoreList.Count > 0)
                    {
                        // Sort the scores in descending order.
                        scoreList.Sort((x, y) => y.Score.CompareTo(x.Score));

                        bool found = false;
                        checkHighscores = false;
                        string name = "";

                        // Check each highscore.
                        for (int i = 0; i < scoreList.Count; i++)
                        {
                            // The player's score is bigger then any of the highscores.
                            if (scoreList[i].Score < pacman.Score)
                            {
                                // No mach found previously. 
                                if (!found)
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }

                        // Try to submit it.
                        if (found)
                        {
                            // Ask if the player would like to submit the score.
                            Console.WriteLine("Your score made it to the highscores, would you like to submit it? Y or N.");
                            ConsoleKey pressed;

                            // Read the answer.
                            do
                            {
                                // Store the given answer.
                                pressed = (ConsoleKey)Console.ReadKey().Key;

                                // Delete the read key input.
                                DeleteKeyInput();
                            } while (pressed != ConsoleKey.Y && pressed != ConsoleKey.N);

                            // Submit.
                            if (pressed == ConsoleKey.Y)
                            {
                                // Get the player's name.
                                Console.WriteLine("Enter your name: ");
                                name = Console.ReadLine();

                                // The player entered a name.
                                if (!string.IsNullOrEmpty(name))
                                {
                                    // Add the highscore to the list.
                                    scoreList.Add(new Highscore(0, name, pacman.Score));
                                    scoreList.Sort((x, y) => y.Score.CompareTo(x.Score));

                                    // Remove the extra items.
                                    if (scoreList.Count >= scoreLimit)
                                    {
                                        int j = scoreList.Count - 1;

                                        while (j >= scoreLimit)
                                        {
                                            scoreList.RemoveAt(j);
                                            j--;
                                        }
                                    }

                                    // Save the highscores.
                                    xmlAccess.SaveScores(scoreList);
                                }
                            }
                        }
                    }
                }
                catch
                { }
            }
            catch
            { }
        }

        #endregion Functions

        #endregion Methods
    }
}
