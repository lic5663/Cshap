using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using System.Net;
using System.Net.Sockets;
using FUP;
using System.Diagnostics;

namespace FileReceiver
{
    class MainApp
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                WriteLine("사용법 : {0} <Directory>", Process.GetCurrentProcess().ProcessName);
                return;
            }
            uint msgId = 0;

            string dir = args[0];
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            const int bindPort = 5425;
            TcpListener server = null;
            try
            {
                // IP주소를 0으로 입력하면 루프백 주소 뿐만아니라 OS에 할당되어 있는 어떤 주소로도 서버에 접속이 가능하다.
                IPEndPoint localAddress = new IPEndPoint(0, bindPort);

                server = new TcpListener(localAddress);
                server.Start();

                WriteLine("파일 업로드 서버 시작...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    WriteLine("클라이언트 접속 {0}", ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

                    NetworkStream stream = client.GetStream();

                    Message reqMsg = MessageUtil.Receive(stream);

                    if (reqMsg.Header.MSGTYPE != CONSTANTS.REQ_FILE_SEND)
                    {
                        stream.Close();
                        client.Close();
                        continue;
                    }

                    BodyRequest reqBody = (BodyRequest)reqMsg.Body;

                    WriteLine("파일 업로드 요청이 왔습니다. 수락하시겠습니까? yes/no");
                    string answer = ReadLine();

                    Message rspMsg = new Message();
                    rspMsg.Body = new BodyResponse()
                    {
                        MSGID = reqMsg.Header.MSGID,
                        RESPONSE = CONSTANTS.ACCEPTED
                    };
                    rspMsg.Header = new Header()
                    {
                        MSGID = msgId++,
                        MSGTYPE = CONSTANTS.REP_FILE_SEND,
                        BODYLEN = (uint)rspMsg.Body.GetSize(),
                        FRAGMENTED = CONSTANTS.NOT_FRAGMENTED,
                        LASTMSG = CONSTANTS.LASTMSG,
                        SEQ = 0
                    };

                    if (answer != "yes")
                    {
                        rspMsg.Body = new BodyResponse()
                        {
                            MSGID = reqMsg.Header.MSGID,
                            RESPONSE = CONSTANTS.DENIED
                        };
                        MessageUtil.Send(stream, rspMsg);
                        stream.Close();
                        client.Close();

                        continue;
                    }
                    else
                        MessageUtil.Send(stream, rspMsg);

                    WriteLine("파일 전송을 시작합니다...");

                    long fileSize = reqBody.FILESIZE;
                    string fileName = Encoding.Default.GetString(reqBody.FILENAME);
                    // 파일 업로드 스트림 생성
                    FileStream file = new FileStream(dir + "\\" + fileName, FileMode.Create);

                    uint? dataMsgId = null;
                    ushort prevSeq = 0;
                    while ((reqMsg = MessageUtil.Receive(stream)) != null)
                    {
                        Write("#");
                        if (reqMsg.Header.MSGTYPE != CONSTANTS.FILE_SEND_DATA)
                            break;

                        if (dataMsgId == null)
                            dataMsgId = reqMsg.Header.MSGID;
                        else
                        {
                            if (dataMsgId != reqMsg.Header.MSGID)
                                break;
                        }

                        // 메세지 순서가 어긋나면 전송 중단
                        if (prevSeq++ != reqMsg.Header.SEQ)
                        {
                            WriteLine("{0}, {1}", prevSeq, reqMsg.Header.SEQ);
                            break;
                        }

                        // 전송받은 스트림을 서버에서 생성한 파일에 기록한다.
                        file.Write(reqMsg.Body.GetBytes(), 0, reqMsg.Body.GetSize());

                        // 분할 메세지가 아니라면 반복을 한번만 하고 빠져나온다.
                        if (reqMsg.Header.FRAGMENTED == CONSTANTS.NOT_FRAGMENTED)
                            break;
                        // 마지막 메세지면 반복문 빠져나옴
                        if (reqMsg.Header.LASTMSG == CONSTANTS.LASTMSG)
                            break;
                    }

                    long recvFileSize = file.Length;
                    file.Close();

                    WriteLine();
                    WriteLine("수신 파일 크기 : {0} bytes", recvFileSize);

                    Message rstMsg = new Message();
                    rstMsg.Body = new BodyResult()
                    {
                        MSGID = reqMsg.Header.MSGID,
                        RESULT = CONSTANTS.SUCCESS
                    };

                    rstMsg.Header = new Header()
                    {
                        MSGID = msgId++,
                        MSGTYPE = CONSTANTS.FILE_SEND_RES,
                        BODYLEN = (uint)rstMsg.Body.GetSize(),
                        FRAGMENTED = CONSTANTS.NOT_FRAGMENTED,
                        LASTMSG = CONSTANTS.LASTMSG,
                        SEQ = 0
                    };

                    if (fileSize == recvFileSize)
                        MessageUtil.Send(stream, rstMsg); // 같으면 성공 메세지를 보낸다
                    else
                    {
                        rstMsg.Body = new BodyResult()
                        {
                            MSGID = reqMsg.Header.MSGID,
                            RESULT = CONSTANTS.FAIL
                        };

                        MessageUtil.Send(stream, rstMsg);
                    }
                    WriteLine("파일 전송을 마쳤습니다.");

                    stream.Close();
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                WriteLine(e);
            }
            finally
            {
                server.Stop();
            }

            WriteLine("서버를 종료합니다");
        }
    }
}
