using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Diagnostics;   // stopwatch 사용을 위해 사용
using System.Threading;


namespace File
{
    enum FIELD
    {
        SPACE,
        WALL
    }
    enum MOVE
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int index { get; set; }

    }
    class Const
    {
        public const int X_SIZE = 70;
        public const int Y_SIZE = 50;
        public const int BASE_SPEED = 1000;
    }

    class Snake
    {
        private Position[] snakePos;
        private int snakeLength;
        private MOVE snakeMoveDirection;

        public Snake()
        {
            snakePos = new Position[(Const.X_SIZE - 4)/2 * (Const.Y_SIZE - 2)];
            snakePos[0].Y = Const.Y_SIZE / 2;
            snakePos[0].X = Const.X_SIZE / 2;
            if (snakePos[0].X % 2 != 0)
                snakePos[0].X += 1;

            // 기존 디폴트 값은 0,0이므로 벽 위치랑 중복되므로 벽이 지워진다. 맵 구성요소에 영향을 미치지 않는 영역 값을 넣아준다.
            for (int i = 1; i< (Const.X_SIZE - 4) / 2 * (Const.Y_SIZE - 2); i++ )
            {
                snakePos[i].Y = Const.Y_SIZE -1;
                snakePos[i].X = Const.X_SIZE;
            }
            

            snakeLength = 1;
            snakeMoveDirection = MOVE.RIGHT;
        }

        public int SnakeLength { get => snakeLength; set => snakeLength = value; }
        internal MOVE SnakeMoveDirection { get => snakeMoveDirection; set => snakeMoveDirection = value; }
        internal Position[] SnakePos { get => snakePos; set => snakePos = value; }
    }

    class DotData
    {
        private Position[] spawnDotArea;
        private int numberOfArea;
        private int endPoint;
        private Position dot;

        public DotData()
        {
            spawnDotArea = new Position[(Const.X_SIZE - 4) / 2 * (Const.Y_SIZE - 2)+1];
            numberOfArea = 0;
            for (int y = 1; y < (Const.Y_SIZE - 1); y++)
            {
                for (int x = 2; x < (Const.X_SIZE - 2); x += 2)
                {
                    Position pos = new Position();
                    pos.X = x;
                    pos.Y = y;
                    pos.index = numberOfArea;
                    spawnDotArea[numberOfArea++] = pos;
                }
            }
            endPoint = numberOfArea-1;
        }

        public int NumberOfArea { get => numberOfArea; set => numberOfArea = value; }
        internal Position[] SpawnDotArea { get => spawnDotArea; set => spawnDotArea = value; }
        internal Position Dot { get => dot; set => dot = value; }

        public void AreaBan(Snake snake)
        {
            for (int i = 0; i < snake.SnakeLength; i++)
            {
                int index = (snake.SnakePos[i].Y-1) * (Const.X_SIZE/2-2) + (snake.SnakePos[i].X/2) -1;
                Position temp = spawnDotArea[index];
                spawnDotArea[index] = spawnDotArea[numberOfArea];
                spawnDotArea[numberOfArea] = temp;
            }
        }

        public void AreaFree(Snake snake)
        {
            int index = spawnDotArea[endPoint].index;
            Position temp = spawnDotArea[index];
            spawnDotArea[index] = spawnDotArea[endPoint];
            spawnDotArea[endPoint] = temp;
        }

        public void CreateDot()
        {
            numberOfArea--;
            Random random = new Random();
            int randomIndex = random.Next(0, numberOfArea - 1);
            dot.X = spawnDotArea[randomIndex].X;
            dot.Y = spawnDotArea[randomIndex].Y;

            Console.SetCursorPosition(dot.X, dot.Y);
            Console.ForegroundColor = ConsoleColor.Green;
            Write("□");
            Console.ForegroundColor = ConsoleColor.White;

        }
    }

    class SnakeGame
    {
        private Snake snake;
        private FIELD[,] gameField;
        private DotData dotData;
        private int score;
        private int speed;
        private int x;
        private bool endFlag;
        private bool pauseFlag;
        private Stopwatch stopwatch;

        public void GameHandler()
        {
            InitGame();
            Thread getKeyboardInputThread = new Thread(GetKeyboardInput);
            getKeyboardInputThread.Start();

            while (!endFlag)
            {
                if (!pauseFlag)
                {
                    Thread printMoveThread = new Thread(PrintAndMove);
                    printMoveThread.Start();
                    printMoveThread.Join();
                    
                }
            }
        }
        public bool InitGame()
        {
            snake = new Snake();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            endFlag = false;
            pauseFlag = false;
            score = 0;
            x = 1;
            speed = Const.BASE_SPEED / (x * 5);

            MakeAndPrintWall();
            PrintManual();

            // create Dot spawn Area
            dotData = new DotData();
            dotData.CreateDot();

            return true;
        }
        public void MakeAndPrintWall()
        {
            gameField = new FIELD[Const.Y_SIZE, Const.X_SIZE];

            // Create Wall
            for (int y = 0; y < Const.Y_SIZE; y++)
            {
                for (int x = 0; x < Const.X_SIZE; x += 2)
                {
                    if (x == 0 | x == (Const.X_SIZE - 2))
                        gameField[y, x] = FIELD.WALL;
                    if (y == 0 | y == (Const.Y_SIZE - 1))
                        gameField[y, x] = FIELD.WALL;
                }
            }

            // print Field
            for (int x = 0; x < Const.X_SIZE; x += 2)
            {
                for (int y = 0; y < Const.Y_SIZE; y++)
                {
                    if (gameField[y, x] == FIELD.WALL)
                    {
                        Console.SetCursorPosition(x, y);
                        Write("■");
                    }
                }
            }
        }
        public void PrintManual()
        {
            int manualX = Const.X_SIZE +1;
            int manualY = 10;

            Console.SetCursorPosition(manualX, manualY++);
            Write("MOVE  : ←↑↓→");
            Console.SetCursorPosition(manualX, manualY++);
            Write("Space : pause");
            Console.SetCursorPosition(manualX, manualY++);
            Write("Z, X  : Speed Control");
            Console.SetCursorPosition(manualX, manualY++);
            Write("Enter : Restart Game");

            manualY += 5;
            Console.SetCursorPosition(manualX, manualY++);
            Write("----DEBUG CONTROL----");
            Console.SetCursorPosition(manualX, manualY++);
            Write("+ : Add Length ");
            Console.SetCursorPosition(manualX, manualY++);
            Write("- : Sub Length ");


        }

        public void MoveSnake()
        {
            for (int i = snake.SnakeLength - 1; i >= 0; i--)
            {
                snake.SnakePos[i + 1] = snake.SnakePos[i];
            }

            switch (snake.SnakeMoveDirection)
            {
                case MOVE.LEFT:
                    snake.SnakePos[0].X -= 2;
                    break;
                case MOVE.RIGHT:
                    snake.SnakePos[0].X += 2;
                    break;
                case MOVE.UP:
                    snake.SnakePos[0].Y -= 1;
                    break;
                case MOVE.DOWN:
                    snake.SnakePos[0].Y += 1;
                    break;
                default:
                    break;
            }

            if (IsCollision())
                endFlag = true;
            else
            {
                dotData.AreaBan(snake);
                dotData.AreaFree(snake);
            }

        }

        public bool IsCollision()
        {
            Position snakeHead = snake.SnakePos[0];

            if (snakeHead.X == 0 || snakeHead.X == (Const.X_SIZE - 2))
                return true;
            else if (snakeHead.Y == 0 || snakeHead.Y == (Const.Y_SIZE - 1))
                return true;

            for (int i = 1; i <= snake.SnakeLength; i++)
            {
                if (System.Object.Equals(snakeHead, snake.SnakePos[i]))
                    return true;
            }

            if (System.Object.Equals(snakeHead,dotData.Dot))
            {
                snake.SnakeLength++;
                score += 1000;
                dotData.CreateDot();
            }
                
            return false;
        }

        public void PrintScreen()
        {
            // print score
            Console.SetCursorPosition(Const.X_SIZE+1, 0);
            Write($"Score : {score}");

            // print time
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            stopwatch.Start();
            Console.SetCursorPosition(Const.X_SIZE+1, 1);
            Write($"Time  : {ts.Seconds}");

            // print x1,x2,x3...
            Console.SetCursorPosition(Const.X_SIZE+1, 2);
            Write($"배속  : x{x}");

            // print snake
            int snakeLen = snake.SnakeLength;
            Console.SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);
            Write("□");
            Console.SetCursorPosition(snake.SnakePos[snakeLen].X, snake.SnakePos[snakeLen].Y);
            Write("  ");

            Console.SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);

        }

        public void PrintAndMove()
        {
            PrintScreen();
            MoveSnake();
            Thread.Sleep(speed);
        }


        public void GetKeyboardInput()
        {
            while (!endFlag)
            {
                ConsoleKeyInfo keys;
                if (Console.KeyAvailable)
                {
                    keys = Console.ReadKey(true);
                    switch (keys.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (snake.SnakeMoveDirection != MOVE.DOWN)
                                snake.SnakeMoveDirection = MOVE.UP;
                            break;
                        case ConsoleKey.DownArrow:
                            if (snake.SnakeMoveDirection != MOVE.UP)
                                snake.SnakeMoveDirection = MOVE.DOWN;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (snake.SnakeMoveDirection != MOVE.RIGHT)
                                snake.SnakeMoveDirection = MOVE.LEFT;
                            break;
                        case ConsoleKey.RightArrow:
                            if (snake.SnakeMoveDirection != MOVE.LEFT)
                                snake.SnakeMoveDirection = MOVE.RIGHT;
                            break;
                        case ConsoleKey.Add:
                            snake.SnakeLength++;
                            dotData.NumberOfArea--;
                            break;
                        case ConsoleKey.Subtract:
                            snake.SnakeLength--;
                            break;
                        case ConsoleKey.Spacebar:
                            if (!pauseFlag)
                            {
                                pauseFlag = !pauseFlag;
                                stopwatch.Stop();
                            }
                            else
                            {
                                pauseFlag = !pauseFlag;
                                stopwatch.Start();
                            }
                            break;
                        case ConsoleKey.Enter:
                            endFlag = true;
                            break;
                        case ConsoleKey.Z:
                            x--;
                            if (x < 1)
                                x = 1;
                            speed = Const.BASE_SPEED / (x * 5);
                            break;
                        case ConsoleKey.X:
                            x++;
                            if (x > 5)
                                x = 5;
                            speed = Const.BASE_SPEED / (x * 5);
                            break;

                        default:
                            break;
                    }
                }
                Thread.Sleep(20);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                SnakeGame snakeGame = new SnakeGame();
                snakeGame.GameHandler();
                Clear();
            }
        }
    }
}
