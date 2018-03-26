using System.ServiceProcess;
using System.Threading;

namespace ImpressaoCCME
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        ServidorTCP servidorTCP = new ServidorTCP();

        protected override void OnStart(string[] args)
        {
            //Sobe servidor TCP
            this.servidorTCP = new ServidorTCP();
            var t = new Thread(servidorTCP.Iniciar);
            t.Start();
        }

        protected override void OnStop()
        {
            this.servidorTCP.Parar();
        }
    }
}
