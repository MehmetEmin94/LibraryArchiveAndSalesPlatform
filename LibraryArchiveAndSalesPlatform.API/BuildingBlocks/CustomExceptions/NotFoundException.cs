namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.CustomExceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        { }
    }
}
