namespace SMAPR
{
    public static class SMAPR
    {
        public static class Copy
        {
            #region Copy Directory
            public static int Directory(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool overwrite, string searchPattern = "*", bool recursive = true)
            {
                return Files(GetFiles(sourceDir, searchPattern, recursive), destinationDir, overwrite);
            }
            public static int Directory(string sourceDir, DirectoryInfo destinationDir, bool overwrite, string searchPattern = "*", bool recursive = true)
            {
                return Files(GetFiles(new DirectoryInfo(sourceDir), searchPattern, recursive), destinationDir, overwrite);
            }
            public static int Directory(DirectoryInfo sourceDir, string destinationDir, bool overwrite, string searchPattern = "*", bool recursive = true)
            {
                return Files(GetFiles(sourceDir, searchPattern, recursive), new DirectoryInfo(destinationDir), overwrite);
            }
            public static int Directory(string sourceDir, string destinationDir, bool overwrite, string searchPattern = "*", bool recursive = true)
            {
                return Files(GetFiles(new DirectoryInfo(sourceDir), searchPattern, recursive), new DirectoryInfo(destinationDir), overwrite);
            }
            #endregion

            #region Copy Files
            public static int Files(FileInfo[] sourceFiles, DirectoryInfo destinationDir, bool overwrite)
            {
                int count = 0;

                foreach (FileInfo file in sourceFiles)
                {
                    if (File(file, destinationDir, overwrite))
                        count++;
                }

                return count;
            }
            public static int Files(FileInfo[] sourceFiles, string destinationDir, bool overwrite)
            {
                return Files(sourceFiles, new DirectoryInfo(destinationDir), overwrite);
            }
            #endregion

            #region Copy File
            public static bool File(FileInfo sourceFile, DirectoryInfo destinationDir, bool overwrite)
            {
                try
                {
                    sourceFile.CopyTo(Path.Combine(destinationDir.FullName, sourceFile.Name), overwrite);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            public static bool File(string sourceFile, DirectoryInfo destinationDir, bool overwrite)
            {
                return File(new FileInfo(sourceFile), destinationDir, overwrite);
            }
            public static bool File(FileInfo sourceFile, string destinationDir, bool overwrite)
            {
                return File(sourceFile, new DirectoryInfo(destinationDir), overwrite);
            }
            public static bool File(string sourceFile, string destinationDir, bool overwrite)
            {
                return File(new FileInfo(sourceFile), new DirectoryInfo(destinationDir), overwrite);
            }
            #endregion
        }

        public static class Sync
        {

        }

        public static class Backup
        {
            private static Analytics? Analytics { get; set; }

            public static void NewBackup(string source, string destination, bool analytics)
            {
                DirectoryInfo backupFolder = CreateFolderStructure(destination);

                if (System.IO.File.Exists(source))
                {
                    File(new FileInfo(source), backupFolder);
                }
                else if (System.IO.Directory.Exists(source))
                {
                    DirectoryInfo sourceDir = new(source);
                    if(analytics)Analytics = new(DirSize(sourceDir), '#', '-', ConsoleColor.Green, ConsoleColor.White);
                    Directory(sourceDir, backupFolder);
                }
                else
                {
                    Console.WriteLine($"Source not found {source}");
                    backupFolder.Delete(true);
                }
            }

            private static void Directory(DirectoryInfo dir, DirectoryInfo destinationDir)
            {
                destinationDir.Create();

                Files(dir.GetFiles(), destinationDir);

                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    subDir.Create();
                    Directory(subDir, new DirectoryInfo(Path.Combine(destinationDir.FullName, subDir.Name)));
                }
            }

            private static void Files(FileInfo[] files, DirectoryInfo destinationDir)
            {
                foreach (FileInfo file in files)
                {
                    File(file, destinationDir);
                }
            }

            private static void File(FileInfo file, DirectoryInfo destinationDir)
            {
                bool status = Copy.File(file, destinationDir, false);

                Analytics?.Update(file, status);
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

        #region Functions
        private static FileInfo[] GetFiles(DirectoryInfo dir, string searchPattern, bool recursive)
        {
            return dir.GetFiles(searchPattern, new EnumerationOptions() { RecurseSubdirectories = recursive });
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
        #endregion
    }
}
