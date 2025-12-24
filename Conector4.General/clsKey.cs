namespace Conector4.General
{

    public class KeyCredential
    {
        public string User { get; set; }
        public string Pass { get; set; }
    }
    public class clsKey
    {

        clsConfig conf = new clsConfig();

        public enum PlataformType
        {
            Presence
        }

        public async Task<List<KeyCredential>> GetKeysAsync(PlataformType plataforma)
        {
            var keys = new List<KeyCredential>();

            using (var db = new Npgsql.NpgsqlConnection(conf.getConfigBD()))
            {
                await db.OpenAsync();

                using (var cmd = new Npgsql.NpgsqlCommand(
                    "SELECT \"user\", pass FROM keys WHERE plataforma = @plataforma",
                    db))
                {
                    cmd.Parameters.AddWithValue("plataforma", plataforma.ToString());

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            keys.Add(new KeyCredential
                            {
                                User = reader["user"].ToString(),
                                Pass = reader["pass"].ToString()
                            });
                        }
                    }
                }
            }

            return keys;
        }

    }
}
