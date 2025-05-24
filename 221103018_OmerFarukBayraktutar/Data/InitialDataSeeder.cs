using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.Data
{
    public static class InitialDataSeeder
    {
        public static async Task SeedData(
            IServiceProvider serviceProvider,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            using var context = new EtkinliklerDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<EtkinliklerDbContext>>());

            // Roller
            await SeedRoles(roleManager);

            // Kullanıcılar
            await SeedUsers(userManager);

            // Kategoriler
            await SeedKategoriler(context);

            // Şehirler
            await SeedSehirler(context);

            // Etkinlikler
            await SeedEtkinlikler(context, userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roleNames = { "Admin", "Organizer", "Customer" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }

        private static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            // Admin kullanıcısı
            var adminUser = new AppUser
            {
                UserName = "admin@biletiniayir.com",
                Email = "admin@biletiniayir.com",
                Ad = "Admin",
                Soyad = "User",
                EmailConfirmed = true,
                KayitTarihi = DateTime.Now
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Organizatör kullanıcısı
            var organizerUser = new AppUser
            {
                UserName = "organizator@biletiniayir.com",
                Email = "organizator@biletiniayir.com",
                Ad = "Organizatör",
                Soyad = "User",
                EmailConfirmed = true,
                KayitTarihi = DateTime.Now
            };

            if (await userManager.FindByEmailAsync(organizerUser.Email) == null)
            {
                var result = await userManager.CreateAsync(organizerUser, "Organizer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(organizerUser, "Organizer");
                }
            }

            // Normal kullanıcı
            var customerUser = new AppUser
            {
                UserName = "kullanici@biletiniayir.com",
                Email = "kullanici@biletiniayir.com",
                Ad = "Müşteri",
                Soyad = "User",
                EmailConfirmed = true,
                KayitTarihi = DateTime.Now
            };

            if (await userManager.FindByEmailAsync(customerUser.Email) == null)
            {
                var result = await userManager.CreateAsync(customerUser, "Customer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                }
            }
        }

        private static async Task SeedKategoriler(EtkinliklerDbContext context)
        {
            if (await context.Kategoriler.AnyAsync())
            {
                return;
            }

            var kategoriler = new List<Kategori>
            {
                new Kategori { KategoriAdi = "Konser" },
                new Kategori { KategoriAdi = "Tiyatro" },
                new Kategori { KategoriAdi = "Sinema" },
                new Kategori { KategoriAdi = "Festival" },
                new Kategori { KategoriAdi = "Sergi" },
                new Kategori { KategoriAdi = "Spor" },
                new Kategori { KategoriAdi = "Workshop" },
                new Kategori { KategoriAdi = "Seminer" }
            };

            await context.Kategoriler.AddRangeAsync(kategoriler);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSehirler(EtkinliklerDbContext context)
        {
            if (await context.Sehirler.AnyAsync())
            {
                return;
            }

            var sehirler = new List<Sehir>
            {
                new Sehir { SehirAdi = "İstanbul" },
                new Sehir { SehirAdi = "Ankara" },
                new Sehir { SehirAdi = "İzmir" },
                new Sehir { SehirAdi = "Antalya" },
                new Sehir { SehirAdi = "Bursa" },
                new Sehir { SehirAdi = "Adana" },
                new Sehir { SehirAdi = "Konya" },
                new Sehir { SehirAdi = "Trabzon" }
            };

            await context.Sehirler.AddRangeAsync(sehirler);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEtkinlikler(EtkinliklerDbContext context, UserManager<AppUser> userManager)
        {
            if (await context.Etkinlikler.AnyAsync())
            {
                return;
            }            var organizator = await userManager.FindByEmailAsync("organizator@biletiniayir.com");
            if (organizator == null)
            {
                return;
            }
            
            var organizatorId = organizator.Id;

            var kategoriler = await context.Kategoriler.ToListAsync();
            var sehirler = await context.Sehirler.ToListAsync();

            var etkinlikler = new List<Etkinlik>
            {
                new Etkinlik
                {
                    EtkinlikAdi = "Rock Konseri",
                    Aciklama = "Harika bir rock konseri deneyimi yaşayın.",
                    Tarih = DateTime.Now.AddDays(10),
                    BaslangicSaati = new TimeSpan(19, 0, 0),
                    BitisSaati = new TimeSpan(23, 0, 0),
                    KategoriId = kategoriler.First(k => k.KategoriAdi == "Konser").KategoriId,
                    SehirId = sehirler.First(s => s.SehirAdi == "İstanbul").SehirId,
                    Adres = "Harbiye Açık Hava Tiyatrosu, İstanbul",
                    ResimYolu = "/img/concert1.jpg",
                    OrganizatorId = organizatorId,
                    BiletFiyati = 150,
                    ToplamKapasite = 1000,
                    KalanKapasite = 1000,
                    OlusturulmaTarihi = DateTime.Now,
                    AktifMi = true
                },
                new Etkinlik
                {
                    EtkinlikAdi = "Klasik Müzik Gecesi",
                    Aciklama = "Ünlü klasik müzik eserlerinin yer aldığı özel bir gece.",
                    Tarih = DateTime.Now.AddDays(15),
                    BaslangicSaati = new TimeSpan(20, 0, 0),
                    BitisSaati = new TimeSpan(22, 30, 0),
                    KategoriId = kategoriler.First(k => k.KategoriAdi == "Konser").KategoriId,
                    SehirId = sehirler.First(s => s.SehirAdi == "Ankara").SehirId,
                    Adres = "CSO Konser Salonu, Ankara",
                    ResimYolu = "/img/concert2.jpg",
                    OrganizatorId = organizatorId,
                    BiletFiyati = 200,
                    ToplamKapasite = 500,
                    KalanKapasite = 500,
                    OlusturulmaTarihi = DateTime.Now,
                    AktifMi = true
                },
                new Etkinlik
                {
                    EtkinlikAdi = "Hamlet",
                    Aciklama = "Shakespeare'in ünlü oyunu.",
                    Tarih = DateTime.Now.AddDays(7),
                    BaslangicSaati = new TimeSpan(19, 30, 0),
                    BitisSaati = new TimeSpan(22, 0, 0),
                    KategoriId = kategoriler.First(k => k.KategoriAdi == "Tiyatro").KategoriId,
                    SehirId = sehirler.First(s => s.SehirAdi == "İstanbul").SehirId,
                    Adres = "Şehir Tiyatroları, İstanbul",
                    ResimYolu = "/img/placeholder/default-event.svg",
                    OrganizatorId = organizatorId,
                    BiletFiyati = 100,
                    ToplamKapasite = 300,
                    KalanKapasite = 300,
                    OlusturulmaTarihi = DateTime.Now,
                    AktifMi = true
                },
                new Etkinlik
                {
                    EtkinlikAdi = "Film Festivali",
                    Aciklama = "Ödüllü filmlerin gösterildiği uluslararası festival.",
                    Tarih = DateTime.Now.AddDays(20),
                    BaslangicSaati = new TimeSpan(10, 0, 0),
                    BitisSaati = new TimeSpan(23, 0, 0),
                    KategoriId = kategoriler.First(k => k.KategoriAdi == "Festival").KategoriId,
                    SehirId = sehirler.First(s => s.SehirAdi == "Antalya").SehirId,
                    Adres = "Antalya Kültür Merkezi",
                    ResimYolu = "/img/festival1.jpg",
                    OrganizatorId = organizatorId,
                    BiletFiyati = 300,
                    ToplamKapasite = 1500,
                    KalanKapasite = 1500,
                    OlusturulmaTarihi = DateTime.Now,
                    AktifMi = true
                },
                new Etkinlik
                {
                    EtkinlikAdi = "Sanat Sergisi",
                    Aciklama = "Modern sanat eserlerinden oluşan sergi.",
                    Tarih = DateTime.Now.AddDays(5),
                    BaslangicSaati = new TimeSpan(10, 0, 0),
                    BitisSaati = new TimeSpan(18, 0, 0),
                    KategoriId = kategoriler.First(k => k.KategoriAdi == "Sergi").KategoriId,
                    SehirId = sehirler.First(s => s.SehirAdi == "İzmir").SehirId,
                    Adres = "İzmir Sanat Galerisi",
                    ResimYolu = "/img/exhibition1.jpg",
                    OrganizatorId = organizatorId,
                    BiletFiyati = 50,
                    ToplamKapasite = 200,
                    KalanKapasite = 200,
                    OlusturulmaTarihi = DateTime.Now,
                    AktifMi = true
                }
            };

            await context.Etkinlikler.AddRangeAsync(etkinlikler);
            await context.SaveChangesAsync();
        }
    }
}
