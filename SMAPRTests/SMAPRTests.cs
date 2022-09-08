using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMAPR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR.Tests
{
    [TestClass()]
    public class SMAPRTests
    {
        public const int Seed = 42069; // i know very mature
        public readonly DirectoryInfo SourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "SourceDir"));
        public readonly DirectoryInfo BackupDir = new(Path.Combine(Directory.GetCurrentDirectory(), "BackupDir"));
        private const long MinimumFileSize = 2048;
        private const long MaximumFileSize = 20480;

        [TestMethod()]
        public void BackupDirManyFiles()
        {
            // Create Directories
            CreateDirectories();

            // Create Files
            int amount = 10;
            FileInfo[] files = new FileInfo[amount];

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new(Path.Combine(SourceDir.FullName, $"{i}.txt"));
            }

            CreateFiles(files);

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            FileInfo[] sourceFiles = GetFiles(SourceDir);
            FileInfo[] backupFiles = GetFiles(BackupDir);

            if (sourceFiles.Length != backupFiles.Length)
                Assert.Fail("SourceFiles and BackupFiles dont have the same Length");

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                if (sourceFiles[i].Length != backupFiles[i].Length)
                    Assert.Fail($"Sourcefile[{i}] does not have the same size as Backupfile[{i}]");
            }

            // Delete Files
            ClearDirectories();
        }

        [TestMethod()]
        public void BackupDirOneFile()
        {
            // Create Directories
            CreateDirectories();

            // Create Files
            int amount = 1;
            FileInfo[] files = new FileInfo[amount];

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new(Path.Combine(Directory.GetCurrentDirectory(), $"{i}.txt"));
            }

            CreateFiles(files);

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            FileInfo[] sourceFiles = GetFiles(SourceDir);
            FileInfo[] backupFiles = GetFiles(BackupDir);

            if (sourceFiles.Length != backupFiles.Length)
                Assert.Fail("SourceFiles and BackupFiles dont have the same Length");

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                if (sourceFiles[i].Length != backupFiles[i].Length)
                    Assert.Fail($"Sourcefile[{i}] does not have the same size as Backupfile[{i}]");
            }

            // Delete Files
            ClearDirectories();
        }

        [TestMethod()]
        public void BackupEmptyDir()
        {
            // Create Directories
            CreateDirectories();

            // Create Files
            int amount = 0;
            FileInfo[] files = new FileInfo[amount];

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new(Path.Combine(SourceDir.FullName, $"{i}.txt"));
            }

            CreateFiles(files);

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            FileInfo[] sourceFiles = GetFiles(SourceDir);
            FileInfo[] backupFiles = GetFiles(BackupDir);

            if (sourceFiles.Length != backupFiles.Length)
                Assert.Fail("SourceFiles and BackupFiles dont have the same Length");

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                if (sourceFiles[i].Length != backupFiles[i].Length)
                    Assert.Fail($"Sourcefile[{i}] does not have the same size as Backupfile[{i}]");
            }

            // Delete Files
            ClearDirectories();
        }

        [TestMethod()]
        public void BackupEmptyDirWithSubDir()
        {
            // Create Directories
            CreateDirectories();

            // Create Files
            int amount = 0;
            int subDirAmount = 1;
            int amountInSubDir = 10;
            FileInfo[] files = new FileInfo[amount];

            for (int i = 0; i < files.Length; i++)
            {
                for(int j = 0; j < subDirAmount; j++)
                {
                    for (int k = 0; k < amountInSubDir; k++)
                    {
                        files[i] = new(Path.Combine(SourceDir.FullName,$"{j}", $"{k}.txt"));
                    }
                }
                files[i] = new(Path.Combine(SourceDir.FullName, $"{i}.txt"));
            }

            CreateFiles(files);

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            FileInfo[] sourceFiles = GetFiles(SourceDir);
            FileInfo[] backupFiles = GetFiles(BackupDir);

            if (sourceFiles.Length != backupFiles.Length)
                Assert.Fail("SourceFiles and BackupFiles dont have the same Length");

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                if (sourceFiles[i].Length != backupFiles[i].Length)
                    Assert.Fail($"Sourcefile[{i}] does not have the same size as Backupfile[{i}]");
            }

            // Delete Files
            ClearDirectories();
        }

        [TestMethod()]
        public void BackupEmptyDirWithSubDirs()
        {
            // Create Directories
            CreateDirectories();

            // Create Files
            int amount = 0;
            int subDirAmount = 10;
            int amountInSubDir = 10;
            FileInfo[] files = new FileInfo[amount];

            for (int i = 0; i < files.Length; i++)
            {
                for (int j = 0; j < subDirAmount; j++)
                {
                    for (int k = 0; k < amountInSubDir; k++)
                    {
                        files[i] = new(Path.Combine(SourceDir.FullName, $"{j}", $"{k}.txt"));
                    }
                }
                files[i] = new(Path.Combine(SourceDir.FullName, $"{i}.txt"));
            }

            CreateFiles(files);

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            FileInfo[] sourceFiles = GetFiles(SourceDir);
            FileInfo[] backupFiles = GetFiles(BackupDir);

            if (sourceFiles.Length != backupFiles.Length)
                Assert.Fail("SourceFiles and BackupFiles dont have the same Length");

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                if (sourceFiles[i].Length != backupFiles[i].Length)
                    Assert.Fail($"Sourcefile[{i}] does not have the same size as Backupfile[{i}]");
            }

            // Delete Files
            ClearDirectories();
        }
        
        [TestMethod()]
        public void BackupEmptyDirWithEmptySubDir()
        {
            // Create Directories
            CreateDirectories();

            int subDirs = 1;

            for (int i = 0; i < subDirs; i++)
            {
                SourceDir.CreateSubdirectory($"{i}");
            }

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            DirectoryInfo[] backupDirs = BackupDir.EnumerateDirectories().ToArray()[0].EnumerateDirectories().ToArray()[0].GetDirectories("*", new EnumerationOptions() { RecurseSubdirectories = true });
            DirectoryInfo[] sourceDirs = SourceDir.GetDirectories();
            
            if(backupDirs.Length != sourceDirs.Length)
                Assert.Fail("SourceDirs and BackupDirs dont have the same Length");

            for(int i = 0; i < backupDirs.Length; i++)
            {
                if (backupDirs[i].Name != sourceDirs[i].Name)
                    Assert.Fail($"SourceDirs[{i}] does not have the same size as BackupDirs[{i}]");
            }

            // Delete Directories
            ClearDirectories();
        }

        [TestMethod()]
        public void BackupEmptyDirWithEmptySubDirs()
        {
            // Create Directories
            CreateDirectories();

            int subDirs = 10;

            for (int i = 0; i < subDirs; i++)
            {
                SourceDir.CreateSubdirectory($"{i}");
            }

            // Backup Files
            SMAPR.Backup(SourceDir, BackupDir, false);

            // Validate
            DirectoryInfo[] backupDirs = BackupDir.EnumerateDirectories().ToArray()[0].EnumerateDirectories().ToArray()[0].GetDirectories("*", new EnumerationOptions() { RecurseSubdirectories = true });
            DirectoryInfo[] sourceDirs = SourceDir.GetDirectories();

            if (backupDirs.Length != sourceDirs.Length)
                Assert.Fail("SourceDirs and BackupDirs dont have the same Length");

            for (int i = 0; i < backupDirs.Length; i++)
            {
                if (backupDirs[i].Name != sourceDirs[i].Name)
                    Assert.Fail($"SourceDirs[{i}] does not have the same size as BackupDirs[{i}]");
            }

            // Delete Directories
            ClearDirectories();
        }

        private static void CreateFiles(FileInfo[] files)
        {
            Random random = new(Seed);

            foreach (FileInfo file in files)
            {
                FileStream fs = file.OpenWrite();
                byte[] fileContent = new byte[random.NextInt64(MinimumFileSize, MaximumFileSize)];

                for (int i = 0; i < fileContent.Length; i++)
                {
                    fileContent[i] = (byte)random.Next(0, 256);
                }

                fs.Write(fileContent, 0, fileContent.Length);
                fs.Close();
            }
        }
    
        private void ClearDirectories()
        {
            SourceDir.Delete(true);
            BackupDir.Delete(true);
        }
        private void CreateDirectories()
        {
            SourceDir.Create();
            BackupDir.Create();
        }

        private static FileInfo[] GetFiles(DirectoryInfo dir)
        {
            return dir.GetFiles("*", new EnumerationOptions() { RecurseSubdirectories = true });
        }
    }
}