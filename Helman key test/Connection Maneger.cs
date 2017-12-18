using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Universal_TCP_Client
{
    internal class ConnectionHandler
    {
       
        private static int index = 0;
        private readonly NetworkStream clientStream;
        private readonly StreamReader clientStreamReader;
        private readonly StreamWriter clientStreamWriter;
        public BigInteger n = BigInteger.Parse("9999999929");

        public ConnectionHandler(string name, bool isclient, string ip = "")
        {
            if (isclient)
            {
                var client = new TcpClient(ip, 6789);
                clientStream = client.GetStream();
                Console.WriteLine("server forbundet");
                clientStreamReader = new StreamReader(clientStream);
                clientStreamWriter = new StreamWriter(clientStream);
                clientStreamWriter.AutoFlush = true;
                var g = BigInteger.Parse(clientStreamReader.ReadLine());
                var serverkey = BigInteger.Parse(clientStreamReader.ReadLine());

                MyKey = getrandombigint();
                Thread.Sleep(1000);
                var nexmessege = (Pow(g, MyKey) % n).ToString();
                clientStreamWriter.WriteLineAsync(nexmessege);
                SharedKey = (Pow(serverkey, MyKey) % n).ToString();
                Console.WriteLine(SharedKey);
            }
            else
            {
                var server = new TcpListener(IPAddress.Any, 6789);
                server.Start();
                var serverclient = server.AcceptTcpClient();
                clientStream = serverclient.GetStream();
                Console.WriteLine("client forbundet");
                clientStreamReader = new StreamReader(clientStream);
                clientStreamWriter = new StreamWriter(clientStream);
                clientStreamWriter.AutoFlush = true;

                var g = getrandombigint();
                MyKey = getrandombigint();
                clientStreamWriter.WriteLineAsync(g.ToString());

                var nexmessege = (Pow(g, MyKey) % n).ToString();
                clientStreamWriter.WriteLineAsync(nexmessege);
                Thread.Sleep(1000);

                var clientkey = BigInteger.Parse(clientStreamReader.ReadLine());
                SharedKey = (Pow(clientkey, MyKey) % n).ToString();
                Console.WriteLine(SharedKey);
            }


            Name = name;
        }

        public string messege { get; set; } = "kør";
        public string Name { get; set; }
        public BigInteger MyKey { get; set; }
        public string SharedKey { get; set; }


        public void Reshive()
        {
            while (!messege.ToLower().Equals("stop"))
                try
                {
                    var messege = clientStreamReader.ReadLine();
                    Console.WriteLine(Decrypt(messege));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
        }

        public void Sendmessege()
        {
            while (!messege.ToLower().Equals("stop"))
                try
                {
                    clientStreamWriter.AutoFlush = true;

                    messege = Name + ": " + Console.ReadLine();
                    clientStreamWriter.WriteLine(Encrypt(messege));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
        }

        public BigInteger getrandombigint()
        {
            BigInteger returnvalue = new BigInteger(10000000000000000000);

            while (returnvalue > n)
            {
                var random = new Random();
                var r = random.Next() % 10 + 1;
                var randombig = "";
                for (var i = 0; i < r; i++)
                {
                    randombig += random.Next() % 10;

                }
                returnvalue = BigInteger.Parse(randombig);
            }
        
           

            return returnvalue;
        }

        public BigInteger Pow(BigInteger value, BigInteger exponent)
        {
            var originalValue = value;
            while (exponent-- > 1)
                value = BigInteger.Multiply(value, originalValue);

            return value;
        }


        public string Encrypt(string clearText)
        {
            var EncryptionKey = SharedKey;
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[]
                    {
                        0x49, 0x76, 0x61, 0x6e, 0x15, 0x20, 0x4d, 0x65, 0x61, 0x6e, 0x15, 0x20, 0x4d, 0x65, 0x64, 0x87,
                        0x76, 0x65, 0x64, 0x65, 0x76
                    });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            var EncryptionKey = SharedKey;
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey,
                    new byte[]
                    {
                        0x49, 0x76, 0x61, 0x6e, 0x15, 0x20, 0x4d, 0x65, 0x61, 0x6e, 0x15, 0x20, 0x4d, 0x65, 0x64, 0x87,
                        0x76, 0x65, 0x64, 0x65, 0x76
                    });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}