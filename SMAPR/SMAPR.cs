namespace SMAPR
{
    public static class SMAPR
    {
        public static Analytics? Analytics;

        private static void CopyDirectory(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool overwrite, bool recursive, bool analytics, bool threaded, string searchoption = "*")
        {
            if (analytics) Analytics = new Analytics(DirSize(sourceDir), '#', '-', ConsoleColor.Green, ConsoleColor.White);

            DirectoryInfo[] subDirs = GetDirectories(sourceDir, recursive);

            foreach (DirectoryInfo subDir in subDirs)
            {
                string path = subDir.FullName.Replace(sourceDir.FullName, destinationDir.FullName);
                DirectoryInfo destinationSubDir = new(path);
                destinationSubDir.Create();
            }

            if (threaded) CopyFilesAsync(GetFiles(sourceDir, searchoption, recursive), destinationDir, overwrite);
            else CopyFiles(GetFiles(sourceDir, searchoption, recursive), destinationDir, overwrite);
        }
        private static void CopyFiles(FileInfo[] files, DirectoryInfo destinationDir, bool overwrite)
        {
            foreach(FileInfo file in files)
            {
                Analytics?.Update(file, CopyFile(file, destinationDir, overwrite));
            }

            Analytics?.Finish();
        }
        private static void CopyFilesAsync(FileInfo[] files, DirectoryInfo destinationDir, bool overwrite)
        {
            Mutex countMutex = new();
            Mutex updateMutex = new();
            int count = 0;

            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Thread thread = new(() =>
                {
                    while (count < files.Length)
                    {
                        countMutex.WaitOne();
                        int amount = count + 10 < files.Length ? 10 : files.Length - count;
                        FileInfo[] l = files[count..(count+amount)];
                        count += amount;
                        countMutex.ReleaseMutex();

                        foreach (FileInfo file in l)
                        {
                            bool status = CopyFile(file, destinationDir, overwrite);

                            updateMutex.WaitOne();
                            Analytics?.Update(file, status);
                            updateMutex.ReleaseMutex();
                        }
                    }

                    Analytics?.Finish();
                });

                thread.Start();
            }
        }
        private static bool CopyFile(FileInfo file, DirectoryInfo destinationDir, bool overwrite)
        {
            try
            {
                file.CopyTo(Path.Combine(destinationDir.FullName, file.Name), overwrite);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static FileInfo[] GetFiles(DirectoryInfo dir, string searchPattern, bool recursive)
        {
            return dir.GetFiles(searchPattern, new EnumerationOptions() { RecurseSubdirectories = recursive });
        }
        private static DirectoryInfo[] GetDirectories(DirectoryInfo dir, bool recursive)
        {
            return dir.GetDirectories("*", new EnumerationOptions() { RecurseSubdirectories = recursive });
        }

        private static long DirSize(DirectoryInfo dir)
        {
            return FileSizes(GetFiles(dir, "*", true));
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

        public static void Backup(string source, string destination, bool analytics, bool threaded)
        {
            DirectoryInfo backupFolder = CreateFolderStructure(destination);

            if (File.Exists(source))
            {
                BackupFile(new FileInfo(source), backupFolder);
            }
            else if (Directory.Exists(source))
            {
                DirectoryInfo sourceDir = new(source);
                BackupDir(sourceDir, backupFolder, analytics, threaded);
            }
            else
            {
                Console.WriteLine($"Source not found {source}");
                backupFolder.Delete(true);
            }
        }
        private static void BackupDir(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool analytics,  bool threaded)
        {
            CopyDirectory(sourceDir, destinationDir, false, true, analytics, threaded);
        }
        private static void BackupFile(FileInfo sourceFile, DirectoryInfo destinationDir)
        {
            CopyFile(sourceFile, destinationDir, false);
        }

        private static DirectoryInfo CreateFolderStructure(string source)
        {
            DateTime now = DateTime.Now;
            string backupFolder = Path.Combine(source, "backup");
            DirectoryInfo newBackupFolder = new(Path.Combine(backupFolder, $"{now.Day}.{now.Month}.{now.Year}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}"));
            newBackupFolder.Create();
            return newBackupFolder;
        }
    }
}