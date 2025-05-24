using _221103018_OmerFarukBayraktutar.DAL.Models;

namespace _221103018_OmerFarukBayraktutar.Utils
{
    /// <summary>
    /// Bu sınıf, projede bulunan null referans uyarılarını çözmek için rehberdir.
    /// Karşılaşılan yaygın uyarı türleri ve çözüm örnekleri içerir.
    /// </summary>
    public static class NullReferenceHelper
    {
        /*
         * UYARI: CS8618 - Null atanamaz özellik için çözüm örnekleri:
         * 
         * 1. Özellikleri gerekli (required) olarak işaretleyin (C# 11+):
         *    public required string MyProperty { get; set; }
         * 
         * 2. Özelliklere varsayılan değer atayın:
         *    public string MyProperty { get; set; } = string.Empty;
         *    public List<string> MyCollection { get; set; } = new();
         * 
         * 3. Özellikleri null atanabilir yapın:
         *    public string? MyProperty { get; set; }
         * 
         * 4. Oluşturucuda değer atayın:
         *    public MyClass() 
         *    {
         *        MyProperty = string.Empty;
         *        MyCollection = new List<string>();
         *    }
         */

        /*
         * UYARI: CS8601/CS8602/CS8604 - Null referans ataması/kullanımı için çözüm örnekleri:
         * 
         * 1. Null kontrolleri ekleyin:
         *    if (myObject != null) 
         *    {
         *        // myObject kullan
         *    }
         * 
         * 2. Null birleştirme operatörü kullanın:
         *    var result = myObject ?? defaultValue;
         * 
         * 3. Null şart operatörünü kullanın:
         *    var length = myString?.Length ?? 0;
         * 
         * 4. NotNull özniteliğini kullanın:
         *    public void MyMethod([NotNull] string parameter)
         * 
         * 5. Zorunlu null ataması (riski siz alıyorsanız):
         *    string notNullValue = possiblyNullValue!;
         */

        /*
         * UYARI: CS8619 - Generic koleksiyonlar için çözüm örnekleri:
         * 
         * 1. Doğru generic tür kullanın:
         *    List<string?> nullableStrings;  // null içerebilen string listesi
         *    List<string> nonNullStrings;    // null içeremeyen string listesi
         * 
         * 2. Null değerleri filtreleyin:
         *    var nonNullList = nullableList.Where(item => item != null).ToList();
         * 
         * 3. Cast kullanın (dikkatli olunmalı):
         *    List<string> safeList = unsafeList.Cast<string>().ToList();
         */

        // Örnek çözüm metodu 1: Null kontrolü ile güvenli dönüş
        public static string GetSafeString(string? input)
        {
            return input ?? string.Empty;
        }

        // Örnek çözüm metodu 2: Null öğeleri filtreleyen liste dönüşümü
        public static List<T> FilterNulls<T>(List<T?> list) where T : class
        {
            return list.Where(item => item != null).Cast<T>().ToList();
        }

        // Örnek çözüm metodu 3: Etkinlik veya benzer model null kontrolü
        public static string GetEtkinlikName(Etkinlik? etkinlik)
        {
            return etkinlik?.EtkinlikAdi ?? "Etkinlik Bulunamadı";
        }
    }
} 