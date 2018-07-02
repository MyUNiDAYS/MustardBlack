using System;
using System.Collections.Specialized;
using System.Web;

// ReSharper disable CheckNamespace
namespace UD.Core.Extensions
{
	public static class HttpRequestBaseExtensions
	{
		public static string UserIpAddress(this HttpRequestBase request)
		{
			if (request == null)
				return null;

			return GetIpAddress(request.ServerVariables, request.UserHostAddress);
		}

		public static string UserIpAddress(this HttpRequest request)
		{
			if (request == null)
				return null;

			//return "213.7.140.67"; // China
			//return "82.152.34.179"; // UK
			return GetIpAddress(request.ServerVariables, request.UserHostAddress);
		}

		public static uint UserIp(this HttpRequestBase request)
		{
			if (request == null)
				return 0;

			// test with: http://www.countryipblocks.net/tools/ip-octet-binary-and-decimal-calculators/
			var ipAddress = request.UserIpAddress();

			return IPAddressExtensions.IPv4ToUInt(ipAddress);
		}
		
		public static uint UserIp(this HttpRequest request)
		{
			if (request == null)
				return 0;

			// test with: http://www.countryipblocks.net/tools/ip-octet-binary-and-decimal-calculators/
			var ipAddress = request.UserIpAddress();

			return IPAddressExtensions.IPv4ToUInt(ipAddress);
		}

		static string GetIpAddress(NameValueCollection serverVariables, string userHostAddress)
		{
			string ip;
			try
			{
				ip = serverVariables["HTTP_X_FORWARDED_FOR"];
				if (!string.IsNullOrEmpty(ip))
				{
					if (ip.IndexOf(",", StringComparison.Ordinal) > 0)
					{
						var ipRange = ip.Split(',');
						ip = ipRange[0].Trim();
					}

					if (ip.ToUpperInvariant() == "UNKNOWN")
						ip = userHostAddress;
				}
				else
				{
					ip = userHostAddress;
				}
			}
			catch
			{
				ip = null;
			}

			return ip;
		}
	}
}