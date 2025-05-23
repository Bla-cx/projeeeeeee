using System;

namespace _221103018_OmerFarukBayraktutar.ViewModels
{
    public class PaymentSuccessViewModel
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string CardLast4Digits { get; set; }
    }
}
