using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SkuVault.LastPass
{
	public class AccountsService
	{
		/// <summary>Will create an "account" entry in LastPass, with Url, name, username, password</summary>
		/// <returns>Created account's AccountId, Action, Message</returns>
		public static AddAccountResponse CreateAccount(Account account, VaultCredentials vaultCredentials, ClientInfo clientInfo, Ui ui)
		{
			var session = Fetcher.Login(vaultCredentials.UserName, vaultCredentials.Password, clientInfo, ui);
			try
			{
				var encryptionKey = Blob.MakeEncryptionKey(vaultCredentials.UserName, vaultCredentials.Password, session.KeyIterationCount);
				return AddAccount( session, encryptionKey, account );
			}
			finally
			{
				Fetcher.Logout(session);
			}
		}

		//Based on lastpass_update_account in https://github.com/lastpass/lastpass-cli/blob/master/endpoints.c#L167
		private static AddAccountResponse AddAccount(Session session, byte[] encryptionKey, Account account)
		{
			using (var webClient = new WebClient())
			{
				string rawResponseText;
				try
				{
					Fetcher.SetSessionCookies(webClient, session);
					var parameters = new NameValueCollection
					{
						{"extjs", "1"},
						{"token", session.Token},
						{"method", "cli"},
						{"name", ParserHelper.Encode64(ParserHelper.EncryptAes256(account.Name, encryptionKey))},	//TODO GUARD-2194 Perhaps create an AccountWithEncrypted : Account
																					//	and populate these in the constructor? Same for the 3 places below
						{"grouping", ParserHelper.Encode64(ParserHelper.EncryptAes256(account.Group, encryptionKey))},
						{"pwprotect", "off"},	//Require master password to view: "on" / "off",
						{"aid", string.IsNullOrWhiteSpace(account.Id) ? "0" : account.Id},
						{"url", Extensions.ToHex(account.Url.ToBytes())},
						{"username", ParserHelper.Encode64(ParserHelper.EncryptAes256(account.Username, encryptionKey))},
						{"password", ParserHelper.Encode64(ParserHelper.EncryptAes256(account.Password, encryptionKey))},
						//"extra", account->note_encrypted,
					};
					rawResponseText = webClient.UploadValues("https://lastpass.com/show_website.php", parameters).ToUtf8();
				}
				catch (WebException ex)
				{
					throw new FetchException(FetchException.FailureReason.WebException, "WebException occurred", ex);
				}

				try
				{
					var response = rawResponseText.Deserialize<AddAccountResponseWrapper>();
					return response.Result;
				}
				catch (FormatException ex)
				{
					throw new FetchException(FetchException.FailureReason.InvalidResponse, "Invalid response", ex);
				}
			}
		}

		/// <summary>Will create an "application" entry in LastPass, with appication, name.
		/// NOTE: Application can also have a list fields and a note. They're currently not saved</summary>
		/// <returns>appaid of the created application</returns>
		public static string CreateApplication(string application, string applicationName, string group, VaultCredentials vaultCredentials, ClientInfo clientInfo, Ui ui)
		{
			var session = Fetcher.Login(vaultCredentials.UserName, vaultCredentials.Password, clientInfo, ui);
			try
			{
				var encryptionKey = Blob.MakeEncryptionKey(vaultCredentials.UserName, vaultCredentials.Password, session.KeyIterationCount);
				string result = AddApplication(session, encryptionKey, application, applicationName, group);
				return result;
			}
			finally
			{
				Fetcher.Logout(session);
			}
		}

		/// <summary>Add "application" in LastPass, with Application, Name, Notes and Fields</summary>
		/// <returns>appaid returned by LastPass</returns>
		//Based on lastpass_update_account in https://github.com/lastpass/lastpass-cli/blob/master/endpoints.c#L167
		private static string AddApplication(Session session, byte[] encryptionKey, string application, string applicationName, string group)
		{
			using (var webClient = new WebClient())
			{
				XDocument response;
				try
				{
					Fetcher.SetSessionCookies(webClient, session);
					var parameters = new NameValueCollection
					{
						{"extjs", "1"},
						{"token", session.Token},
						{"method", "cli"},
						{"name", ParserHelper.Encode64(ParserHelper.EncryptAes256(applicationName, encryptionKey))},
						{"grouping", ParserHelper.Encode64(ParserHelper.EncryptAes256(group, encryptionKey))},
						{"pwprotect", "off"},	//Require master password to view: "on" / "off",
						{"ajax", "1"},
						{"cmd", "updatelpaa"},
						{"appname", application},
					};
					response = XDocument.Parse(webClient.UploadValues("https://lastpass.com/addapp.php", parameters).ToUtf8());
				}
				catch (WebException ex)
				{
					throw new FetchException(FetchException.FailureReason.WebException, "WebException occurred", ex);
				}

				try
				{
					var appaid = response.Elements("xmlresponse").Elements("result").Attributes().First(a => a.Name == "appaid").Value;	//attribute msg="accountadded" might be useful too
					return appaid;
				}
				catch (FormatException ex)
				{
					throw new FetchException(FetchException.FailureReason.InvalidResponse, "Invalid response", ex);
				}
			}
		}
	}
}
