using System.Threading.Tasks;

namespace IdentitySample.Repositories
{
    public interface IMessageSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
    }
}
