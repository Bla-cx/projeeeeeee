using _221103018_OmerFarukBayraktutar.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.BLL.Interfaces
{
    public interface IRezervasyonService
    {
        Task<IEnumerable<Rezervasyon>> GetKullaniciRezervasyonlarAsync(int userId);
        Task<Rezervasyon> GetRezervasyonDetayAsync(int rezervasyonId);
        Task<string> CreateRezervasyonAsync(int etkinlikId, int userId, int biletSayisi);
        Task<bool> IptalEtAsync(int rezervasyonId);
        Task<bool> BiletKontrolAsync(string barkodNo);
    }
}
