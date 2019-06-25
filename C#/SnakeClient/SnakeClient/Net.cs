using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace SnakeClient
{
    public class Defines
    {
        public static readonly short HEADERSIZE = 2;
    }
    public class StateObject
    {
        // Client socket.  f
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 131072;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        public Queue<byte> streamQueue = new Queue<byte>(131072);
    }

    public class AsynchronousClient
    {
        // The port number for the remote device.  
        private const int port = 11000;
        private static int totalReceive = 0;
        private static short msgSize = 0;
        private static IPEndPoint remoteEP;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        private static Socket mainclient = null;


        public static Socket Mainclient { get => mainclient; set => mainclient = value; }

        public static void StartClient()
        {
            // Connect to a remote device.  
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                //IPAddress ipAddress = IPAddress.Parse("192.168.1.31");
                //IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
                Mainclient = client;

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Receive(client);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void EndClient()
        {
            Socket client = Mainclient;
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;


                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();



            }
            catch (Exception e)
            {
                //WriteLine(e.ToString());

                WriteLine("서버 연결에 실패했습니다. 재시도하려면 아무 키나 입력해주세요.");
                ReadLine();
                Socket client = (Socket)ar.AsyncState;
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);

            }
        }

        

        // string 전용
        public static void Send(Socket handler, char protocolType, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            short size = (short)(sizeof(char) + data.Length);
            byte[] header = BitConverter.GetBytes(size);
            byte[] protocol = BitConverter.GetBytes(protocolType);
            byte[] body = Encoding.UTF8.GetBytes(data);

            byte[] packet = new byte[header.Length + protocol.Length + body.Length];
            Array.Copy(header, 0, packet, 0, header.Length);
            Array.Copy(protocol, 0, packet, header.Length, protocol.Length);
            Array.Copy(body, 0, packet, header.Length + protocol.Length, body.Length);

            // Begin sending the data to the remote device.  
            handler.BeginSend(packet, 0, packet.Length, 0, new AsyncCallback(SendCallback), handler);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                // 수신 버퍼 내용을을 누적 버퍼에 이어 붙인다.
                //System.Buffer.BlockCopy(state.buffer, 0, state.accumulateBuffer, totalReceive, bytesRead);

                // 수신 버퍼의 내용을 큐에 삽입한다.
                for (int i = 0; i < bytesRead; i++)
                {
                    state.streamQueue.Enqueue(state.buffer[i]);
                }

                totalReceive += bytesRead;

                if (totalReceive > 130000)
                {
                    Clear();
                    WriteLine("장시간 응답이 없으므로 강제 종료합니다.");
                    ReadLine();

                    Environment.Exit(0);
                }
                    

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
                                //Thread decodeThread = new Thread(() => decodeMesseage(databuffer, state));
                                //decodeThread.Start();
                                decodeMesseage(databuffer, state);

                                

                                totalReceive -= (Defines.HEADERSIZE + msgSize); // 한 패킷을 버퍼로 옮겼으므로 총 받은 데이터에서 해당 패킷만큼 감소시킨다.
                                msgSize = 0;

                                
                            }

                        }
                    }
                }
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }
        }

        private static void decodeMesseage(byte[] byteMessage, StateObject state)
        {
            char protocol = BitConverter.ToChar(byteMessage, 0);
            byte[] byteData = new byte[byteMessage.Length - sizeof(char)];
            Buffer.BlockCopy(byteMessage, 2, byteData, 0, byteMessage.Length - sizeof(char));

            switch (protocol)
            {
                case 's': // string Data
                    string strData = Encoding.UTF8.GetString(byteData);
                    switch (strData)
                    {
                        case "Game Start":
                            MainApp.gameStart = true;
                            break;

                        case "Yet Not All Ready":
                            WriteLine(strData);
                            break;

                        default:
                            break;
                    }

                    break;

                case 'p': // Position Data
                    string posData = Encoding.UTF8.GetString(byteData);
                    DecodePosData(posData);
                    break;

                case 'S': // Game Speed Data
                    string speedData = Encoding.UTF8.GetString(byteData);
                    MainApp.clientHandler.GameSpeed = Int32.Parse(speedData);
                    MainApp.clientHandler.printSpeed();
                    break;

                case 'd': // dot Data
                    string dotData = Encoding.UTF8.GetString(byteData);
                    DecodeDotData(dotData);
                    break;

                case 'D': // Die Snake Name
                    string deathData = Encoding.UTF8.GetString(byteData);
                    DecodeDeathData(deathData);
                    break;

                case 'V': // Victory or Defeate
                    string victoryData = Encoding.UTF8.GetString(byteData);
                    DecodeVictoryData(victoryData);
                    break;



                default:
                    break;
            }

        }

        private static void DecodePosData(string posData)
        {
            
            string[] posArr = posData.Split(new string[] { " " }, StringSplitOptions.None);

            UserDataHandler.UpdateUserPos(posArr);

            MainApp.clientHandler.PrintSnake();
            MainApp.clientHandler.PrintScreen();
        }

        private static void DecodeDotData(string dotData)
        {
            string[] dotArr = dotData.Split(new string[] { " " }, StringSplitOptions.None);
            
            int dotNum = Int32.Parse(dotArr[0]);
            MainApp.clientHandler.DotNum = dotNum;

            for (int i = 0; i < dotNum; i++)
            {
                string[] dotXY = dotArr[i+1].Split(new string[] { "," }, StringSplitOptions.None);
                MainApp.clientHandler.DotPositions[i].X = Int32.Parse(dotXY[0]);
                MainApp.clientHandler.DotPositions[i].Y = Int32.Parse(dotXY[1]);
            }

            MainApp.clientHandler.PrintDot();
        }


        private static void DecodeDeathData(string deathData)
        {
            string[] dieArr = deathData.Split(new string[] { " " }, StringSplitOptions.None);

            UserDataHandler.ChangeDiedUserSnake(dieArr);
        }

        private static void DecodeVictoryData(string victoryData)
        {
            Clear();
            SetCursorPosition(0, 5);

            if (victoryData == "Victory")
            {
                WriteLine(" _   _  _        _                       ");
                WriteLine("| | | |(_)      | |                      ");
                WriteLine("| | | | _   ___ | |_   ___   _ __  _   _ ");
                WriteLine("| | | || | / __|| __| / _ \\ | '__|| | | |");
                WriteLine("\\ \\_/ /| || (__ | |_ | (_) || |   | |_| |");
                WriteLine(" \\___/ |_| \\___| \\__| \\___/ |_|    \\__, |");
                WriteLine("                                    __/ |");
                WriteLine("                                   |___/ ");
                    
            }
            else
            {
                WriteLine("______         __               _   ");
                WriteLine("|  _ \\       / _|             | |  ");
                WriteLine("| | | |  ___ | |_   ___   __ _ | |_ ");
                WriteLine("| | | | / _ \\|  _| / _ \\ / _` || __|");
                WriteLine("| |/ / |  __/| |  |  __/| (_| || |_ ");
                WriteLine("|___/   \\___||_|   \\___| \\__,_| \\__|");
                WriteLine("                                    ");
                WriteLine("                                    ");
            }
            MainApp.clientHandler.EndFlag = true;
        }
        

    }

}
