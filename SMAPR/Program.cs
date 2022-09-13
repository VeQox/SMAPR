namespace SMAPR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.SetWindowSize(120, 20);

            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            Console.WriteLine("Creating Files...");

            string location = Path.Combine(Directory.GetCurrentDirectory(), "SmallFiles");
            string destination = Path.Combine(Directory.GetCurrentDirectory(), "SmallFilesCopy");

            DirectoryInfo locationDir = new(location);
            locationDir.Create();
            locationDir.Delete(true);
            locationDir.Create();

            DirectoryInfo destinationDir = new(destination);
            destinationDir.Create();
            destinationDir.Delete(true);
            destinationDir.Create();

            FileInfo[] files = new FileInfo[10000];
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = new(Path.Combine(location, $"{i}.txt"));
            }

            CreateFiles(files);

            Console.WriteLine("Files Created");

            Console.WriteLine("\nPress enter to proceed");
            Console.ReadKey();

            Console.Clear();

            SMAPR.Backup(location, destination, true, false);
        }

        private static void CreateFiles(FileInfo[] files)
        {
            Random random = new();

            foreach (FileInfo file in files)
            {
                FileStream fs = file.OpenWrite();
                byte[] fileContent = new byte[random.NextInt64(2048, 40960)];

                for (int i = 0; i < fileContent.Length; i++)
                {
                    fileContent[i] = (byte)random.Next(0, 256);
                }

                fs.Write(fileContent, 0, fileContent.Length);
                fs.Close();
            }
        }
    }
}