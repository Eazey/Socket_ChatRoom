using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace Socket_ChatHome_TCPServer_taikr
{
    class Server
    {
        //客户端的连接列表
        static List<Client> clientList = new List<Client>();

        //向客户端广播收到的消息
        public static void BroadcastMessage(string message)
        {
            List<Client> removeClient = new List<Client>();
            //只向处于连接状态的进行广播，断开的作好记录并进行移除
            foreach (var client in clientList)
            {
                if (client.Connected)
                    client.SendMessage(message);
                else
                    removeClient.Add(client);
            }

            foreach (var rclient in removeClient)
            {
                clientList.Remove(rclient);
            }
        }

        static void Main(string[] args)
        {
            //实例服务端的Socket
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定终结点
            tcpServer.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.104"), 7788));
            //监听
            tcpServer.Listen(100);
            Console.WriteLine("Server running...");

            //死循环用于反复监听是否有客户端连接
            //接收到客户端连接前会阻塞
            //不必担心程序卡死
            while (true)
            {
                Socket clientSocket = tcpServer.Accept();
                Console.WriteLine("A client is connected!");
                //把与每个客户端通信的逻辑放在Client类中进行处理
                Client client = new Client(clientSocket);
                clientList.Add(client);
            }
        }
    }
}
