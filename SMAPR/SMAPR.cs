using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    static class SMAPR
    {
        private const string NAS = @"\\192.168.0.22\public";

        public static void Backup(string source) 
        {
            DirectoryInfo newBackupFolder = CreateFolderStructure();

            if (File.Exists(source))
            {
                FileInfo sourceFile = new FileInfo(source);
                ProgressBar bar = new(sourceFile.Length);
                BackupFile(sourceFile, newBackupFolder, bar);
            }
            else if (Directory.Exists(source))
            {
                DirectoryInfo sourcedir = new DirectoryInfo(source);
                ProgressBar bar = new(DirSize(sourcedir));
                BackupDir(sourcedir, newBackupFolder, bar);
            }
            else
            {
                Console.WriteLine($"File not found {source}");
                newBackupFolder.Delete(true);
            }
        }


        private static DirectoryInfo CreateFolderStructure()
        {
            DateTime now = DateTime.Now;
            string backupFolder = Path.Combine(NAS, "backup");
            DirectoryInfo newBackupFolder = new(Path.Combine(backupFolder, $"{now.Day}.{now.Month}.{now.Year}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}"));
            newBackupFolder.Create();
            return newBackupFolder;
        }


        private static void BackupDir(DirectoryInfo dir, DirectoryInfo destinationDir, ProgressBar bar)
        {
            destinationDir.Create();

            BackupFiles(dir.GetFiles(), destinationDir, bar);

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                BackupDir(subDir, new DirectoryInfo(Path.Combine(destinationDir.FullName, subDir.Name)), bar);
            }
        }

        private static void BackupFiles(FileInfo[] files, DirectoryInfo destinationDir, ProgressBar bar)
        {
            foreach (FileInfo file in files)
            {
                BackupFile(file, destinationDir, bar);
            }
        }

        private static void BackupFile(FileInfo file, DirectoryInfo destinationDir, ProgressBar bar)
        {
            try
            {
                File.Copy(file.FullName, Path.Combine(destinationDir.FullName, file.Name));
                bar.Update(file, ProgressBar.Backup.Successfull);
            }
            catch
            {
                bar.Update(file, ProgressBar.Backup.Failed);
            }

        }


        private static long DirSize(DirectoryInfo d)
        {
            return FileSizes(d.GetFiles());
        }

        private static long FileSizes(FileInfo[] files)
        {
            long size = 0;
            foreach (FileInfo file in files)
            {
                size += FileSize(file);
            }
            return size;
        }

        private static long FileSize(FileInfo file)
        {
            return file.Length;
        }
    }
}
