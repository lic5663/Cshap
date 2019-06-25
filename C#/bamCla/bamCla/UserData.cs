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
        public static List<UserData> userDatas = new List<UserData>();

        // Server로부터 받은 유저 Snake 위치 업데이트
        public static void UpdateUserPos(string[] posData)
        {
            string userName = posData[0];

            UserData user = userDatas.Find(x => x.UserName.Contains(userName));

            if (user == null)
            {
                user = new UserData(userName);
                userDatas.Add(user);
            }

            user.Snake.SnakeLength = Int32.Parse(posData[1]);

            while (user.Snake.SnakeLength > (user.Snake.SnakePos.Count -1))
            {
                user.Snake.SnakePos.Add(new Position());
            }

            for (int i = 0; i < user.Snake.SnakeLength + 1; i++)
            {
                string[] pos = posData[i + 2].Split(new string[] { "," }, StringSplitOptions.None);
                user.Snake.SnakePos[i].X = Int32.Parse(pos[0]);
                user.Snake.SnakePos[i].Y = Int32.Parse(pos[1]);
            }
        }

        public static void ChangeDiedUserSnake(string[] dieData)
        {

            Snake dieSnake = new Snake();
            dieSnake.SnakeLength = Int32.Parse(dieData[1]);

            while (dieSnake.SnakeLength > (dieSnake.SnakePos.Count - 1))
            {
                dieSnake.SnakePos.Add(new Position());
            }

            for (int i = 1; i <= dieSnake.SnakeLength; i++)
            {
                string[] pos = dieData[i + 2].Split(new string[] { "," }, StringSplitOptions.None);
                dieSnake.SnakePos[i].X = Int32.Parse(pos[0]);
                dieSnake.SnakePos[i].Y = Int32.Parse(pos[1]);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (Position position in dieSnake.SnakePos)
            {
                SetCursorPosition(position.X, position.Y);
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
