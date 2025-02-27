using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFileConversionService
    {
        Task<List<PurchaseOrderHeader>> ConvertExcelToJsonAsync(IFormFile file);
    }
}
