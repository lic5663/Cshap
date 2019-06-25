using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace SnakeServer
{
    class DotData
    {
        private List<Position> dots;
        private int preDotsCount;

        public DotData()
        {
            dots = new List<Position>();
            preDotsCount = 0;
        }
        internal List<Position> Dots { get => dots; set => dots = value; }


        public void CreateDot()
        {
            Position dotPosition = FindEmptySpace();
            dots.Add(dotPosition);
            SnakeMain.snakeGame.IsDotChange = true;

        }

        public void PrintDot()
        {
            ForegroundColor = ConsoleColor.Green;
            for (int i = preDotsCount; i < dots.Count; i++ )
            {
                SetCursorPosition(dots[i].X, dots[i].Y);
                Write("□");
            }
            ForegroundColor = ConsoleColor.White;
            preDotsCount = dots.Count;

        }

        public void SendDot()
        {
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                string dotPos = "";
                dotPos += dots.Count.ToString() + " ";
                foreach (Position dot in dots)
                {
                    dotPos += string.Format("{0},{1} ", dot.X, dot.Y);
                }
                AsynchronousSocketListener.Send(client.Client, 'd', dotPos);
            }
        }



        public void DeleteDot(int x , int y)
        {
            int index = dots.FindIndex( f => f.X == x && f.Y == y);

            dots[index].X = dots[(dots.Count - 1)].X;
            dots[index].Y = dots[(dots.Count - 1)].Y;

            dots.RemoveAt(dots.Count - 1);
            preDotsCount--;
            if (preDotsCount < 0)
                preDotsCount = 0;
            SnakeMain.snakeGame.IsDotChange = true;
        }

        public Position FindEmptySpace()
        {
            List<Position> spaceInUse = new List<Position>();
            Position emptySpace;

            // 각 유저들 뱀이 점유하는 지점들을 사용중인 영역에 추가한다.
            foreach (ClientData client in AsynchronousSocketListener.ClientsData)
            {
                Snake snake = client.Snake;
                if (snake.SnakePos.Count > 0)
                {
                    for (int i = 0; i < snake.SnakeLength; i++)
                    {
                        spaceInUse.Add(snake.SnakePos[i]);
                    }
                }
            }
            // Dot가 존재하는 지점을 사용중인 영역에 추가한다.
            foreach (Position dotPos in dots)
            {
                spaceInUse.Add(dotPos);
            }

            // 랜덤 포지션 설정 후, 사용중인 영역인지 검사
            while (true)
            {
                Random random = new Random();
                int y = random.Next(1, Const.Y_SIZE-1);
                int x = random.Next(2, Const.X_SIZE-4);
                if (x % 2 != 0)
                    x += 1;

                 emptySpace = new Position(x, y);

                if (!spaceInUse.Contains(emptySpace))
                    break;
            }

            return emptySpace;
        }
    }
}
