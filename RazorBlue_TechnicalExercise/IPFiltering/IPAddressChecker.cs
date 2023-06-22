using System;
using System.Linq;
using System.Net;

public class IPRangeChecker
{
    private readonly string[] ipRanges; // Assuming this is the stored list of IP ranges

    public IPRangeChecker()
    {
        // Initialize the list of IP ranges
        ipRanges = new string[]
        {
            "10.0.0.0/16",
            "172.16.0.0/12",
            "10.168.1.1",
            "198.51.100.0 - 198.51.100.100",
            "192.168.1.0/24"
        };
    }

    public bool IsAddressInRanges(string ipAddress)
    {
        // Convert the provided IP address to an IPAddress object
        if (!IPAddress.TryParse(ipAddress, out IPAddress address))
        {
            throw new ArgumentException("Invalid IP address format.");
        }

        // Check if the provided IP address is within any of the stored IP ranges
        foreach (var range in ipRanges)
        {
            if (IsAddressInRange(address, range))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAddressInRange(IPAddress address, string range)
    {
        // Parse the IP range or CIDR notation
        if (range.Contains("/"))
        {
            string[] parts = range.Split('/');

            int IP_addr = BitConverter.ToInt32(IPAddress.Parse(address.ToString()).GetAddressBytes(), 0);
            int CIDR_addr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int CIDR_mask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((IP_addr & CIDR_mask) == (CIDR_addr & CIDR_mask));


        }
        else if (range.Contains("-"))
        {
            // Range of IP addresses
            var startAddress = range.Split('-')[0].Trim();
            var endAddress = range.Split('-')[1].Trim();

            long ipStart = BitConverter.ToInt32(IPAddress.Parse(startAddress).GetAddressBytes().Reverse().ToArray(), 0);

            long ipEnd = BitConverter.ToInt32(IPAddress.Parse(endAddress).GetAddressBytes().Reverse().ToArray(), 0);

            long ip = BitConverter.ToInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

            return ip >= ipStart && ip <= ipEnd;


        }
        else
        {
            // Single IP address
            var target = IPAddress.Parse(range);
            return address.Equals(target);
        }
    }

    static void Main(string[] args)
    {

        IPRangeChecker checker = new IPRangeChecker();

        bool ans = checker.IsAddressInRanges("10.168.1.1");
        Console.WriteLine($"Is in the range? {ans}");
        Console.ReadLine();
    }
}