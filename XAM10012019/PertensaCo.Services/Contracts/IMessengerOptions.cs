namespace PertensaCo.Services.Contracts
{
	public interface IMessengerOptions
	{
		string DefaultMailboxName { get; set; }
		int ImapServerPort { get; set; }
		int ImapServerSecurePort { get; set; }
		string ImapServerName { get; set; }
		string ImapServerPassword { get; set; }
		string ImapServerUsername { get; set; }
		int Pop3ServerPort { get; set; }
		int Pop3ServerSecurePort { get; set; }
		string Pop3ServerName { get; set; }
		string Pop3ServerPassword { get; set; }
		string Pop3ServerUsername { get; set; }
		int SmtpServerPort { get; set; }
		int SmtpServerSecurePort { get; set; }
		string SmtpServerName { get; set; }
		string SmtpServerPassword { get; set; }
		string SmtpServerUsername { get; set; }
	}
}
