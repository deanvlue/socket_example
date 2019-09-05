using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace server
{
    class Program
    {

        private static List<Socket> _clientSockets = new List<Socket>();
        // defininf gglobal socket
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static byte[] _buffer = new byte[1024];
        static void Main(string[] args)
        {
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer(){
            Console.WriteLine("Setting up server....");

            // Binds the socket to any local address available local, and on port 100
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));

            // amount of connections to be made
            _serverSocket.Listen(2);

            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);



        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(socket);
            Console.WriteLine("Client connects.");
            socket.BeginReceive(_buffer,0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback),socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);

            Console.WriteLine("Text Received: {0}", text);

            string response = string.Empty;

            if (text.ToLower() != "get time")
            {
                response = "Invalid Request";
                
            }else{
                response = DateTime.Now.ToLongTimeString();
            }
            
            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            
            socket.BeginReceive(_buffer,0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback),socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }


        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
