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

		/// <summary>Will create an "application" entry in LastPass, with appication, name.
		/// NOTE: Application can also have a list fields and a note. They're currently not saved</summary>
		/// <returns>id of the created application</returns>
		public static string CreateApplication(string application, string applicationName, string group, VaultCredentials vaultCredentials, ClientInfo clientInfo, Ui ui)
		{
			var session = Fetcher.Login(vaultCredentials.UserName, vaultCredentials.Password, clientInfo, ui);
			try
			{
				var encryptionKey = Blob.MakeEncryptionKey(vaultCredentials.UserName, vaultCredentials.Password, session.KeyIterationCount);
				return Fetcher.AddApplication(session, encryptionKey, application, applicationName, group);
			}
			finally
			{
				Fetcher.Logout(session);
			}
		}
	}
}
