using System.Text;
using System;
using System.IO;
using System.Security.Cryptography;
using RestSharp;
using System.Linq;
using System.Collections.Generic;

namespace Brady.ImageRecognition
{
	public class CloudRecognition
	{

		string _accessKey = "a9438f16283f0d17d6b8d7598526ca7742a1a102";
		string _secretKey = "b7834db5fc8b71fca49f46751ef3ca53f7e0f17d";
		string _hexDigest = "d41d8cd98f00b204e9800998ecf8427e"; // Hex digest of an empty string
		string _contentType = "multipart/form-data";
		string _cloudServicesUrl = "https://cloudreco.vuforia.com";
		string _boundary = "----------------------------28947758029299";
		string _imagePath = "Y1159546.jpg";

		public string RequestMatch()
		{
			RestClient client = new RestClient(_cloudServicesUrl);
			RestRequest request = new RestRequest("v1/query");

			var currentTimeString = GetCurrentUTCTimeString();
			Console.Write($"Time: {currentTimeString}\n");


			string imagePath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents"), _imagePath);
			byte[] image = File.ReadAllBytes(imagePath);

			request.AlwaysMultipartFormData = true;
			request.AddFile("image", image, _imagePath, "image/jpeg");
			request.RequestFormat = DataFormat.Json;

			using (MD5 md5Hash = MD5.Create())
			{
				string body = CreateBoundaryBody(image);
				_hexDigest = CreateMD5HashString(md5Hash, Encoding.UTF8.GetBytes(body));
				Console.Write($"md5 hash: {_hexDigest}\n");
			}
			request.Method = Method.POST;
			request.AddHeader("Date", currentTimeString);
			request.AddHeader("Content-MD5", _hexDigest);
			request.AddHeader("Content-Type", _contentType);

			var authorizationHeader = BuildAuthorizationHeader(request);
			Console.Write($"Authorization Header: {authorizationHeader}\n");

			request.AddHeader("Authorization", authorizationHeader);



			var restResponse = client.Execute(request);

			return restResponse.Content;
		}

		string BuildAuthorizationHeader(RestRequest request)
		{
			StringBuilder headerValue = new StringBuilder($"VWS {_accessKey}:");

			string signature = new SignatureBuilder().GetSignature(request, _secretKey, _hexDigest);

			headerValue.Append(signature);
			return headerValue.ToString();
		}

		string GetCurrentUTCTimeString()
		{
			return DateTime.UtcNow.ToString("R");
		}

		public string CreateMD5HashString(MD5 md5Hash, byte[] data)
		{
			byte[] hash = md5Hash.ComputeHash(data);
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString().ToLowerInvariant();
			//return Convert.ToBase64String(hash);
		}

		string CreateBoundaryBody(byte[] data)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(_boundary);
			sb.AppendLine("Content-Disposition: form-data; name=\"image\"; filename=\"Y1159546.jpg\"\nContent-Type: image/jpeg");
			sb.Append("\n");
			sb.AppendLine(Encoding.UTF8.GetString(data));
			sb.AppendLine(_boundary + "--");

			return sb.ToString();
		}
	}
}
