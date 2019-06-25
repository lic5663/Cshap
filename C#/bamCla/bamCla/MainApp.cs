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
    }

    class MainApp
    {
        public static bool gameStart = false;
        public static ClientHandler clientHandler = new ClientHandler();
        static void Main(string[] args)
        {
            AsynchronousClient.StartClient();
            string msg = "";
            while (!gameStart)
            {
                if (msg !="r" && msg != "R")
                {
                    msg = ReadLine();
                    AsynchronousClient.Send(AsynchronousClient.Mainclient, 's', msg);
                }
                
            }

            try
            {
                Clear();
                clientHandler.ProcedureGameHandler();

                
            }
            catch (Exception)
            {
                WriteLine("오류 발생");
                throw;
            }
            



        }

    }
}
