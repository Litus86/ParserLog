using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ParserLog
{
    static class  Program
    {
        static void Main(string[] args)
        {

            var outputLog = new List<Logs>();

           // var startTimeSpan = TimeSpan.Zero;
           // var periodTimeSpan = TimeSpan.FromMinutes(5);      

            // var timer = new System.Threading.Timer((e) =>
            //  {
            string path = ConfigurationManager.AppSettings["input"];
            string outputPath = ConfigurationManager.AppSettings["output"];
            
            Console.WriteLine(ConfigurationManager.AppSettings.Count < 2 ? "Check app.config" : "");

            Console.WriteLine(File.Exists(path) ? "Input file exists." : "Input file doesn't exist. Check key 'input' in app.config");

            if(File.Exists(path)){
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var log = line.Split(new char[] { ' ' });

                        if (log[3].Equals("ERROR:"))
                        {
                            outputLog.Add(new Logs { dateTime = log[0] + " " + log[1], sessionCode = log[2], message = string.Join(" ", log, 3, log.Length - 3) });
                        }
                    }
                    sr.Close();
                    Console.WriteLine(sr.BaseStream == null ? "Input is close." : "Input file doesn't close");
                    System.Console.WriteLine("Log was read");
                }
                outputLog.ExportToFile(outputPath, ' ');

                //}, null, startTimeSpan, periodTimeSpan);
                Console.WriteLine(File.Exists(outputPath) ? "Output file exists,logfile with only error is write." : "Output file does not exist, somthing wrong");
                
            }

            System.Console.WriteLine("Enter any key ");
            System.Console.ReadKey();
        }

      
        static void ExportToFile<T>(this IEnumerable<T> data, string FileName, char ColumnSeperator)
        {
            using (var sw = File.CreateText(FileName))
            {
                var plist = typeof(T).GetProperties().Where(p => p.CanRead && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)) && p.GetIndexParameters().Length == 0).ToList();
                if (plist.Count > 0)
                {
                    var seperator = ColumnSeperator.ToString();
                    sw.WriteLine(string.Join(seperator, plist.Select(p => p.Name)));
                    foreach (var item in data)
                    {
                        var values = new List<object>();
                        foreach (var p in plist) values.Add(p.GetValue(item, null));
                        sw.WriteLine(string.Join(seperator, values));
                    }
                    sw.Close();
                    Console.WriteLine(sw.BaseStream == null ? "Output is close." : "Output file doesn't close");
                }
            }
        }
      
    }
   

    [Serializable()]
    public class Logs {
        public string dateTime { get; set; }
        public string sessionCode { get; set; }
        public string message { get; set; }
    }
}
