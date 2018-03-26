using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp2
{
    class Processo
    {
        private Process process;// = new Process();
        public int NumProcesso;
        private TcpClient tcpClient;// = new TcpClient();
        // Declaração de propriedades
        Byte[] Bentrada = new Byte[1024];
        String dados = null;
        Byte[] Bsaida = new Byte[1024]; //resposta para o cliente
        Byte[] Berro = new Byte[1024]; //variavel retorno do erro para o cliente
        int i; //tamanho do buffer de recebimento
        public Int32 executando = 1;

        //construtor
        public Processo(int NumProcesso, TcpClient tcpClient)
        {
            this.NumProcesso = NumProcesso;
            this.tcpClient = tcpClient;
        }

        public void Iniciar()
        {
            LogEventos.Log("ProcessoCCME", "======== CONECTADO =======", NumProcesso.ToString());
            //serverAguardandoConexao = false;

            // Get a stream object for reading and writing
            NetworkStream stream = tcpClient.GetStream();

            while ((i = stream.Read(Bentrada, 0, Bentrada.Length)) != 0)
            {
                try
                {
                    // Traduzir de bytes para ASCII string.
                    char[] retirarCaracteres = { '\r', '\n' };
                    dados = Encoding.ASCII.GetString(Bentrada).Substring(0, i).Trim(retirarCaracteres);
                    //
                    LogEventos.Log("ProcessoCCME", "Tamanho mensagem: " + i, null);
                    LogEventos.Log("ProcessoCCME", "Mensagem: " + dados, null);

                    if (dados != "")
                    {
                        process = new Process();
                        string arquivoREP = String.Empty;
                        string paramRelatorios = String.Empty;
                        string arquivoPDF = String.Empty;
                        string ambiente = String.Empty;
                        //string pastaArqREP = "c:\\a\\rep\\";
                        //string pastaArqREP = "\\C0090\\obj_pc\\usr\\procger\\CCME\\";
                        string pastaArqREP = String.Empty;
                        string pastaPDF = String.Empty;

                        PrcRunReport(ambiente, pastaArqREP, arquivoREP, paramRelatorios, arquivoPDF, ref Bsaida, ref Berro, stream);
                    }
                }
                catch (Exception e)
                {
                    /******************************************/
                    //Desvio de Fluxo
                    /*****************************************/

                    //Log eventos
                    LogEventos.Log("ProcessoCCME", "ERRO DESVIO DE FLUXO", e.ToString());
                    //tcpClient.Close(); //fecha o socket
                    //executando = false;
                    //break;

                }
                /************************************************/

            }
            //Fecha conexão com o cliente
            //tcpClient.Close();
            ServidorTCP.contadorProcessos = 0;
            LogEventos.Log("ProcessoCCME", "======= CONEXAO FINALIZADA =======", NumProcesso.ToString());
            //executando = false;

        }

        private void PrcRunReport(string ambiente, string pastaArqREP, string arquivoREP, string paramRelatorios, string arquivoPDF, ref byte[] Bsaida, ref byte[] Berro, NetworkStream stream)
        {
            //DECLARAÇÃO DE VARIAVEIS
            //declarações
            //string v_erro;
            //string v_output;

            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            int processTimeout = 15000;
            //string exeFileName = "ping";
            //string arguments = "google.com -t";


            string exeFileName = "cmd.exe";
            string arguments = @"/c copy \\C0828\obj_pc\usr\procger\CCME\CCME1854.rep C:\ServicoImpressaoCCME";

            //const string EXECUTAVEL = "CMD /C copy c:\\a\\" + relatorio + "  c:\\a\\" + arquivoPDF;
            //string EXECUTAVEL = "cmd /c copy c:\\a\\" + arquivoREP + " c:\\a\\" + arquivoPDF;
            //paramRelatorios = " /c copy " + pastaServico + arquivoREP + " " + pastaServico + arquivoPDF;
            //string EXECUTAVEL = "cmd.exe";



            /*
                       //-> Copiar arquivo *.rep
                       try
                       {
                           //System.IO.File.Copy(pastaArqREP + arquivoREP, pastaServico + arquivoREP, true);
                           System.IO.File.Copy(pastaArqREP + arquivoREP, pastaAplicacao + arquivoREP, true);
                       }
                       catch (Exception e)
                       {
                           //Log eventos
                           LogEventos.Log("Iniciar", "ERRO COPIAR REP", e.Message);

                           //retorno do erro para o cliente
                           Berro = Encoding.UTF8.GetBytes("ERRO COPIAR REP: " + e.Message);
                           stream.Write(Berro, 0, Berro.Length);

                           throw new Exception();
                       }
                       */



            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = exeFileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    // WorkingDirectory = @"C:\a",
                    //WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                };
                process.StartInfo = processStartInfo;
                process.EnableRaisingEvents = false;
                //process.OutputDataReceived += (sender, eventArgs) => stdout.AppendLine(eventArgs.Data);
                process.OutputDataReceived += (sender, eventArgs) => Console.WriteLine("PROCESSO (" + NumProcesso + ") --> " + eventArgs.Data);
                process.ErrorDataReceived += (sender, eventArgs) => stderr.AppendLine(eventArgs.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                var processExited = process.WaitForExit(processTimeout);
                if (processExited == false) // we timed out...
                {
                    process.Kill(); //mata o processo
                    //Environment.ExitCode = 1;
                    LogEventos.Log("PrcRunReport", "ERRO: Processo levou muito tempo para finalizar", stderr.ToString());
                    //throw new Exception("ERRO: PrcRunReport.Processo levou muito tempo para finalizar");
                    //executando = false;
                    ServidorTCP.contadorProcessos = 0;
                }
                else if (process.ExitCode != 0)
                {
                    var output = stdout.ToString();
                    LogEventos.Log("PrcRunReport", "ERRO: Processo terminou com erro(exit code): " + process.ExitCode, stderr.ToString());
                    //Environment.ExitCode = process.ExitCode;
                    //executando = false;
                }
                else
                {
                    LogEventos.Log("PrcRunReport", "OK", stdout.ToString());
                    //Environment.ExitCode = 0;
                    //executando = false;
                }
            }
            finally
            {
                process.Close();
                //executando = false;
                ServidorTCP.contadorProcessos = 0;
            }
        }

        public void Parar()
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
                process.Close();
                LogEventos.Log("PrcRunReport", "OK Processo parado: ", NumProcesso.ToString());
                //Console.WriteLine("Processo parado: " + NumProcesso);
            }
            catch (Exception e)
            {
                LogEventos.Log("PrcRunReport", "ERRO: Parar()", e.Message);
            }
        }
    }
}
