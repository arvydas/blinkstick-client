using System;
using System.Text;
using System.Collections.Generic;
using Plossum.CommandLine;

namespace ex2
{
    [CommandLineManager(ApplicationName="Example 2", Copyright="Copyright (C) Peter Palotas 2007",
        EnabledOptionStyles=OptionStyles.Group | OptionStyles.LongUnix)]
    [CommandLineOptionGroup("commands", Name="Commands", Require=OptionGroupRequirement.ExactlyOne)] 
    [CommandLineOptionGroup("options", Name="Options")]
    class Options
    {
        [CommandLineOption(Name="filter", RequireExplicitAssignment=true, 
            Description="Specifies a filter on which files to include or exclude", GroupId="options")]
        public List<string> Filters
        {
            get { return mFilters; }
            set { mFilters = value; }
        }
	
        [CommandLineOption(Name="v", Aliases="verbose", Description="Produce verbose output", GroupId="options")]
        public bool Verbose
        {
            get { return mVerbose; }
            set { mVerbose = value; }
        }
	
        [CommandLineOption(Name="z", Aliases="use-compression", Description="Compress or decompress the archive", 
            GroupId="options")]
        public bool UseCompression
        {
            get { return mUseCompression; }
            set { mUseCompression = value; }
        }

	    [CommandLineOption(Name="c", Aliases="create", 
            Description="Create a new archive", GroupId="commands")]
        public bool Create
        {
            get { return mCreate; }
            set { mCreate = value; }
        }

        [CommandLineOption(Name = "x", Aliases = "extract", 
            Description = "Extract files from archive", GroupId = "commands")]
        public bool Extract
	    {
		    get { return mExtract;}
		    set { mExtract = value;}
	    }
	
        [CommandLineOption(Name="f", Aliases="file", Description="Specify the file name of the archive",
            MinOccurs=1)]
	    public string FileName
	    {
		    get { return mFileName;}
		    set { mFileName = value;}
	    }

        [CommandLineOption(Name="h", Aliases="help", Description="Shows this help text", GroupId="commands")]
        public bool Help
        {
            get { return mHelp; }
            set { mHelp = value; }
        }

        private bool mHelp;
        private List<string> mFilters = new List<string>();
        private bool mCreate;
        private bool mExtract;
        private string mFileName;
        private bool mUseCompression;
        private bool mVerbose;
    }

    class Program
    {
        static int Main(string[] args)
        {
            Options options = new Options();
            CommandLineParser parser = new CommandLineParser(options);
            parser.Parse();

            if (options.Help)
            {
                Console.WriteLine(parser.UsageInfo.ToString(78, false));
                return 0;
            }
            else if (parser.HasErrors)
            {
                Console.WriteLine(parser.UsageInfo.ToString(78, true));
                return -1;
            }

            // No errors present and all arguments correct 
            // Do work according to arguments   
            return 0;         
        }
    }
}
