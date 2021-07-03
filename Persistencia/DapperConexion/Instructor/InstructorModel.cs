using System;
namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorModel
    {   
        //Tiene que tener los mismos nombres que en la tabla de bas de datos
        //O puede poner un alias en la BD con "AS"

        //Mapeo de info que vendra de la BD
        public Guid InstructorId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Grado { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}