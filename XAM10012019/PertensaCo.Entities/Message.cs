using System;
using System.Collections.Generic;
using static PertensaCo.Common.Constants.HTTPConstants;

namespace PertensaCo.Entities
{
	public class Message : Entity<int>, IEquatable<Message>
	{
		public Message(string subject, string sender)
		{
			Sender = sender;
			Subject = subject;
		}

		public string Content { get; set; }
		public string ContentType { get; set; } = TextMimeType;
		public DateTimeOffset? DateReceived { get; set; }
		public DateTimeOffset DateSent { get; set; }
		public string Sender { get; set; }
		public string Subject { get; set; }

		public virtual ICollection<Attachment> Attachments { get; set; } = new HashSet<Attachment>();

		public static bool operator ==(Message m, Message n)
		{
			return m?.Id == n?.Id && m?.DateSent == n?.DateSent;
		}

		public static bool operator !=(Message m, Message n)
		{
			return !(m == n);
		}

		public bool Equals(Message that)
		{
			if (that is null) return false;
			return this.Id.Equals(that.Id)
			&& this.DateSent.Equals(that.DateSent);
		}

		public override bool Equals(object other)
		{
			Message that = other as Message;
			if (that is null) return false;
			else return this.Equals(that);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ DateSent.GetHashCode();
		}

		public override string ToString()
		{
			return Content;
		}
	}
}
