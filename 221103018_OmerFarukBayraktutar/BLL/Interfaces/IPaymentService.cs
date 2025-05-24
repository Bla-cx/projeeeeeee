using _221103018_OmerFarukBayraktutar.ViewModels;
using _221103018_OmerFarukBayraktutar.BLL.Services;
using System.Threading.Tasks;

namespace _221103018_OmerFarukBayraktutar.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentViewModel payment);
    }
}
