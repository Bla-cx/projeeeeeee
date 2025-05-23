# Biletini Ayır - Etkinlik Rezervasyon Sistemi

"Biletini Ayır" projesi, etkinlik yönetimi ve rezervasyon işlemlerini kolaylaştıran bir web uygulamasıdır. Bu uygulama kullanıcıların etkinlikleri görüntülemesine, rezervasyon yapmasına ve etkinlik organizatörlerinin etkinliklerini yönetmesine olanak tanır.

## Özellikler

- Üç farklı kullanıcı rolü: Admin, Organizatör ve Müşteri
- Etkinlik oluşturma, düzenleme ve silme
- Kategori ve şehir bazlı filtreleme
- Rezervasyon oluşturma ve iptal etme
- Bilet kontrol sistemi
- Sepet ve ödeme işlemleri
- Kullanıcı profil yönetimi
- E-posta bildirimleri

## Teknolojiler

- ASP.NET Core MVC 8.0
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap 5
- Serilog

## Kurulum ve Çalıştırma

### Ön Koşullar

- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 veya sonrası (önerilen)

### Adımlar

1. Projeyi klonlayın veya indirin:
```
git clone https://github.com/kullanici/biletini-ayir.git
```

2. appsettings.json dosyasında veritabanı bağlantı ayarlarını yapılandırın:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=sunucu_adı;Database=etkinliklerydb;User Id=kullanıcı_adı;Password=şifre;TrustServerCertificate=True;"
}
```

3. .NET CLI veya Visual Studio Package Manager Console kullanarak migration'ları uygulayın:
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Projeyi çalıştırın:
```
dotnet run
```

5. Tarayıcınızda http://localhost:5000 adresine gidin.

## Veritabanı Şeması

Proje aşağıdaki veritabanı tablolarını kullanır:

- AspNetUsers (Identity kullanıcıları)
- AspNetRoles (Identity rolleri)
- Kategoriler
- Sehirler
- Etkinlikler
- Rezervasyonlar
- Sepet
- Bildirimler
- Loglar

## Kullanıcı Rolleri ve İşlevleri

### Admin
- Tüm kullanıcıları yönetir
- Kategorileri ve şehirleri yönetir
- Etkinlikleri onaylar/reddeder

### Organizatör
- Etkinlikler oluşturur ve yönetir
- Rezervasyonları görüntüler
- Biletleri kontrol eder

### Müşteri
- Etkinlikleri görüntüler ve arar
- Rezervasyon yapar
- Biletlerini görüntüler

## Varsayılan Kullanıcılar

Program ilk çalıştırıldığında aşağıdaki test kullanıcıları otomatik olarak oluşturulur:

- Admin: admin@biletiniayir.com / Admin123!
- Organizatör: organizator@biletiniayir.com / Organizer123!
- Müşteri: kullanici@biletiniayir.com / Customer123!

## İletişim

Proje ile ilgili sorularınız için: iletisim@biletiniayir.com
