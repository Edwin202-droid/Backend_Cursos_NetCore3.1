using System;
using System.Collections.Generic;

namespace Dominio
{
    public class Curso
    {
        public Guid CursoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public byte[] FotoPortada { get; set; }
        public DateTime? FechaCreacion { get; set; }

        //Precio: Relacion Uno a Uno
        public Precio PrecioPromocion { get; set; }

        //Comentario: Relacion Uno A mUCHOS
        public ICollection<Comentario> ComentarioLista { get; set; }

        //Instructor: Relacion muchos a muchos
        public ICollection<CursoInstructor> InstructoresLink { get; set; }
    }
}