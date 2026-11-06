using System;
using System.Collections.Generic;
using System.Text;

namespace Conector4.Presence.Models.Estoque
{

    public class moEstoqueR
    {

        public string status { get; set; }
        public string method { get; set; }
        public int nextOffset { get; set; }
        public List<moEstoque> data { get; set; }

    
    }


    public class moEstoque
    {

        public string codigo { get; set; } 

        public string loja { get;set; }

        public double quantidade { get;set; }

    }
}
