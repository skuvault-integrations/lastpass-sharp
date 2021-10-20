// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

using System;
using System.Linq;
using NUnit.Framework;

namespace SkuVault.LastPass.Test
{
    [TestFixture]
    class VaultTest
    {
        [Test]
        public void GenerateRandomClientId_returns_32_characters()
        {
            var id = Vault.GenerateRandomClientId();
            Assert.That(id.Length, Is.EqualTo(32));
        }

        [Test]
        public void GenerateRandomClientId_returns_different_ids()
        {
            var id1 = Vault.GenerateRandomClientId();
            var id2 = Vault.GenerateRandomClientId();

            Assert.That(id1, Is.Not.EqualTo(id2));
        }

        //
        // TODO: Figure out how to test this!
        //       All methods require username/password which I don't want to expose here.
        //       Actually, I'm pretty sure the password is lost and the whole test blob
        //       needs to be regenerated.
        //       Currently all the vault tests that deal with decryption are disabled.
        //

        [Test]
        public void Create_throws_on_truncated_blob()
        {
            var tests = new[] {1, 2, 3, 4, 5, 10, 100, 1000};
            foreach (var i in tests)
            {
                var e = Assert.Throws<ParseException>(() => Vault.Create(
                    new Blob(TestData.Blob.Take(TestData.Blob.Length - i).ToArray(), 1, ""),
                    username: "",
                    password: ""));
                Assert.AreEqual(ParseException.FailureReason.CorruptedBlob, e.Reason);
                Assert.AreEqual("Blob is truncated", e.Message);
            }
        }

        //[Test]
        public void Create_returns_vault_with_correct_accounts()
        {
            var vault = Vault.Create(new Blob(TestData.Blob, 1, ""), username: "", password: "");
            Assert.AreEqual(TestData.Accounts.Length, vault.Accounts.Length);
            Assert.AreEqual(TestData.Accounts.Select(i => i.Id), vault.Accounts.Select(i => i.Id));
            Assert.AreEqual(TestData.Accounts.Select(i => i.Url), vault.Accounts.Select(i => i.Url));
        }
    }
}
