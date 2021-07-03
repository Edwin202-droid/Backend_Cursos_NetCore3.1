using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Instructor
{
    //Implementar interface
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConection factoryConection;

        public InstructorRepositorio(IFactoryConection factoryConection)
        {
            this.factoryConection = factoryConection;
        }

        public async Task<int> Actualizar(Guid instructorId, string nombre, string apellidos, string grado)
        {
            var storeProcedure = "usp_Instructor_Editar";
            try
            {
                var connection = factoryConection.GetConnection();
                var resultado = await connection.ExecuteAsync(
                    storeProcedure,
                    new {
                        InstructorId = instructorId,
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Grado = grado
                    },
                    commandType: CommandType.StoredProcedure
                );

                factoryConection.CloseConnection();

                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo editar la data del instructor", e);
            }
        }

        public async Task<int> Eliminar(Guid id)
        {
            var storeProcedure = "usp_Instructor_Eliminar";
            try
            {
                var connection = factoryConection.GetConnection();
                var resultado = await connection.ExecuteAsync(
                    storeProcedure,
                    new {
                        InstructorId = id
                    },
                    commandType: CommandType.StoredProcedure
                );

                factoryConection.CloseConnection();
                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo eliminar el instructor",e);
            }
        }

        public async Task<int> Nuevo(string nombre, string apellidos, string grado)
        {
            var storeProcedure = "usp_Instructor_Nuevo";
            try
            {
                var connection = factoryConection.GetConnection();
                var resultado = await connection.ExecuteAsync(storeProcedure, new {
                    InstructorId = Guid.NewGuid(),
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Grado = grado
                    }, commandType: CommandType.StoredProcedure); 

                factoryConection.CloseConnection();

                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se puedo guardar el nuevo instructor",e);
            }
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorList = null;

            //Nombre del procedimiento almacenado
            var storeProcedure = "usp_Obtener_Instructores";

            try
            {
                //Logica
                var connection = factoryConection.GetConnection();
                instructorList = await connection.QueryAsync<InstructorModel>(storeProcedure,null,commandType: CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                //Error
                throw new Exception("Error en la consulta de datos", e);
            }
            finally
            {
                //Siempre se ejecuta
                //Cerrar la conexion
                factoryConection.CloseConnection();
            }
            return instructorList;
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            var storeProcedure = "usp_Instructor_Por_ID";
            InstructorModel instructor = null;
            try
            {
                var connection = factoryConection.GetConnection();
                instructor = await connection.QueryFirstAsync<InstructorModel>(
                    storeProcedure,
                    new {
                        Id = id
                    },
                    commandType: CommandType.StoredProcedure
                );
                factoryConection.CloseConnection();
                return instructor;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo encontrar el instructor",e);
            }
        }
    }
}