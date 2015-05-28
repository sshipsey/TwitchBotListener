using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBotListener
{
    public class Sniperino
    {
        public string name { get; set; }
        public int challenge { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }

        public Sniperino(string n, int c)
        {
            name = n;
            challenge = c;
        }
    }
}
