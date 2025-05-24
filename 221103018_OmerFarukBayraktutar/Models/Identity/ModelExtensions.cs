using _221103018_OmerFarukBayraktutar.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace _221103018_OmerFarukBayraktutar.Models.Identity
{
    /// <summary>
    /// Model sınıfları için null güvenli extension metotlarını içerir
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Etkinlik için güvenli ad alır
        /// </summary>
        public static string GetSafeEtkinlikAdi(this Etkinlik? etkinlik)
        {
            return etkinlik?.EtkinlikAdi ?? string.Empty;
        }

        /// <summary>
        /// Etkinlik için güvenli açıklama alır
        /// </summary>
        public static string GetSafeAciklama(this Etkinlik? etkinlik)
        {
            return etkinlik?.Aciklama ?? string.Empty;
        }

        /// <summary>
        /// Etkinlik için güvenli resim yolu alır
        /// </summary>
        public static string GetSafeResimYolu(this Etkinlik? etkinlik) 
        {
            return etkinlik?.ResimYolu ?? "/img/defaults/default-event.svg";
        }

        /// <summary>
        /// Kullanıcı için güvenli profil resim yolu alır
        /// </summary>
        public static string GetSafeProfileImagePath(this AppUser? user)
        {
            return user?.ProfilResimYolu ?? "/img/defaults/default-user.svg";
        }

        /// <summary>
        /// Kullanıcı için güvenli ad soyad alır
        /// </summary>
        public static string GetSafeFullName(this AppUser? user)
        {
            if (user == null)
                return string.Empty;

            return $"{user.Ad} {user.Soyad}".Trim();
        }

        /// <summary>
        /// Kullanıcı için güvenli email alır
        /// </summary>
        public static string GetSafeEmail(this AppUser? user)
        {
            return user?.Email ?? string.Empty;
        }

        /// <summary>
        /// Liste elemanlarını null-güvenli olarak filtreleyip döndürür
        /// </summary>
        public static IEnumerable<T> SafeWhere<T>(this IEnumerable<T>? source, Func<T, bool> predicate)
        {
            if (source == null)
                return Enumerable.Empty<T>();

            return source.Where(predicate);
        }

        /// <summary>
        /// Liste elemanlarını güvenli şekilde dönüştürür
        /// </summary>
        public static IEnumerable<TResult> SafeSelect<TSource, TResult>(
            this IEnumerable<TSource>? source, 
            Func<TSource, TResult> selector)
        {
            if (source == null)
                return Enumerable.Empty<TResult>();

            return source.Select(selector);
        }

        /// <summary>
        /// Rezervasyon durumunun güvenli görünen adını alır
        /// </summary>
        public static string GetDurumGosterimAdi(this Rezervasyon? rezervasyon)
        {
            if (rezervasyon == null)
                return "Bilinmiyor";
            
            if (rezervasyon.Iptal)
                return "İptal Edildi";
            
            if (rezervasyon.Kullanildi)
                return "Kullanıldı";
            
            return "Aktif";
        }

        /// <summary>
        /// Kategori adını güvenli bir şekilde döndürür
        /// </summary>
        public static string GetSafeKategoriAdi(this Etkinlik? etkinlik)
        {
            return etkinlik?.Kategori?.KategoriAdi ?? "Kategori Belirtilmemiş";
        }

        /// <summary>
        /// Şehir adını güvenli bir şekilde döndürür
        /// </summary>
        public static string GetSafeSehirAdi(this Etkinlik? etkinlik)
        {
            return etkinlik?.Sehir?.SehirAdi ?? "Şehir Belirtilmemiş";
        }
    }
} 