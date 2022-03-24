using System;
using System.Net;
using System.Net.Sockets;
namespace Net;


class MainClass
{
    public static void Main(String[] args)
    {

        ClieNet Client = new ClieNet();
        Client.Connection();
        Client.Send("这是一条来自 客户端的测试");
        Client.Close();
        Console.ReadLine();
        


    }
}