using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using _221103018_OmerFarukBayraktutar.Models;
using System.Diagnostics;

namespace _221103018_OmerFarukBayraktutar.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        // Geliştirme ortamında e-posta göndermeyi devre dışı bırakın
        private readonly bool _isDevelopment = true;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Geliştirme ortamında e-posta göndermeyi atla
            if (_isDevelopment)
            {
                Debug.WriteLine($"DEV MODE: E-posta gönderiliyor:");
                Debug.WriteLine($"Alıcı: {email}");
                Debug.WriteLine($"Konu: {subject}");
                Debug.WriteLine($"Mesaj: {message.Substring(0, Math.Min(100, message.Length))}...");
                
                // E-posta gönderme işlemini simüle etmek için küçük bir gecikme
                await Task.Delay(500);
                return;
            }
            
            // Gerçek ortamda e-posta gönderme
            var mail = new MailMessage
            {
                From = new MailAddress(_emailSettings.Mail, _emailSettings.DisplayName),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mail.To.Add(new MailAddress(email));

            using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(mail);
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var subject = "E-Posta Doğrulama - Biletini Ayır";
            var message = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Montserrat', Arial, sans-serif;
                            color: #333;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            width: 600px;
                            margin: 20px auto;
                            padding: 0;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
                            background-color: #ffffff;
                        }}
                        .header {{
                            background-color: #0d6efd;
                            color: white;
                            padding: 20px;
                            text-align: center;
                        }}
                        .logo {{
                            font-size: 24px;
                            font-weight: bold;
                            margin-bottom: 10px;
                        }}
                        .content {{
                            padding: 30px;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 12px 30px;
                            margin: 20px 0;
                            background-color: #0d6efd;
                            color: white;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: bold;
                            text-align: center;
                        }}
                        .footer {{
                            background-color: #f9f9f9;
                            padding: 15px;
                            text-align: center;
                            font-size: 12px;
                            color: #666;
                        }}
                        .social {{
                            margin: 15px 0;
                        }}
                        .social a {{
                            display: inline-block;
                            margin: 0 5px;
                            color: #0d6efd;
                            text-decoration: none;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>Biletini Ayır</div>
                            <div>Etkinlik Biletleme Platformu</div>
                        </div>
                        <div class='content'>
                            <h2>E-Posta Adresinizi Doğrulayın</h2>
                            <p>Merhaba,</p>
                            <p>Biletini Ayır platformuna kayıt olduğunuz için teşekkür ederiz. Hesabınızı aktifleştirmek ve tüm özellikleri kullanabilmek için lütfen e-posta adresinizi doğrulayın.</p>
                            <div style='text-align: center;'>
                                <a href='{confirmationLink}' class='button'>E-Posta Adresimi Doğrula</a>
                            </div>
                            <p>Bu bağlantı 24 saat boyunca geçerlidir. Bu işlemi siz talep etmediyseniz, lütfen bu e-postayı dikkate almayın.</p>
                            <p>Herhangi bir sorunuz varsa, destek ekibimizle iletişime geçebilirsiniz.</p>
                            <p>Saygılarımızla,<br/><strong>Biletini Ayır Ekibi</strong></p>
                        </div>
                        <div class='footer'>
                            <div class='social'>
                                <a href='#'>Facebook</a> | 
                                <a href='#'>Twitter</a> | 
                                <a href='#'>Instagram</a> | 
                                <a href='#'>LinkedIn</a>
                            </div>
                            <p>© @DateTime.Now.Year Biletini Ayır. Tüm hakları saklıdır.</p>
                            <p>Bu e-posta {email} adresine gönderilmiştir.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, message);
        }

        public async Task SendPasswordResetAsync(string email, string resetLink)
        {
            var subject = "Şifre Sıfırlama - Biletini Ayır";
            var message = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Montserrat', Arial, sans-serif;
                            color: #333;
                            background-color: #f5f5f5;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            width: 600px;
                            margin: 20px auto;
                            padding: 0;
                            border-radius: 8px;
                            overflow: hidden;
                            box-shadow: 0 4px 10px rgba(0,0,0,0.1);
                            background-color: #ffffff;
                        }}
                        .header {{
                            background-color: #0d6efd;
                            color: white;
                            padding: 20px;
                            text-align: center;
                        }}
                        .logo {{
                            font-size: 24px;
                            font-weight: bold;
                            margin-bottom: 10px;
                        }}
                        .content {{
                            padding: 30px;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 12px 30px;
                            margin: 20px 0;
                            background-color: #0d6efd;
                            color: white;
                            text-decoration: none;
                            border-radius: 50px;
                            font-weight: bold;
                            text-align: center;
                        }}
                        .warning {{
                            background-color: #fff8e1;
                            border-left: 4px solid #ffc107;
                            padding: 15px;
                            margin: 20px 0;
                        }}
                        .footer {{
                            background-color: #f9f9f9;
                            padding: 15px;
                            text-align: center;
                            font-size: 12px;
                            color: #666;
                        }}
                        .social {{
                            margin: 15px 0;
                        }}
                        .social a {{
                            display: inline-block;
                            margin: 0 5px;
                            color: #0d6efd;
                            text-decoration: none;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>Biletini Ayır</div>
                            <div>Etkinlik Biletleme Platformu</div>
                        </div>
                        <div class='content'>
                            <h2>Şifrenizi Sıfırlayın</h2>
                            <p>Merhaba,</p>
                            <p>Biletini Ayır hesabınız için şifre sıfırlama talebinde bulundunuz. Şifrenizi sıfırlamak için aşağıdaki butona tıklayın:</p>
                            <div style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Şifremi Sıfırla</a>
                            </div>
                            <div class='warning'>
                                <strong>Önemli:</strong> Bu bağlantı 1 saat boyunca geçerlidir ve yalnızca bir kez kullanılabilir. Bu işlemi siz talep etmediyseniz, lütfen bu e-postayı dikkate almayın ve hesabınızın güvenliğini kontrol edin.
                            </div>
                            <p>Herhangi bir sorunuz varsa veya yardıma ihtiyacınız olursa, destek ekibimizle iletişime geçebilirsiniz.</p>
                            <p>Saygılarımızla,<br/><strong>Biletini Ayır Ekibi</strong></p>
                        </div>
                        <div class='footer'>
                            <div class='social'>
                                <a href='#'>Facebook</a> | 
                                <a href='#'>Twitter</a> | 
                                <a href='#'>Instagram</a> | 
                                <a href='#'>LinkedIn</a>
                            </div>
                            <p>© @DateTime.Now.Year Biletini Ayır. Tüm hakları saklıdır.</p>
                            <p>Bu e-posta {email} adresine gönderilmiştir.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, message);
        }
    }
}
