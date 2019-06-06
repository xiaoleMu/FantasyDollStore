using System;
using System.Globalization;
using RSG;
using LitJson;
using UnityEngine;

namespace TabTale
{
	/// <summary>
	/// Regional service.
	/// 
	/// Usage example:
	/// GetCountryName().Done(name => updateUI(name));
	/// 
	/// </summary>
	public class RegionalService
	{
		[Inject]
		public ConnectionHandler connectionHandler { get; set; }

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		private string _countryCode = "";

		/// <summary>
		/// If we already have the country name - return it now, otherwise return it async
		/// </summary>
		/// <returns>The country name.</returns>
		public Promise<string> GetCountryName(bool limitedLength = true)
		{
			var promise = new Promise<string>();

			GetCountryCode()
				.Catch(e => promise.Reject(e))
				.Done(code => {
				var myCountry = ISO3166.FromAlpha2(code);

				logger.Log("RegionalService","Code :" + myCountry.Name);

				string countryName = myCountry.Name;
				if(limitedLength)
				{
					countryName = countryName.SSubstring(12);
				}
				promise.Resolve(countryName);
			});

			return promise;
		}

		/// <summary>
		/// If we already have the country code - return it immediately, otherwise return it async
		/// </summary>
		/// <returns>The country code.</returns>
		public Promise<string> GetCountryCode()
		{
			if(!String.IsNullOrEmpty(_countryCode))
			{
				var promise = new Promise<string>();
				promise.Resolve(_countryCode);
				return promise;
			}
			else
			{
				return RefreshCountryCode();
			}
		}

		/// <summary>
		/// Refresh the country code, can be called in advance in case we don't want to wait for the country code
		/// </summary>
		/// <returns>The country code.</returns>
		public Promise<string> RefreshCountryCode()
		{
			var promise = new Promise<string>();
			Action<ConnectionHandler.RequestResult, string> HandleGetCountry = (result,response) => { 

				if (result != ConnectionHandler.RequestResult.Ok)
				{
					promise.Reject(new Exception(response));
					return;
				}

				JsonData responseJsonObject= JsonMapper.ToObject(response);
				_countryCode = (string)responseJsonObject["country"];

				promise.Resolve(_countryCode);
			};

			routineRunner.StartCoroutine(connectionHandler.SendRequest(ConnectionHandler.RequestType.GetCountry, HandleGetCountry));

			return promise;
		}
	}
}

