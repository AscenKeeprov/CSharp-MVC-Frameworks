namespace PertensaCo.Entities
{
	public abstract class Entity<TIdentifier>
	{
		public TIdentifier Id { get; set; }
	}
}
