using System;
using Xunit.Abstractions;

namespace SqlBulkCopyMerge.Tests.Docker
{
    public class XUnitLogger : ILogger
    {
        private readonly ITestOutputHelper _output;
        public XUnitLogger(ITestOutputHelper output)
        {
            _output = output;
        }
        
        public void Verbose(string message, params object[] args)
        {
            _output.WriteLine(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            _output.WriteLine(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            _output.WriteLine(message, args);
        }

        public void Warning(Exception ex, string message, params object[] args)
        {
            _output.WriteLine(message, args);
            _output.WriteLine(ex.ToString());
        }

        public void Error(string message, params object[] args)
        {
            _output.WriteLine(message, args);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            _output.WriteLine(message, args);
            _output.WriteLine(ex.ToString());
        }
    }
}
