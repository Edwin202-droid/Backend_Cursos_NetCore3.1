using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Persistencia.DapperConexion
{
    //Conectar a la base de  datos mediante Dapper
    public class FactoryConection : IFactoryConection
    {
        private IDbConnection connection;
        private readonly IOptions<ConexionConfiguracion> configs;
        public FactoryConection(IOptions<ConexionConfiguracion> configs)
        {
            this.configs= configs;
        }

        public void CloseConnection()
        {
            if(connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public IDbConnection GetConnection()
        {
            if(connection == null)
            {
                connection = new SqlConnection(configs.Value.DefaultConnection);
            }
            if(connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }
    }
}