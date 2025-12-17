using MyWebApi.DTOs;
namespace ValidationDemo.Services
{
    public interface IApiService
    {
        Task<List<ProductDto>?> GetProductsAsync(string? category = null, bool? isActive = null);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> CreateProductAsync(CreateProductDto productDto);
        Task<bool> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<List<string>?> GetCategoriesAsync();
    }
}
