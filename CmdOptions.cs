using System;
using System.Collections.Generic;
using System.Text;

namespace APTester
{
    using CommandLine;

    // Commandline options
    public class CmdOptions
    {
        // Ap information
        [Option("ap-job-name", Default = "TestAPRunnerWorkflow", HelpText = "Name/Tag for AP job (only support letters and numbers in the name).")]
        public string JobName { get; set; }

        [Option('e', "ap-environment", HelpText = "AP Installation. Analog or Delaware.")]
        public ApEnvironment Environment { get; set; }

        [Option('c', "ap-cluster", HelpText = "Name of AP cluster.")]
        public string Cluster { get; set; }

        // others
        [Option('d', "debug", HelpText = "Debug the experiment job locally.")]
        public bool Debug { get; set; }

        [Option('w', "ap-wait-job", HelpText = "Wait for experiment job complete.")]
        public bool Wait { get; set; }

        [Option('i', "csv-path", HelpText = "csv path.")]
        public string csvPath { get; set; }
    }
}
