using System.Net.Http.Json;
using ValidationDemo.DTOs;

namespace ValidationDemo.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductApiService> _logger;

        public ProductApiService(HttpClient httpClient, ILogger<ProductApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products");
                return products ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products from API");
                return new List<ProductDto>();
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductDto>($"api/products/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product {id} from API");
                return null;
            }
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products", dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return null;
            }
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", dto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {id}");
                return null;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {id}");
                return false;
            }
        }
    }
}