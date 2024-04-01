using System;
using System.Drawing;
using System.Threading;

class Program
{
    // Het doolhof als een reeks strings
    public static string[] maze =
    {
        "\u250F" + new string('\u2501', 68) + "\u2513",
        "\u2503.\u263B....................................╦╦╦╦╦╦╦.......................\u2503",
        "\u2503.......................♥..............╦╦╦╦╦╦╦.......................\u2503",
        "\u2503......................................╦╦╦╦╦╦╦.......................\u2503",
        "\u2503..........................................................♣.........\u2503",
        "\u2503...╦╦╦╦╦╦╦....╦╦╦╦╦╦╦...............................................\u2503",
        "\u2503...╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦............♦..................................\u2503",
        "\u2503...╦╦╦╦╦╦╦....╦╦╦╦╦╦╦...............................................\u2503",
        "\u2503....................................................................\u2517\u2501\u2501",
        "\u2503..........................╦╦╦╦╦╦╦....╦╦╦╦╦╦╦...........♥.............☼",
        "\u2503..........................╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╠╣.............\u250F\u2501\u2501",
        "\u2503....♥.....................╦╦╦╦╦╦╦....╦╦╦╦╦╦╦.........╠╣.............\u2503",
        "\u2503..................................................╦╦╦╦╦╦╦...........\u2503",
        "\u2503......╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦.............................╦╦╦╦╦╦╦...........\u2503",
        "\u2503......╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦.............................╦╦╦╦╦╦╦...........\u2503",
        "\u2503......╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦╦............♣.................................\u2503",
        "\u2503.............................................................♦......\u2503",
        "\u2517" + new string('\u2501', 68) + "\u251B",
    };

    // Score van het spel
    public static int score = 0;

    // Object voor synchronisatie tussen threads
    public static object lockObject = new object();

    static void Main(string[] args)
    {
        try
        {
            // Initialisatie van de console
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            bool gameRunning = true;
            bool needsRedraw = true;
            bool gameOver = false;

            // Creëer Pac-Man en spoken
            Character pacMan = new PacMan(2, 1);
            Character ghost1 = new Ghost(14, 10);
            Character ghost2 = new Ghost(50, 7);
            Character ghost3 = new Ghost(16, 4);
            Character ghost4 = new Ghost(40, 15);

            // Start threads voor het bewegen van spoken
            Thread ghost1Thread = new Thread(() => GhostMovementLoop(ghost1, pacMan));
            ghost1Thread.Start();

            Thread ghost2Thread = new Thread(() => GhostMovementLoop(ghost2, pacMan));
            ghost2Thread.Start();

            Thread ghost3Thread = new Thread(() => GhostMovementLoop(ghost3, pacMan));
            ghost3Thread.Start();

            Thread ghost4Thread = new Thread(() => GhostMovementLoop(ghost4, pacMan));
            ghost4Thread.Start();

            // Hoofdloop van het spel
            while (gameRunning)
            {
                // Logica voor het bijwerken en weergeven van het spel

                if (needsRedraw)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Welkom op One Happy Man!!!\n");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Instructies:");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Gebruik de pijltjestoetsen ▲▼◄ ► om Pac-Man ☻ te bewegen.");

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Verzamel de bonusitems (♥, ♣, ♦) om punten te verdienen.");

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vermijd de spoken (Д) om te overleven!\n");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    DrawMaze(pacMan, ghost1, ghost2, ghost3, ghost4);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nScore: {score}");

                    needsRedraw = false;
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        pacMan.Move(0, -1);
                        needsRedraw = true;
                        break;
                    case ConsoleKey.DownArrow:
                        pacMan.Move(0, 1);
                        needsRedraw = true;
                        break;
                    case ConsoleKey.LeftArrow:
                        pacMan.Move(-1, 0);
                        needsRedraw = true;
                        break;
                    case ConsoleKey.RightArrow:
                        pacMan.Move(1, 0);
                        needsRedraw = true;
                        break;
                    case ConsoleKey.Escape:
                        gameRunning = false;
                        break;
                }


                if (needsRedraw && (pacMan.CollidesWith(ghost1) || pacMan.CollidesWith(ghost2) || pacMan.CollidesWith(ghost3) || pacMan.CollidesWith(ghost4)))
                {
                    gameOver = true;
                    needsRedraw = true;
                }
                if (needsRedraw && gameOver)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game Over! Druk op een toets om opnieuw te starten.");
                    Console.ReadKey(true);
                    ResetGame(pacMan, ghost1, ghost2, ghost3, ghost4);
                    gameOver = false;
                    needsRedraw = true;
                }


            }
        }
        catch (Exception ex)
        {
            // Vang eventuele fouten op en geef een bericht weer
            Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
        }
    }

    // Tekent het doolhof met Pac-Man en spoken
    // Tekent het doolhof met Pac-Man en spoken
    static void DrawMaze(Character pacMan, Character ghost1, Character ghost2, Character ghost3, Character ghost4)
    {
        // Loop door het doolhof en plaats Pac-Man en spoken op de juiste posities
        for (int y = 0; y < maze.Length; y++)
        {
            char[] rowChars = maze[y].ToCharArray();

            // Replace the characters in the maze with ' ' before redrawing the characters
            for (int x = 0; x < rowChars.Length; x++)
            {
                if (rowChars[x] == '\u263B' || rowChars[x] == 'Д')
                {
                    rowChars[x] = '.'; // Replace with food, assuming Pac-Man or ghosts were there

                }

            }
            

            // Place Pac-Man in the maze
            if (y == pacMan.Y)rowChars[pacMan.X] = '\u263B';

            

            // Place ghosts in the maze
            if (y == ghost1.Y)rowChars[ghost1.X] = 'Д';
            if (y == ghost2.Y) rowChars[ghost2.X] = 'Д';
            if (y == ghost3.Y) rowChars[ghost3.X] = 'Д';
            if (y == ghost4.Y) rowChars[ghost4.X] = 'Д';
           
            // Convert the row back to a string and write it to the console
            Console.WriteLine(new string(rowChars));
        }

        // Reset console color after drawing
        Console.ForegroundColor = ConsoleColor.White;
    }

    // Reset het spel naar de beginstatus
    static void ResetGame(Character pacMan, Character ghost1, Character ghost2, Character ghost3, Character ghost4)
    {
        // Reset de positie van Pac-Man en spoken, de score en het doolhof
        pacMan.X = 2;
        pacMan.Y = 1;

        // Reset de positie van de spoken
        ghost1.X = 14;
        ghost1.Y = 10;
        ghost2.X = 50;
        ghost2.Y = 7;
        ghost3.X = 16;
        ghost3.Y = 4;
        ghost4.X = 40;
        ghost4.Y = 15;

        // Reset de score
        score = 0;

        // Reset het doolhof door een diepe kopie te maken van het originele doolhof
        maze = maze.Select(row => new string(row)).ToArray();

        // Reset de posities waar Pac-Man heeft gelopen en de bonusitems opnieuw heeft geplaatst
        for (int y = 0; y < maze.Length; y++)
        {
            for (int x = 0; x < maze[y].Length; x++)
            {
                // Check of de huidige cel een muur, Pac-Man, een spook of een bonusitem bevat
                if (maze[y][x] != '\u2501' && maze[y][x] != '\u2503' && // muren
                    maze[y][x] != '╦' && maze[y][x] != '╠' && maze[y][x] != '╣' && // muurdelen
                    maze[y][x] != '♥' && maze[y][x] != '♣' && maze[y][x] != '♦') // bonusitems
                {
                    // Is de huidige cel geen speciaal karakter, dan reset naar voedsel
                    char[] rowChars = maze[y].ToCharArray();
                    rowChars[x] = '.'; // Reset naar voedsel
                    maze[y] = new string(rowChars);
                }
            }
        }

        // Teken het doolhof opnieuw na het resetten van het spel
        DrawMaze(pacMan, ghost1, ghost2, ghost3, ghost4);
    }

    // Methode voor het bewegen van spoken in een aparte thread
    static void GhostMovementLoop(Character ghost, Character pacMan)
    {
        while (true)
        {
            // Verkrijg een slot op het lockObject om thread veiligheid te waarborgen
            lock (lockObject)
            {
                // Beweeg het spook
                ghost.Move(pacMan.X, pacMan.Y);
            }
            // Pauzeer het thread voor een korte periode om de beweging van de spoken te simuleren
            Thread.Sleep(500);
        }
    }
}

//Polymorphisme
//Abstracte klasse voor karakters in het spel
abstract class Character
{
    public int X { get; set; }
    public int Y { get; set; }

    // Abstracte methode voor het bewegen van een karakter
    public abstract void Move(int targetX, int targetY);

    // Virtuele methode om te controleren op botsingen tussen karakters
    public virtual bool CollidesWith(Character other)
    {
        return X == other.X && Y == other.Y;
    }
}

// Klasse voor Pac-Man, die een subklasse is van Character
class PacMan : Character
{
    // Constructor om de beginpositie van Pac-Man in te stellen
    public PacMan(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Implementatie van de Move-methode voor Pac-Man
    public override void Move(int targetX, int targetY)
    {
        // Logica voor het bewegen van Pac-Man en het controleren op botsingen
        lock (Program.lockObject)
        {
            int newY = Y + targetY;
            int newX = X + targetX;

            if (newY >= 0 && newY < Program.maze.Length && newX >= 0 && newX < Program.maze[newY].Length &&
                Program.maze[newY][newX] != '\u2501' && Program.maze[newY][newX] != '\u2503' &&
                Program.maze[newY][newX] != '╦' && Program.maze[newY][newX] != '╠' && Program.maze[newY][newX] != '╣')
            {
                if (Program.maze[newY][newX] == '♥') Program.score += 10;
                else if (Program.maze[newY][newX] == '♣') Program.score += 7;
                else if (Program.maze[newY][newX] == '♦') Program.score += 5;
                else if (Program.maze[newY][newX] == '.') Program.score++;

                Program.maze[Y] = Program.maze[Y].Substring(0, X) + ' ' + Program.maze[Y].Substring(X + 1);

                Y = newY;
                X = newX;

                Program.maze[Y] = Program.maze[Y].Substring(0, X) + '\u263B' + Program.maze[Y].Substring(X + 1);
            }
        }


    }

    

}

// Klasse voor spoken, die een subklasse is van Character
//Algoritme toegevoegd om Pacman te volgen
class Ghost : Character
{
    // Vorige positie van Pac-Man (voor bewegingslogica van het spook)
    private int prevPacManX;
    private int prevPacManY;

    // Constructor om de beginpositie van een spook in te stellen
    public Ghost(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Implementatie van de Move-methode voor spoken
    public override void Move(int targetX, int targetY)
    {
        // Verkrijg een slot op het lockObject om thread veiligheid te waarborgen
        lock (Program.lockObject)
        {
            // Bepaal de x en y afstanden van het spook naar Pac-Man
            int deltaX = targetX - X;
            int deltaY = targetY - Y;

            // Bepaal de richting waarin het spook moet bewegen door de afstanden te normaliseren
            int moveX = deltaX == 0 ? 0 : (deltaX > 0 ? 1 : -1);
            int moveY = deltaY == 0 ? 0 : (deltaY > 0 ? 1 : -1);

            // Probeer horizontaal te bewegen als dat mogelijk is
            if (moveX != 0 && CanMoveGhost(X + moveX, Y, Program.maze))
            {
                X += moveX;
            }
            // Als horizontaal bewegen niet mogelijk is, probeer dan verticaal te bewegen
            else if (moveY != 0 && CanMoveGhost(X, Y + moveY, Program.maze))
            {
                Y += moveY;
            }
            // Als geen enkele beweging mogelijk is, blijft het spook staan
        }
    }

    // Controleert of een spook kan bewegen naar een bepaalde positie in het doolhof
    static bool CanMoveGhost(int x, int y, string[] maze)
    {
        // Haal het karakter op de gewenste positie op en bepaal of het een muur of obstakel is
        char cell = maze[y][x];
        return cell != '\u2501' && cell != '\u2503' && cell != '╦' && cell != '╠' &&
               cell != '╣' && cell != '♣' && cell != '♦' && cell != '♥';
    }

    

}


