using System;
using System.IO;
using System.Text;

namespace FilterRegisteredDomains
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "domain_results.txt";
            string outputFile = "output.txt";

            try
            {
                using (StreamReader reader = new StreamReader(inputFile))
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    StringBuilder entry = new StringBuilder();

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            entry.AppendLine(line);

                            if (line.StartsWith("Status: registered"))
                            {
                                entry.Clear();
                            }
                        }
                        else if (entry.Length > 0)
                        {
                            // Write the entry to the output file if it's not registered
                            if (!entry.ToString().Contains("Status: registered"))
                            {
                                writer.Write(entry.ToString());
                                writer.WriteLine();
                            }

                            entry.Clear();
                        }
                    }

                    // Write the last entry if it's not registered
                    if (!entry.ToString().Contains("Status: registered"))
                    {
                        writer.Write(entry.ToString());
                    }
                }

                Console.WriteLine("Filtered output file created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
