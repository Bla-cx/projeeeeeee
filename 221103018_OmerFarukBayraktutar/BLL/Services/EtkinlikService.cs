using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace _221103018_OmerFarukBayraktutar.BLL.Services
{
    public class EtkinlikService : IEtkinlikService
    {
        private readonly EtkinliklerDbContext _context;
        private readonly ILogger<EtkinlikService> _logger;

        public EtkinlikService(EtkinliklerDbContext context, ILogger<EtkinlikService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Etkinlik>> GetAllAsync()
        {
            return await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Include(e => e.Organizator)
                .OrderByDescending(e => e.OlusturulmaTarihi)
                .ToListAsync();
        }

        public async Task<List<Etkinlik>> GetActiveAsync()
        {
            return await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Include(e => e.Organizator)
                .Where(e => e.AktifMi && e.Tarih > DateTime.Now)                .OrderBy(e => e.Tarih)
                .ToListAsync();
        }        public async Task<List<Etkinlik>> GetByOrganizatorIdAsync(int organizatorId)
        {
            return await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Where(e => e.OrganizatorId == organizatorId)
                .OrderByDescending(e => e.OlusturulmaTarihi).ToListAsync();
        }

        public async Task<Etkinlik> GetByIdAsync(int id)
        {
            return await _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Include(e => e.Organizator)
                .FirstOrDefaultAsync(e => e.EtkinlikId == id);
        }

        public async Task<List<Etkinlik>> SearchAsync(string query, int? kategoriId, int? sehirId, DateTime? baslangicTarihi, DateTime? bitisTarihi)
        {
            var etkinlikler = _context.Etkinlikler
                .Include(e => e.Kategori)
                .Include(e => e.Sehir)
                .Where(e => e.AktifMi && e.Tarih > DateTime.Now);

            if (!string.IsNullOrWhiteSpace(query))
            {
                etkinlikler = etkinlikler.Where(e => e.EtkinlikAdi.Contains(query) || e.Aciklama.Contains(query));
            }

            if (kategoriId.HasValue)
            {
                etkinlikler = etkinlikler.Where(e => e.KategoriId == kategoriId);
            }

            if (sehirId.HasValue)
            {
                etkinlikler = etkinlikler.Where(e => e.SehirId == sehirId);
            }

            if (baslangicTarihi.HasValue)
            {
                etkinlikler = etkinlikler.Where(e => e.Tarih >= baslangicTarihi);
            }

            if (bitisTarihi.HasValue)
            {
                etkinlikler = etkinlikler.Where(e => e.Tarih <= bitisTarihi);
            }

            return await etkinlikler.OrderBy(e => e.Tarih).ToListAsync();
        }

        public async Task<Etkinlik> CreateAsync(Etkinlik etkinlik)
        {
            _context.Etkinlikler.Add(etkinlik);
            await _context.SaveChangesAsync();
            return etkinlik;
        }

        public async Task UpdateAsync(Etkinlik etkinlik)
        {
            _context.Etkinlikler.Update(etkinlik);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(id);
            if (etkinlik != null)
            {
                etkinlik.AktifMi = false;
                _context.Etkinlikler.Update(etkinlik);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DecrementCapacityAsync(int etkinlikId, int amount)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(etkinlikId);
            if (etkinlik == null || etkinlik.KalanKapasite < amount)
            {
                return false;
            }

            etkinlik.KalanKapasite -= amount;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementCapacityAsync(int etkinlikId, int amount)
        {
            var etkinlik = await _context.Etkinlikler.FindAsync(etkinlikId);
            if (etkinlik == null)
            {
                return false;
            }

            etkinlik.KalanKapasite += amount;
            if (etkinlik.KalanKapasite > etkinlik.ToplamKapasite)
            {
                etkinlik.KalanKapasite = etkinlik.ToplamKapasite;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
