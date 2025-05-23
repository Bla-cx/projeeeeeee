using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace _221103018_OmerFarukBayraktutar.BLL.Services
{
    public class RezervasyonService : IRezervasyonService
    {
        private readonly EtkinliklerDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<RezervasyonService> _logger;

        public RezervasyonService(EtkinliklerDbContext context, IEmailService emailService, ILogger<RezervasyonService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<Rezervasyon>> GetKullaniciRezervasyonlarAsync(int userId)
        {
            return await _context.Rezervasyonlar
                .Where(r => r.KullaniciId == userId)
                .Include(r => r.Etkinlik)
                .ThenInclude(e => e.Kategori)
                .Include(r => r.Etkinlik)
                .ThenInclude(e => e.Sehir)
                .OrderByDescending(r => r.RezervasyonTarihi)
                .ToListAsync();
        }

        public async Task<Rezervasyon> GetRezervasyonDetayAsync(int rezervasyonId)
        {
            return await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .ThenInclude(e => e.Kategori)
                .Include(r => r.Etkinlik)
                .ThenInclude(e => e.Sehir)
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.RezervasyonId == rezervasyonId);
        }

        public async Task<string> CreateRezervasyonAsync(int etkinlikId, int userId, int biletSayisi)
        {
            // DbContext'in kendi execution strategy'sini kullanıyoruz
            var strategy = _context.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var etkinlik = await _context.Etkinlikler.FindAsync(etkinlikId);
                    if (etkinlik == null)
                        throw new Exception("Etkinlik bulunamadı.");

                    if (etkinlik.KalanKapasite < biletSayisi)
                        throw new Exception("Yetersiz kapasite.");

                    // Barkod numarası oluştur
                    var barkodNo = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

                    // Rezervasyon oluştur
                    var rezervasyon = new Rezervasyon
                    {
                        EtkinlikId = etkinlikId,
                        KullaniciId = userId,
                        BiletSayisi = biletSayisi,
                        RezervasyonTarihi = DateTime.Now,
                        ToplamTutar = etkinlik.BiletFiyati * biletSayisi,
                        OdemeDurumu = true,
                        BarkodNo = barkodNo
                    };

                    _context.Rezervasyonlar.Add(rezervasyon);

                    // Etkinlik kapasitesini güncelle
                    etkinlik.KalanKapasite -= biletSayisi;
                    _context.Etkinlikler.Update(etkinlik);

                    // Sepetten kaldır
                    var sepetOge = await _context.Sepet
                        .FirstOrDefaultAsync(s => s.EtkinlikId == etkinlikId && s.KullaniciId == userId);
                    if (sepetOge != null)
                    {
                        _context.Sepet.Remove(sepetOge);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // E-posta bildirimi gönder
                    var kullanici = await _context.Users.FindAsync(userId);
                    if (kullanici != null)
                    {
                        var subject = "Rezervasyon Onayı - Biletini Ayır";
                        var message = $@"
                            <html>
                            <head>
                                <style>
                                    body {{
                                        font-family: Arial, sans-serif;
                                        color: #333;
                                    }}
                                    .container {{
                                        width: 600px;
                                        margin: 0 auto;
                                        padding: 20px;
                                        border: 1px solid #ddd;
                                    }}
                                    .header {{
                                        background-color: #0056b3;
                                        color: white;
                                        padding: 10px;
                                        text-align: center;
                                    }}
                                    .ticket {{
                                        border: 2px dashed #ccc;
                                        padding: 15px;
                                        margin: 20px 0;
                                        background-color: #f9f9f9;
                                    }}
                                    .barcode {{
                                        font-family: monospace;
                                        font-size: 18px;
                                        letter-spacing: 5px;
                                        text-align: center;
                                        padding: 10px;
                                        background-color: #eee;
                                        margin: 10px 0;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h2>Biletini Ayır - Rezervasyon Onayı</h2>
                                    </div>
                                    <h3>Merhaba {kullanici.Ad} {kullanici.Soyad},</h3>
                                    <p>Rezervasyonunuz başarıyla oluşturulmuştur. Rezervasyon detayları aşağıdadır:</p>
                                    <div class='ticket'>
                                        <h3>{etkinlik.EtkinlikAdi}</h3>
                                        <p><strong>Tarih:</strong> {etkinlik.Tarih.ToShortDateString()}</p>
                                        <p><strong>Saat:</strong> {etkinlik.BaslangicSaati?.ToString(@"hh\:mm") ?? "Belirtilmemiş"}</p>
                                        <p><strong>Bilet Sayısı:</strong> {biletSayisi}</p>
                                        <p><strong>Toplam Tutar:</strong> {rezervasyon.ToplamTutar} TL</p>
                                        <div class='barcode'>
                                            {barkodNo}
                                        </div>
                                        <p>Bu barkod numarasını etkinlik girişinde göstermeniz gerekmektedir.</p>
                                    </div>
                                    <p>İyi eğlenceler dileriz!</p>
                                    <p>Saygılarımızla,<br/>Biletini Ayır Ekibi</p>
                                </div>
                            </body>
                            </html>";

                        await _emailService.SendEmailAsync(kullanici.Email, subject, message);
                    }

                    return barkodNo;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Rezervasyon oluşturulurken hata: {Message}", ex.Message);
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<bool> IptalEtAsync(int rezervasyonId)
        {
            // DbContext'in kendi execution strategy'sini kullanıyoruz
            var strategy = _context.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var rezervasyon = await _context.Rezervasyonlar
                        .Include(r => r.Etkinlik)
                        .Include(r => r.Kullanici)
                        .FirstOrDefaultAsync(r => r.RezervasyonId == rezervasyonId);

                    if (rezervasyon == null)
                        return false;

                    if (rezervasyon.Iptal) // Zaten iptal edilmiş
                        return false;

                    // Etkinliğe 24 saatten az kaldıysa iptal edemez
                    if (rezervasyon.Etkinlik.Tarih <= DateTime.Now.AddDays(1))
                        return false;

                    // Rezervasyonu iptal et
                    rezervasyon.Iptal = true;
                    _context.Rezervasyonlar.Update(rezervasyon);

                    // Etkinliğin kapasitesini geri ekle
                    var etkinlik = rezervasyon.Etkinlik;
                    etkinlik.KalanKapasite += rezervasyon.BiletSayisi;
                    _context.Etkinlikler.Update(etkinlik);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Kullanıcıya iptal bilgisi gönder
                    var kullanici = rezervasyon.Kullanici;
                    if (kullanici != null && !string.IsNullOrEmpty(kullanici.Email))
                    {
                        var subject = "Biletini Ayır - Rezervasyon İptali";
                        var message = $@"<!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <style>
                                body {{ font-family: Arial, sans-serif; color: #333; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                .header {{ background-color: #f8f9fa; padding: 10px; text-align: center; }}
                                h2, h3 {{ color: #007bff; }}
                                p {{ line-height: 1.5; }}
                                strong {{ color: #dc3545; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h2>Biletini Ayır - Rezervasyon İptali</h2>
                                </div>
                                <h3>Merhaba {kullanici.Ad} {kullanici.Soyad},</h3>
                                <p>Aşağıdaki etkinlik için rezervasyonunuz iptal edilmiştir:</p>
                                <p><strong>{rezervasyon.Etkinlik.EtkinlikAdi}</strong> - {rezervasyon.Etkinlik.Tarih.ToShortDateString()}</p>
                                <p>İptal edilen bilet sayısı: {rezervasyon.BiletSayisi}</p>
                                <p>İade edilen tutar: {rezervasyon.ToplamTutar} TL</p>
                                <p>Saygılarımızla,<br/>Biletini Ayır Ekibi</p>
                            </div>
                        </body>
                        </html>";

                        await _emailService.SendEmailAsync(kullanici.Email, subject, message);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Rezervasyon iptal edilirken hata: {Message}", ex.Message);
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<bool> BiletKontrolAsync(string barkodNo)
        {
            var rezervasyon = await _context.Rezervasyonlar
                .Include(r => r.Etkinlik)
                .FirstOrDefaultAsync(r => r.BarkodNo == barkodNo);

            return rezervasyon != null && rezervasyon.OdemeDurumu && rezervasyon.Etkinlik.Tarih.Date >= DateTime.Now.Date;
        }
    }
}
