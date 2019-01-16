using System.Threading.Tasks;

namespace PertensaCo.Services.Contracts
{
	public interface ISmsSender
	{
		Task SendSmsAsync(string phoneNumber, string message);
	}
}
