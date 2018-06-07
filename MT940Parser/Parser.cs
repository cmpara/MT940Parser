using programmersdigest.MT940Parser.Parsing;
using System;
using System.Collections.Generic;
using System.IO;

namespace programmersdigest.MT940Parser
{
    public class Parser : IDisposable
    {
        private StreamReader _reader;
        private StatementParser _statementParser;

        public Parser(string path, string encoding = "utf-8")
        {
            //use specified encoding to read non latin-1 letters
            string text = File.ReadAllText(path, System.Text.Encoding.GetEncoding(encoding));
            //change file end of line to CRLF
            text = text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
            //add missed - before :20: field if any
            text = text.Replace("\r\n:20:", "\r\n-\r\n:20:").Replace("\r\n-\r\n-", "\r\n-");
            //save to original file as utf-8 and than read again
            File.WriteAllText(path, text, System.Text.Encoding.GetEncoding("utf-8"));
            _reader = new StreamReader(path, encoding: System.Text.Encoding.GetEncoding("utf-8"));
            _statementParser = new StatementParser(_reader);
        }
        public Parser(Stream stream)
        {
            _reader = new StreamReader(stream);
            _statementParser = new StatementParser(_reader);
        }

        public IEnumerable<Statement> Parse()
        {
            while (!_reader.EndOfStream)
            {
                var statement = _statementParser.ReadStatement();

                if (statement != null)
                {
                    yield return statement;
                }
            }
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}
