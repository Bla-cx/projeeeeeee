using _221103018_OmerFarukBayraktutar.DAL.Models;

namespace _221103018_OmerFarukBayraktutar.BLL.Interfaces
{    public interface IEtkinlikService
    {        Task<List<Etkinlik>> GetAllAsync();
        Task<List<Etkinlik>> GetActiveAsync();
        Task<List<Etkinlik>> GetByOrganizatorIdAsync(int organizatorId);
        Task<Etkinlik> GetByIdAsync(int id);
        Task<List<Etkinlik>> SearchAsync(string query, int? kategoriId, int? sehirId, DateTime? baslangicTarihi, DateTime? bitisTarihi);
        Task<Etkinlik> CreateAsync(Etkinlik etkinlik);
        Task UpdateAsync(Etkinlik etkinlik);
        Task DeleteAsync(int id);
        Task<bool> DecrementCapacityAsync(int etkinlikId, int amount);
        Task<bool> IncrementCapacityAsync(int etkinlikId, int amount);
    }
}
