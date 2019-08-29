using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAsyncShellConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            // read input
            var fileIn = args[0];
            var allLines = File.ReadAllLines(fileIn);
            System.Threading.Thread.Sleep(1000);

            // write to output
            var fileOut = args[1];
            File.WriteAllLines(fileOut, allLines.Reverse().ToArray());
            System.Threading.Thread.Sleep(1000);

            return 0;
        }
    }
}
