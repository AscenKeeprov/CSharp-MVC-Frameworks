using System.Collections.Generic;

namespace PertensaCo.Entities.Contracts
{
    public interface IMessageBox : ICollection<Message>
    {
	IReadOnlyCollection<Message> Received { get; }
	IReadOnlyCollection<Message> Sent { get; }
    }
}
