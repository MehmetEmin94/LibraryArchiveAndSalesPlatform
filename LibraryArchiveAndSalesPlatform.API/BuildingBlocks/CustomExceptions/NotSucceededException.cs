namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.CustomExceptions
{
    public class NotSucceededException : Exception
    {
        public NotSucceededException(string message) : base(message)
        { }
    }
}
