using RestSharp;
using System;
using System.Text;
using Brady.ImageRecognition;
using System.Security.Cryptography;

namespace ImageRecognitionSandbox
{
	public class CallTargets
	{
		string _host = "https://vws.vuforia.com";
		string _path = "targets";

		string _accessKey = "7458a5660527153421b27237413fd496daaad38b";
		string _secretKey = "2d56033a170817bff5cb31a2fe0211bdbb7c5cee";
		string _hexDigest = "d41d8cd98f00b204e9800998ecf8427e"; // Hex digest of an empty string
		string _targetId = "3d5bce8046f142e69960a679f35098b4";

		public string GetTargets()
		{
			RestClient client = new RestClient(_host);
			RestRequest request = new RestRequest($"{_path}");

			var currentTimeString = GetCurrentUTCTimeString();
			Console.Write($"Time: {currentTimeString}\n");

			request.Method = Method.GET;
			request.AddHeader("Date", currentTimeString);

			var authorizationHeader = BuildAuthorizationHeader(request);
			Console.Write($"Authorization Header: {authorizationHeader}\n");

			request.AddHeader("Content-Type", "application/json");

			request.AddHeader("Authorization", authorizationHeader);

			var restResponse = client.Execute(request);

			return restResponse.Content;
		}

		string GetCurrentUTCTimeString()
		{
			return DateTime.UtcNow.ToString("R");
		}

		string BuildAuthorizationHeader(RestRequest request)
		{
			StringBuilder headerValue = new StringBuilder($"VWS {_accessKey}:");

			string signature = new SignatureBuilder().GetSignature(request, _secretKey, _hexDigest);

			headerValue.Append(signature);
			return headerValue.ToString();
		}

		public string CreateMD5HashString(MD5 md5Hash, byte[] data)
		{
			byte[] hash = md5Hash.ComputeHash(data);
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
			//return Convert.ToBase64String(hash);
		}
	}
}
