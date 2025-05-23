using _221103018_OmerFarukBayraktutar.ViewModels;
using _221103018_OmerFarukBayraktutar.BLL.Interfaces;
using System;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.BLL.Services
{
    public class FakePaymentService : IPaymentService
    {
        private readonly Random _random = new Random();

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentViewModel payment)
        {
            // Ödeme işlemini simüle et (1-2 saniye)
            await Task.Delay(_random.Next(1000, 2000));

            // %90 başarılı ödeme, %10 başarısız ödeme simülasyonu
            bool isSuccessful = _random.Next(1, 11) <= 9;

            // Kartın son 4 hanesi
            string last4Digits = payment.CardNumber.Substring(payment.CardNumber.Length - 4);

            return new PaymentResult
            {
                IsSuccessful = isSuccessful,
                TransactionId = isSuccessful ? Guid.NewGuid().ToString("N") : null,
                ErrorMessage = isSuccessful ? null : GetRandomErrorMessage(),
                CardLast4Digits = last4Digits,
                PaymentDate = DateTime.Now,
                PaymentAmount = payment.Amount
            };
        }

        private string GetRandomErrorMessage()
        {
            string[] errorMessages = new string[]
            {
                "Yetersiz bakiye",
                "Kart bilgileri hatalı",
                "Kartınız işleme kapalı",
                "Bankanız tarafından işlem reddedildi",
                "CVV kodu hatalı",
                "Son kullanma tarihi geçmiş"
            };

            return errorMessages[_random.Next(errorMessages.Length)];
        }
    }

    public class PaymentResult
    {
        public bool IsSuccessful { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public string CardLast4Digits { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}
