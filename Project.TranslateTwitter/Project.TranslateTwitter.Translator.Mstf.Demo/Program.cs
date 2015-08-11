﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Project.TranslateTwitter.Translator.Microsoft.Auth;
using Project.TranslateTwitter.Translator.Microsoft.Commands;

namespace Project.TranslateTwitter.Translator.Mstf.Demo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string clientId = "Project_TranslateTwitter";
			string clientSecret = Environment.GetEnvironmentVariable(
				"Project_TranslateTwitter.ClientSecret", EnvironmentVariableTarget.User);

			var authenticationContext = new AuthenticationContext(clientId, clientSecret);

			//TestLanguageDetection(clientId, clientSecret);
			//TestTranslating(clientId, clientSecret);
			TestSupportedLanguages(authenticationContext);

			Console.ReadKey();
		}

		private static void TestSupportedLanguages(AuthenticationContext authenticationContext)
		{
			var lister = new LanguageLister(authenticationContext);
			foreach (string language in lister.GetSupportedLanguages())
			{
				Console.WriteLine("Language => {0}", language);
			}
		}


		/// <remarks>
		/// https://msdn.microsoft.com/en-us/library/Ff512411.aspx
		/// </remarks>
		private static void TestLanguageDetection(string clientId, string clientSecret)
		{
			try
			{
				Console.WriteLine("Enter Text to detect language:");
				string textToDetect = Console.ReadLine();
				//textToDetect = "会場限定";

				var languageDetector = new LanguageDetector(new AuthenticationContext(clientId, clientSecret));
				var detectedLanguage = languageDetector.Detect(textToDetect);

				Console.WriteLine("Language Detected: {0}", detectedLanguage);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}
			catch (WebException e)
			{
				ProcessWebException(e);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
			}
		}

		private static void ProcessWebException(WebException e)
		{
			Console.WriteLine("{0}", e.ToString());
			// Obtain detailed error information
			string strResponse = String.Empty;
			using (HttpWebResponse response = (HttpWebResponse)e.Response)
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(responseStream, Encoding.ASCII))
					{
						strResponse = sr.ReadToEnd();
					}
				}
			}
			Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
		}


		/// <remarks>
		/// http://blogs.msdn.com/b/translation/p/gettingstarted2.aspx
		/// </remarks>
		private static void TestTranslating(string clientId, string clientSecret)
		{
			var authenticationContext = new AuthenticationContext(clientId, clientSecret);

			Console.WriteLine("Enter Text to translate:");
			string txtToTranslate = "안녕 세상아";
			txtToTranslate = Console.ReadLine();

			var languageDetector = new LanguageDetector(authenticationContext);
			var detectedLanguage = languageDetector.Detect(txtToTranslate);

			var translator = new LanguageTranslator(authenticationContext);
			var translatedText = translator.Translate(new LanguageTranslatorArg(txtToTranslate, detectedLanguage));

			Console.WriteLine("Detected Language : {0} and Translation is: {1}", detectedLanguage, translatedText);
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(true);
		}
	}
}
