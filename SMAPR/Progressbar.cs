﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    class ProgressBar
    {
        private int Top { get; set; }
        private int Length = 50;
        public long Total { get; set; }
        public long Progress { get; set; }

        public List<string> successfullFiles = new();
        public List<string> failedFiles = new();

        public ProgressBar(long total)
        {
            Total = total;
            Progress = 0;
            Top = Console.CursorTop;
        }

        public void Update(string fileName, long fileSize)
        {
            Console.Clear();
            Console.SetCursorPosition(0, Top);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");

            Progress += fileSize;
            double relativeLength = (double)Progress/Total*Length;

            for (int i = 0; i < Length; i++)
            {
                if (i < relativeLength)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("#");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("-");
                }
            }
            Console.ForegroundColor= ConsoleColor.White;
            Console.Write("]");

            Console.Write($" {fileName,10} {Progress / 1024 / 1024}/{Total / 1024 / 1024} MiB");
        }

        public void Finish()
        {
            Console.WriteLine($"{successfullFiles.Count} files moved successfully");
            Console.WriteLine($"{failedFiles.Count} files failed");
        }
    }
}