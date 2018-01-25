using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace replacestring
{
    class Program
    {
        /**
            Quick script to look through big ass sql file (7MB) and do some find/replaces
            Technically doesn't have to be sql since args[0] provides
            the path to the source file. But that's what I made it for.
            
            How I Build it
            --------------
            clear;dotnet publish -r osx-x64;clear;time ./bin/Debug/netcoreapp2.0/osx-x64/replacestring ./ProductionBackup.sql ./LocalDB.sql

         */
        static void Main(string[] args)
        {
            // Get Command Line Arguments
            if ( args.Length < 2 ) {
                Console.WriteLine("Syntax:  COMMAND SrcFile DestFile");
                return;
            }
            string srcfile = args[0];
            string destfile = args[1];
            
            // Read source file
            StringBuilder b = ReadFile(srcfile);

            // Read and Parse the Matches
            // matches.json in same directory as executable. I don't plan
            // on making a new one each time. So I keep it there.
            string exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string matchsfile = Path.Combine(exeDirectory, "matches.json");
            List<Pair> matches = LoadMatches(matchsfile);
            
            // Find and Replace
            foreach (Pair p in matches)
            {
                Console.WriteLine("Checking For: " + p.Before);
                b.Replace(p.Before, p.After);
            }

            // Write content to disk
            WriteFile(b.ToString(), destfile);
        }


        /**
         *  
         */
        static StringBuilder ReadFile( string filepath ) {
            // DB File is usually 7MB - 32k lines. I checked sb.capacity
            // and I'm giving it a little more than it needed.
            StringBuilder b = new StringBuilder(8000000);
            using (StreamReader reader = new StreamReader(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    b.AppendLine(line);
                }
            }
            return b;
        }


        /**
         *  
         */
        static void WriteFile( string body, string filepath ) {
            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(body);
            }
        }
        
        /**
         *  
         */
        static List<Pair> LoadMatches(string filepath)
        {
            StringBuilder b = ReadFile(filepath);
            List<Pair> matches = JsonConvert.DeserializeObject<List<Pair>>(b.ToString());
            return matches;
        }


    }

    class Pair {
        public string Before { get; set; }
        public string After { get; set; }
    }

}
