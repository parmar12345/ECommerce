using ECommerce.Application.Common.Models;

namespace ECommerce.Application.DTOs.Products;

public class ProductQueryRequest : PaginationRequest
{
    public string? Search { get; set; }

    public Guid? CategoryId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string SortBy { get; set; } = "createdAt";

    public string SortDirection { get; set; } = "desc";
}