
using EmployeeModelPackage;
using EmployeePortalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace EmployeePortalWeb.Controllers
{
    public class ApiService
    {
        private readonly string _readUrl;
        private readonly string _insertUrl;

        private readonly string _updateUrl;
        private readonly string _deleteUrl;
        private readonly string _usermanagementUrl;
        //private readonly string _bankUrl;

        public ApiService()
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();

            _readUrl = config.GetSection("ReadAPI").Value;
            _insertUrl = config.GetSection("InsertAPI").Value;

            _updateUrl = config.GetSection("UpdateAPI").Value;
            _deleteUrl = config.GetSection("Delete").Value;
            _usermanagementUrl = config.GetSection("UserManagement").Value;
            //_bankUrl = config.GetSection("BankAPI").Value;

        }

        public async Task<List<T>> GetJsonAsync<T>(string apiUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var deserialized = JsonConvert.DeserializeObject<List<T>>(json);
                    return deserialized;
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        public async Task<List<EmployeeModel>> GetEmployeeList()
        {
            string apiUrl = $"{_readUrl}api/Read/GetEmployees";
            return await GetJsonAsync<EmployeeModel>(apiUrl);
        }


        //public async Task<List<BankModel>> GetEmployeeListAsync()
        //{
        //    string apiUrl = $"{_bankUrl}api/Bank/GetEmployees";
        //    return await GetJsonAsync<BankModel>(apiUrl);
        //}




        public async Task<T> GetScalarListObject<T>(string url, StringContent content)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    Uri completeUri = new Uri(url);
                    httpClient.Timeout = TimeSpan.FromSeconds(5000); // 50s is enough; 50000 was overkill

                    // Send POST request
                    HttpResponseMessage response = await httpClient.PostAsync(completeUri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        // Check if the response body is not empty
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            T deserialized = JsonConvert.DeserializeObject<T>(json);
                            return deserialized;
                        }
                        else
                        {
                            return default(T); // Return default if empty response
                        }
                    }
                    else
                    {
                        throw new HttpRequestException(
                            $"Request to {url} failed with status code {(int)response.StatusCode} ({response.ReasonPhrase})"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while calling {url}: {ex.Message}");

                return default(T);
            }
        }

        public async Task<T> GetScalarObject<T>(string url)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    Uri completeUri = new Uri(url);
                    httpClient.Timeout = TimeSpan.FromSeconds(5000); // 50s is enough; 50000 was overkill

                    // Send POST request
                    HttpResponseMessage response = await httpClient.GetAsync(completeUri);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        // Check if the response body is not empty
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            T deserialized = JsonConvert.DeserializeObject<T>(json);
                            return deserialized;
                        }
                        else
                        {
                            return default(T); // Return default if empty response
                        }
                    }
                    else
                    {
                        throw new HttpRequestException(
                            $"Request to {url} failed with status code {(int)response.StatusCode} ({response.ReasonPhrase})"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while calling {url}: {ex.Message}");

                return default(T);
            }
        }
        public async Task<T> GetScalarListObject<T>(EmployeeModel model)
        {
            string apiUrl = $"{_insertUrl}api/Employee/InsertEmployee";
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await GetScalarListObject<T>(apiUrl, content);
            return response;
        }


        public async Task<Tuple<int, string, bool>> UpdateEmployeeData(EmployeeModel model)
        {
            string apiUrl = $"{_updateUrl}api/Update/UpdateEmployee";

            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await GetScalarListObject<Tuple<int, string, bool>>(apiUrl, content);

            return response;
        }


        public async Task<string> DeleteEmployee(int Id)
        {
            string apiUrl = $"{_deleteUrl}api/Employee/DeleteEmployee/{Id}";
            var jsonData = JsonConvert.SerializeObject(Id);

            var response = await PostJsonAsync<string>(apiUrl, null);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            return response;
        }




        public async Task<T> PostJsonAsync<T>(string url, object data)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();

            // Handle plain string response (not JSON)
            if (typeof(T) == typeof(string))
            {
                string raw = await response.Content.ReadAsStringAsync();
                return (T)(object)raw!;
            }

            // Handle JSON response
            return await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new Exception("Failed to deserialize response.");
        }





        public async Task<T> GetScalarJsonAsync<T>(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Handle plain string response (not JSON)
            if (typeof(T) == typeof(string))
            {
                string raw = await response.Content.ReadAsStringAsync();
                return (T)(object)raw!;
            }

            // Handle JSON response
            return await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new Exception("Failed to deserialize response.");
        }
        public async Task<bool> SaveEmployee(RegisterModel model)
        {
            string apiUrl = $"{_usermanagementUrl}/api/UserManagement/RegisterEmployee";
            var jsonData = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await GetScalarListObject<bool>(apiUrl, content);
            return response;

        }
        public async Task<RegisterModel> GetRegistrationDetails(RegisterModel model)
        {
            string apiUrl = $"{_usermanagementUrl}/api/UserManagement/GetRegistrationDetails/" + model.UserName + "/" + model.Password;
            var response = await GetScalarObject<RegisterModel>(apiUrl);
            return response;

        }
        public async Task<bool> IsUserExists(string name)
        {
            string apiUrl = $"{_usermanagementUrl}/api/UserManagement/IsUserExists/" + name;
            var response = await GetScalarResult<bool> (apiUrl);
            return response;


        }

        public async Task<T> GetScalarResult<T>(string url)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromSeconds(300);
                    Uri completeurl = new Uri(url);
                    client.Timeout = TimeSpan.FromSeconds(5000);
                    Task<HttpResponseMessage> response = client.GetAsync(completeurl);
                    response.Wait();
                    var webresult = response.Result;
                    if (webresult.IsSuccessStatusCode)
                    {
                        string responses = webresult.Content.ReadAsStringAsync().Result;

                        var jsonresult = JsonConvert.DeserializeObject<T>(responses);

                        return jsonresult;
                    }
                    else
                    {
                        return default(T);
                    }
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }
        }


    }
}
