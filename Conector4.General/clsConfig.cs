using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.General
{
    public class clsConfig
    {

        public string getConfigBD()
        {
            string ipDb = "vms.websamos.com.br";
            string userDb = "ti";
            string passDb = "Ti@2015!*";
            string nameDb = "dw";
            string portDb = "5432";

            return $"Host={ipDb};Port={portDb};Username={userDb};Password={passDb};Database={nameDb};";
        }


    }
}
