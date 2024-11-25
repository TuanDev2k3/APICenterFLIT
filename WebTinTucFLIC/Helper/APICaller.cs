using Newtonsoft.Json;
using System.Text;

namespace WebTinTucFLIC.Helper
{
    public class APICaller<T> where T : class
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public APICaller(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            var urlApi = EncryptionHelper.DecryptString(_configuration["ApiCall:ApiUrl"]!);
            //var urlApi = _configuration["ApiCall:ApiUrl"]!;
            _httpClient.BaseAddress = new Uri(urlApi);
        }

        public async Task<Response> GetListAsync(string endpoint)
        {
            Response res = new Response();
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    res = JsonConvert.DeserializeObject<Response>(responseData)!;
                }
                else
                {
                    throw new Exception("Không thể kết nối đến API");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gọi API: {ex.Message}");
            }

            return res;
        }

        public async Task<Response> CreateAsync(string endpoint, T model)
        {
            Response res = new Response();
            try
            {
                var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    res = JsonConvert.DeserializeObject<Response>(responseData)!;
                }
                else
                {
                    throw new Exception("Không thể kết nối đến API");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gọi API: {ex.Message}");
            }

            return res;
        }

        public async Task<Response> UpdateAsync(string endpoint, T model)
        {
            Response res = new Response();
            try
            {
                var jsonContent = new StringContent(
                JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(endpoint, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    res = JsonConvert.DeserializeObject<Response>(responseData)!;
                }
                else
                {
                    throw new Exception("Không thể kết nối đến API");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gọi API: {ex.Message}");
            }

            return res;
        }

        public async Task<Response> DeleteAsync(string endpoint)
        {
            Response res = new Response();
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    res = JsonConvert.DeserializeObject<Response>(responseData)!;
                }
                else
                {
                    throw new Exception("Không thể kết nối đến API");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi gọi API: {ex.Message}");
            }

            return res;
        }
    }
}
