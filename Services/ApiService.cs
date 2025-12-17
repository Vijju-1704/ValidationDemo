using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MyWebApi.DTOs;

namespace ValidationDemo.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Set base address from configuration
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/api";
            _httpClient.BaseAddress = new Uri(baseUrl);

            // Configure headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<ProductDto>?> GetProductsAsync(string? category = null, bool? isActive = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(category))
                    queryParams.Add($"category={Uri.EscapeDataString(category)}");
                if (isActive.HasValue)
                    queryParams.Add($"isActive={isActive.Value}");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"products{queryString}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ProductDto>>(content, _jsonOptions);
                }

                _logger.LogError("API returned error status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching products");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products");
                return null;
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProductDto>(content, _jsonOptions);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return null;
                }

                _logger.LogError("API returned error status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product {ProductId}", id);
                return null;
            }
        }

        public async Task<ProductDto?> CreateProductAsync(CreateProductDto productDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(productDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("products", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProductDto>(responseContent, _jsonOptions);
                }

                _logger.LogError("API returned error status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(productDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"products/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("API returned error status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product {ProductId}", id);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _logger.LogError("API returned error status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product {ProductId}", id);
                return false;
            }
        }

        public async Task<List<string>?> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("products/categories");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<string>>(content, _jsonOptions);
                }

                _logger.LogError("API returned error status: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories");
                return null;
            }
        }
    }
}
