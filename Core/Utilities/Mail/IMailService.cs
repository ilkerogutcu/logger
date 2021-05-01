using System.Threading.Tasks;
using Core.Entities.Concrete;

namespace Core.Utilities.Mail
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}