using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

using Charon.Cluster;
using Charon.Common;
using Charon.Dataflow;
using CommandLine;
using Newtonsoft.Json.Linq;
using Charon.Workflow.Client.Nui;
using Charon.Workflow.Client;

using CsvHelper;
using LumenWorks.Framework.IO.Csv;


namespace APTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdOptions = Parser.Default.ParseArguments<CmdOptions>(args);
            cmdOptions.WithParsed(
                options =>
                {
                    if (options == null)
                    {
                        throw new ArgumentNullException(nameof(options));
                    }

                    List<CsvTags> clips = ParseCSV(options.csvPath);

                    var wfClient = WorkflowClient.Default;

                    InsertData(wfClient, clips).Wait();
                });
        }

        public class CsvTags
        {
            public string UseCase { get; set; }
            public string FullName { get; set; }
            public string Uri { get; set; }
            public string CsObjectName { get; set; }
            public string ObjectType { get; set; }
            public string ClipType { get; set; }
        }

        public static List<CsvTags> ParseCSV(string path)
        {
            List<CsvTags> res = new List<CsvTags>();

            using (var csv = new CachedCsvReader(new StreamReader(path), true))
            {
                while (csv.ReadNextRecord())
                {
                    res.Add(
                        new CsvTags
                        {
                            UseCase = csv["UseCase"],
                            FullName = csv["FullName"],
                            Uri = csv["File101.Uri"],
                            CsObjectName = csv["CsObjectName"],
                            ObjectType = csv["ObjectType"],
                            ClipType = csv["ClipType"],
                        });
                }
            }

            return res;
        }

        static async Task InsertData(WorkflowClient wfClient, List<CsvTags> clips)
        {
            var storage = await wfClient.GetFileStorageAsync();

            foreach (var c in clips)
            {
                var clip = await wfClient.CreateOrGetObjectAsync<Clip>(c.CsObjectName);

                clip.SetTagValue("UseCase", c.UseCase);
                clip.SetTagValue("FullName", c.FullName);
                clip.SetTagValue("File101Uri", c.Uri); // note: as June 2021, we can't do "File101.Uri", it will run-time crash
                clip.SetTagValue("CsObjectName", c.CsObjectName);
                clip.SetTagValue("ObjectType", c.ObjectType);
                clip.SetTagValue("ClipType", c.ClipType);

                clip.SetFile(new WorkflowFile(c.Uri, 101));

                storage.PlaceFilesAndStoreObject(clip, deleteFileFromSourceLocation: false, retainFileName: false);
            }
        }
    }
}
