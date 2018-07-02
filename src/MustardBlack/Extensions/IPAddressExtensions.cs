using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Serilog;

namespace UD.Core
{
	public static class IPAddressExtensions
	{
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
		public static uint IPv4ToUInt(string ipAddress)
		{
			// shift counter
			var shift = 3;

			try
			{
				// loop through the octets and compute the decimal version
				var octets = ipAddress.Split('.').Select(UInt32.Parse);
				return octets.Aggregate((uint)0, (total, octet) => (total + (octet << (shift-- * 8))));
			}
			catch (FormatException)
			{
				return 0;
			}
		}

		public static uint IPv4ToUInt(this IPAddress ipAddress)
		{
			if(ipAddress.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException("Can only process IPv4 addresses, this is an `" + ipAddress.AddressFamily + "` address");

			return IPv4ToUInt(ipAddress.ToString());
		}

		public static IPAddress UIntToIpAddress(uint numericIpAddress)
		{
			if (numericIpAddress <= 0)
			{
				log.Warning("uint ipaddress is not valid");
				return IPAddress.Parse("0.0.0.0");
			}

			numericIpAddress = SwapOctetsUInt32(numericIpAddress);

			var ipAddress = new IPAddress(numericIpAddress);

			return ipAddress;
		}


		public static Tuple<uint, uint> CIDRToRange(string cidr)
		{
			var parts = cidr.Split('.', '/');

			var ipnum = (Convert.ToUInt32(parts[0]) << 24) |
			             (Convert.ToUInt32(parts[1]) << 16) |
			             (Convert.ToUInt32(parts[2]) << 8) |
			             Convert.ToUInt32(parts[3]);

			var maskbits = Convert.ToInt32(parts[4]);
			var mask = 0xffffffff;
			mask <<= 32 - maskbits;

			var ipstart = ipnum & mask;
			var ipend = ipnum | (mask ^ 0xffffffff);

			return new Tuple<uint, uint>(ipstart, ipend);
		}

		static uint SwapOctetsUInt32(uint toSwap)
		{
			uint tmp = 0;
			tmp = toSwap >> 24;
			tmp = tmp | ((toSwap & 0xff0000) >> 8);
			tmp = tmp | ((toSwap & 0xff00) << 8);
			tmp = tmp | ((toSwap & 0xff) << 24);
			return tmp;
		}

		// http://www.myregexp.com/examples.html
		public const string RegexValidationPattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

		public const string RegexCidrValidationPattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\/([0-9]|[1-2][0-9]|3[0-2]))$";
	}
}
