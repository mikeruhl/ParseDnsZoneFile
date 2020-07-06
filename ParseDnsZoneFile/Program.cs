using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ParseDnsZoneFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Need to supply path");
                Environment.Exit(1);
            }

            var path = args[0];
            if (!File.Exists(path))
            {
                Console.WriteLine("Supplied file does not exist");
                Environment.Exit(1);
            }

            var lines = File.ReadAllLines(path);

            var inputFile = new FileInfo(path);
            var outputFile = $"{inputFile.FullName.Substring(0, inputFile.FullName.Length - inputFile.Extension.Length)}-list.csv";
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (var sw = File.AppendText(outputFile))
            {
                sw.WriteLine("name,type,ttl,value");
            }

            string currentDomain = null;
            var hashes = new Dictionary<string, List<string>>();

            foreach (var l in lines)
            {
                if (string.IsNullOrEmpty(l) || l.StartsWith(";"))
                {
                    continue;
                }

                if ("abcdefghijklmnopqrstuvwxyz@".Contains(l[0]))
                {
                    currentDomain = l.Substring(0, l.IndexOf(' '));
                }
                else if (!l.StartsWith("\t") || string.IsNullOrEmpty(currentDomain)) //continuation of previous line
                {
                    continue;
                }

                var splits = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string ttl = null;
                string recordType = null;
                string record = null;

                var i = 0;
                if (splits.Length >= 5)
                {
                    i = 1;
                }

                if (splits[i + 1] != "IN")
                {
                    Console.WriteLine("what's up with this: " + l);
                    continue;
                }

                ttl = splits[i + 0];
                recordType = splits[i + 2];
                record = splits[i + 3];

                if (recordType != "A" && recordType != "CNAME")
                {
                    continue;
                }

                if (record.StartsWith("\"") && record.EndsWith("\""))
                {
                    record = record.Substring(1, record.Length - 2);
                }
                using (var sw = File.AppendText(outputFile))
                {
                    sw.WriteLine($"\"{currentDomain}\",{ttl},\"{recordType}\", \"{record}\"");
                }
                if (hashes.ContainsKey(record))
                {
                    hashes[record].Add(currentDomain);
                }
                else
                {
                    hashes.Add(record, new List<string>{currentDomain});
                }
                
            }
            var hashFile = $"{inputFile.FullName.Substring(0, inputFile.FullName.Length - inputFile.Extension.Length)}-hashes.csv";
            if (File.Exists(hashFile))
                File.Delete(hashFile);
            using (var sw = File.CreateText(hashFile))
            {
                foreach (var h in hashes)

                {
                    var f = string.Join(",", h.Value.Select(s => s.EndsWith(".") ? s.Substring(0, s.Length - 1) : s));

                    sw.WriteLine($"\"{h.Key}\",\"{f}\"");
                }
            }
        }
    }
}
