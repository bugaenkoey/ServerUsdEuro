using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerUsdEuro
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ServerUsdEuro");

            Listener();

        }

        private static void Listener()
        {
            TcpListener list;
            try
            {
                list = new TcpListener(IPAddress.Loopback, 80);
                //начало прослушивания клиентов
                list.Start();
                //создание отдельного потока
                //для чтения сообщения
                Thread thread = new Thread(new ThreadStart(ThreadFun));
                thread.IsBackground = true;
                //запуск потока
                Console.WriteLine("запуск потока");
                thread.Start();

            }
            catch (SocketException sockEx)
            {
                Console.WriteLine("Ошибка сокета:" + sockEx.Message);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Ошибка:" + Ex.Message);
            }
            Console.ReadLine();

            void ThreadFun()
            {
                while (true)
                {
                    //сервер сообщает клиенту о готовности
                    //к соединению
                    TcpClient client = list.AcceptTcpClient();
                    Console.Beep();
                    //чтение данных из сети в формате
                    //Unicode
                    NetworkStream networkStream = null;
                    networkStream = client.GetStream();


                    // получаем ответ
                    byte[] data = new byte[32]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (networkStream.DataAvailable);

                    string message = builder.ToString();

                    Console.WriteLine("получено " + message);

                    if (message.ToUpper() == "USD EURO")
                    {
                        string str = "USD EURO = 0.88";
                        Write(networkStream, str);

                    }
                    if (message.ToUpper() == "EURO USD")
                    {
                        string str = "EURO USD = " + 1.12;
                        Write(networkStream, str);
                    }

                    //при получении сообщения EXIT
                    //завершить приложение
                    if (message.ToUpper() == "EXIT")
                    {
                        Console.WriteLine("EXIT");
                        list.Stop();
                        // this.Close();

                    }
                    client.Close();

                }
            }
        }

        private static void Write(NetworkStream networkStream, string str)
        {
            Console.WriteLine(str);
            byte[] by = Encoding.Unicode.GetBytes(str);
            networkStream.Write(by, 0, by.Length);
        }
    }
}
