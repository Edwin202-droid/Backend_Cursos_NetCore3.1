using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionRepositorio : IPaginacion
    {
        private readonly IFactoryConection factoryConection;

        public PaginacionRepositorio(IFactoryConection factoryConection)
        {
            this.factoryConection = factoryConection;
        }
        public async Task<PaginacionModel> devolverPaginacion(string storeProcedure, int numeroPagina, int cantidadElementos, IDictionary<string, object> parametrosFiltro, string ordenamientoColumna)
        {
            PaginacionModel paginacionModel = new PaginacionModel();
            List<IDictionary<string,object>> listaReporte = null;
            int totalRecords = 0;
            int totalPaginas = 0;
            try
            {
                var connection = factoryConection.GetConnection();

                //Pasar parametros
                DynamicParameters parametros = new DynamicParameters();

                //Filtro- Entrada
                foreach (var param in parametrosFiltro)
                {
                    parametros.Add("@" + param.Key, param.Value);
                }
                //Entrada: Ingresan a la base de datos
                parametros.Add("@NumeroPagina", numeroPagina);
                parametros.Add("@CantidadElementos",cantidadElementos);
                parametros.Add("@Ordenamiento",ordenamientoColumna);

                //SALIDA: Lo que nos regresara la base de datos
                parametros.Add("@TotalRecords",totalRecords, DbType.Int32, ParameterDirection.Output);
                parametros.Add("@TotalPaginas",totalPaginas, DbType.Int32, ParameterDirection.Output);

                var result = await connection.QueryAsync(storeProcedure, parametros, commandType: CommandType.StoredProcedure);
                //Llenar Lista reporte con lo que viene de la base de datos
                //Convertir el Enumerable a Dictionary
                listaReporte = result.Select(x => (IDictionary<string,object>)x).ToList();
                paginacionModel.ListaRecords = listaReporte;
                paginacionModel.NumeroPaginas= parametros.Get<int>("@TotalPaginas");
                paginacionModel.TotalRecords= parametros.Get<int>("@TotalRecords");

            }
            catch (Exception e)
            {
                throw new Exception("No se pudo ejecutar el procedimiento almacenado",e);
            }
            finally
            {
                factoryConection.CloseConnection();
            }

            return paginacionModel;
        }
    }
}