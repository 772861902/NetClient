using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Net
{
    internal class ClieNet
    {
        Socket socket;
        int buffCount=0;
        byte[] lenBytes = new byte[sizeof(UInt32)];
        Int32 msgLength = 0;
        const int BUFFER_SIZE = 1024;
        byte[] readbuffer = new byte[BUFFER_SIZE];
        public void Connection()
        {
            socket  = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 1234);
            Console.WriteLine("连接服务器成功");

            //string str = "hello";
           // byte[] bytes  = System.Text.Encoding.UTF8.GetBytes(str);
            //socket.Send(bytes);
            //Recv
            socket.BeginReceive(readbuffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);

            


            // socket.Close();
        }
        //接收回调
        private void ReceiveCb(IAsyncResult ar)
        {
         
            //Conn conn = (Conn)ar.AsyncState;
            try
            {
                int count = socket.EndReceive(ar);
                //关闭信号
               
                buffCount += count;
                ProcessData();
                //继续接收，
                socket.BeginReceive(readbuffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);

               
            }
            catch (Exception e)
            {
               // Console.WriteLine("收到[" + conn.GetAddress() + "]断开连接");
                socket.Close();
            }
        }


        //实现缓冲区的消息处理(涉及粘包分包处理）
        public void ProcessData()
        {
            if (buffCount < sizeof(Int32))
            {
                return;
            }
            Array.Copy(readbuffer, lenBytes, sizeof(Int32));
            msgLength = BitConverter.ToInt32(readbuffer, 0);
            if (buffCount < msgLength + sizeof(Int32))
            {
                return;
            }
            string str = System.Text.Encoding.Default.GetString(readbuffer, sizeof(Int32), msgLength);
            Console.WriteLine("收到服务端信息 ： @ 消息长度 ：" + msgLength + "@ 消息内容：" + str);
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readbuffer, sizeof(Int32) + msgLength, readbuffer, 0, count);
            buffCount = count;
            if (buffCount > 0)
            {
                ProcessData();

            }

        }

        //消息发送
        public void Send( string str)
        {
            //string str = "这是来自客户端发送的测试消息";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            socket.Send(sendbuff);
           
                
         }

        public void Close()
        {
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        socket.Close();
                        Console.WriteLine("客户端socket关闭成功");
                        break;
                    default:
                        continue;
                }
            }
        }

    }
}
