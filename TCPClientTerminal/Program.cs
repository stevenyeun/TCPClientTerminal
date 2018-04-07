using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPClientTerminal
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ipaddrTCPServer = null;
            int nTCPServerPort = 0;
            //아이피 포트 입력받기
            while (true)
            {
                System.Console.Write("TCP 서버의 아이피 주소를 입력 : ");
                string strIPAddr = System.Console.ReadLine();

                if (IPAddress.TryParse(strIPAddr, out ipaddrTCPServer) == false)
                {
                    System.Console.WriteLine("올바르지않은 아이피 주소.");
                    continue;
                }
                break;
            }

            while (true)
            {
                System.Console.Write("TCP 서버의 포트번호를 입력 : ");
                string strPort = System.Console.ReadLine();

                if (int.TryParse(strPort, out nTCPServerPort) == false)
                {
                    System.Console.WriteLine("올바르지않은 포트번호.");
                    continue;
                }
                break;
            }

            IPEndPoint EP = new IPEndPoint(ipaddrTCPServer, nTCPServerPort);



            TcpClient tcpClient = null;
            NetworkStream stream = null;
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(EP);
                stream = tcpClient.GetStream();
                System.Console.WriteLine("아이피={0}, 원격포트={1}에 접속 성공.", ipaddrTCPServer, nTCPServerPort);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("아이피={0}, 원격포트={1}에 접속 실패!!(ExceptionMessage : {2})", ipaddrTCPServer, nTCPServerPort, e.Message);
            }


            //TCP 수신 스레드
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        const int BUF_SIZE = 1024;
                        Byte[] recvBuffer = new Byte[BUF_SIZE];

                        if (stream != null)
                        {
                            //Console.WriteLine("패킷 대기중");
                            Int32 bytes = stream.Read(recvBuffer, 0, recvBuffer.Length);
                            if (bytes == 0)
                            {
                                break;
                            }
                            string str = Encoding.UTF8.GetString(recvBuffer, 0, bytes);
                            Console.WriteLine("수신메세지 : " + str);
                        }
                        else
                        {
                            Console.WriteLine("NetworkStream 객체 == null");
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("ExceptionMessage : {0}" + e.Message);
                        break;
                    }
                    Thread.Sleep(1);
                }
            });

            System.Console.WriteLine("Ctrl+C 누르면 종료.");
            while (true)
            {
                System.Console.Write("전송메세지입력 : ");
                string readMessage = System.Console.ReadLine();
                readMessage += '\r';
                //if (readKey == 'Q' || readKey == 'q')
                //    break;
              
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(readMessage);

                try
                {
                    stream.Write(msg, 0, msg.Length);
                    Thread.Sleep(1000);

                }
                catch(Exception e)
                {
                    System.Console.WriteLine("ExceptionMessage : {0}" + e.Message);
                    break;
            
                }
            }
            
        }
    }
}
