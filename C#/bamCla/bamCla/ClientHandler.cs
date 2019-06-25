using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace SnakeClient
{
    public class ClientHandler
    {
        MOVE moveDirection;
        private bool isDirectionChange;
        private FIELD[,] gameField;
        //private int score;
        private int gameSpeed;
        //private bool endFlag;
        //private bool pauseFlag;
        private Stopwatch stopwatch;
        private List<Position> dotPositions;
        private bool eos;

        public List<Position> DotPositions { get => dotPositions; set => dotPositions = value; }
        public int GameSpeed { get => gameSpeed; set => gameSpeed = value; }
        public bool Eos { get => eos; set => eos = value; }

        public void ProcedureGameHandler()
        {
            HandlerInit();

            while (true)
            {
                while (!isDirectionChange)
                {
                    GetKeyboardInput();
                    Thread.Sleep(10);
                }
                SnakeDirectionSend();
                

                isDirectionChange = false;
            }
        }

        public void HandlerInit()
        {
            stopwatch = new Stopwatch();
            dotPositions = new List<Position>();
            moveDirection = MOVE.RIGHT;
            isDirectionChange = false;
            MakeAndPrintWall();
            PrintManual();
            eos = false;
            gameSpeed = Const.BASE_SPEED / 5;
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
            int manualX = Const.X_SIZE + 1;
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


        public void PrintPhase()
        {
            try
            {
                PrintScreen();
                PrintDot();
                PrintSnake();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void PrintScreen()
        {
            //// print score
            //Console.SetCursorPosition(Const.X_SIZE + 1, 0);
            //Write($"Score : {score}");

            //// print time
            //stopwatch.Stop();
            //TimeSpan ts = stopwatch.Elapsed;
            //stopwatch.Start();
            //Console.SetCursorPosition(Const.X_SIZE + 1, 1);
            //Write($"Time  : {ts.Seconds}");

            //// print x1,x2,x3...
            //Console.SetCursorPosition(Const.X_SIZE + 1, 2);
            //Write($"배속  : x{x}");

            //// print snake
            //int snakeLen = snake.SnakeLength;
            //Console.SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);
            //Write("□");
            //Console.SetCursorPosition(snake.SnakePos[snakeLen].X, snake.SnakePos[snakeLen].Y);
            //Write("  ");

            //Console.SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);
        }
        public void PrintDot()
        {
            foreach (Position dot in dotPositions)
            {
                SetCursorPosition(dot.X, dot.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Write("□");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public void PrintSnake()
        {
            foreach (UserData user in UserDataHandler.userDatas)
            {
                Snake snake = user.Snake;

                //foreach (Position snakepos in snake.SnakePos)
                //{
                //    SetCursorPosition(snakepos.X, snakepos.Y);
                //    Write("□");
                //}
                //SetCursorPosition(snake.SnakePos[snake.SnakeLength].X, snake.SnakePos[snake.SnakeLength].Y);
                //Write("  ");

                SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);
                Write("□");
                SetCursorPosition(snake.SnakePos[snake.SnakeLength].X, snake.SnakePos[snake.SnakeLength].Y);
                Write("  ");

            }
        }

        public void SnakeDirectionSend()
        {
            switch (moveDirection)
            {
                case MOVE.LEFT:
                    AsynchronousClient.Send(AsynchronousClient.Mainclient, 's', "left");
                    break;
                case MOVE.RIGHT:
                    AsynchronousClient.Send(AsynchronousClient.Mainclient, 's', "right");
                    break;
                case MOVE.UP:
                    AsynchronousClient.Send(AsynchronousClient.Mainclient, 's', "up");
                    break;
                case MOVE.DOWN:
                    AsynchronousClient.Send(AsynchronousClient.Mainclient, 's', "down");
                    break;
                default:
                    break;
            }
        }

        public void GetKeyboardInput()
        {
            ConsoleKeyInfo keys;
            if (Console.KeyAvailable)
            {
                keys = Console.ReadKey(true);
                switch (keys.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (moveDirection != MOVE.DOWN)
                        {
                            moveDirection = MOVE.UP;
                            isDirectionChange = true;
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (moveDirection != MOVE.UP)
                        {
                            moveDirection = MOVE.DOWN;
                            isDirectionChange = true;
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        if (moveDirection != MOVE.RIGHT)
                        {
                            moveDirection = MOVE.LEFT;
                            isDirectionChange = true;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (moveDirection != MOVE.LEFT)
                        {
                            moveDirection = MOVE.RIGHT;
                            isDirectionChange = true;
                        }

                        break;

                    case ConsoleKey.R:
                        Refresh();
                        break;

                    //case ConsoleKey.Add:
                    //    snake.SnakeLength++;
                    //    dotData.NumberOfArea--;
                    //    break;
                    //case ConsoleKey.Subtract:
                    //    snake.SnakeLength--;
                    //    break;
                    //case ConsoleKey.Spacebar:
                    //    if (!pauseFlag)
                    //    {
                    //        pauseFlag = !pauseFlag;
                    //        stopwatch.Stop();
                    //    }
                    //    else
                    //    {
                    //        pauseFlag = !pauseFlag;
                    //        stopwatch.Start();
                    //    }
                    //    break;
                    //case ConsoleKey.Enter:
                    //    endFlag = true;
                    //    break;
                    //case ConsoleKey.Z:
                    //    x--;
                    //    if (x < 1)
                    //        x = 1;
                    //    speed = Const.BASE_SPEED / (x * 5);
                    //    break;
                    //case ConsoleKey.X:
                    //    x++;
                    //    if (x > 5)
                    //        x = 5;
                    //    speed = Const.BASE_SPEED / (x * 5);
                    //    break;

                    default:
                        break;
                }
            }
        }
            public void Refresh()
        {
            Clear();
            PrintManual();
            MakeAndPrintWall();

            foreach (UserData user in UserDataHandler.userDatas)
            {
                Snake snake = user.Snake;

                for (int i = 0; i< snake.SnakeLength; i++)
                {
                    SetCursorPosition(snake.SnakePos[i].X, snake.SnakePos[i].Y);
                    Write("□");
                }
                SetCursorPosition(snake.SnakePos[snake.SnakeLength].X, snake.SnakePos[snake.SnakeLength].Y);
                Write("  ");

            }
        }

    }
}
