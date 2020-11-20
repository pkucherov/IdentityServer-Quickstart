using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string baseAddress = "https://localhost:6001";
            Console.WriteLine("App Start");
            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
           // client.AccessTokenType = AccessTokenType.Reference;
            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = "client",
                ClientSecret = "secret",
                Scope = "api1"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            //setRequestHeader("Authorization", "Bearer " + user.access_token);
            var accessToken = tokenResponse.Json["access_token"].ToString();
            // WeatherForecast
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var weatherGet = await client.GetStringAsync(baseAddress + "/WeatherForecast");


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseAddress + "/identity");
            request.Headers.Add("Authorization", new List<string>() { "Bearer " + accessToken });
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resp =  await client.SendAsync(request);
        

        var identityGet = await client.GetStringAsync(baseAddress + "/identity");
            Console.WriteLine($"GET: {identityGet}");

        }

        /*
        static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var authorizationHeader = _httpContextAccesor.HttpContext
                .Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }

            var token = await GetToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request);
        }
        */
    }
}
