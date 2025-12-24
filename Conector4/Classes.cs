using Conector4.General;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Conector4
{
    public class ApiPagedResponse
    {
        public string status { get; set; } = "";
        public int nextOffSet { get; set; }
        public List<object>? data { get; set; }
    }


    public class Fn
    {
        public async Task<DataSet> returnServico()
        {
            var ds = new DataSet();
            var _config = new clsConfig();

            string query = @"
                SELECT 
                    id AS ""Codigo"", 
                    nome AS ""Serviço"",
                    ativo AS ""Ativo"",
                    classe AS ""Classe"", 
                    metodo AS ""Metodo""
                FROM servicos";

            try
            {
                await using var conne = new NpgsqlConnection(_config.getConfigBD());
                await conne.OpenAsync();

                using var cmd = new NpgsqlCommand(query, conne);
                using var da = new NpgsqlDataAdapter(cmd);

                da.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = new DataSet();
                Console.WriteLine("Erro ao buscar serviços: " + ex.Message);
            }

            return ds;
        }

    }
}
