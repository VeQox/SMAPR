using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    class Progressbar
    {
        public enum Backup
        {
            Successfull,
            Failed
        }

        private readonly int Length = Console.BufferWidth;

        public long Total { get; set; }
        public long Progress { get; set; }
        private Files Files { get; set; }


        public Progressbar(long total)
        {
            Files = new();
            Total = total;
        }

        public void Update(FileInfo file, Backup status)
        {
            PrintProgressBar();
            PrintFileInfo(file);
            UpdateFiles(status);
        }

        private void UpdateFiles(Backup status)
        {
            if (status == Backup.Successfull)
                Files.Successful++;
            else if (status == Backup.Failed)
                Files.Failed++;
        }


        private void PrintProgressBar()
        {
            // Amount of # in the ProgressBar
            double mask = (double)Progress / Total * Length;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");

            for (int i = 0; i < Length; i++)
            {
                if (i < mask)
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

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("]");
        }

        private static void PrintFileInfo(FileInfo file)
        {
            Console.Write($"\nName: {file.Name}");
            Console.Write($"\nPath: {file.FullName}");
            Console.Write($"\nSize: {GetFileSize(file)}");
        }


        private static string GetFileSize(FileInfo file)
        {
            string[] FileSizes =
            {
                "B",
                "KB",
                "MB",
                "GB",
                "TB",
            };

            long size = file.Length;

            for (int i = 1; i < size || i < FileSizes.Length; i = 1024 ^ i)
            {
                if(size / i < 1024)
                {
                    return $"{size / i} {FileSizes[i]}";
                }
            }

            return size.ToString();
        }

        public void Finish()
        {
            PrintProgressBar();

            Console.Write("\nBackup Finished");
            Console.Write($"\n\n{Files.Successful} Files backuped");
            Console.Write($"\n{Files.Failed} Files failed");
        }
    }
}