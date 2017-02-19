using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Socket_ChatHome_TCPServer_taikr
{
    /// <summary>
    /// 用来跟客户端做通信
    /// </summary>
    class Client
    {
        private Socket clientSocket;
        private Thread t;
        private byte[] data = new byte[1024];//数据容器

        //是否连接的属性
        public bool Connected
        {
            get
            {
                return clientSocket.Connected;
            }
        }

        public Client(Socket s)
        {
            clientSocket = s;
            //启动一个线程，处理客户端的数据接收
            t = new Thread(ReceiveMessage);
            t.Start();
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                //在接收数据之前 判断一下socket连接是否断开
                //客户端断开后会发送0字节数据，可以以此判断客户端是否关闭
                int length = clientSocket.Receive(data);
                if (length == 0)
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Disconnect(true);
                    clientSocket.Close();
                    break;
                }
                string message = Encoding.UTF8.GetString(data, 0, length);
                //接收到数据的时候  要把这个数据分发到客户端
                Console.WriteLine("收到了消息:" + message);
                //将来自客户端的消息广播到所有的客户端内。
                Server.BroadcastMessage(message);
            }
        }

        //发送数据
        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(data);
        }
    }
}
