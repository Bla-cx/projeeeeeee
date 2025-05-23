using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.BLL.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveEventImageAsync(IFormFile file);
        Task<string> SaveUserImageAsync(IFormFile file);
        Task<string> SaveImageAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string filePath);
    }
}
