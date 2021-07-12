using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Laundry.Radar.ServerlessApi.Models;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using NodaTime.TimeZones;
using NodaTime;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Laundry.Radar.ServerlessApi
{
    public class Functions
    {
        public static string authApi;
        
        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            authApi = "http://laundryapi.cerebratesolutions.com/api/auth/login";
        }


        public APIGatewayProxyResponse GetSearchLocations(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Headers["Authorization"]))
            {
                var ru = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    //Body = JsonConvert.SerializeObject(loginModel),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };

                return ru;
            }

            Console.WriteLine($"GetSerachLocations: {JsonConvert.SerializeObject(request)} : {authApi}");

            string accessToken = request.Headers["Authorization"];
            string baseUrl = "http://laundryapi.cerebratesolutions.com/api/";

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> entry in request.QueryStringParameters)
            {
                sb.Append("&");
                sb.Append(entry.Key);
                sb.Append("=");
                sb.Append(entry.Value);
            }

            string endpoint = $"{baseUrl}asset/locations/search?{sb}";

            Console.WriteLine($"URL: {endpoint}");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"{accessToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage apiResponse = httpClient.GetAsync(endpoint).Result;

                if (apiResponse.IsSuccessStatusCode)
                {
                    string thisResponse = apiResponse.Content.ReadAsStringAsync().Result;
                    var objResponse = JsonConvert.DeserializeObject<ApiResponse>(thisResponse);

                    if (objResponse.Success.Value == true)
                    {
                        var sr = new APIGatewayProxyResponse
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = JsonConvert.SerializeObject(objResponse),
                            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                        };

                        return sr;
                    }
                }
            }

            var er = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                //Body = JsonConvert.SerializeObject(timeZones),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return er;
        }

        public APIGatewayProxyResponse GetTypes(APIGatewayProxyRequest request, ILambdaContext context)
        {

            if (string.IsNullOrWhiteSpace(request.Headers["Authorization"]))
            {
                var ru = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    //Body = JsonConvert.SerializeObject(loginModel),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };

                return ru;
            }

            Console.WriteLine($"GetTypes: {JsonConvert.SerializeObject(request)}");

            string accessToken = request.Headers["Authorization"];
            string baseUrl = "http://laundryapi.cerebratesolutions.com/api/";
            string objectCode = request.QueryStringParameters["objectCode"];

            string endpoint = $"{baseUrl}system/types?objectCode={objectCode}";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"{accessToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage apiResponse = httpClient.GetAsync(endpoint).Result;

                if (apiResponse.IsSuccessStatusCode)
                {
                    string thisResponse = apiResponse.Content.ReadAsStringAsync().Result;
                    var objResponse = JsonConvert.DeserializeObject<ApiResponse>(thisResponse);

                    if (objResponse.Success.Value == true)
                    {
                        var sr = new APIGatewayProxyResponse
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            Body = JsonConvert.SerializeObject(objResponse),
                            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                        };

                        return sr;
                    }
                }
            }


            var er = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                //Body = JsonConvert.SerializeObject(timeZones),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return er;

        }

        public APIGatewayProxyResponse PostEvents(APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Headers["Authorization"]))
            {
                var ru = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    //Body = JsonConvert.SerializeObject(loginModel),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };

                return ru;
            }

            Console.WriteLine($"PostEvents: {JsonConvert.SerializeObject(request)}");

            List<Laundry.Radar.ServerlessApi.Models.TimeZoneInfo> timeZones = new List<Laundry.Radar.ServerlessApi.Models.TimeZoneInfo>();

            foreach (var location in TzdbDateTimeZoneSource.Default.ZoneLocations)
            {
                timeZones.Add(GetZoneInfo(location));
            }

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(timeZones),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        public async Task<APIGatewayProxyResponse> PostAuth(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Console.WriteLine($"PostAuth: {JsonConvert.SerializeObject(request)}");

            AuthModel loginModel = new AuthModel();
            loginModel.Username = "gkiecalocal";
            loginModel.Password = "Dupek!951";
            loginModel.ClientCode = "WEBAPP";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

                    using (var result = await httpClient.PostAsync("http://laundryapi.cerebratesolutions.com/api/auth/login", content))
                    {
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            string apiResponse = await result.Content.ReadAsStringAsync();
                            var tokenResponse = JsonConvert.DeserializeObject<AuthResponse>(apiResponse);

                            var response = new APIGatewayProxyResponse
                            {
                                StatusCode = (int)HttpStatusCode.OK,
                                Body = JsonConvert.SerializeObject(tokenResponse),
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };

                            return response;

                        }
                        else
                        {
                            var response = new APIGatewayProxyResponse
                            {
                                StatusCode = (int)HttpStatusCode.Unauthorized,
                                //Body = JsonConvert.SerializeObject(loginModel),
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };

                            return response;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                
                var response = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    //Body = JsonConvert.SerializeObject(loginModel),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };

                return response;
            }
        }

        private Laundry.Radar.ServerlessApi.Models.TimeZoneInfo GetZoneInfo(TzdbZoneLocation location)
        {
            var zone = DateTimeZoneProviders.Tzdb[location.ZoneId];

            // Get the start and end of the year in this zone
            var startOfYear = zone.AtStartOfDay(new LocalDate(2017, 1, 1));
            var endOfYear = zone.AtStrictly(new LocalDate(2018, 1, 1).AtMidnight().PlusNanoseconds(-1));

            // Get all intervals for current year
            var intervals = zone.GetZoneIntervals(startOfYear.ToInstant(), endOfYear.ToInstant()).ToList();

            // Try grab interval with DST. If none present, grab first one we can find
            var interval = intervals.FirstOrDefault(i => i.Savings.Seconds > 0) ?? intervals.FirstOrDefault();

            return new Laundry.Radar.ServerlessApi.Models.TimeZoneInfo
            {
                TimeZoneId = location.ZoneId,
                Offset = interval.StandardOffset.ToTimeSpan(),
                DstOffset = interval.WallOffset.ToTimeSpan(),
                CountryCode = location.CountryCode,
                CountryName = location.CountryName
            };
        }


    }
}
