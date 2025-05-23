using _221103018_OmerFarukBayraktutar.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _221103018_OmerFarukBayraktutar.DAL
{
    // int'i ID tipi olarak kullanmak için IdentityDbContext parametrelerini güncelliyoruz
    public class EtkinliklerDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public EtkinliklerDbContext(DbContextOptions<EtkinliklerDbContext> options) : base(options)
        {
        }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Sehir> Sehirler { get; set; }
        public DbSet<Etkinlik> Etkinlikler { get; set; }
        public DbSet<Rezervasyon> Rezervasyonlar { get; set; }
        public DbSet<Sepet> Sepet { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
        public DbSet<Log> Loglar { get; set; }        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<Kategori>()
                .HasIndex(k => k.KategoriAdi)
                .IsUnique();

            modelBuilder.Entity<Sehir>()
                .HasIndex(s => s.SehirAdi)
                .IsUnique();
                  // Identity tabloları için özel ayarlamalar
            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasMaxLength(128);
                entity.Property(e => e.ProviderKey).HasMaxLength(128);
            });

            modelBuilder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.Property(e => e.LoginProvider).HasMaxLength(128);
                entity.Property(e => e.Name).HasMaxLength(128);
            });// AppUser özelleştirmeleri
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Ad)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("");
                
                entity.Property(e => e.Soyad)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValue("");
                
                entity.Property(e => e.Telefon)
                    .HasMaxLength(20);
                
                entity.Property(e => e.ProfilResimYolu)
                    .HasMaxLength(255);
                
                entity.Property(e => e.KayitTarihi)
                    .HasDefaultValueSql("GETDATE()");
                
                // Identity tablosuna ait özellikler
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Rezervasyon>()
                .HasIndex(r => r.BarkodNo)
                .IsUnique()
                .HasFilter("[BarkodNo] IS NOT NULL");            // Relationships
            modelBuilder.Entity<Etkinlik>()
                .HasOne(e => e.Kategori)
                .WithMany(k => k.Etkinlikler)
                .HasForeignKey(e => e.KategoriId);

            modelBuilder.Entity<Etkinlik>()
                .HasOne(e => e.Sehir)
                .WithMany(s => s.Etkinlikler)
                .HasForeignKey(e => e.SehirId);

            modelBuilder.Entity<Etkinlik>()
                .HasOne(e => e.Organizator)
                .WithMany(u => u.Etkinlikler)
                .HasForeignKey(e => e.OrganizatorId)
                .OnDelete(DeleteBehavior.Restrict);            modelBuilder.Entity<Rezervasyon>()
                .HasOne(r => r.Etkinlik)
                .WithMany(e => e.Rezervasyonlar)
                .HasForeignKey(r => r.EtkinlikId);

            modelBuilder.Entity<Rezervasyon>()
                .HasOne(r => r.Kullanici)
                .WithMany(u => u.Rezervasyonlar)
                .HasForeignKey(r => r.KullaniciId);

            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.Kullanici)
                .WithMany(u => u.SepetOgeleri)
                .HasForeignKey(s => s.KullaniciId);

            modelBuilder.Entity<Sepet>()
                .HasOne(s => s.Etkinlik)
                .WithMany(e => e.SepetOgeleri)
                .HasForeignKey(s => s.EtkinlikId);

            modelBuilder.Entity<Bildirim>()
                .HasOne(b => b.Kullanici)
                .WithMany(u => u.Bildirimler)
                .HasForeignKey(b => b.KullaniciId);

            modelBuilder.Entity<Log>()
                .HasOne(l => l.Kullanici)
                .WithMany(u => u.Loglar)
                .HasForeignKey(l => l.KullaniciId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
