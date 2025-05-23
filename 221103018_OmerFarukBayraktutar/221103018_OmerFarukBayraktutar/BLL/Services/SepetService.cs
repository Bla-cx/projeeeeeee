using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.BLL.Services
{
    public class SepetService : ISepetService
    {
        private readonly EtkinliklerDbContext _context;

        public SepetService(EtkinliklerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sepet>> GetKullaniciSepetAsync(int userId)
        {
            return await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .Include(s => s.Etkinlik)
                .ThenInclude(e => e.Kategori)
                .Include(s => s.Etkinlik)
                .ThenInclude(e => e.Sehir)
                .OrderByDescending(s => s.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<int> GetSepetOgeSayisiAsync(int userId)
        {
            return await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .CountAsync();
        }

        public async Task AddToCartAsync(int userId, int etkinlikId, int biletSayisi = 1)
        {
            // Etkinliği kontrol et
            var etkinlik = await _context.Etkinlikler.FindAsync(etkinlikId);
            if (etkinlik == null)
                throw new Exception("Etkinlik bulunamadı.");

            if (etkinlik.KalanKapasite < biletSayisi)
                throw new Exception("Yetersiz kapasite.");

            // Kullanıcının bu etkinlik için sepette ögesi var mı kontrol et
            var existingItem = await _context.Sepet
                .FirstOrDefaultAsync(s => s.KullaniciId == userId && s.EtkinlikId == etkinlikId);

            if (existingItem != null)
            {
                // Varsa bilet sayısını güncelle
                existingItem.BiletSayisi += biletSayisi;
                _context.Sepet.Update(existingItem);
            }
            else
            {
                // Yoksa yeni sepet ögesi oluştur
                var sepetItem = new Sepet
                {
                    KullaniciId = userId,
                    EtkinlikId = etkinlikId,
                    BiletSayisi = biletSayisi,
                    EklenmeTarihi = DateTime.Now
                };

                _context.Sepet.Add(sepetItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int sepetId)
        {
            var sepetItem = await _context.Sepet.FindAsync(sepetId);
            if (sepetItem != null)
            {
                _context.Sepet.Remove(sepetItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateBiletSayisiAsync(int sepetId, int biletSayisi)
        {
            var sepetItem = await _context.Sepet
                .Include(s => s.Etkinlik)
                .FirstOrDefaultAsync(s => s.SepetId == sepetId);

            if (sepetItem == null)
                throw new Exception("Sepet ögesi bulunamadı.");

            if (sepetItem.Etkinlik.KalanKapasite < biletSayisi)
                throw new Exception("Yetersiz kapasite.");

            sepetItem.BiletSayisi = biletSayisi;
            _context.Sepet.Update(sepetItem);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var sepetItems = await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .ToListAsync();

            _context.Sepet.RemoveRange(sepetItems);
            await _context.SaveChangesAsync();
        }
        
        public async Task<decimal> GetSepetToplamAsync(int userId)
        {
            return await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .Include(s => s.Etkinlik)
                .SumAsync(s => s.BiletSayisi * s.Etkinlik.BiletFiyati);
        }

        public async Task<int> GetSepetCountAsync(int userId)
        {
            // Toplam bilet sayısını döndürür (sepet öğesi sayısı değil)
            return await _context.Sepet
                .Where(s => s.KullaniciId == userId)
                .SumAsync(s => s.BiletSayisi);
        }
        
        // RemoveFromSepetAsync metodu RemoveFromCartAsync ile aynı işlevi gördüğü için kaldırıldı
        // ve ilgili çağrılar RemoveFromCartAsync metodu ile değiştirilmelidir
    }
}
