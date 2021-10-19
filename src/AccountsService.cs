namespace SkuVault.LastPass
{
	public class AccountsService
	{
		public static Account CreateAccount(Account account, VaultCredentials vaultCredentials, ClientInfo clientInfo, Ui ui)
		{
			var session = Fetcher.Login(vaultCredentials.UserName, vaultCredentials.Password, clientInfo, ui);
			try
			{
				var encryptionKey = Blob.MakeEncryptionKey(vaultCredentials.UserName, vaultCredentials.Password, session.KeyIterationCount);
				return Fetcher.AddAccount(session, encryptionKey, account);
			}
			finally
			{
				Fetcher.Logout(session);
			}
		}

		public static string CreateApplication(string accountName, string accountGroup, VaultCredentials vaultCredentials, ClientInfo clientInfo, Ui ui)
		{
			var session = Fetcher.Login(vaultCredentials.UserName, vaultCredentials.Password, clientInfo, ui);
			try
			{
				var encryptionKey = Blob.MakeEncryptionKey(vaultCredentials.UserName, vaultCredentials.Password, session.KeyIterationCount);
				return Fetcher.AddApplication(session, encryptionKey, accountName, accountGroup);
			}
			finally
			{
				Fetcher.Logout(session);
			}
		}
	}
}
