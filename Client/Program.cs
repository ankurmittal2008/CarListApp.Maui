using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            HttpClient httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

            httpClient.BaseAddress = new Uri("https://localhost:7075");
            var loginResponse = await httpClient.PostAsync("/login", new StringContent("{\"username\":\"admin@localhost.com\",\"password\":\"P@ssword1\"}", Encoding.UTF8, "application/json"));
            loginResponse.EnsureSuccessStatusCode();
            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponseModel>(loginResponseContent);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse?.Token ?? throw new InvalidOperationException("Authentication failed"));
            
            var response = await httpClient.GetStringAsync("/cars");
            var cars = JsonConvert.DeserializeObject<List<Car>>(response);

            response = await httpClient.GetStringAsync(string.Format("/cars/{0}",11));
            var car = JsonConvert.DeserializeObject<Car>(response);

            Console.WriteLine(response);
        }
    }

    public class AuthResponseModel
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public required string Token { get; set; }
    }

    public abstract class BaseEntity
    {
        public int Id { get; set; }
    }

    public class Car : BaseEntity
    {
        public required string Make { get; set; }
        public required string Model { get; set; }
        public required string Vin { get; set; }
    }
}
