using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portalum.Zvt;
using Portalum.Zvt.Models;

namespace ZumTesten
{
    class VonZuZvt
    {
        public enum WasTun
        {
            KAUF,
            STORNO,
            TAGESABSCHLUSS
        }

        public delegate void ZVTereignisEventHandler(object sender, StatusInformation e);
        public event ZVTereignisEventHandler ZVTereignis = null;

        public delegate void ZVTLineEventHandler(object sender, string e);
        public event ZVTLineEventHandler ZVTLine = null;


        private String term_adresse = "";
        private int term_port = 0;
        

        public VonZuZvt(String TermAdresse,int TermPort, ZVTereignisEventHandler zVTereignisEventHandler,ZVTLineEventHandler zVTLineEventHandler)
        {
            term_adresse = TermAdresse;
            term_port = TermPort;
            ZVTereignis = zVTereignisEventHandler;
            ZVTLine = zVTLineEventHandler;
        }


        public bool Auftrag(WasTun wastun,System.Decimal summe,int Rechnungsnummer)
        {
            try
            {
                    _ = wastunAsync(wastun, summe,Rechnungsnummer);
                    return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private async Task wastunAsync(WasTun wastun,System.Decimal summe,int Rechnungsnummer)
        {
            var deviceCommunication = new TcpNetworkDeviceCommunication(term_adresse, term_port);
            await deviceCommunication.ConnectAsync();
            if (deviceCommunication.IsConnected)
            {

                var zvtClient = new ZvtClient(deviceCommunication);

                RegistrationConfig rconf = new RegistrationConfig();
                rconf.SendIntermediateStatusInformation = true;
                rconf.ReceiptPrintoutForAdministrationFunctionsViaPaymentTerminal = true;
                await zvtClient.RegistrationAsync(rconf);

                zvtClient.StatusInformationReceived += (statusInformation) => ZVTereignis(this, statusInformation);
                zvtClient.IntermediateStatusInformationReceived += (text) => ZVTLine(this, text);
                zvtClient.LineReceived += (printlineinfo) => Console.WriteLine(printlineinfo.Text);
                zvtClient.ReceiptReceived += (rechnung) => test(rechnung);

                switch (wastun)
                {
                    case WasTun.KAUF:
                        await zvtClient.PaymentAsync(summe);
                        break;

                    case WasTun.STORNO:
                        await zvtClient.ReversalAsync(Rechnungsnummer);
                        break;

                    case WasTun.TAGESABSCHLUSS:
                        await zvtClient.EndOfDayAsync();
                        break;
                }
                await deviceCommunication.DisconnectAsync();
            }
            else
            {
                ZVTLine(this, "Keine Verbindung zum Terminal !");
            }
        }

        void test(ReceiptInfo rinfo)
        {
            string tret = "";
        }


    }
}
