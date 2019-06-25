using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeClient
{
    public class Snake
    {
        private Position[] snakePos;
        private int snakeLength;
        private MOVE snakeMoveDirection;

        public Snake()
        {
            snakePos = new Position[Const.SNAKE_SIZE];

            for (int i = 0; i < Const.SNAKE_SIZE; i++)
            {
                snakePos[i] = new Position();
            }

            snakeLength = 1;
            snakeMoveDirection = MOVE.RIGHT;
        }

        public Snake(Position startPos)
        {
            snakePos = new Position[Const.SNAKE_SIZE];

            for (int i = 0; i < Const.SNAKE_SIZE; i++)
            {
                snakePos[i] = new Position();
            }

            if (startPos.X % 2 != 0)
                startPos.X += 1;

            snakePos[0].X = startPos.X;
            snakePos[0].Y = startPos.Y;

            snakePos[1].X = startPos.X - 2;
            snakePos[1].Y = startPos.Y;

            snakeLength = 1;
            snakeMoveDirection = MOVE.RIGHT;
        }

        public int SnakeLength { get => snakeLength; set => snakeLength = value; }
        public Position[] SnakePos { get => snakePos; set => snakePos = value; }
        internal MOVE SnakeMoveDirection { get => snakeMoveDirection; set => snakeMoveDirection = value; }

    }
}
