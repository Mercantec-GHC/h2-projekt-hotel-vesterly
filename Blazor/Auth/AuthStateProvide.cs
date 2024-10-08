using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Session;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Blazor.Auth
{
   
    /// AuthStateProvide is a custom authenticationStateProvider that we will use to handle the state of the users login
  
    public class AuthStateProvide : AuthenticationStateProvider
    {
        // here we instanciate global variables within the class that lets us use the localstorage, httpclient features,
        // and an empty cliams principal for handling un-authenticated users. 
        private readonly ILocalStorageService _sesseionStorage;
        private readonly HttpClient _http;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        // here we create a constructor that takes in the localstorage and http client
        public AuthStateProvide(ILocalStorageService sessionStore, HttpClient http)
        {
            _sesseionStorage = sessionStore;
            _http = http;
        }

       
        /// This function is an already existing function within the AuthenticationStateProvider class, so here we use an override to adjust it to our needs.
        /// The GetAuthenticationStateAsync gets the state of the currently logged in (authenticated) user. 
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // gets the token from localstorage
            var token = await _sesseionStorage.GetItemAsync<string>("authToken");

            //checks if the there is a token in the lcoal storage, if not it will return the state of the empty claimsprincipal named _anonymous.
            //Basiccaly it will return a guest user if there is no token in the local storage.
            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(_anonymous);
            }
            // we get the claims identity by calling the method GetCLaimsIdentity that we have defined further down. 
            // It takes the token as an argument because we need to extract the claims from jwt token. 
            ClaimsIdentity identity = GetClaimsIdentity(token);

            // here we set the claimsprincipal, defining the user
            var user = new ClaimsPrincipal(identity);

            // And at last we sets the authentication state to be of the currently logged in user, and returns it so that they logged in throughout the application.
            var authState = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
            return authState;
        }

        
        /// This function is used to authenticate the user, it takes in a token as an argument and sets the token in the local storage.
      
        public async Task AuthenticateUser(string token)
        {
            await _sesseionStorage.SetItemAsync("authToken", token);
            var identity = GetClaimsIdentity(token);
            var user = new ClaimsPrincipal(identity);
            // Here we set the authentication state to be of the user that is logging in. 
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

      
        /// This function is used to get the claims identity from the token.
       
        public ClaimsIdentity GetClaimsIdentity(string token)
        {
            // Here we use the JwtSecurityTokenHandler to read the jwt token and extract the claims from it.
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return new ClaimsIdentity(claims, "jwt");
        }

      
        /// This function is used to log out the user, it removes the token from the local storage.
       
        public async Task Logout()
        {
            await _sesseionStorage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

    }
}
