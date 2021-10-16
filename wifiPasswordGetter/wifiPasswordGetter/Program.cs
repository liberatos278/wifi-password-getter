using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace wifiPasswordGetter
{
    internal class WifiNetwork
    {
        private static void Main(string[] args)
        {
            GetWifiPasswords();
        }

        private static string GetWifiNetworks()
        {
            var processWifi = new Process();
            processWifi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processWifi.StartInfo.FileName = "netsh";
            processWifi.StartInfo.Arguments = "wlan show profile";

            processWifi.StartInfo.UseShellExecute = false;
            processWifi.StartInfo.RedirectStandardError = true;
            processWifi.StartInfo.RedirectStandardInput = true;
            processWifi.StartInfo.RedirectStandardOutput = true;
            processWifi.StartInfo.CreateNoWindow = true;
            processWifi.Start();

            var output = processWifi.StandardOutput.ReadToEnd();

            processWifi.WaitForExit();
            return output;
        }

        private static string ReadWifiPassword(string wifiName)
        {
            var argument = "wlan show profile name=\"" + wifiName + "\" key=clear";
            var processWifi = new Process();
            processWifi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processWifi.StartInfo.FileName = "netsh";
            processWifi.StartInfo.Arguments = argument;


            processWifi.StartInfo.UseShellExecute = false;
            processWifi.StartInfo.RedirectStandardError = true;
            processWifi.StartInfo.RedirectStandardInput = true;
            processWifi.StartInfo.RedirectStandardOutput = true;
            processWifi.StartInfo.CreateNoWindow = true;
            processWifi.Start();

            var output = processWifi.StandardOutput.ReadToEnd();

            processWifi.WaitForExit();
            return output;
        }

        private static string GetWifiPassword(string wifiName)
        {
            var getPassword = ReadWifiPassword(wifiName);
            using (var reader = new StringReader(getPassword))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var regex2 = new Regex(@"Key Content * : (?<after>.*)");
                    var match2 = regex2.Match(line);

                    if (match2.Success)
                    {
                        var currentPassword = match2.Groups["after"].Value;
                        return currentPassword;
                    }
                }
            }

            return "Open Network - NO PASSWORD";
        }

        private static void GetWifiPasswords()
        {
            Console.WriteLine("--------------------------------------------------------");
            var wifiNetworks = GetWifiNetworks();
            using (var reader = new StringReader(wifiNetworks))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var regex1 = new Regex(@"All User Profile * : (?<after>.*)");
                    var match1 = regex1.Match(line);

                    if (match1.Success)
                    {
                        var wifiName = match1.Groups["after"].Value;
                        var wifiPassword = GetWifiPassword(wifiName);

                        var output = String.Format("{0,10}: {1,10}", wifiName, wifiPassword);
                        Console.WriteLine(output);
                    }
                }

                Console.WriteLine("--------------------------------------------------------");
                Console.ReadLine();
            }
        }
    }
}