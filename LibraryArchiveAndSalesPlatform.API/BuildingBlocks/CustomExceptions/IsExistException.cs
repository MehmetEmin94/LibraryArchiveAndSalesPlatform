namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.CustomExceptions
{
    public class IsExistException : Exception
    {
        public IsExistException(string message) : base(message)
        { }
    }
}
