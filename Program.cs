using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DASDomainChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverAddress = "das.domreg.lt";
            int serverPort = 4343;

            char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

			string resultFileName = "result.csv";
			File.Delete(resultFileName);
            int domainsScanned = 0;
            Dictionary<string, int> map = new Dictionary<string, int>();
            List<string> errors = new List<string>();

			foreach (char firstLetter in letters)
            {
                foreach (char secondLetter in letters)
                {
                    domainsScanned++;
					string domain = $"{firstLetter}{secondLetter}.lt";
                    string query = $"get 1.0 {domain}\n";

                    try
                    {
                        using (TcpClient client = new TcpClient(serverAddress, serverPort))
                        {
                            byte[] data = Encoding.ASCII.GetBytes(query);
                            NetworkStream stream = client.GetStream();

                            stream.Write(data, 0, data.Length);

                            byte[] responseBytes = new byte[client.ReceiveBufferSize];
                            int bytesRead = stream.Read(responseBytes, 0, client.ReceiveBufferSize);
                            string response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

							string status = response.Substring(response.IndexOf("Status: ") + "Status: ".Length).Trim();
							File.AppendAllText(resultFileName, $"{domain};{status}\n");

                            if (map.ContainsKey(status)) map[status] = map[status] + 1; else map.Add(status, 1);

							Console.Clear();
                            Console.WriteLine("Scanning two letter .lt domains...");
                            Console.WriteLine($"{domainsScanned}/676 domains scanned ({domain})");
                            Console.WriteLine();
                            Console.WriteLine("Found statuses:");
                            Console.WriteLine(new String('-', 32));
							Console.WriteLine(String.Format("| {0,-20} | {1,-5} |", "Status", "Count"));
							Console.WriteLine(new String('-', 32));
                            foreach (var item in map)
                            {
                                Console.WriteLine(String.Format("| {0,-20} | {1,-5} |", item.Key, item.Value));
                            }
							Console.WriteLine(new String('-', 32));
							Console.WriteLine($"Full output: {resultFileName}");

							if (errors.Count > 0) {
								Console.WriteLine();
								Console.WriteLine($"Failed to query domains: {String.Join(", ", errors)}");
							}
						}
					}
                    catch (Exception ex)
                    {
						File.AppendAllText(resultFileName, $"{domain};unknown\n");
						errors.Append(domain);
					}
                }
            }

			Console.WriteLine();
			Console.WriteLine("Domain scan complete.");
            Console.ReadKey();
        }
    }
}
