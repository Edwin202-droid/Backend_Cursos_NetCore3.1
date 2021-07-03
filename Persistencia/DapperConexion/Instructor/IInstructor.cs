using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
         //Lo que quiero que me retorne, interface
         Task<IEnumerable<InstructorModel>> ObtenerLista();
         Task<InstructorModel> ObtenerPorId(Guid id);
         Task<int> Nuevo(string nombre, string apellidos, string grado);
         Task<int> Actualizar(Guid instructorId, string nombre, string apellidos, string grado);
         Task<int> Eliminar(Guid id);
    }
}