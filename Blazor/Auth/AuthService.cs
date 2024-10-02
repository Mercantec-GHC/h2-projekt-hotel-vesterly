using Blazor.Auth.Contracts;
using Blazored.LocalStorage;
using DomainModels.DTO;
using System.Net.Http.Headers;

namespace Blazor.Auth
{
    /// <summary>
    /// AuthService is a service that handles the login and registration of users.
    /// </summary>
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        /// <summary>
        /// SetAuthorHeader is a function that sets the authorization header for the http client, with the jwt token from the local storage as the bearer token.
        /// This is used to access the protected endpoints in the API.
        /// </summary>
        /// <returns></returns>
        public async Task SetAuthorHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (token != null)
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Login is a function that sends a post request to the API to login the user.
        /// it returns the response from the API, in the format of AuthResponse class/object.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AuthResponse?> Login(LoginDTO request)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7207/Authentication/Login", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        /// <summary>
        /// This is used to register a user, it sends a post request to the API with the user information.
        /// it returns a boolean value, true if the registration was successful, false if not.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Register(CreateUserDTO request)
        {
            var response = await _http.PostAsJsonAsync("https://localhost:7207/Authentication/Register", request);
            return response;
        }

        public async Task<T?> GetFrom<T>(string url)
        {
            await SetAuthorHeader();
            return await _http.GetFromJsonAsync<T?>(url);
        }

        public HttpClient GetHttpClient() => _http;

    }
}
