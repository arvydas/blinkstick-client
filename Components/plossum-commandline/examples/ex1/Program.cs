using System;
using Plossum.CommandLine;
using Plossum;

namespace Ex1
{
    [CommandLineManager(ApplicationName = "Hello World", Copyright = "Copyright (c) Peter Palotas")]
    class Options
    {

        [CommandLineOption(Description = "Displays this help text")]
        public bool Help = false;

        [CommandLineOption(Description = "Specifies the input file", MinOccurs = 1)]
        public string Name
        {
            get { return mName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new InvalidOptionValueException("The name must not be empty", false);
                mName = value;
            }
        }

        private string mName;
    }

    class Program
    {
        static int Main(string[] args)
        {
            Options options = new Options();
            CommandLineParser parser = new CommandLineParser(options);
            parser.Parse();

            Console.WriteLine(parser.UsageInfo.GetHeaderAsString(78));
            if (options.Help)
            {
                Console.WriteLine(parser.UsageInfo.GetOptionsAsString(78));
                return 0;
            }
            else if (parser.HasErrors)
            {
                Console.WriteLine(parser.UsageInfo.GetErrorsAsString(78));
                return -1;
            }
            Console.WriteLine("Hello {0}!", options.Name);

            return 0;
        }
    }
}
