
using System;
using System.Collections.Generic;
using System.Text;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            mxGraph.vsdx.utils.vsdxBatchConvert.execute(@"D:\1.vsdx");
            Console.Read();

        }
    }
}
