using System.Threading.Tasks;

namespace PertensaCo.Services.Contracts
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string subject, string content, string sender, params string[] recipients);
	}
}
