using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PertensaCo.Entities.Contracts;

namespace PertensaCo.Entities
{
    public class MessageBox : IMessageBox
    {
	private readonly IDictionary<string, HashSet<Message>> filterByDateReceived;
	private readonly IDictionary<string, HashSet<Message>> filterBySender;
	private List<Message> received;
	private List<Message> sent;

	public MessageBox()
	{
	    filterByDateReceived = new Dictionary<string, HashSet<Message>>();
	    filterBySender = new Dictionary<string, HashSet<Message>>();
	    received = new List<Message>();
	    sent = new List<Message>();
	}

	public IReadOnlyCollection<Message> Received => received.AsReadOnly();
	public IReadOnlyCollection<Message> Sent => sent.AsReadOnly();

	public int Count => Received.Count;
	public bool IsReadOnly => true;

	public IEnumerator<Message> GetEnumerator()
	{
	    foreach (var message in Received)
	    {
		yield return message;
	    }
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
	    return Received.GetEnumerator();
	}

	public void Add(Message message)
	{
	    if (message == null)
	    {
		throw new ArgumentNullException(nameof(message));
	    }
	    message.DateReceived = DateTimeOffset.Now;
	    received.Add(message);
	    if (filterBySender.ContainsKey(message.Sender))
	    {
		filterBySender[message.Sender].Add(message);
	    }
	    foreach (var key in filterByDateReceived.Keys.Where(k
		=> k.ToLower().Contains($"year:{message.DateReceived.Value.Year}")
		|| k.ToLower().Contains($"month:{message.DateReceived.Value.Month}")
		|| k.ToLower().Contains($"day:{message.DateReceived.Value.Day}")))
	    {
		filterByDateReceived[key].Add(message);
	    }
	}

	public void Clear()
	{
	    filterByDateReceived.Clear();
	    filterBySender.Clear();
	    received.Clear();
	    sent.Clear();
	}

	public bool Contains(Message message)
	{
	    return received.Contains(message);
	}

	public void CopyTo(Message[] array, int arrayIndex)
	{
	    received.CopyTo(array, arrayIndex);
	}

	public bool Remove(Message message)
	{
	    return received.Remove(message);
	}
    }
}
