using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using PertensaCo.Data;
using PertensaCo.Services.Contracts;
using static PertensaCo.Common.Constants.HTTPConstants;

namespace PertensaCo.Services
{
	public class MessengerService : IMessengerService
	{
		private readonly IOptions<MessengerOptions> options;
		private readonly PertensaDbContext dbContext;

		public MessengerService(IOptions<MessengerOptions> options, PertensaDbContext dbContext)
		{
			this.options = options;
			this.dbContext = dbContext;
		}

		public Task ReceiveEmailViaImap()
		{
			//using (var client = new ImapClient())
			//{
			//try
			//{
			//    // For demo-purposes, accept all SSL certificates
			//    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
			//    client.Connect("imap.friends.com", 993, true);
			//    client.Authenticate("joey", "password");
			//    // The Inbox folder is always available on all IMAP servers...
			//    var inbox = client.Inbox;
			//    inbox.Open(FolderAccess.ReadOnly);
			//    Console.WriteLine("Total messages: {0}", inbox.Count);
			//    Console.WriteLine("Recent messages: {0}", inbox.Recent);
			//    for (int i = 0; i < inbox.Count; i++)
			//    {
			//	var message = inbox.GetMessage(i);
			//	Console.WriteLine("Subject: {0}", message.Subject);
			//    }
			//    client.Disconnect(true);
			//}
			//catch (Exception exception)
			//{
			//    return Task.FromException(exception);
			//}
			//However, you probably want to do more complicated things with IMAP such as fetching summary information so that you can display a list of messages in a mail client without having to first download all of the messages from the server:
			//foreach (var summary in inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId))
			//{
			//    Console.WriteLine("[summary] {0:D2}: {1}", summary.Index, summary.Envelope.Subject);
			//}
			//The results of a Fetch command can also be used to download individual MIME parts rather than downloading the entire message. For example:
			//foreach (var summary in inbox.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure))
			//{
			//    if (summary.TextBody != null)
			//    {
			//	// this will download *just* the text/plain part
			//	var text = inbox.GetBodyPart(summary.UniqueId, summary.TextBody);
			//    }
			//    if (summary.HtmlBody != null)
			//    {
			//	// this will download *just* the text/html part
			//	var html = inbox.GetBodyPart(summary.UniqueId, summary.HtmlBody);
			//    }
			//    // if you'd rather grab, say, an image attachment... it might look something like this:
			//    if (summary.Body is BodyPartMultipart)
			//    {
			//	var multipart = (BodyPartMultipart)summary.Body;

			//	var attachment = multipart.BodyParts.OfType<BodyPartBasic>().FirstOrDefault(x => x.FileName == "logo.jpg");
			//	if (attachment != null)
			//	{
			//	    // this will download *just* the attachment
			//	    var part = inbox.GetBodyPart(summary.UniqueId, attachment);
			//	}
			//    }
			//}
			//You may also be interested in sorting and searching...

			// let's search for all messages received after Jan 12, 2013 with "MailKit" in the subject...
			//var query = SearchQuery.DeliveredAfter(DateTime.Parse("2013-01-12"))
			//    .And(SearchQuery.SubjectContains("MailKit")).And(SearchQuery.Seen);

			//foreach (var uid in inbox.Search(query))
			//{
			//    var message = inbox.GetMessage(uid);
			//    Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
			//}
			//// let's do the same search, but this time sort them in reverse arrival order
			//var orderBy = new[] { OrderBy.ReverseArrival };
			//foreach (var uid in inbox.Search(query, orderBy))
			//{
			//    var message = inbox.GetMessage(uid);
			//    Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
			//}
			//// you'll notice that the orderBy argument is an array... this is because you
			//// can actually sort the search results based on multiple columns:
			//orderBy = new[] { OrderBy.ReverseArrival, OrderBy.Subject };
			//foreach (var uid in inbox.Search(query, orderBy))
			//{
			//    var message = inbox.GetMessage(uid);
			//    Console.WriteLine("[match] {0}: {1}", uid, message.Subject);
			//}
			//}
			//Of course, instead of downloading the message, you could also fetch the summary information for the matching messages or do any of a number of other things with the UIDs that are returned.
			//How about navigating folders? MailKit can do that, too:
			// Get the first personal namespace and list the toplevel folders under it.
			//   var personal = client.GetFolder(client.PersonalNamespaces[0]);
			//   foreach (var folder in personal.GetSubfolders(false))
			//Console.WriteLine("[folder] {0}", folder.Name);
			//If the IMAP server supports the SPECIAL - USE or the XLIST(GMail) extension, you can get ahold of the pre - defined All, Drafts, Flagged(aka Important), Junk, Sent, Trash, etc folders like this:
			//   if ((client.Capabilities & (ImapCapabilities.SpecialUse | ImapCapabilities.XList)) != 0)
			//   {
			//var drafts = client.GetFolder(SpecialFolder.Drafts);
			//   }
			//   else
			//   {
			//// maybe check the user's preferences for the Drafts folder?
			//   }
			return Task.CompletedTask;
		}

		public Task ReceiveEmailViaPop3()
		{
			using (var pop3Client = new Pop3Client())
			{
				try
				{
					pop3Client.Connect(options.Value.Pop3ServerName, options.Value.Pop3ServerPort, false);
					pop3Client.Authenticate(options.Value.Pop3ServerUsername, options.Value.Pop3ServerPassword);
					for (int i = 0; i < pop3Client.Count; i++)
					{
						var message = pop3Client.GetMessage(i);
						/* DISTRIBUTE MESSAGES AMONG DEPARTMENT MessageBox-es BASED ON RECIPIENT ADDRESSES OR PROCESS THEM IN SOME OTHER WAY */
					}
					pop3Client.Disconnect(true);
				}
				catch (Exception exception)
				{
					return Task.FromException(exception);
				}
			}
			return Task.CompletedTask;
		}

		public async Task SendEmailAsync(string subject, string content, string sender, params string[] recipients)
		{
			var message = new MimeMessage();
			if (!recipients.Any())
			{
				throw new ArgumentNullException(nameof(recipients));
			}
			if (string.IsNullOrWhiteSpace(subject))
			{
				throw new ArgumentNullException(nameof(subject));
			}
			foreach (var recipient in recipients)
			{
				if (!string.IsNullOrWhiteSpace(recipient))
				{
					message.To.Add(new MailboxAddress(recipient));
				}
			}
			message.Subject = subject;
			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = content;
			message.Body = bodyBuilder.ToMessageBody();
			if (string.IsNullOrWhiteSpace(sender))
			{
				message.From.Add(new MailboxAddress(options.Value.DefaultMailboxName, GoogleSmtpServerPrincipal));
			}
			else message.From.Add(new MailboxAddress(sender));
			using (var smtpClient = new SmtpClient())
			{
				await smtpClient.ConnectAsync(options.Value.SmtpServerName, options.Value.SmtpServerPort, useSsl: false);
				await smtpClient.AuthenticateAsync(options.Value.SmtpServerUsername, options.Value.SmtpServerPassword);
				await smtpClient.SendAsync(message);
				await smtpClient.DisconnectAsync(true);
			}
		}

		public Task SendSmsAsync(string phoneNumber, string message)
		{
			throw new NotImplementedException();
		}
	}
}
