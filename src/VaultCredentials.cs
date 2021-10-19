namespace SkuVault.LastPass
{
	public class VaultCredentials
	{
		public string UserName { get; }
		public string Password { get; }
		public string Id { get; }
		public string Description { get; }

		public VaultCredentials( string userName, string password, string id, string description )
		{
			UserName = userName;
			Password = password;
			Id = id;
			Description = description;
		}
	}
}
