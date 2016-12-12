using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using RestSharp;
using System.Linq;

namespace Brady.ImageRecognition
{
	public class SignatureBuilder
	{
		string _method = "POST";
		string _contentType = "multipart/form-data";
		string _dateValue = "";
		public string GetSignature(RestRequest request, string secretKey, string hexDigest)
		{
			var dateParameter = request.Parameters.First(p => p.Name == "Date");
			_dateValue = dateParameter.Value.ToString();
			_contentType = request.Parameters.First(p => p.Name == "Content-Type").Value.ToString();
			string digestString = $"{_method}\n{hexDigest}\n{_contentType}\n{_dateValue}\n/{request.Resource}";

			byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

			return CreateRFC2104HMAC(digestString, secretKeyBytes);
		}

		string CreateRFC2104HMAC(string input, byte[] key)
		{
			byte[] byteArray = Encoding.UTF8.GetBytes(input);
			using (HMACSHA1 myhmacsha1 = new HMACSHA1(key))
			{
				MemoryStream stream = new MemoryStream(byteArray);
				byte[] hmacBytes = myhmacsha1.ComputeHash(stream);

				return Convert.ToBase64String(hmacBytes);
			}
		}
	}
}