using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.BLL.Services;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.Middlewares;
using _221103018_OmerFarukBayraktutar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;
using System.Globalization;
using Microsoft.AspNetCore.StaticFiles;

namespace _221103018_OmerFarukBayraktutar
{
    // Türkçe hata mesajları için özel IdentityErrorDescriber sınıfı
    public class TurkceIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"Bilinmeyen bir hata oluştu." }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "İyimser eşzamanlılık hatası, nesne değiştirildi." }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(PasswordMismatch), Description = "Hatalı şifre." }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Geçersiz token." }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Bu giriş bilgisine sahip bir kullanıcı zaten var." }; }
        public override IdentityError InvalidUserName(string? userName) { return new IdentityError { Code = nameof(InvalidUserName), Description = $"'{userName}' kullanıcı adı geçersiz, sadece harf ve rakamlar içerebilir." }; }
        public override IdentityError InvalidEmail(string? email) { return new IdentityError { Code = nameof(InvalidEmail), Description = $"'{email}' e-posta adresi geçersiz." }; }
        public override IdentityError DuplicateUserName(string? userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"'{userName}' kullanıcı adı zaten alınmış." }; }
        public override IdentityError DuplicateEmail(string? email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"'{email}' e-posta adresi zaten alınmış." }; }
        public override IdentityError InvalidRoleName(string? role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"'{role}' rol adı geçersiz." }; }
        public override IdentityError DuplicateRoleName(string? role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"'{role}' rol adı zaten alınmış." }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Kullanıcının zaten bir şifresi var." }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Bu kullanıcı için hesap kilitleme etkin değil." }; }
        public override IdentityError UserAlreadyInRole(string? role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"Kullanıcı zaten '{role}' rolünde." }; }
        public override IdentityError UserNotInRole(string? role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"Kullanıcı '{role}' rolünde değil." }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Şifre en az {length} karakter uzunluğunda olmalıdır." }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Şifre en az bir özel karakter içermelidir (!@#$ vb.)." }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Şifre en az bir rakam içermelidir ('0'-'9')." }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Şifre en az bir küçük harf içermelidir ('a'-'z')." }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Şifre en az bir büyük harf içermelidir ('A'-'Z')." }; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Serilog.Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Database context setup with fallback
            // Try primary connection, fallback to secondary if available
            var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
            var localConnection = builder.Configuration.GetConnectionString("LocalConnection");
            
            builder.Services.AddDbContext<EtkinliklerDbContext>(options =>
            {
                var connectionString = defaultConnection;
                
                // Log which connection we're using
                var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");
                logger.LogInformation("Veritabanı bağlantısı: {ConnectionString}", connectionString);
                
                options.UseSqlServer(connectionString, sqlServerOptions => 
                {
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlServerOptions.CommandTimeout(30);
                });
            });

            // Add exception handling middleware
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            }

            // Add Identity services
            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options => 
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<EtkinliklerDbContext>()
            .AddErrorDescriber<TurkceIdentityErrorDescriber>()
            .AddDefaultTokenProviders();

            // Configure cookie policy
            builder.Services.ConfigureApplicationCookie(options => 
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            });

            // Add custom services
            builder.Services.AddScoped<IEtkinlikService, EtkinlikService>();
            builder.Services.AddScoped<IRezervasyonService, RezervasyonService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<ISepetService, SepetService>();
            builder.Services.AddScoped<IPaymentService, FakePaymentService>();

            // Configure EmailSettings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Set Turkish culture as default
            var cultureInfo = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }
            
            // Add our custom database exception middleware
            app.UseDatabaseExceptionMiddleware();
            
            // Add global exception handler
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html";
                    
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;
                    
                    await context.Response.WriteAsync($@"
                    <html>
                        <head>
                            <title>Hata Oluştu</title>
                            <style>
                                body {{ font-family: Arial, sans-serif; margin: 20px; }}
                                h1 {{ color: #d9534f; }}
                                .error-details {{ background-color: #f8f9fa; padding: 15px; border-radius: 5px; }}
                            </style>
                        </head>
                        <body>
                            <h1>Bir Hata Oluştu</h1>
                            <p>Uygulama şu anda çalışamıyor. Lütfen tekrar deneyiniz.</p>
                            <div class=""error-details"">
                                <p>Hata: {(app.Environment.IsDevelopment() ? exception?.Message : "Sunucu hatası")}</p>
                                {(app.Environment.IsDevelopment() ? $"<p>Yol: {exceptionHandlerPathFeature?.Path}</p>" : "")}
                                {(app.Environment.IsDevelopment() ? $"<p>Stack Trace: {exception?.StackTrace}</p>" : "")}
                            </div>
                            <p><a href=""/"">Ana Sayfaya Dön</a></p>
                        </body>
                    </html>");
                });
            });

            app.UseHttpsRedirection();
            
            // Add critical startup error handling
            if (app.Environment.IsDevelopment())
            {
                app.Map("/debug-startup", async context =>
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(@"
                    <html>
                        <head><title>Sistem Hazır</title></head>
                        <body style='font-family:sans-serif; padding:20px'>
                            <h1>Debug sayfası çalışıyor</h1>
                            <p>Bu sayfa görüntülenebiliyorsa, temel HTTP pipeline çalışıyor demektir.</p>
                            <a href='/'>Ana sayfaya dön</a>
                        </body>
                    </html>");
                });
            }
            
            // Statik dosyalar için yapılandırma
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".svg"] = "image/svg+xml";
            
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    // İstemci tarafında cacheleme için header'lar ekle
                    var headers = ctx.Context.Response.Headers;
                    
                    // Uzantıya göre cache politikası belirle
                    string extension = Path.GetExtension(ctx.File.Name).ToLowerInvariant();
                    
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || 
                        extension == ".svg" || extension == ".gif" || extension == ".webp" ||
                        extension == ".woff" || extension == ".woff2" || extension == ".ttf" || 
                        extension == ".eot")
                    {
                        // Resim ve fontlar için uzun süreli cache
                        headers["Cache-Control"] = "public,max-age=31536000";
                    }
                    else 
                    {
                        // Diğer statik dosyalar için kısa süreli cache
                        headers["Cache-Control"] = "public,max-age=86400";
                    }
                }
            });

            // Preload middleware'i
            app.Use(async (context, next) =>
            {
                // Kritik resimler için preload headerları ekle
                if (context.Request.Path.Value == "/")
                {
                    context.Response.Headers.Append("Link", 
                        "</img/biletini-ayir-logo.png>; rel=preload; as=image");
                    context.Response.Headers.Append("Link",
                        "</img/favicon.svg>; rel=preload; as=image");
                }
                
                await next();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Area routes
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // Default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Seed initial data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                    var dbContext = services.GetRequiredService<EtkinliklerDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    
                    // Try to connect with timeout
                    bool dbConnectionSuccessful = false;
                    try 
                    {
                        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                        {
                            dbConnectionSuccessful = dbContext.Database.CanConnect();
                        }
                    }
                    catch (Exception dbEx)
                    {
                        logger.LogError(dbEx, "Veritabanı bağlantı testi başarısız oldu");
                        Console.WriteLine($"Veritabanı bağlantı hatası: {dbEx.Message}");
                    }
                    
                    if (dbConnectionSuccessful)
                    {
                        // Ensure database is created
                        dbContext.Database.EnsureCreated();
                        
                        // Only seed if connection is successful
                        try
                        {
                            _221103018_OmerFarukBayraktutar.Data.InitialDataSeeder.SeedData(services, userManager, roleManager).Wait();
                            logger.LogInformation("Veritabanı başarıyla başlatıldı ve veriler seed edildi.");
                        }
                        catch (Exception seedEx)
                        {
                            logger.LogError(seedEx, "Veritabanı seed işlemi sırasında bir hata oluştu.");
                            Console.WriteLine("---------------------------------------------------------------");
                            Console.WriteLine("HATA: Veritabanı seed işlemi başarısız oldu: " + seedEx.Message);
                            Console.WriteLine("Stack Trace: " + seedEx.StackTrace);
                            Console.WriteLine("İç Hata: " + seedEx.InnerException?.Message);
                            Console.WriteLine("---------------------------------------------------------------");
                        }
                    }
                    else
                    {
                        logger.LogCritical("Veritabanına bağlanılamıyor! Uygulama sınırlı işlevlerle çalışacak.");
                        Console.WriteLine("---------------------------------------------------------------");
                        Console.WriteLine("UYARI: Veritabanı bağlantısı kurulamadı, uygulama sınırlı işlevlerle çalışacak.");
                        Console.WriteLine("Veritabanı bağlantı ayarlarını kontrol ediniz.");
                        Console.WriteLine("Hata ayıklama için: /debug-startup adresini ziyaret edin.");
                        Console.WriteLine("---------------------------------------------------------------");
                        
                        // Add a global flag to indicate DB connection issue
                        app.Use(async (context, next) =>
                        {
                            context.Items["DatabaseConnected"] = false;
                            await next();
                        });
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Uygulama başlatılırken beklenmedik bir hata oluştu.");
                    
                    Console.WriteLine("---------------------------------------------------------------");
                    Console.WriteLine("KRİTİK HATA: Uygulama başlatılırken beklenmedik bir hata: " + ex.Message);
                    Console.WriteLine("Stack Trace: " + ex.StackTrace);
                    Console.WriteLine("İç Hata: " + ex.InnerException?.Message);
                    Console.WriteLine("---------------------------------------------------------------");
                }
            }

            app.Run();
        }
    }
}
