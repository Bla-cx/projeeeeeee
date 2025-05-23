using System.Collections.Generic;
using System.Threading.Tasks;
using _221103018_OmerFarukBayraktutar.DAL.Models;

namespace _221103018_OmerFarukBayraktutar.BLL.Interfaces
{
    public interface ISepetService
    {
        Task<IEnumerable<Sepet>> GetKullaniciSepetAsync(int userId);
        Task<int> GetSepetOgeSayisiAsync(int userId);
        Task<int> GetSepetCountAsync(int userId);
        Task AddToCartAsync(int userId, int etkinlikId, int biletSayisi = 1);
        Task RemoveFromCartAsync(int sepetId);
        Task UpdateBiletSayisiAsync(int sepetId, int biletSayisi);
        Task ClearCartAsync(int userId);
        Task<decimal> GetSepetToplamAsync(int userId);
        // RemoveFromSepetAsync metodu RemoveFromCartAsync ile aynı işlevi gördüğü için kaldırıldı
    }
}
