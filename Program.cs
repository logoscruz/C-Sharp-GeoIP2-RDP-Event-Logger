using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using MaxMind.GeoIP2;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace GeoLogger
{
    [SupportedOSPlatform("windows")] // Ensures the program is recognized as Windows-specific
    class Program
    {
        private static long lastProcessedEventId = 0;

        static async Task Main(string[] args)
        {
            string dbPath = @"C:\<REPLACE_WITH_DATABASE_PATH>"; // Path to MaxMind database
            string logFilePath = @"C:\<REPLACE_WITH_OUTPUT_LOCATION>"; // Path to output log file

            try
            {
                using (var reader = new DatabaseReader(dbPath))
                {
                    while (true)
                    {
                        // Query failed RDP login attempts (EventID 4625)
                        string query = "*[System[Provider[@Name='Microsoft-Windows-Security-Auditing'] and (EventID=4625)]]";
                        var eventLogQuery = new EventLogQuery("Security", PathType.LogName, query);
                        var eventLogReader = new EventLogReader(eventLogQuery);

                        EventRecord eventRecord;
                        var eventDetails = new Dictionary<string, DateTime>();

                        while ((eventRecord = eventLogReader.ReadEvent()) != null)
                        {
                            // Skip already processed events
                            if ((eventRecord.RecordId ?? 0) <= lastProcessedEventId) continue;

                            var message = eventRecord.FormatDescription();
                            var ipAddress = ExtractIpAddress(message);
                            var eventTime = eventRecord.TimeCreated ?? DateTime.Now; // Fallback to current time if null

                            // If we have a valid IP address, store it for GeoIP lookup
                            if (!string.IsNullOrEmpty(ipAddress))
                            {
                                eventDetails[ipAddress] = eventTime;
                            }

                            // Update last processed ID
                            lastProcessedEventId = eventRecord.RecordId ?? lastProcessedEventId;
                        }

                        // Write new data to the log file
                        using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                        {
                            foreach (var entry in eventDetails)
                            {
                                var ip = entry.Key;
                                var attemptTime = entry.Value; // The actual time of the RDP failure

                                try
                                {
                                    var response = reader.Country(ip);
                                    string logEntry = $"{attemptTime}, {response.Country.Name}, {ip}";
                                    await writer.WriteLineAsync(logEntry);
                                    Console.WriteLine($"Logged: {logEntry}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Could not look up IP {ip}: {ex.Message}");
                                }
                            }
                        }

                        // Wait 5 minutes before checking for new events
                        await Task.Delay(TimeSpan.FromMinutes(5));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static string ExtractIpAddress(string message)
        {
            if (message == null) return string.Empty; // Return empty string to avoid null issues

            var match = Regex.Match(message, @"(?:\d{1,3}\.){3}\d{1,3}");
            return match.Success ? match.Value : string.Empty; // Return empty string instead of null
        }
    }
}

