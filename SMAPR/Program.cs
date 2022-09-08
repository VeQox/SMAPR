using System;
using System.Xml.Linq;

namespace SMAPR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.SetBufferSize(Console.BufferWidth, Console.WindowHeight);

            string location = Path.Combine(Directory.GetCurrentDirectory(), "ligma");

            SMAPR.Backup(location);
        }
    }
}