using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Portalum.Zvt;
using Portalum.Zvt.Models;

namespace ZumTesten
{
    public partial class Form1 : Form
    {
        public enum seppl
        {
            TEST,
            MAX,
            HALLO
        }

        public delegate void Anzeige(string Text, seppl Wo);
        Anzeige anz;

        TestEreignis te;
        VonZuZvt vzzvt = null;

        int Rechnungsnummer;

        public Form1()
        {

            InitializeComponent();





            anz = (text, wo) =>
            {
                try
                {
                    this.Invoke(new Action(() =>
                    {
                        this.Text = ($"{ text}---{ wo}");
                    }));
                }
                catch (Exception ex)
                {
                    ;
                }
            };

            try
            {
                te = new TestEreignis(1, new TestEreignis.ZeitereignisEventHandler(ZeitEreignis));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }




            anz("WAS IMMER", seppl.MAX);


            Console.WriteLine(zuDatum(1, 2, 3));
            seppl max = new seppl();
            max = seppl.HALLO;
            Console.WriteLine(max.ToString());
        }

        void ZeitEreignis(object sender, string e)
        {
            anz(e, seppl.TEST);
        }


        public int[] ParallelForEach()
        {
            var array = new int[1000000];
            Parallel.For(0, 1000000, i => { array[i] = i; });
            return array;

        }



        private static (string day, int month, int year) zuDatum(int day, int month, int year)
        {
            return (day.ToString(), month, year);
        }

        private void Ergebnis()
        {

        }



        void zvtereignis(object sender, StatusInformation e)
        {
            StatusInformation statusInformation = (StatusInformation)e;
            Console.WriteLine(statusInformation.ErrorMessage + " " + statusInformation.CardName + " " + statusInformation.Amount);
            Rechnungsnummer = statusInformation.ReceiptNumber;
            try
            {
                this.Invoke(new Action(() =>
                {
                    label1.Text = statusInformation.ErrorMessage;
                    label2.Text = statusInformation.CardName;
                    label3.Text = statusInformation.Amount.ToString("0.00");
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        void zvtline(object sender, string e)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    label4.Text = e;
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        private async Task zahleAsync()
        {
            var deviceCommunication = new TcpNetworkDeviceCommunication("192.168.0.101", port: 20007);
            await deviceCommunication.ConnectAsync();


            var zvtClient = new ZvtClient(deviceCommunication);
            zvtClient.StatusInformationReceived += (statusInformation) => Console.WriteLine(statusInformation.ErrorMessage + " " + statusInformation.CardName + " " + statusInformation.Amount);

            await zvtClient.PaymentAsync(10.5M);

            await deviceCommunication.DisconnectAsync();
        }



        delegate int RechenOperation(int a, int b);
        RechenOperation multipliziere = (x, y) => x * y;


        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";

            vzzvt = new VonZuZvt("192.168.0.101", 20007, new VonZuZvt.ZVTereignisEventHandler(zvtereignis),new VonZuZvt.ZVTLineEventHandler (zvtline));
            vzzvt.Auftrag(VonZuZvt.WasTun.KAUF, 8.0M, 0);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";

            vzzvt = new VonZuZvt("192.168.0.101", 20007, new VonZuZvt.ZVTereignisEventHandler(zvtereignis), new VonZuZvt.ZVTLineEventHandler(zvtline));
            vzzvt.Auftrag(VonZuZvt.WasTun.STORNO, 8.0M, Rechnungsnummer);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";

            vzzvt = new VonZuZvt("192.168.0.101", 20007, new VonZuZvt.ZVTereignisEventHandler(zvtereignis), new VonZuZvt.ZVTLineEventHandler(zvtline));
            vzzvt.Auftrag(VonZuZvt.WasTun.TAGESABSCHLUSS, 0, 0);
            //zahleAsync();
        }
        //zahleAsync();
    }
}
 