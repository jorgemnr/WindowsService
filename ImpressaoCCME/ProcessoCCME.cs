using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ImpressaoCCME
{
    class ProcessoCCME
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
        public ProcessoCCME(int NumProcesso, TcpClient tcpClient)
        {
            this.NumProcesso = NumProcesso;
            this.tcpClient = tcpClient;
        }

        public void Iniciar()
        {
            LogEventos.Log("ProcessoCCME", "======== CONECTADO =======", null);
            //serverAguardandoConexao = false;

            // Get a stream object for reading and writing
            NetworkStream stream = tcpClient.GetStream();
            try
            {
                while ((i = stream.Read(Bentrada, 0, Bentrada.Length)) != 0)
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
                        const Char DELIMITADOR = ' ';
                        const Char DELIMITADOR1 = '_';

                        /************************************************/
                        //try principal do fluxo de cada execução
                        try
                        {
                            //VERIFICAR SE TRATAMENTO SHIPPMENT OU IMPRRESSÃO AUTOMATICA
                            if (dados.StartsWith("IMPR:"))
                            {
                                //-->> IMPRESSÃO AUTOMATICA
                                dados = dados.Substring(5);

                                //-> SEPARANDO OS PARAMETROS
                                try
                                {
                                    foreach (string s in dados.Split(DELIMITADOR))
                                    {
                                        /*if (s.StartsWith("MODULE="))
                                        {
                                            //arquivoREP = s;
                                            arquivoREP = s.Substring(s.IndexOf("=") + 1);
                                        }
                                        else*/
                                        if (s.StartsWith("DESNAME="))
                                        {
                                            //pastaPDF = s.Substring(0, s.LastIndexOf('\\') - 1);
                                            // int b = s.Length;
                                            //int a = s.LastIndexOf("\\");
                                            //pastaPDF = s.Substring(s.IndexOf("=") + 1, s.LastIndexOf("\\") - 8);
                                            //arquivoPDF = s.Substring(s.LastIndexOf('\\') + 1);
                                            arquivoPDF = s.Substring(0, s.IndexOf(".pdf") + 4);
                                            ambiente = s.Substring(s.IndexOf(@"\\") + 2, s.IndexOf(@"\") - 3);
                                            pastaArqREP = @"\\" + ambiente + @"\obj_pc\usr\procger\CCME\";

                                            //MODULE
                                            string[] sArqREP = s.Split(DELIMITADOR1);
                                            arquivoREP = sArqREP[7] + ".rep";
                                        }
                                        else
                                        {
                                            //paramRelatorios = paramRelatorios + " " + s;
                                            paramRelatorios += s + ' ';
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    //Log eventos
                                    LogEventos.Log("ProcessoCCME", "ERRO SEPARAR PARAM", e.Message);

                                    //retorno do erro para o cliente
                                    Berro = Encoding.ASCII.GetBytes("ERRO SEPARAR PARAM: " + e.Message + "\n\r");
                                    stream.Write(Berro, 0, Berro.Length);

                                    throw new Exception();
                                }
                            }
                            else
                            {
                                //-->> SHIPPING
                                arquivoREP = dados.Substring(5, 8); //MODULE
                                dados = dados.Substring(14);

                                try
                                {
                                    foreach (string s in dados.Split(DELIMITADOR))
                                    {
                                        /*if (s.StartsWith("MODULE="))
                                        {
                                            //arquivoREP = s;
                                            arquivoREP = s.Substring(s.IndexOf("=") + 1);
                                        }
                                        else*/
                                        if (s.StartsWith("DESNAME="))
                                        {
                                            //pastaPDF = s.Substring(0, s.LastIndexOf('\\') - 1);
                                            // int b = s.Length;
                                            //int a = s.LastIndexOf("\\");
                                            //pastaPDF = s.Substring(s.IndexOf("=") + 1, s.LastIndexOf("\\") - 8);
                                            //arquivoPDF = s.Substring(s.LastIndexOf('\\') + 1);
                                            arquivoPDF = s.Substring(0, s.IndexOf(".pdf") + 4);
                                            ambiente = s.Substring(s.IndexOf(@"\\") + 2, s.IndexOf(@"\") - 3);
                                            pastaArqREP = @"\\" + ambiente + @"\obj_pc\usr\procger\CCME\";
                                        }
                                        else
                                        {
                                            //paramRelatorios = paramRelatorios + " " + s;
                                            paramRelatorios += s + ' ';
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    //Log eventos
                                    LogEventos.Log("ProcessoCCME", "ERRO SEPARAR PARAM SHIP", e.Message);

                                    //retorno do erro para o cliente
                                    Berro = Encoding.ASCII.GetBytes("ERRO SEPARAR PARAM SHIP: " + e.Message + "\n\r");
                                    stream.Write(Berro, 0, Berro.Length);

                                    throw new Exception();
                                }

                            }


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
                            // CONVERTER PARA PDF
                            //prcRunReport(ambiente, pastaArqREP, arquivoREP, paramRelatorios, arquivoPDF, ref Bsaida, ref Berro, stream);
                            PrcRunReport(ambiente, pastaArqREP, arquivoREP, paramRelatorios, arquivoPDF, ref Bsaida, ref Berro, stream);

                            /*
                            // Copiar PDF para pasta destino
                            try
                            {
                                //System.IO.File.Move("c:\\a\\" + arquivoPDF, pastaPDF+arquivoPDF);
                                System.IO.File.Copy(pastaServico + arquivoPDF, pastaPDF +"\\"+ arquivoPDF, true);
                            }
                            catch (Exception e)
                            {
                                //Log eventos
                                LogEventos.Log("Iniciar", "ERRO COPIAR PDF", e.Message);

                                //retorno do erro para o cliente
                                Berro = Encoding.UTF8.GetBytes("ERRO COPIAR PDF: " + e.Message);
                                stream.Write(Berro, 0, Berro.Length);
                            }

                            //-> Excluir arquivo *.rep
                            try
                            {
                                System.IO.File.Delete(pastaServico + arquivoREP);
                            }
                            catch (Exception e)
                            {
                                //Log eventos
                                LogEventos.Log("Iniciar", "ERRO EXCLUIR REP", e.Message);

                                //retorno do erro para o cliente
                                Berro = Encoding.UTF8.GetBytes("ERRO COPIAR REP: " + e.Message);
                                stream.Write(Berro, 0, Berro.Length);
                            }


                            //-> Excluir arquivo *.pdf
                            try
                            {
                                System.IO.File.Delete(pastaServico + arquivoPDF);
                            }
                            catch (Exception e)
                            {
                                //Log eventos
                                LogEventos.Log("Iniciar", "ERRO EXCLUIR PDF", e.Message);

                                //retorno do erro para o cliente
                                Berro = Encoding.UTF8.GetBytes("ERRO COPIAR PDF: " + e.Message);
                                stream.Write(Berro, 0, Berro.Length);
                            }
                            */
                        }

                        catch (Exception e)
                        {
                            /******************************************/
                            //Desvio de Fluxo
                            /*****************************************/

                            //Log eventos
                            LogEventos.Log("ProcessoCCME", "ERRO DESVIO DE FLUXO", e.ToString());
                            //break; //while

                            //tcpClient.Close(); //fecha o socket
                            //break;
                        }/*
                            finally
                            {
                                //-> Excluir arquivo *.rep
                                try
                                {
                                    System.IO.File.Delete(pastaServico + arquivoREP);
                                }
                                catch (Exception e)
                                {
                                    //Log eventos
                                    LogEventos.Log("Iniciar", "ERRO EXCLUIR REP1", e.Message);

                                    //retorno do erro para o cliente
                                    Berro = Encoding.UTF8.GetBytes("ERRO COPIAR REP1: " + e.Message);
                                    stream.Write(Berro, 0, Berro.Length);
                                }

                                //-> Excluir arquivo *.pdf
                                try
                                {
                                    System.IO.File.Delete(pastaServico + arquivoPDF);
                                }
                                catch (Exception e)
                                {
                                    //Log eventos
                                    LogEventos.Log("Iniciar", "ERRO EXCLUIR PDF1", e.Message);

                                    //retorno do erro para o cliente
                                    Berro = Encoding.UTF8.GetBytes("ERRO COPIAR PDF1: " + e.Message);
                                    stream.Write(Berro, 0, Berro.Length);
                                }
                            }
                            /************************************************/

                    }
                }
            }
            catch (Exception e)
            {
                //Log eventos
                LogEventos.Log("ProcessoCCME", "ERRO: Conexao", e.Message);
            }
            finally
            {
                //Fecha conexão com o cliente
                //tcpClient.Close();
                LogEventos.Log("ProcessoCCME", "======= CONEXAO FINALIZADA =======", null);
                ServidorTCP.contadorProcessos = 0;
            }
        }

        private void PrcRunReport(string ambiente, string pastaArqREP, string arquivoREP, string paramRelatorios, string arquivoPDF, ref byte[] Bsaida, ref byte[] Berro, NetworkStream stream)
        {
            //DECLARAÇÃO DE VARIAVEIS
            //declarações
            //string v_erro;
            //string v_output;

            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            int processTimeout = 30000;

            const string EXECUTAVEL = "C:\\oracle\\dev6i\\BIN\\rwrun60";
            //const string EXECUTAVEL = "CMD /C copy c:\\a\\" + relatorio + "  c:\\a\\" + arquivoPDF;
            //string EXECUTAVEL = "cmd /c copy c:\\a\\" + arquivoREP + " c:\\a\\" + arquivoPDF;
            //paramRelatorios = " /c copy " + pastaServico + arquivoREP + " " + pastaServico + arquivoPDF;
            //string EXECUTAVEL = "cmd.exe";
            const string LINHA_COMANDO = "DESTYPE=FILE DESFORMAT=PDF BATCH=YES PARAMFORM=NO BACKGROUND=NO";
            //const string CONEXAO_BD_HOM = "USERID=jrodrig9/Abril20166.@C0828STA";
            //const string CONEXAO_BD_PRD = "USERID=robosrp/rbc001450@C0090PRD";
            string ConexaoBdHom = "USERID=" + ServidorTCP.cfg.ConexaoBdHom;
            string ConexaoBdPrd = "USERID=" + ServidorTCP.cfg.ConexaoBdPrd;

            string ConexaoBd;

            if (ambiente.ToUpper() == "C0828")
                ConexaoBd = ConexaoBdHom;
            else
                ConexaoBd = ConexaoBdPrd;

            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = EXECUTAVEL,
                    Arguments = ConexaoBd + " REPORT=" + pastaArqREP + arquivoREP + " " + arquivoPDF + " " + LINHA_COMANDO + " " + paramRelatorios,
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

                    //resposta para o cliente
                    Bsaida = Encoding.ASCII.GetBytes("ERRO: Processo levou muito tempo para finalizar: " + stderr.ToString() + "\n\r");
                    stream.Write(Bsaida, 0, Bsaida.Length);
                    //log
                    LogEventos.Log("PrcRunReport", "ERRO: Processo levou muito tempo para finalizar", stderr.ToString());
                    LogEventos.Log("PrcRunReport", "ERRO: Output", stdout.ToString());
                }
                else if (process.ExitCode != 0)
                {
                    //resposta para o cliente
                    Bsaida = Encoding.ASCII.GetBytes("ERRO : Processo terminou com erro(exit code): " + process.ExitCode + " - " + stdout.ToString() + "\n\r");
                    stream.Write(Bsaida, 0, Bsaida.Length);
                    //log
                    LogEventos.Log("PrcRunReport", "ERRO: Processo terminou com erro(exit code): " + process.ExitCode, stdout.ToString());
                    LogEventos.Log("PrcRunReport", "ERRO:", stderr.ToString());
                }
                else
                {
                    //resposta para o cliente
                    Bsaida = Encoding.ASCII.GetBytes("\n\r");
                    stream.Write(Bsaida, 0, Bsaida.Length);
                    //log
                    LogEventos.Log("PrcRunReport", "OK", null);
                }
            }
            finally
            {
                process.Close();
            }

            /*

            Process si = new Process();

            //"C:\\oracle\\dev6i\\BIN\\rwrun60 MODULE=Z:\\ccme1867 DESNAME=Z:\\CCME1867_RELATORIO.PDF DESTYPE=FILE DESFORMAT=PDF BATCH=YES PARAMFORM=NO USERID=jrodrig9/Abril20166.@c0828sta BACKGROUND=NO P_CD_LISTA_CARGA=500075";
            //si.StartInfo.FileName = dados;
            si.StartInfo.FileName = EXECUTAVEL;
            //si.StartInfo.Arguments = "MODULE=Z:\\ccme1867 DESNAME=Z:\\CCME1867_RELATORIO.PDF DESTYPE=FILE DESFORMAT=PDF BATCH=YES PARAMFORM=NO USERID=jrodrig9/Abril20166.@c0828sta BACKGROUND=NO P_CD_LISTA_CARGA=500075";
            //si.StartInfo.Arguments = paramRelatorios;
            //si.StartInfo.Arguments = CONEXAO_BD + " REPORT=" + pastaServico + arquivoREP + " DESNAME=" + pastaServico + arquivoPDF + " " + LINHA_COMANDO + " " + paramRelatorios;
            si.StartInfo.Arguments = ConexaoBd + " REPORT=" + pastaArqREP + arquivoREP + " " + arquivoPDF + " " + LINHA_COMANDO + " " + paramRelatorios;
            si.StartInfo.UseShellExecute = false;
            si.StartInfo.CreateNoWindow = true;
            si.StartInfo.RedirectStandardError = true;
            si.StartInfo.RedirectStandardOutput = true;
            si.StartInfo.RedirectStandardInput = true;

            //
            //executa comando no sistema operacional
            //
            si.Start();

            v_erro = si.StandardError.ReadToEnd();
            v_output = si.StandardOutput.ReadToEnd();

            si.WaitForExit(60000); //1 minuto
            //si.WaitForExit(); //1 minuto

            //v_erro = "";
            //v_output = "";

            si.Close();



            //Se houver erro
            //if (!String.IsNullOrEmpty(v_erro.Trim()))
            if (v_erro.Trim() != "")
            {
                //Log eventos
                LogEventos.Log("prcRunReport", "ERRO PROCESSO", v_erro.Trim());

                //resposta para o cliente
                Bsaida = Encoding.ASCII.GetBytes("ERRO PROCESSO: " + v_erro.Trim() + "\n\r");
                stream.Write(Bsaida, 0, Bsaida.Length);

                //Desvio de fluxo
                throw new Exception();
            }
            else
            {
                //Log eventos
                LogEventos.Log("prcRunReport", "OK: " + v_output.Trim(), null);

                //resposta para o cliente
                Bsaida = Encoding.ASCII.GetBytes("OK: " + v_output.Trim() + "\r\n");
                //Bsaida = Encoding.ASCII.GetBytes(" \r\n");
                stream.Write(Bsaida, 0, Bsaida.Length);
                //stream.WriteByte(0x1A);
                //stream.Flush();
            }

        }
        catch (InvalidOperationException e)
        {
            //Log eventos
            LogEventos.Log("prcRunReport", "ERRO DE OPERACAO INVALIDA", e.Message);

            //retorno do erro para o cliente
            Berro = Encoding.ASCII.GetBytes("ERRO DE OPERACAO INVALIDA: " + e.Message + "\n\r");
            stream.Write(Berro, 0, Berro.Length);

            //Desvio de fluxo
            throw new Exception();

        }
        catch (Win32Exception e)
        {
            //Log eventos
            LogEventos.Log("prcRunReport", "ERRO [" + EXECUTAVEL + "] COMANDO INEXISTENTE", e.Message);

            //retorno do erro para o cliente
            Berro = Encoding.ASCII.GetBytes("ERRO [" + EXECUTAVEL + "] COMANDO INEXISTENTE: " + e.Message);
            stream.Write(Berro, 0, Berro.Length);

            //Desvio de fluxo
            throw new Exception();
        }
        */

        }

        public void Parar()
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
                process.Close();

                Console.WriteLine("Processo parado: " + NumProcesso);
            }
            catch (Exception e)
            { Console.WriteLine("ERRO Parar: " + e.Message); }
        }
    }
}
