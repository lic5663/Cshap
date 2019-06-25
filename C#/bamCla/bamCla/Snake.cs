using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeClient
{
    class Snake
    {
        //private Position[] snakePos;
        private List<Position> snakePos;
        private int snakeLength;
        private MOVE snakeMoveDirection;

        public Snake()
        {
            snakePos = new List<Position>();

            //int y = Const.Y_SIZE / 2;
            //int x = Const.X_SIZE / 2;
            //if (x % 2 != 0)
            //    x += 1;

            //snakePos.Add(new Position(x, y));
            //snakePos.Add(new Position(x - 2, y));

            snakeLength = 1;
            snakeMoveDirection = MOVE.RIGHT;
        }

        public Snake(Position startPos)
        {
            snakePos = new List<Position>();

            if (startPos.X % 2 != 0)
                startPos.X += 1;
            snakePos.Add(new Position(startPos.X, startPos.Y));
            snakePos.Add(new Position(startPos.X - 2, startPos.Y));


            snakeLength = 1;
            snakeMoveDirection = MOVE.RIGHT;
        }

        public int SnakeLength { get => snakeLength; set => snakeLength = value; }
        public List<Position> SnakePos { get => snakePos; set => snakePos = value; }
        internal MOVE SnakeMoveDirection { get => snakeMoveDirection; set => snakeMoveDirection = value; }

    }
}
