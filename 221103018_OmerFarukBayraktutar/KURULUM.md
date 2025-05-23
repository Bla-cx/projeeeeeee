# Biletini Ayır Projesi Kurulum ve Çalıştırma

## Migration ve Veritabanı Oluşturma

Aşağıdaki komutları sırasıyla Package Manager Console'da çalıştırın:

```
Add-Migration InitialCreate
Update-Database
```

veya .NET CLI kullanarak:

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Proje Yapısı

Proje, katmanlı mimariye sahiptir:

1. DAL (Data Access Layer): Veritabanı modelleri ve context
2. BLL (Business Logic Layer): İş mantığı servisleri
3. Presentation Layer: Controller'lar ve View'lar

## Kullanıcı Rolleri

- Admin: Tüm sistemi yönetir
- Organizer: Etkinlikleri oluşturur ve yönetir
- Customer: Etkinliklere katılır ve bilet alır

## Varsayılan Kullanıcılar

Program ilk çalıştırıldığında aşağıdaki test kullanıcıları otomatik olarak oluşturulur:

- Admin: admin@biletiniayir.com / Admin123!
- Organizatör: organizator@biletiniayir.com / Organizer123!
- Müşteri: kullanici@biletiniayir.com / Customer123!

## Hata Ayıklama

Hata mesajları logs klasörü altında Serilog tarafından kaydedilir. Uygulama çalışırken bir hata oluşursa, logları kontrol edin.
