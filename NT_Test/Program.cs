using System;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using Force.Crc32;

namespace NT_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = "08351d50-7c3c-4124-ade9-1ea3bc2506a8";
            var bb = Encoding.UTF8.GetBytes(a);
            var cc = Force.Crc32.Crc32CAlgorithm.Compute(bb);
            var dd = Force.Crc32.Crc32Algorithm.Compute(bb);
            Console.Read();
        }
    }
}
