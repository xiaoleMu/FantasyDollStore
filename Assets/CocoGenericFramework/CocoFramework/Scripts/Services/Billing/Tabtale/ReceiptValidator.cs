using System;
using RSG;
using System.Collections;
using LitJson;
using System.Text;
using UnityEngine;
using TabTale.Analytics;
using System.Collections.Generic;

namespace TabTale
{
	public class ReceiptValidator : IReceiptValidator
	{
		private class ReceiptValidationData 
		{
			public string receipt;
			public string platform;
			public string iapBundleId;
			public string appBundleId;
			public string nonce;
		}

		private class ReceiptValidationResponse
		{
			public string error;
			public string nonce;
			public bool verified;
		}

		[Inject]
		public IRoutineRunner routineRunner { get; set; }

		[Inject]
		public IAppInfo appInfoService { get; set; }

		[Inject]
		public IGameDB gameDB { get; set; }

		[Inject]
		public ILogger logger { get; set; }

		[Inject]
		public IAnalyticsService analyticsService { get; set; }

		private ICoroutineFactory _coroutineFactory;
		private ConnectionHandler _connectionHandler;

		private const string Tag = "ReceiptValidator";

		[PostConstruct]
		public void Init()
		{
			_connectionHandler = new ConnectionHandler();
			_coroutineFactory = GameApplication.Instance.CoroutineFactory;
			_connectionHandler.Init(gameDB,_coroutineFactory);
		}

		#region IReceiptValidator implementation

		public IPromise<bool> Validate (InAppPurchasableItem inAppItem)
		{
			var promise = new Promise<bool>();

			if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
			{
				promise.Resolve(true);
			}
			else
			{
				routineRunner.StartCoroutine(RemoteValidationCoro(inAppItem, promise));
			}

			return promise;
		}

		#endregion

		IEnumerator RemoteValidationCoro(InAppPurchasableItem item, Promise<bool> promise)
		{
			var data = new ReceiptValidationData();

			var receiptJson = JsonMapper.ToObject(item.receipt);

			data.receipt = receiptJson["Payload"].ToString();
			data.appBundleId = appInfoService.BundleIdentifier;
			data.iapBundleId = item.id;

			#if UNITY_ANDROID
			data.receipt = Convert.ToBase64String(Encoding.UTF8.GetBytes(data.receipt));
			data.platform = "Android";
			#elif UNITY_IPHONE
			data.platform = "iOS";
			#endif

			data.nonce = DateTime.Now.ToString();

			string validationData = JsonMapper.ToJson(data);

			Action<ConnectionHandler.RequestResult, string> receiptValidationResponse = (result, responseData) => { 

				logger.Log (Tag,"HandleLoginResponse (result=" + result + ")\nresponse=" + responseData.SSubstring(300));

				if (result == ConnectionHandler.RequestResult.Ok)
				{
					ReceiptValidationResponse response = JsonMapper.ToObject<ReceiptValidationResponse>(responseData);

					string encryptedNonce = Sha256 (data.nonce);

					if (response.nonce.Equals(encryptedNonce))
					{
						if (response.verified)
						{
							logger.Log(Tag, "Validation successful");
							analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, Tag, new Dictionary<string, object>() {{"Validation","successful"}}, false);
							promise.Resolve(true);
						}
						else
						{
							logger.LogError(Tag, "Validation failed - not verified");
							analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, Tag, new Dictionary<string, object>() {{"Validation","failed"},{"Reason","Not verified"}}, false);
							promise.Resolve(false);
						}
						
					}
					else
					{
						logger.LogError(Tag, "Validation failed - non matching nonce");
						analyticsService.LogEvent(AnalyticsTargets.ANALYTICS_TARGET_FLURRY, Tag, new Dictionary<string, object>() {{"Validation","failed"},{"Reason","Non matching nonce"}}, false);
						promise.Resolve(false);
					}

				}
				else
				{
					logger.LogError(Tag, "Purchase Validation Failure");
				}
			};

			yield return _coroutineFactory.StartCoroutine(() => _connectionHandler.SendRequest(ConnectionHandler.RequestType.ReceiptValidation,
				receiptValidationResponse,validationData));
		}

		static string Sha256(string str)
		{
			str = "tt-" + str;
			System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
			System.Text.StringBuilder hash = new System.Text.StringBuilder();

			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));

			hash.Append(Convert.ToBase64String(crypto));

			return hash.ToString();
		}

	}
}

