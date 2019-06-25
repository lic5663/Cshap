using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace SnakeServer
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

    public class Position
    {
        private int x;
        private int y;

        public Position ()
        {
            x = Const.X_SIZE + 2;
            y = Const.Y_SIZE - 1;
        }

        public Position (int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
    }

    class Const
    {
        public const int X_SIZE = 70;
        public const int Y_SIZE = 50;
        public const int INIT_DOT_NUM = 10;
        public const int BASE_SPEED = 1000;
        public const int VICTORY_LENGTH = 50;
        public const int X2_LENGTH = 10;
        public const int X3_LENGTH = 20;
        public const int X4_LENGTH = 30;
        public const int X5_LENGTH = 40;
    }

    class SnakeGame
    {
        private static int numberOfUser;
        private Position[] startingPoint;
        private FIELD[,] gameField;
        private DotData dotData;
        private int score;
        private int gameSpeed;
        private int x;
        private bool pauseFlag;
        private Stopwatch Printstopwatch;
        private Stopwatch stopwatch;
        private bool isDotChange;
        private bool endFlag;
        private Stopwatch mytimer;

        public bool IsDotChange { get => isDotChange; set => isDotChange = value; }

        public void ProcedureGameHandler()
        {
            InitUsers();
            InitGame();

            while (true)
            {
                stopwatch.Start();

                UserCheck(); // 현재 유저 상태 확인
                MoveSnake(); // snake 움직임 연산
                VictoryCheck();
                SpeedCheck();
                if (endFlag)
                    break;

                PrintPhase();
                SendPhase();
                stopwatch.Stop();

                while(pauseFlag || stopwatch.ElapsedMilliseconds < gameSpeed)
                {
                    //if (!pauseFlag)
                        stopwatch.Start();
                    GetKeyboardInput();
                    stopwatch.Stop();
                }

                stopwatch.Reset();
                IsDotChange = false;
            }

            EndGame();
        }

        public void InitUsers()
        {
            numberOfUser = AsynchronousSocketListener.ClientsData.Count();
            startingPoint = new Position[numberOfUser];

            for (int i = 0; i < numberOfUser; i++)
            {

                int x = (Const.X_SIZE / (numberOfUser + 2)) * (i + 1);
                int y = (Const.Y_SIZE / (numberOfUser + 2)) * (i + 1);
                Position pos = new Position(x,y);

                startingPoint[i] = pos;
            }

            int index = 0;
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                client.Snake = new Snake(startingPoint[index++]);
            }

        }
        public bool InitGame()
        {
            stopwatch = new Stopwatch();
            Printstopwatch = new Stopwatch();
            Printstopwatch.Start();
            mytimer = new Stopwatch();
            pauseFlag = false;
            endFlag = false;
            score = 0;
            x = 1;
            gameSpeed = Const.BASE_SPEED / (5 * x);

            isDotChange = false;

            MakeAndPrintWall();
            PrintManual();

            // create Dot
            dotData = new DotData();
            for (int i = 0; i < Const.INIT_DOT_NUM; i++)
            {
                dotData.CreateDot();
                Thread.Sleep(20);
            }
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
            int manualX = Const.X_SIZE + 1;
            int manualY = 10;

            Console.SetCursorPosition(manualX, manualY++);
            Write("MOVE  : ←↑↓→");
            Console.SetCursorPosition(manualX, manualY++);
            Write("Space : pause");
            //Console.SetCursorPosition(manualX, manualY++);
            //Write("Z, X  : Speed Control");
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

        public void UserCheck()
        {
            List<ClientData> ExitUserArr = new List<ClientData>();

            foreach (ClientData clientData in AsynchronousSocketListener.ClientsData)
            {
                if (!clientData.Client.Connected)
                {
                    ExitUserArr.Add(clientData);
                }
            }

            foreach (ClientData clientData in ExitUserArr)
            {
                AsynchronousSocketListener.ClientsData.Remove(clientData);
                SnakeDeath(clientData);
            }
                
            
        }
        public void SendPhase()
        {
            
            SnakePosSend();

            if (isDotChange)
                dotData.SendDot();

        }

        public void PrintPhase()
        {
            PrintScreen();

            if (isDotChange)
                dotData.PrintDot();

            PrintSnake();
        }

        public void PrintScreen()
        {
            int yPos = 0;
            // print length
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
                Write("{0} : {1}   ", ((IPEndPoint)client.Client.RemoteEndPoint).ToString(), client.Snake.SnakeLength);
            }

            // pirnt Dot num
            Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
            Write("Dot Number : {0}", dotData.Dots.Count);

            // print score
            Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
            Write($"Score : {score}");

            // print time
            Printstopwatch.Stop();
            TimeSpan ts = Printstopwatch.Elapsed;
            Printstopwatch.Start();
            Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
            Write($"Time  : {ts.Seconds}");

            // print x1,x2,x3...
            Console.SetCursorPosition(Const.X_SIZE + 1, yPos++);
            Write($"배속  : x{x}");

        }

        public void PrintSnake()
        {
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                Snake snake = client.Snake;
                int snakeLen = snake.SnakeLength;

                Console.SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);
                Write("□");
                Console.SetCursorPosition(snake.SnakePos[snakeLen].X, snake.SnakePos[snakeLen].Y);
                Write("  ");

                Console.SetCursorPosition(snake.SnakePos[0].X, snake.SnakePos[0].Y);
            }
        }


        //모든 유저 snake pos를 보내준다.
        public void SnakePosSend()
        {
            string allSnakePos = "";
            int allSnakeLen = 0;
            int[] snakelenArr = new int[AsynchronousSocketListener.ClientsData.Count];
            int lenIndex = 0;

            foreach (ClientData clientData in AsynchronousSocketListener.ClientsData)
            {
                Snake snake = clientData.Snake;
                
                for (int i = 0; i <= snake.SnakeLength; i++)
                {
                    allSnakePos += String.Format("{0},{1} ", snake.SnakePos[i].X, snake.SnakePos[i].Y);
                    
                }
                snakelenArr[lenIndex++] = snake.SnakeLength;
                allSnakeLen += (snake.SnakeLength +1);
            }
            string lenData = "";
            foreach (int len in snakelenArr)
            {
                lenData += string.Format("{0},", len);
            }
            string snakeData = string.Format("{0} {1} {2}", allSnakeLen, lenData ,allSnakePos);

            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                AsynchronousSocketListener.Send(client.Client, 'p', snakeData);
            }

        }


        public void DieSend(ClientData DiedUser)
        {
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                Snake snake = DiedUser.Snake;
                string clientSankePosData = "";
                clientSankePosData += ((IPEndPoint)DiedUser.Client.RemoteEndPoint).ToString() + " " + DiedUser.Snake.SnakeLength + " ";
                for (int i = 0; i <= snake.SnakeLength; i++)
                {
                    clientSankePosData += String.Format("{0},{1} ", snake.SnakePos[i].X, snake.SnakePos[i].Y);
                }
                AsynchronousSocketListener.Send(client.Client, 'D', clientSankePosData);

            }
        }
        public void MoveSnake()
        {
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                Snake snake = client.Snake;
                int snakeLen = snake.SnakeLength;

                for (int i = snake.SnakeLength - 1; i >= 0; i--)
                {
                    snake.SnakePos[i + 1].X = snake.SnakePos[i].X;
                    snake.SnakePos[i + 1].Y = snake.SnakePos[i].Y;
                    
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

                if (IsCollision(snake))
                {
                    SnakeDeath(client);
                }
                    
            }
        }

        public void VictoryCheck()
        {
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                if (client.Snake.SnakeLength >= Const.VICTORY_LENGTH)
                {
                    client.Victory = true;
                    endFlag = true;
                }   
            }
        }

        public void SpeedCheck()
        {

            int preX = x;
            int longestLength = 0;

            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                if (client.Snake.SnakeLength > longestLength)
                    longestLength = client.Snake.SnakeLength;
            }

            if (longestLength >= Const.X5_LENGTH)
                x = 5;
            else if (longestLength >= Const.X4_LENGTH)
                x = 4;
            else if (longestLength >= Const.X3_LENGTH)
                x = 3;
            else if (longestLength >= Const.X2_LENGTH)
                x = 2;
            else if (longestLength > 0)
                x = 1;
            

            if (preX != x)
            {
                gameSpeed = Const.BASE_SPEED / (x * 5);

                foreach (ClientData client in AsynchronousSocketListener.ClientsData)
                {
                    AsynchronousSocketListener.Send(client.Client, 'S', x.ToString());
                }
            }

        }

        public void EndGame()
        {
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                client.Ready = false;

                if (client.Victory)
                {
                    AsynchronousSocketListener.Send(client.Client, 'V', "Victory");
                }
                else
                {
                    AsynchronousSocketListener.Send(client.Client, 'V', "Defeate");
                }
            }
        }

        public bool IsCollision(Snake snake)
        {
            Position snakeHead = snake.SnakePos[0];

            if (snakeHead.X == 0 || snakeHead.X == (Const.X_SIZE - 2))
                return true;
            else if (snakeHead.Y == 0 || snakeHead.Y == (Const.Y_SIZE - 1))
                return true;

            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                Snake EnemySnake = client.Snake;
                int snakeLen = EnemySnake.SnakeLength;

                for (int i = 0; i <= EnemySnake.SnakeLength; i++)
                {
                    // 대상 물체가 자신의 머리 일때는 충돌로 치지 않는다.
                    if (i == 0 && System.Object.Equals(snake, EnemySnake))
                        continue;

                    if (snakeHead.X == EnemySnake.SnakePos[i].X && (snakeHead.Y == EnemySnake.SnakePos[i].Y))
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i<dotData.Dots.Count; i++)
            {

                if(snakeHead.X == dotData.Dots[i].X && snakeHead.Y == dotData.Dots[i].Y)
                {
                    snake.SnakePos.Add(new Position());
                    snake.SnakeLength++;
                    score += 1000;
                    dotData.DeleteDot(snakeHead.X, snakeHead.Y);

                    if (dotData.Dots.Count < 15)
                    {
                        dotData.CreateDot();
                    }
                    break;
                }
            }

            return false;
        }

        // 뱀이 죽을 경우. 몸뚱이를 전부 dot로 치환한다.
        public void SnakeDeath(ClientData DiedUser)
        {
            Snake dieSnake = DiedUser.Snake;
            DieSend(DiedUser);

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 1; i <= dieSnake.SnakeLength; i++)
            {
                dotData.Dots.Add(dieSnake.SnakePos[i]);
                SetCursorPosition(dieSnake.SnakePos[i].X, dieSnake.SnakePos[i].Y);
                Write("□");
            }
            Console.ForegroundColor = ConsoleColor.White;

            dieSnake.SnakePos.Clear();
            
            dieSnake.SnakePos.Add(dotData.FindEmptySpace());
            dieSnake.SnakePos.Add(new Position());
            dieSnake.SnakeLength = 1;

            
        }

        public void GetKeyboardInput()
        {
            ConsoleKeyInfo keys;
            if (Console.KeyAvailable)
            {
                keys = Console.ReadKey(true);

                foreach (ClientData client in AsynchronousSocketListener.ClientsData)
                {
                    Snake snake = client.Snake;
                    int snakeLen = snake.SnakeLength;

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
                            snake.SnakePos.Add(new Position());
                            snake.SnakeLength++;
                            break;
                        case ConsoleKey.Subtract:
                            snake.SnakePos.RemoveAt(snake.SnakeLength);
                            snake.SnakeLength--;
                            break;
                        case ConsoleKey.Spacebar:
                            if (!pauseFlag)
                            {
                                pauseFlag = !pauseFlag;
                                Printstopwatch.Stop();
                            }
                            else
                            {
                                pauseFlag = !pauseFlag;
                                Printstopwatch.Start();
                            }
                            break;
                        case ConsoleKey.Enter:
                            break;
                        case ConsoleKey.Z:
                            x--;
                            if (x < 1)
                                x = 1;
                            gameSpeed = Const.BASE_SPEED / (x * 5);
                            break;
                        case ConsoleKey.X:
                            x++;
                            if (x > 5)
                                x = 5;
                            gameSpeed = Const.BASE_SPEED / (x * 5);
                            break;

                        default:
                            break;
                    }
                }
            }

        }
    }
}

