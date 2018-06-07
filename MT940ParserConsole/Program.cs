using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using programmersdigest.MT940Parser;

namespace MT940ParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                string filename = args[1];
                string encoding = args[2];
                List<Statement> li = new List<Statement>();
                int i = 0;
                using (var parser = new Parser(filename, encoding))
                {
                    foreach (Statement statement in parser.Parse())
                    {
                        li.Add(statement);
                        i += statement.Lines.Count;
                    }
                }
                Console.WriteLine(string.Format("{0} operation info fields found, {1} transaction blocks found", i, li.Count));
            }
            else
            {
                Console.WriteLine("Usage: MT940ParserConsole.exe \"path_to_file\" \"encoding_name\"");
            }
        }
    }
}
