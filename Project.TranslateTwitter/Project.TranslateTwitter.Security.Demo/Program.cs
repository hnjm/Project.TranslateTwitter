﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Nito.AsyncEx;

namespace Project.TranslateTwitter.Security.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			AsyncContext.Run(() => TestSignInWithXAuth());
		}

		private async static void TestSignInWithXAuth()
		{
			Console.WriteLine("Enter User Name...");
			string userName = Console.ReadLine();
			Console.WriteLine("Enter Password...");
			string password = Console.ReadLine();

			var auth = new XAuthAuthorizer
			{
				CredentialStore = new XAuthCredentials
				{
					ConsumerKey = OAuthProperties.ConsumerKey,
					ConsumerSecret = OAuthProperties.ConsumerKeySecret,
					UserName = userName,
					Password = password
				}
			};

			try
			{
				await auth.AuthorizeAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			using (var twitterCtx = new TwitterContext(auth))
			{
				//Log
				twitterCtx.Log = Console.Out;

				List<LinqToTwitter.User> users =
					(from tweet in twitterCtx.User
					 where tweet.Type == UserType.Show
							&& tweet.ScreenName == "JoeMayo"
					 select tweet)
					.ToList();

				users.ForEach(user =>
				{
					var status =
						user.Protected || user.Status == null ?
							"Status Unavailable" :
							user.Status.Text;

					Console.WriteLine(
						"ID: {0}, Name: {1}\nLast Tweet: {2}\n",
						user.UserID, user.ScreenName, status);
				});
			}
		}
	}
}
