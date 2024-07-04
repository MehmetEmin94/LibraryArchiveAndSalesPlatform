namespace LibraryArchiveAndSalesPlatform.API.Domain.ValueObjects
{
    public class FileContent
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
