using IdentityModel;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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
                var tokenResponse2 = await client.RequestPasswordTokenAsync(new IdentityModel.Client.PasswordTokenRequest
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
                var tokenResponse4 = await client4.RequestPasswordTokenAsync(new IdentityModel.Client.PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    Password = "Pass@word1",
                    UserName = "demouser@microsoft.com",
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
            {
                var client5 = new HttpClient();
                //reference token
                var tokenResponse4 = TestClient.RequestPasswordToken(client5,new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    Password = "Pass@word1",
                    UserName = "demouser@microsoft.com",
                    Scope = "api1",
                    ClientId = "ro.client2",
                    ClientSecret = "secret"
                });

                if (!tokenResponse4.IsSuccessStatusCode)
                {
                    Console.WriteLine(tokenResponse4);
                    return;
                }

                Console.WriteLine(tokenResponse4);
                Console.WriteLine("\n\n");

                /*
                var accessToken4 = tokenResponse4.Json["access_token"].ToString();
                // WeatherForecast
                //       client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var weatherGet4 = await client5.GetStringAsync(baseAddress + "/WeatherForecast");


                HttpRequestMessage request4 = new HttpRequestMessage(HttpMethod.Get, baseAddress + "/identity");
                request4.Headers.Add("Authorization", new List<string>() { "Bearer " + accessToken4 });
                request4.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken4);
                var resp4 = await client5.SendAsync(request4);

                client5.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken4);
                var identityGet4 = await client5.GetStringAsync(baseAddress + "/identity");
                Console.WriteLine($"GET: {identityGet4}");
                */
            }
        }
    }

    class TestClient
    {
        public static HttpResponseMessage RequestPasswordToken(HttpMessageInvoker client, PasswordTokenRequest request)
        {
            var clone = request;

            clone.Parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
            clone.Parameters.Add(OidcConstants.TokenRequest.UserName, request.UserName);
            clone.Parameters.Add(OidcConstants.TokenRequest.Password, request.Password);
            clone.Parameters.Add(OidcConstants.TokenRequest.Scope, request.Scope);

            return RequestToken(client, clone);
        }

        static HttpResponseMessage RequestToken(HttpMessageInvoker client, PasswordTokenRequest request)
        {
            request.Prepare();
            request.Method = HttpMethod.Post;

            HttpResponseMessage response = null;
            try
            {
                CancellationToken cancellationToken = default;
                response = client.Send(request, cancellationToken);
            }
            catch (Exception ex)
            {
                
            }

            return response;
        }
    }

    
    public class PasswordTokenRequest : HttpRequestMessage
    {
        public string Address { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ClientAssertion ClientAssertion { get; set; }
        public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;
        public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;
        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Scope { get; set; }
        public string GrantType { get; set; }

        public PasswordTokenRequest()
        {
            Headers.Accept.Clear();
            Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Prepare()
        {
            if (ClientId != null)
            {
                if (ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
                {
                    if (AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                    {
                        this.SetBasicAuthenticationOAuth(ClientId, ClientSecret ?? "");
                    }
                    else if (AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                    {
                        this.SetBasicAuthentication(ClientId, ClientSecret ?? "");
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported basic authentication header style");
                    }
                }
                else if (ClientCredentialStyle == ClientCredentialStyle.PostBody)
                {
                    Parameters.Add(OidcConstants.TokenRequest.ClientId, ClientId);
                    Parameters.Add(OidcConstants.TokenRequest.ClientSecret, ClientSecret);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported client credential style");
                }
            }

            if (ClientAssertion != null)
            {
                if (ClientAssertion.Type != null && ClientAssertion.Value != null)
                {
                    Parameters.Add(OidcConstants.TokenRequest.ClientAssertionType, ClientAssertion.Type);
                    Parameters.Add(OidcConstants.TokenRequest.ClientAssertion, ClientAssertion.Value);
                }
            }

            if (Address!=null)
            {
                RequestUri = new Uri(Address);
            }

            if (Parameters!=null)
            {
                Content = new FormUrlEncodedContent(Parameters);
            }
        }
    }
}
