namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters
{
    public record BookQueryObject
    {
        public string? Genre { get; init; } = "";
        public string? Name { get; init; } = "";
        public QueryObject QueryObject { get; init; } = new QueryObject();
    }
}
