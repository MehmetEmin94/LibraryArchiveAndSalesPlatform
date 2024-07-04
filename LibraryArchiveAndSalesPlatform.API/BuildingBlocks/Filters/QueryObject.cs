namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Filters
{
    public record QueryObject
        ( 
          string? SortBy  = "",
          bool IsDecsending  = false,
          bool IsDeleted  = false,
          int PageIndex = 0,
          int PageSize = 10
        );
}
