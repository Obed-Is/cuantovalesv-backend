using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Services
{
    public class ValidacionesRequests
    {
        public void ValidacionProductoBuscar(string productoBuscar)
        {
            if (string.IsNullOrWhiteSpace(productoBuscar))
                throw new ArgumentException("Ingrese el producto a buscar");

            if (productoBuscar.Length < 3)
                throw new ArgumentException("El producto debe contener un minimo de 3 caracteres");

            if (productoBuscar.Length > 15)
                throw new ArgumentException("El producto debe contener un maximo de 15 caracteres");

            return;
        }
    }
}
