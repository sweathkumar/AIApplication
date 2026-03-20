using System.Data.SqlClient;
using System.Diagnostics;

namespace AIApplication.Data
{

    public class SqlService
    {
        private readonly string _connection;

        public SqlService(IConfiguration config)
        {
            _connection = config.GetConnectionString("Default") ?? "";
        }

        [Obsolete]
        public async Task Save(string text, List<float> vector)
        {
            using var conn = new SqlConnection(_connection);
            try
            {
                var vectorString = "[" + string.Join(",", vector) + "]";

                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "INSERT INTO Documents (Content, Embedding) VALUES (@c, @e)", conn);

                cmd.Parameters.AddWithValue("@c", text);
                cmd.Parameters.AddWithValue("@e", vectorString);

                await cmd.ExecuteNonQueryAsync();
                conn.Close();
            }
            catch (Exception ex) {
                conn.Close();
                Debug.WriteLine(ex.ToString());
                return;
            }
        }
    }
}
