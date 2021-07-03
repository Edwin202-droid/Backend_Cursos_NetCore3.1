using System.Data;
namespace Persistencia.DapperConexion
{
    public interface IFactoryConection
    {
         void CloseConnection();
         IDbConnection GetConnection();
    }
}