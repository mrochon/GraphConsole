using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace GraphConsole
{
    internal class UpdateUsers
    {
        private readonly static string clientId = "8f941f02-1ce1-4704-960f-896d37fb32d3";
        private static readonly string clientSecret = "...";
        private readonly static string authority = "https://login.microsoftonline.com/mrochonb2cprod.onmicrosoft.com";
        private HttpClient http = new HttpClient();

        private IConfidentialClientApplication auth;
        public UpdateUsers()
        {
            auth = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
        }
        public async Task<int> Update()
        {
            var graphUrl = "https://graph.microsoft.com/v1.0/users";
            var url = $"{graphUrl}?$select=id,displayName&$top=10";
            var count = 0;
            //var tokens = new { AccessToken = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IjVQX0ZWM20zR0NBVTlOWVdHNG5VSnV4a0RGa1NpNUV6VW9BaFRtOUhSY3ciLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9jZjZjNTcyYy1jNzJlLTRmMzEtYmQwYi03NTYyM2QwNDA0OTUvIiwiaWF0IjoxNjc1OTc4ODYwLCJuYmYiOjE2NzU5Nzg4NjAsImV4cCI6MTY3NTk4Mjc2MCwiYWlvIjoiRTJaZ1lNZ3QrbWUrUTFlVHJYbnF0NzBxSDU0OUJBQT0iLCJhcHBfZGlzcGxheW5hbWUiOiJVc2VyQXBkYXRlciBjb25zb2xlIGFwcCIsImFwcGlkIjoiOGY5NDFmMDItMWNlMS00NzA0LTk2MGYtODk2ZDM3ZmIzMmQzIiwiYXBwaWRhY3IiOiIxIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvY2Y2YzU3MmMtYzcyZS00ZjMxLWJkMGItNzU2MjNkMDQwNDk1LyIsImlkdHlwIjoiYXBwIiwib2lkIjoiMTYzODdhNzMtNWI0NC00MzZiLWI3ZjctZjNmYTliNWMwNTg3IiwicmgiOiIwLkFSMEFMRmRzenk3SE1VLTlDM1ZpUFFRRWxRTUFBQUFBQUFBQXdBQUFBQUFBQUFBZEFBQS4iLCJyb2xlcyI6WyJVc2VyLlJlYWRXcml0ZS5BbGwiXSwic3ViIjoiMTYzODdhNzMtNWI0NC00MzZiLWI3ZjctZjNmYTliNWMwNTg3IiwidGVuYW50X3JlZ2lvbl9zY29wZSI6Ik5BIiwidGlkIjoiY2Y2YzU3MmMtYzcyZS00ZjMxLWJkMGItNzU2MjNkMDQwNDk1IiwidXRpIjoiaThIeUtuZW4tVXFYMXRXSml4eklBQSIsInZlciI6IjEuMCIsIndpZHMiOlsiMDk5N2ExZDAtMGQxZC00YWNiLWI0MDgtZDVjYTczMTIxZTkwIl0sInhtc190Y2R0IjoxNDg4NDAwMTg1fQ.sye3fNER0kNF2se9_c9Km4R3E0UAWgIL2sSd4ntV5UllEoHRiUI8xUBMj4CmYoavNCvqUnc4G5yKS5Fgf1H7ig8JL6RjBTCwNOTvkSv48RIPmYJoTd8np7FIn0vuMjlAOtH6vAFq-BlJdKuWaTpYd_q30skllAguawYXxE3_MhxDj2RXEFZVvVdPOL9PrEfTnX5xcTmfVDDzYTDMAVjEfUeyGkajtHhyy9BAqF2KZ9ILcZllrubvWGvJTtQgkl4oC4Ik7AuRMvoKiXG0fIN4GUvbxhP-KgWKRZexW4Tbps-zivB_jzQJPeeeJBhFO7fveysiI5iogK9ebvzBSnXH8A" };
            while (!String.IsNullOrEmpty(url))
            {
                string json = await CallGraph(new HttpRequestMessage(HttpMethod.Get, url));
                var users = JsonDocument.Parse(json);
                JsonElement cont;
                if (users.RootElement.TryGetProperty("@odata.nextLink", out cont))
                    url = cont.GetString();
                else
                    url = String.Empty;
                foreach (var user in users.RootElement.GetProperty("value").EnumerateArray())
                {
                    var displayName = user.GetProperty("displayName").GetString();
                    var id = user.GetProperty("id").GetString();
                    Console.WriteLine(displayName);
                    var updatedValue = new
                    {
                        displayname = displayName.ToUpper()
                    };
                    await CallGraph(new HttpRequestMessage(HttpMethod.Patch, $"{graphUrl}/{id}")
                    {
                        Content = new StringContent(JsonSerializer.Serialize(updatedValue), Encoding.UTF8, "application/json")
                    });
                    ++count;
                }
            }
            return count;
        }

        private async Task<string> CallGraph(HttpRequestMessage req)
        {
            var tokens = await auth.AcquireTokenForClient(new string[] { "https://graph.microsoft.com/.default" })
                .ExecuteAsync();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            var retry = true;
            while (retry)
            {
                var resp = await http.SendAsync(req);
                if (resp.IsSuccessStatusCode)
                {
                    retry = false;
                    return(await resp.Content.ReadAsStringAsync());
                }
                else
                {
                    if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        var delay = int.Parse(resp.Headers.First(h => h.Key == "Retry-After").Value.First());
                        await Task.Delay(delay);
                    }
                }
            }
            return String.Empty;
        }
    }
}
