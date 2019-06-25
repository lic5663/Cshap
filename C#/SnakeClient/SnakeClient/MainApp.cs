using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using static System.Console;
using System.IO;
using System.Threading;

namespace SnakeClient
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
        private int index;

        public Position()
        {
            x = Const.X_SIZE + 2;
            y = Const.Y_SIZE - 1;
        }

        public Position(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }

        public Position(int xPos, int ypos, int indexValue)
        {
            x = xPos;
            y = ypos;
            index = indexValue;
        }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Index { get => index; set => index = value; }
    }
    class Const
    {
        public const int X_SIZE = 70;
        public const int Y_SIZE = 50;
        public const int BASE_SPEED = 1000;
        public const int SNAKE_SIZE = 1000;
        public const int DOT_SIZE = 10000;
    }

    class MainApp
    {
        public static bool gameStart = false;
        public static ClientHandler clientHandler = new ClientHandler();
        static void Main(string[] args)
        {
            AsynchronousClient.StartClient();

            while (true)
            {
                Clear();
                WriteLine("준비하려면 R을 눌러주세요.");
                string msg = "";
                while (!gameStart)
                {
                    
                    if (msg != "r" && msg != "R")
                    {
                        msg = ReadLine();
                        AsynchronousClient.Send(AsynchronousClient.Mainclient, 's', msg);
                    }
                }

                Clear();
                clientHandler.ProcedureGameHandler();
                WriteLine("계속하려면 아무 키나 입력해주세요.");
                ReadLine();

                gameStart = false;
            }
        }

    }
}
