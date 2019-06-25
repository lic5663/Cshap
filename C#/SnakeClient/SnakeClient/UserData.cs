using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using static System.Console;

namespace SnakeClient
{

    public class UserDataHandler
    {
        //public static List<UserData> userDatas = new List<UserData>();
        public static Snake AllSnakeData = new Snake();
        public static Snake DieSnake = new Snake();
        public static int[] endIndex;

        // Server로부터 받은 유저 Snake 위치 업데이트
        public static void UpdateUserPos(string[] posData)
        {
            AllSnakeData.SnakeLength = Int32.Parse(posData[0]);

            string[] lenArr = posData[1].Split(new string[] { "," }, StringSplitOptions.None);
            endIndex = new int[lenArr.Length - 1];
            for (int i = 0; i < lenArr.Length -1; i++)
            {
                endIndex[i] = Int32.Parse(lenArr[i]);
            }

            for (int i = 0; i < AllSnakeData.SnakeLength; i++)
            {
                string[] pos = posData[i + 2].Split(new string[] { "," }, StringSplitOptions.None);

                AllSnakeData.SnakePos[i].X = Int32.Parse(pos[0]);
                AllSnakeData.SnakePos[i].Y = Int32.Parse(pos[1]);
            }
        }

        public static void ChangeDiedUserSnake(string[] dieData)
        {
            DieSnake.SnakeLength = Int32.Parse(dieData[1]);


            for (int i = 1; i <= DieSnake.SnakeLength; i++)
            {
                string[] pos = dieData[i + 2].Split(new string[] { "," }, StringSplitOptions.None);
                DieSnake.SnakePos[i].X = Int32.Parse(pos[0]);
                DieSnake.SnakePos[i].Y = Int32.Parse(pos[1]);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < DieSnake.SnakeLength; i++)
            {
                SetCursorPosition(DieSnake.SnakePos[i].X, DieSnake.SnakePos[i].Y);
                Write("□");
            }
            Console.ForegroundColor = ConsoleColor.White;

        }
    }

    public class UserData
    {
        private string userName;
        private Snake snake;

        public UserData(string Name)
        {
            userName = Name;
            snake = new Snake();
        }

        public string UserName { get => userName; set => userName = value; }
        internal Snake Snake { get => snake; set => snake = value; }
    }

    
}
