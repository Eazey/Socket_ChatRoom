
//In Unity3D

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client : MonoBehaviour {

    private string _IPAddress = "192.168.1.104";
    private int _port = 7788;

    private Socket clientSocket;
    private Thread t;

    
    public InputField inputField; //输入框
    public Text chatForm;         //界面文本框
    private string receiveMessage = "";
    private byte[] data = new byte[2048];

	// Use this for initialization
    void Start()
    {
        ConnectToServer();
        t = new Thread(MyReceiveMessage);
        t.Start();
    }
	
	// Update is called once per frame
	void Update () {

        //如果消息不为空，则在界面上更新消息
        if (receiveMessage != null && receiveMessage != "")
        {
            chatForm.text += ("\n" + receiveMessage);
            receiveMessage = "";
        }
	}

    //连接服务器
    private void ConnectToServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        clientSocket.Connect(new IPEndPoint(IPAddress.Parse(_IPAddress), _port));
    }

    //接收数据并转换
    private void MyReceiveMessage()
    {
        while (true)
        {
            int length = clientSocket.Receive(data);
            if (length == 0)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(true);
                clientSocket.Close();
                break;
            }

            receiveMessage = Encoding.UTF8.GetString(data, 0, length);
        }
    }

    //发送数据
    private void MySendMessage(string message)
    {
        data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);
    }

    //发送按钮事件——发送消息
    public void OnSendButtonClick()
    {
        string value = inputField.text;
        inputField.text = "";
        MySendMessage(value);
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy");
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Disconnect(true);
        clientSocket.Close();
    }
}
