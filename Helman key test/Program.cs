using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using Universal_TCP_Client;

namespace Helman_key_test
{
    class Program
    {
        static void Main(string[] args)
        {
 
                Console.WriteLine("Please enter name");
                string name = Console.ReadLine();
                Console.WriteLine("press 1 for client and 2 for server");
                ConsoleKey pressedkey = Console.ReadKey(true).Key;

                if (pressedkey.Equals(ConsoleKey.D1) || pressedkey.Equals(ConsoleKey.NumPad1))
                {
                    try
                    {
                        Console.WriteLine("please enter ip");
                        string ip = Console.ReadLine();
                        ConnectionHandler handler = new ConnectionHandler(name, true, ip);
                        Thread Reshivethread = new Thread(handler.Reshive);
                        Thread Sendthread = new Thread(handler.Sendmessege);
                        Reshivethread.Start();
                        Sendthread.Start();
                        Console.WriteLine("Du kan nu skrive");

                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e);
                    }
                }

                else if (pressedkey.Equals(ConsoleKey.D2) || pressedkey.Equals(ConsoleKey.NumPad2))
                {



                    ConnectionHandler handler = new ConnectionHandler(name, false);
                    Thread Reshivethread = new Thread(handler.Reshive);
                    Thread Sendthread = new Thread(handler.Sendmessege);
                    Reshivethread.Start();
                    Sendthread.Start();
                    Console.WriteLine("Du kan nu skrive");
                }
                else
                {
                    Console.WriteLine("button not regconiced");
                }

        }
    }
}
