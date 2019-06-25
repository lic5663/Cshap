using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using static System.Console;
using System.Threading;
using System.IO;

namespace SnakeServer
{

    class SnakeMain
    {
        public static ManualResetEvent started = new ManualResetEvent(false);
        public static SnakeGame snakeGame;
        static void Main(string[] args)
        {
            // 서버 생성 및 클라이언트 연결, 모든 클라이언트 ready 대기
            AsynchronousSocketListener.StartListening();
            
            while(true)
            {
                started.WaitOne();
                Clear();
                started.Reset();
                
                snakeGame = new SnakeGame();
                snakeGame.ProcedureGameHandler();
            }
        }
    }
}
