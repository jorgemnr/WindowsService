using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp2
{
    class ServidorTCP
    {
        //ler configuracoes
        public static Configuracoes cfg = new Configuracoes();
        //
        private static string nome = Dns.GetHostName();
        private static IPAddress[] ip = Dns.GetHostAddresses(nome);
        private static IPAddress localAddr = ip[1].MapToIPv4(); //IP DE ESCUTA
        private TcpListener tcpListener = new TcpListener(localAddr, cfg.PortaTcp);
        private Thread t;
        private TcpClient tcpClient;
        //private Processo proc;
        private ProcessoCCME proc;

        //private static IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); //Dns.GetHostEntry("localhost").AddressList[0];

        //
        public static int contadorProcessos;

        //
        /*
        //https://pt.wikipedia.org/wiki/Lista_de_portas_dos_protocolos_TCP_e_UDP#Portas_1058_a_47808
        //static IPAddress localAddr = ip[1].MapToIPv4(); //IP DE ESCUTA
        //static IPAddress localAddr = IPAddress.Parse(cfg.NumeroIP); //IP DE ESCUTA

        public ServidorTCP()
        {
*/

        public void Iniciar()
        {
            try
            {
                tcpListener.Start();
                LogEventos.Log("ServidorTCP", "===================================================================", null);
                LogEventos.Log("ServidorTCP", "Configurações Porta TCP", cfg.PortaTcp.ToString());
                LogEventos.Log("ServidorTCP", "Configurações Conexao BD HOM", cfg.ConexaoBdHom);
                LogEventos.Log("ServidorTCP", "Configurações Conexao BD PRD", cfg.ConexaoBdPrd);
                LogEventos.Log("ServidorTCP", "===================================================================", null);
                LogEventos.Log("ServidorTCP", "Aguardando por uma conexão...", null);
                LogEventos.Log("ServidorTCP", "===================================================================", null);

                while (true)
                {
                    if (!tcpListener.Pending())
                    {
                        Console.WriteLine("---------- Sorry, no connection requests have arrived ------------");
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (contadorProcessos < 1)
                        {
                            //Accept the pending client connection and return a TcpClient object initialized for communication.
                            tcpClient = tcpListener.AcceptTcpClient();

                            //Cria processo
                            //proc = new Processo(contadorProcessos, tcpClient);
                            proc = new ProcessoCCME(contadorProcessos, tcpClient);

                            //iniciando processos em uma thread
                            t = new Thread(proc.Iniciar);
                            t.Start();

                            contadorProcessos += 1;
                        }
                        else
                        {
                            TcpClient tcpClient = tcpListener.AcceptTcpClient();
                            NetworkStream stream = tcpClient.GetStream();
                            Byte[] Bmsg = Encoding.ASCII.GetBytes("Qtde Conexao maxima alcancada: " + contadorProcessos);
                            stream.Write(Bmsg, 0, Bmsg.Length);
                            Thread.Sleep(2000);
                            tcpClient.Close();
                            LogEventos.Log("ServidorTCP", "Qtde Conexao maxima alcancada", contadorProcessos.ToString());
                        }
                    }
                }

            }
            catch (InvalidOperationException i)
            {
                LogEventos.Log("ServidorTCP", "OK servidor TCP fechado", i.Message);
            }
            catch (Exception e)
            {
                LogEventos.Log("ServidorTCP", "ERRO Loop de escuta TCP", e.Message);
            }
        }

        public void Parar()
        {
            try
            {
                //
                //Fechar processo
                if (proc != null)
                    if (proc.executando == 1)
                    {
                        proc.Parar();
                        LogEventos.Log("ServidorTCP", "Processo Parado", null);
                    }
                //
                //Fechar Thread
                if (t != null)
                    if (t.IsAlive)
                    {
                        t.Abort();
                        LogEventos.Log("ServidorTCP", "Thread Parado", null);
                    }
                //
                //Fechar sockets
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    LogEventos.Log("ServidorTCP", "Socket Parado", null);
                }
                //
                //fechar servidorTCP
                tcpListener.Stop();
                LogEventos.Log("ServidorTCP", "Servidor Parado", null);
            }
            catch (Exception e)
            {
                LogEventos.Log("ServidorTCP", "ERRO Parar()", e.Message);
            }
        }
    }
}