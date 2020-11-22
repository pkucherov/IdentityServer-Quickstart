using IdentityModel;
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
            DiscoveryDocumentResponse disco;
            // discover endpoints from metadata
            {
                var client = new HttpClient();
                disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }
            }
            
            // client.AccessTokenType = AccessTokenType.Reference;
            // request token
            {
                var client = new HttpClient();
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
                var resp = await client.SendAsync(request);


                var identityGet = await client.GetStringAsync(baseAddress + "/identity");
                Console.WriteLine($"GET: {identityGet}");

            }

            {
                var client = new HttpClient();
                var tokenResponse2 = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    Password = "Pass@word1",
                    UserName = "demouser@microsoft.com",
                    Scope = "api1",
                    ClientId = "ro.client",
                    ClientSecret = "secret"
                });

                if (tokenResponse2.IsError)
                {
                    Console.WriteLine(tokenResponse2.Error);
                    return;
                }

                Console.WriteLine(tokenResponse2.Json);
                Console.WriteLine("\n\n");

            }
            {
                var client = new HttpClient();
                var tokenResponse3 = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,

                    ClientId = "client11",
                    ClientSecret = "secret11",
                    Scope = "api1"
                });

                if (tokenResponse3.IsError)
                {
                    Console.WriteLine(tokenResponse3.Error);
                    return;
                }

                Console.WriteLine(tokenResponse3.Json);
            }
            
            {
                var client4 = new HttpClient();
                //reference token
                var tokenResponse4 = await client4.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    Password = "password",
                    UserName = "alice",
                    Scope = "api1",
                    ClientId = "ro.client2",
                    ClientSecret = "secret"
                });

                if (tokenResponse4.IsError)
                {
                    Console.WriteLine(tokenResponse4.Error);
                    return;
                }

                Console.WriteLine(tokenResponse4.Json);
                Console.WriteLine("\n\n");

                var accessToken4 = tokenResponse4.Json["access_token"].ToString();
                // WeatherForecast
                //       client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var weatherGet4 = await client4.GetStringAsync(baseAddress + "/WeatherForecast");


                HttpRequestMessage request4 = new HttpRequestMessage(HttpMethod.Get, baseAddress + "/identity");
                request4.Headers.Add("Authorization", new List<string>() { "Bearer " + accessToken4 });
                request4.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken4);
                var resp4 = await client4.SendAsync(request4);

                client4.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken4);
                var identityGet4 = await client4.GetStringAsync(baseAddress + "/identity");
                Console.WriteLine($"GET: {identityGet4}");
            }
        }
    }
}
