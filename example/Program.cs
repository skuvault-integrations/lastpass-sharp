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
				// Fetch and create the vault from LastPass
				var vault = OpenVault();
				DisplayAllAccounts( vault.Accounts );
			}
			catch ( LoginException e )
			{
				Console.WriteLine( "Something went wrong: {0}", e );
			}
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

		private static Vault OpenVault()
		{
			// Read LastPass credentials from a file
			// The file should contain 4 lines:
			//   - username
			//   - password
			//   - client ID
			//   - client description
			// See credentials.txt.example for an example.
			var credentials = File.ReadAllLines( "../../Files/credentials.txt" );
			var username = credentials[0];
			var password = credentials[1];
			var id = credentials[2];
			var description = credentials[3];
			return Vault.Open( username,
						password,
						new ClientInfo( Platform.Desktop, id, description, false ),
						new TextUi() );
		}
	}
}
