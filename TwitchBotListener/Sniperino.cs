using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TwitchBotListener
{
    public class Sniperino
    {
        public string name { get; set; }
        public int challenge { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }

        /// <summary>
        /// Constructor with name and challenge
        /// </summary>
        /// <param name="n"></param>
        /// <param name="c"></param>
        public Sniperino(string n, int c)
        {
            name = n;
            challenge = c;
            wins = 0;
            losses = 0;
            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(@"Json\Sniperinos.json", json);
        }
    }
}
