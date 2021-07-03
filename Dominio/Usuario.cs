using Microsoft.AspNetCore.Identity;

namespace Dominio
{
    public class Usuario : IdentityUser
    {
        //Identity ya nos da los ids, password, etc
        public string NombreCompleto { get; set; }

    }
}