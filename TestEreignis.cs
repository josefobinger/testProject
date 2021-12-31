using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZumTesten
{
    class TestEreignis
    {
        public delegate void ZeitereignisEventHandler(object sender, String  e);
        public event ZeitereignisEventHandler Zeitereignis = null;

        private System.Timers.Timer timer1;
        private int mySekunden;
        private int zaehler = 0;

        public TestEreignis(int Sekunden,ZeitereignisEventHandler zeitereignisEventHandler)
        {
            Zeitereignis+= zeitereignisEventHandler;
            mySekunden = Sekunden;
            timer1 = new System.Timers.Timer(mySekunden * 1000);
            timer1.Enabled = true;
            timer1.Elapsed += Timer1_Elapsed;
        }

       

        private void Timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                zaehler++;
                System.Timers.Timer timer = (System.Timers.Timer)sender;
                if (Zeitereignis != null)
                    Zeitereignis(timer, "SEPPL " + zaehler);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
            timer1.Enabled = true;

        }
    }
}
