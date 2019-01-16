using PertensaCo.Services.Contracts;

namespace PertensaCo.Services
{
    public class MessengerOptions : IMessengerOptions
    {
	public string DefaultMailboxName { get; set; }

	public int ImapServerPort { get; set; } = 143;
	public int ImapServerSecurePort { get; set; } = 993;
	public string ImapServerName { get; set; }
	public string ImapServerPassword { get; set; }
	public string ImapServerUsername { get; set; }

	public int Pop3ServerPort { get; set; } = 110;
	public int Pop3ServerSecurePort { get; set; } = 995;
	public string Pop3ServerName { get; set; }
	public string Pop3ServerPassword { get; set; }
	public string Pop3ServerUsername { get; set; }

	public int SmtpServerPort { get; set; } = 25;
	public int SmtpServerSecurePort { get; set; } = 465;
	public string SmtpServerName { get; set; }
	public string SmtpServerPassword { get; set; }
	public string SmtpServerUsername { get; set; }
    }
}
