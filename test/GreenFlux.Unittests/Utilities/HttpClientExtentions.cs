using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreenFlux.Unittests.Utilities
{
    public static class HttpClientExtentions
    { 
        public static async Task<HttpResponseMessage> SendAsync(this HttpClient httpClient, HttpMethod method, string requestUri)
        {
            var result = await SendAsync<object, object>(httpClient, method, requestUri, null);
            return result.Response;
        }

        public static Task<(HttpResponseMessage Response, TResponse Content)> SendAsync<TResponse>(
            this HttpClient httpClient, 
            HttpMethod method, 
            string requestUri) 
            where TResponse : class 
            => SendAsync<object, TResponse>(httpClient, method, requestUri, null);
       
        public static async Task<(HttpResponseMessage Response, TResponse Content)> SendAsync<TRequest, TResponse>(
            this HttpClient httpClient, 
            HttpMethod method, 
            string requestUri, 
            TRequest requestContent) 
            where TRequest : class 
            where TResponse : class
        {
            var request = new HttpRequestMessage(method, requestUri);

            if (requestContent != null)
            {
                string stringContent = JsonSerializer.Serialize(requestContent);
                request.Content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);
            
            var responseContentString = await response.Content.ReadAsStringAsync();
            TResponse deserializedContent = string.IsNullOrEmpty(responseContentString)
                ? null
                : JsonSerializer.Deserialize<TResponse>(responseContentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return (response, deserializedContent);
        }
    }
}
