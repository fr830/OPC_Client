using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace OpcClientLibrary
{
    public static class NetworkBrowser
    {
        public static async Task<List<string>> GetComputersOnNetworkAsync()
        {
            List<string> _ret = new List<string>();

            Process netUtility = new Process();
            netUtility.StartInfo.FileName = "arp.exe";
            netUtility.StartInfo.CreateNoWindow = true;
            netUtility.StartInfo.Arguments = "-a";
            netUtility.StartInfo.RedirectStandardOutput = true;
            netUtility.StartInfo.UseShellExecute = false;
            netUtility.StartInfo.RedirectStandardError = true;
            netUtility.Start();

            StreamReader streamReader = new StreamReader(netUtility.StandardOutput.BaseStream, netUtility.StandardOutput.CurrentEncoding);

            string line = "";
            while ((line = streamReader.ReadLine()) != null)
            {

                if (line.StartsWith("  "))
                {
                    var Itms = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (Itms.Length == 3)
                    {
                        try
                        {
                            var a = await Dns.GetHostEntryAsync(Itms[0]);
                            if (a != null && a.HostName.Length > 0)
                                _ret.Add(a.HostName);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            streamReader.Close();
            return _ret;
        }        
    }
}