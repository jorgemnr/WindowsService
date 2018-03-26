using System;
using System.Threading;

namespace ConsoleApp2
{
    class Program
    {

        static void Main(string[] args)
        {
            //Sobe servidor TCP
            ServidorTCP servidorTCP = new ServidorTCP();
            var t = new Thread(servidorTCP.Iniciar);
            t.Start();

            //Aguarda
            string key = null;
            while (key == null)
            {
                key = Console.ReadLine();
                if (key == "1")
                {
                    servidorTCP.Parar();
                    key = null;
                }
                else
                {
                    servidorTCP.Parar();
                }
            }

            Console.ReadKey();
        }
    }
}
