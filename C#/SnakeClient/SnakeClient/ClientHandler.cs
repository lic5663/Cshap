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
        private int dotNum;
        private Position[] dotPositions;
        private bool isDotFirst;
        private bool endFlag;


        public int GameSpeed { get => gameSpeed; set => gameSpeed = value; }
        public Position[] DotPositions { get => dotPositions; set => dotPositions = value; }
        public int DotNum { get => dotNum; set => dotNum = value; }
        public bool EndFlag { get => endFlag; set => endFlag = value; }

        public void ProcedureGameHandler()
        {
            HandlerInit();

            while (!endFlag)
            {
                while (!endFlag && !isDirectionChange)
                {
                    GetKeyboardInput();
                    Thread.Sleep(1);
                }
                SnakeDirectionSend();
                

                isDirectionChange = false;
            }
            
        }

        public void HandlerInit()
        {
            dotNum = 0;
            stopwatch = new Stopwatch();
            dotPositions = new Position[Const.DOT_SIZE];
            moveDirection = MOVE.RIGHT;
            isDirectionChange = false;
            isDotFirst = true;
            endFlag = false;
            gameSpeed = 1;
            MakeAndPrintWall();
            PrintManual();
            printSpeed();


            for (int i = 0; i <Const.DOT_SIZE; i++)
            {
                dotPositions[i] = new Position();
            }

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
            Write("Len == 50 => WIN");

            manualY += 5;
            //Console.SetCursorPosition(manualX, manualY++);
            //Write("----DEBUG CONTROL----");
            //Console.SetCursorPosition(manualX, manualY++);
            //Write("+ : Add Length ");
            //Console.SetCursorPosition(manualX, manualY++);
            //Write("- : Sub Length ");


        }


        public void PrintPhase()
        {
            try
            {
                PrintScreen();
                PrintDot();
                PrintSnake();
                printSpeed();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void PrintScreen()
        {
            int yPos = 0;
            int index = 1;
            Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
            Write($"길이");
            foreach (int len in UserDataHandler.endIndex)
            {
                Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
                Write("Player{0} : {1} ", index++ ,len);
            }

            
        }

        public void printSpeed()
        {
            Console.SetCursorPosition(Const.X_SIZE + 1, Const.Y_SIZE/2 - 5);
            Write($"배속 : x{gameSpeed}");
        }
        public void PrintDot()
        {

            if (isDotFirst)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                for (int i = 0; i < dotNum; i++)
                {
                    SetCursorPosition(dotPositions[i].X, dotPositions[i].Y);
                    Write("□");
                }
                Console.ForegroundColor = ConsoleColor.White;
                isDotFirst = false;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                SetCursorPosition(dotPositions[dotNum - 1].X, dotPositions[dotNum - 1].Y);
                Write("□");
                Console.ForegroundColor = ConsoleColor.White;
            }


            

        }
        public void PrintSnake()
        {
            Snake snake = UserDataHandler.AllSnakeData;
            int[] snakeLen = UserDataHandler.endIndex;
            int index = 0;

            for (int i=0; i< snakeLen.Length; i++)
            {
                SetCursorPosition(snake.SnakePos[index].X, snake.SnakePos[index].Y);
                Write("□");

                index += snakeLen[i];

                SetCursorPosition(snake.SnakePos[index].X, snake.SnakePos[index].Y);
                Write("  ");

                index++;
            }
            SetCursorPosition(Const.X_SIZE+2, Const.Y_SIZE -1);


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

            PrintPhase();
        }

    }
}
