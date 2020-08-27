using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public interface IBlobHelper
    {

        //IFormFile es el archivo en memoria, el cual declaramos en el controlador CategoryViewModel
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);

        Task<Guid> UploadBlobAsync(byte[] file, string containerName);  

        Task<Guid> UploadBlobAsync(string image, string containerName);

    }
}
