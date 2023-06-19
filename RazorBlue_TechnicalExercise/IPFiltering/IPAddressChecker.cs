using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;

public class IPAddressChecker
{
    static void Main(string[] args)
    {
        string ipAddress = "192.168.1.00";
        List<string> ipRanges = new List<string>()
        {
            "192.168.1.1",
            "192.168.1.0/24",
            "10.0.0.0/8",
            "172.16.0.0/12"
        };

        bool isInRange = IPAddressChecker.IsIPInRange(ipAddress, ipRanges);

        if (isInRange)
        {
            Console.WriteLine("The IP address is within the specified ranges.");
        }
        else
        {
            Console.WriteLine("The IP address is not within the specified ranges.");
        }

        Console.ReadLine();

    }

    public static bool IsIPInRange(string ipAddress, List<string> ipRanges)
    {
        IPAddress address;
        if (!IPAddress.TryParse(ipAddress, out address))
        {
            Console.WriteLine("Invalid IP address format.");
            return false;
        }

        foreach (string range in ipRanges)
        {
            string[] rangeParts = range.Split('/');
            string rangeStart = rangeParts[0];
            int prefixLength = 32;

            if (rangeParts.Length == 2 && !int.TryParse(rangeParts[1], out prefixLength))
            {
                Console.WriteLine("Invalid CIDR format.");
                continue;
            }

            IPAddress startAddress;
            if (!IPAddress.TryParse(rangeStart, out startAddress))
            {
                Console.WriteLine("Invalid IP range start address format.");
                continue;
            }

            if (address.AddressFamily != startAddress.AddressFamily)
            {
                Console.WriteLine("IP address and range start address have different address families.");
                continue;
            }

            uint ipAddressValue = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint startAddressValue = BitConverter.ToUInt32(startAddress.GetAddressBytes(), 0);

            if (prefixLength == 32 && ipAddressValue == startAddressValue)
            {
                return true; // Single IP address match
            }

            uint subnetMask = prefixLength == 32 ? uint.MaxValue : ~(uint.MaxValue >> prefixLength);

            if ((ipAddressValue & subnetMask) == (startAddressValue & subnetMask))
            {
                return true; // IP address is within the range or CIDR range
            }
        }

        return false; // No match found
    }

    //Using SQL database data
    public bool IsIPAddressInRangeSQL(string ipAddress)
    {
        IPAddress targetAddress;
        if (!IPAddress.TryParse(ipAddress, out targetAddress))
        {
            throw new ArgumentException("Invalid IP address format.", nameof(ipAddress));
        }

        using (SqlConnection connection = new SqlConnection("connection string"))
        {
            connection.Open();

            string query = "SELECT StartIP, EndIP FROM IPRangeTable";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                IPAddress startIP = IPAddress.Parse(reader["StartIP"].ToString());
                IPAddress endIP = IPAddress.Parse(reader["EndIP"].ToString());

                if (IsIPAddressInRangeSQL(targetAddress, startIP, endIP))
                {
                    reader.Close();
                    return true;
                }
            }

            reader.Close();
        }

        return false;
    }
    private bool IsIPAddressInRangeSQL(IPAddress targetAddress, IPAddress startIP, IPAddress endIP)
    {
        byte[] targetBytes = targetAddress.GetAddressBytes();
        byte[] startBytes = startIP.GetAddressBytes();
        byte[] endBytes = endIP.GetAddressBytes();

        bool isInRange = true;

        for (int i = 0; i < targetBytes.Length; i++)
        {
            if (targetBytes[i] < startBytes[i] || targetBytes[i] > endBytes[i])
            {
                isInRange = false;
                break;
            }
        }

        return isInRange;
    }


}
