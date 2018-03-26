using System;

namespace ImpressaoCCME
{
    static class LogEventos
    {
        public static void Log(string sFonte, string sLog, string sEvento)
        {
            try
            {
                //if (!System.IO.Directory.Exists(pastaAplicacao))
                //System.IO.Directory.CreateDirectory(pastaAplicacao);
                if (!System.IO.File.Exists(ServidorTCP.cfg.pastaLog + ServidorTCP.cfg.arquivoLog))
                    System.IO.File.Create(ServidorTCP.cfg.pastaLog + ServidorTCP.cfg.arquivoLog).Close();

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ServidorTCP.cfg.pastaLog + ServidorTCP.cfg.arquivoLog, true))
                {
                    sw.WriteLine("Data: " + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "\tFonte: " + sFonte + "\tLog: " + sLog + "\tEvento: " + sEvento);
                    Console.WriteLine("Data: " + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "\tFonte: " + sFonte + "\tLog: " + sLog + "\tEvento: " + sEvento);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Nao foi possivel gravar arquivo de log: " + e.Message);
                Console.ReadKey();
                System.Environment.Exit(1);
            }
        }



    }
}
