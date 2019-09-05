using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            LoopConnect();
            SendLoop();
            //Console.ReadLine();
        }

        private static void SendLoop(){
            while(true){
                Console.Write("Enter a request: ");
                string req = Console.ReadLine();

                byte[] buffer = Encoding.ASCII.GetBytes(req);

                _clientSocket.Send(buffer);

                byte[] receivedBuf = new byte[1024];
                int rec = _clientSocket.Receive(receivedBuf);

                byte[] data = new byte[rec];
                Array.Copy(receivedBuf, data, rec);

                Console.WriteLine("Received: {0}", Encoding.ASCII.GetString(data));
            }
        }

        private static void LoopConnect(){

            int attempts = 0;
            while (!_clientSocket.Connected){

                try{
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 5555);
                }
                catch (SocketException){
                    Console.Clear();
                    Console.WriteLine("Connection attempts {0}", attempts.ToString());
                }
            }

            Console.Clear();
            Console.WriteLine("Connected...");
        }
    }
}
