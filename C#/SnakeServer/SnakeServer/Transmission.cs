using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;


namespace SnakeServer
{
    public class Defines
    {
        public static readonly short HEADERSIZE = 2;
    }

    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize]; // 송,수신 휘발성 버퍼.
        public Queue<byte> streamQueue = new Queue<byte>(131072);
    }

    public class ClientData
    {
        private Socket client;
        private bool ready;
        private bool victory;
        private Snake snake;
        public Socket Client { get => client; set => client = value; }
        public bool Ready { get => ready; set => ready = value; }
        public bool Victory { get => victory; set => victory = value; }
        internal Snake Snake { get => snake; set => snake = value; }
    }

    public class AsynchronousSocketListener
    {
        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static List<ClientData> clientsData = new List<ClientData>();
        private static int totalReceive = 0;
        private static short msgSize = 0;
        public static Stopwatch sendTimeStopWatch = new Stopwatch();

        internal static List<ClientData> ClientsData { get => clientsData; set => clientsData = value; }

        public static async void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            //IPAddress ipAddress = IPAddress.Parse("192.168.1.31");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback),listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                    await Task.Delay(10);       
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            ClientData clientData = new ClientData()
            {
                Client = handler,
                Ready = false
            };

            clientsData.Add(clientData);

            WriteLine("{0} 클라이언트와 접속되었습니다. 현재 접속자수 : {1}", ((IPEndPoint)handler.RemoteEndPoint).ToString(), clientsData.Count());
            
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                // 수신 버퍼의 내용을 큐에 삽입한다.
                for (int i = 0; i < bytesRead; i++)
                {
                    state.streamQueue.Enqueue(state.buffer[i]);
                }

                totalReceive += bytesRead;

                // 읽어 온 값이 존재할경우 진행
                while (totalReceive > 0)
                {
                    // 아직 헤더값을 받지 못한 상태
                    if (msgSize == 0)
                    {
                        // 총 읽어 온 값이 헤더 사이즈보다 작은 경우 어디까지 읽어야할지 모르므로 다시 읽는다.
                        if (totalReceive < Defines.HEADERSIZE)
                        {
                            break;
                        }
                        else
                        {
                            if (msgSize == 0)
                            {
                                byte[] size = new byte[2];
                                size[0] = state.streamQueue.Dequeue();
                                size[1] = state.streamQueue.Dequeue();

                                msgSize = BitConverter.ToInt16(size, 0);
                            }

                            // 받은 데이터가 패킷 길이보다 적을 경우. 다시 받는다.
                            if (totalReceive < (Defines.HEADERSIZE + msgSize))
                            {
                                break;
                            }
                            // 받은 데이터 바이트가 패킷보다 많을 경우 한 패킷이 전송이 완료됬으므로 데이터를 복제한다.
                            else
                            {
                                byte[] databuffer = new byte[msgSize];

                                for (int i = 0; i < msgSize; i++)
                                {
                                    databuffer[i] = state.streamQueue.Dequeue();
                                }

                                // 패킷 분석
                                decodeMesseage(databuffer, state);


                                totalReceive -= (Defines.HEADERSIZE + msgSize); // 한 패킷을 버퍼로 옮겼으므로 총 받은 데이터에서 해당 패킷만큼 감소시킨다.
                                msgSize = 0;

                                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                            }

                        }
                    }
                }
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception)
            {
                
            }
        }

        public static void Send(Socket handler, char protocolType , String data)
        {
            // 비정상 종료시에 여기서 처리하자.
            if (!handler.Connected)
            {
                ClientUnusalExit(handler);
                return;
            }


            // Convert the string data to byte data using ASCII encoding.  
            short size = (short)(sizeof(char) + data.Length);
            byte[] header = BitConverter.GetBytes(size);
            byte[] protocol = BitConverter.GetBytes(protocolType);
            byte[] body = Encoding.UTF8.GetBytes(data);

            byte[] packet = new byte[header.Length + protocol.Length + body.Length];
            Array.Copy(header, 0, packet, 0, header.Length);
            Array.Copy(protocol, 0, packet, header.Length, protocol.Length);
            Array.Copy(body, 0, packet, header.Length + protocol.Length , body.Length);

            // Begin sending the data to the remote device.  
            try
            {
                handler.BeginSend(packet, 0, packet.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        // 바이트 데이터 해독
        private static void decodeMesseage(byte[] byteMessage, StateObject state)
        {

            ClientData user = clientsData.Find(x => x.Client == state.workSocket);

            char protocol = BitConverter.ToChar(byteMessage, 0);
            byte[] byteData = new byte[byteMessage.Length - sizeof(char)];
            Buffer.BlockCopy(byteMessage, 2, byteData, 0 , byteMessage.Length - sizeof(char));

            switch (protocol)
            {
                case 's': // string Data
                    string strData = Encoding.UTF8.GetString(byteData);
                    switch (strData)
                    {
                        // 대기방 레디 준비
                        case "R":
                        case "r":
                            user.Ready = true;
                            ReadyCheck();
                            break;

                        case "N":
                        case "n":
                            user.Ready = false;
                            ReadyCheck();
                            break;

                        // 화살표 키 입력
                        case "up":
                            user.Snake.SnakeMoveDirection = MOVE.UP;
                            break;
                        case "down":
                            user.Snake.SnakeMoveDirection = MOVE.DOWN;
                            break;
                        case "left":
                            user.Snake.SnakeMoveDirection = MOVE.LEFT;
                            break;
                        case "right":
                            user.Snake.SnakeMoveDirection = MOVE.RIGHT;
                            break;


                        default:
                            break;
                    }

                    break;

                case 'p': // Position Data
                    break;

                case 'S': // Snake Data
                    break;


                default:
                    break;
            }

        }

        private static void ReadyCheck()
        {
            bool allReady = true;
            foreach (ClientData clientData in clientsData)
            {
                if (clientData.Ready == false)
                {
                    allReady = false;
                    break;
                }
            }

            if (allReady)
            {
                WriteLine("모든 사람이 준비 되었습니다.");

                foreach (ClientData user in clientsData)
                {
                    Send(user.Client, 's' ,"Game Start");
                }
                SnakeMain.started.Set();
            }
            else
            {
                foreach (ClientData user in clientsData)
                {
                    Send(user.Client, 's' ,"Yet Not All Ready");
                }

                WriteLine("준비 되지 않는 사람이 존재합니다.");
            }
        }


        private static void ClientUnusalExit(Socket clientSocket)
        {
            int ExitUserIndex = ClientsData.FindIndex(x => x.Client == clientSocket);
            //ClientsData.RemoveAt(ExitUserIndex);
            
        }



    }

    
}
