using System;
using System.Net;

namespace ImpressaoCCME
{
    /**************************************
    //  Configurações de toda a aplicação
    ***************************************/

    //classe para ler arquivo cfg\config.txt
    class Configuracoes
    {
        public const string PASTA_APLICACAO = @"C:\ImpressaoCCME\";
        public string pastaLog = PASTA_APLICACAO + @"\log\";
        public string arquivoLog = "log.txt";
        //
        //https://pt.wikipedia.org/wiki/Lista_de_portas_dos_protocolos_TCP_e_UDP#Portas_1058_a_47808
        //ip da máquina
        //static string NumeroIP { get; set; }
        static string nome = Dns.GetHostName();
        public string NumeroIp = Dns.GetHostAddresses(nome)[0].ToString();//  IPAddress.Parse("127.0.0.1"); //Dns.GetHostEntry("localhost").AddressList[0];

        //Pegar config do arquivo
        ///
        ///Porta
        public Int32 PortaTcp;
        //conexao BDs
        public string ConexaoBdHom;
        public string ConexaoBdPrd;


        //Construtor
        public Configuracoes()
        {
            try
            {
                string pastaConfig = PASTA_APLICACAO + @"\cfg\";
                string arquivoConfig = "config.txt";
                string separador = "=";

                if (!System.IO.File.Exists(pastaConfig + arquivoConfig))
                    throw new Exception("Impossivel continuar. Arquivo de configuracao nao encontrado: " + pastaConfig + arquivoConfig);

                //ler arquivo txt de configurações
                using (System.IO.StreamReader sr = new System.IO.StreamReader(pastaConfig + arquivoConfig))
                {
                    string linha = sr.ReadLine();
                    while (linha != null)
                    {
                        if (!linha.StartsWith("#"))
                        {
                            string chave = linha.Substring(0, linha.IndexOf(separador)).ToUpper();
                            switch (chave)
                            {
                                /*case "NUMERO_IP":
                                    NumeroIP = linha.Substring(linha.IndexOf(separador) + 1);
                                    break;*/
                                case "PORTA_TCP":
                                    PortaTcp = Int32.Parse(linha.Substring(linha.IndexOf(separador) + 1));
                                    break;
                                case "CONEXAO_BD_HOM":
                                    ConexaoBdHom = linha.Substring(linha.IndexOf(separador) + 1);
                                    break;
                                case "CONEXAO_BD_PRD":
                                    ConexaoBdPrd = linha.Substring(linha.IndexOf(separador) + 1);
                                    break;
                                default:
                                    break;
                            }
                        }
                        linha = sr.ReadLine();
                    }
                    if (PortaTcp == 0 || String.IsNullOrEmpty(ConexaoBdHom) || String.IsNullOrEmpty(ConexaoBdPrd))
                        throw new Exception("Arquivo de configuracao incompleto");
                }
            }
            catch (Exception e)
            {
                //Log eventos
                //LogEventos("LerConfiguracoes", "LerConfiguracoes", e.Message);
                Console.WriteLine(e.Message);
                Console.ReadKey();
                System.Environment.Exit(1);
            }

        }
    }
}
