// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

using System;
using System.IO;
using SkuVault.LastPass;

namespace SkuVault.LastPass.IntegrationTests
{
	class Program
	{
		// Very simple text based user interface that demonstrates how to respond to
		// to Vault UI requests.
		private class TextUi : Ui
		{
			public override string ProvideSecondFactorPassword( SecondFactorMethod method )
			{
				return GetAnswer( string.Format( "Please enter {0} code", method ) );
			}

			public override void AskToApproveOutOfBand( OutOfBandMethod method )
			{
				Console.WriteLine( "Please approve out-of-band via {0}", method );
			}

			private static string GetAnswer( string prompt )
			{
				Console.WriteLine( prompt );
				Console.Write( "> " );
				var input = Console.ReadLine();

				return input == null ? "" : input.Trim();
			}
		}

		static void Main( string[] args )
		{
			try
			{
				VaultCredentials vaultCredentials = ReadCredentialsFromFile();
				ClientInfo clientInfo = new ClientInfo( Platform.Desktop, vaultCredentials.Id, vaultCredentials.Description, false );
				var vault = OpenVault(vaultCredentials, clientInfo);
				DisplayAllAccounts(vault.Accounts);
				//AddAccount(vaultCredentials, clientInfo);
				AddApplication(vaultCredentials, clientInfo);
			}
			catch ( LoginException e )
			{
				Console.WriteLine( "Something went wrong: {0}", e );
			}
		}

		private static void AddAccount( VaultCredentials vaultCredentials, ClientInfo clientInfo )
		{
			string emptyId = string.Empty;
			const string noGroup = "(none)";
			var account = new Account( emptyId, "entry " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), "bob", "pass", "www.yahoo.com", noGroup );
			AccountsService.CreateAccount(account, vaultCredentials, clientInfo, new TextUi());
		}

		private static void AddApplication( VaultCredentials vaultCredentials, ClientInfo clientInfo )
		{
			const string noGroup = "(none)";
			string accountName = "entry " + DateTime.UtcNow.ToString( "yyyy-MM-dd HH:mm:ss" );
			AccountsService.CreateApplication(accountName, noGroup, vaultCredentials, clientInfo, new TextUi());
		}

		private static void DisplayAllAccounts( Account[] accounts )
		{
			// Dump all the accounts
			for ( var i = 0; i < accounts.Length; ++i )
			{
				var account = accounts[i];
				Console.WriteLine( "{0}:\n" +
							"        id: {1}\n" +
							"      name: {2}\n" +
							"  username: {3}\n" +
							"  password: {4}\n" +
							"       url: {5}\n" +
							"     group: {6}\n",
							i + 1,
							account.Id,
							account.Name,
							account.Username,
							account.Password,
							account.Url,
							account.Group );
			}
		}

		private static VaultCredentials ReadCredentialsFromFile()
		{
  			// Read LastPass credentials from a file
			// The file should contain 4 lines:
			//   - username
			//   - password
			//   - client ID
			//   - client description
			// See credentials.txt.example for an example.
			var credentialsStr = File.ReadAllLines( "../../Files/credentials.txt" );
			return new VaultCredentials(userName: credentialsStr[0], password: credentialsStr[1],
				id: credentialsStr[2], description: credentialsStr[3]);
		}

		private static Vault OpenVault(VaultCredentials vaultCredentials, ClientInfo clientInfo)
		{
			return Vault.Open( vaultCredentials.UserName,
						vaultCredentials.Password,
						clientInfo,
						new TextUi() );
		}
	}
}
