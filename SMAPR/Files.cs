namespace SMAPR
{
    class Files
    {
        public int Successful { get; set; }
        public int Failed { get; set; }
        public int Count { get { return Successful + Failed; } }
    }
}