using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveEventImageAsync(IFormFile file)
        {
            return await SaveImageAsync(file, "etkinlikler");
        }

        public async Task<string> SaveUserImageAsync(IFormFile file)
        {
            return await SaveImageAsync(file, "kullanicilar");
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Klasör yolunu oluştur
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "img", folder);
            
            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Dosya adını belirle (benzersiz olması için GUID ekle)
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Veritabanında kullanmak için web yolunu dön
            return $"/img/{folder}/{uniqueFileName}";
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            // Dosya yolunu wwwroot'a göre ayarla
            filePath = filePath.TrimStart('/');
            string fullPath = Path.Combine(_environment.WebRootPath, filePath);

            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
            }
        }
    }
}
