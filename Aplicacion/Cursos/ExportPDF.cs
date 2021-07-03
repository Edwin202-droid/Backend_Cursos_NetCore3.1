
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class ExportPDF
    {
        //Encargada de conectarse con la BD
        public class Consulta : IRequest<Stream> {}

        public class Manejador : IRequestHandler<Consulta, Stream>
        {
            private readonly CursosOnlineContext context;

            public Manejador(CursosOnlineContext context)
            {
                this.context = context;
            }
            public async Task<Stream> Handle(Consulta request, CancellationToken cancellationToken)
            {
                //Creando fuente
                Font fuenteTitulo = new Font (Font.HELVETICA, 8f, Font.BOLD, BaseColor.Blue) ;
                Font fuenteHeader = new Font (Font.HELVETICA, 7f, Font.BOLD, BaseColor.Black);
                Font fuenteData = new Font (Font.HELVETICA, 7f, Font.NORMAL, BaseColor.Black);
                var cursos = await context.Curso.ToListAsync();

                //Creacion archivo PDF
                MemoryStream workStream = new MemoryStream();
                Rectangle rect = new Rectangle(PageSize.A4); //Tamaño de la hoja

                Document document = new Document( rect, 0, 0, 50, 100);//Margenes
                    //Escribir en el PDF
                PdfWriter writer = PdfWriter.GetInstance( document, workStream);
                writer.CloseStream = false;

                document.Open();

                document.AddTitle("Lista de Curso ASP.Net Core");
                
                //Tabla
                PdfPTable tabla = new PdfPTable(1); //Columna
                tabla.WidthPercentage = 90; //Ancho
                    //Crear Celda y añadir a la tabla
                PdfPCell celda = new PdfPCell( new Phrase("Lista de Cursos",fuenteTitulo));
                celda.Border = Rectangle.NO_BORDER; //Sin Bordes
                tabla.AddCell(celda);//Agregan celda a tabla
                document.Add(tabla);//Agregando tabla a pdf

                PdfPTable tablaCursos = new PdfPTable(2); //2 columnas
                float[] widths = new float[]{40, 60 };//Ancho
                tablaCursos.SetWidthPercentage(widths, rect);

                PdfPCell celdaHeaderTitulo = new PdfPCell( new Phrase("Curso", fuenteHeader));
                tablaCursos.AddCell(celdaHeaderTitulo);
                PdfPCell celdaHeaderDescripcion = new PdfPCell(new Phrase("Descripcion", fuenteHeader));
                tablaCursos.AddCell(celdaHeaderDescripcion);
                tablaCursos.WidthPercentage = 90;

                //Agregar cursos de la BD al PDF
                foreach (var cursoElemento in cursos)
                {
                    PdfPCell celdaDataTitulo = new PdfPCell( new Phrase(cursoElemento.Titulo, fuenteData));
                    tablaCursos.AddCell(celdaDataTitulo);

                    PdfPCell celdaDataDescripcion = new PdfPCell( new Phrase(cursoElemento.Descripcion, fuenteData));
                    tablaCursos.AddCell(celdaDataDescripcion);
                }
                document.Add(tablaCursos);

                document.Close();

                //pdf -> stream
                byte[] byteData = workStream.ToArray();
                workStream.Write(byteData, 0, byteData.Length);
                workStream.Position = 0;

                return workStream;
            }
        }
    }
}