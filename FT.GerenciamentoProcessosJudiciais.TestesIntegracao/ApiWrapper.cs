using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FT.GerenciamentoProcessosJudiciais.TestesIntegracao
{
    static class ApiWrapper
    {
        const string ApiUrl = "http://localhost:5001";

        public static async Task<T> ApiGetAsync<T>(this string requestUri, object args)
        {
            using (var client = new HttpClient())
            {
                var qs = GetQueryString(args);
                var url = $"{ApiUrl}{requestUri}?{qs}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(content);
                return result;
            }
        }

        public static async Task<T> ApiPostAsync<T>(this string requestUri, object data)
        {
            using (var client = new HttpClient())
            {
                var obj = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                });
                var datacontent = new StringContent(obj, Encoding.UTF8, "application/json");
                var url = $"{ApiUrl}{requestUri}";
                var response = await client.PostAsync(url, datacontent);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException(content);

                var result = JsonConvert.DeserializeObject<T>(content);
                return result;

            }
        }

        public static async Task<T> ApiPutAsync<T>(this string requestUri, object data)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                var obj = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                });
                var datacontent = new StringContent(obj, Encoding.UTF8, "application/json");
                var url = $"{ApiUrl}{requestUri}";
                var response = await client.PutAsync(url, datacontent);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException(content);

                var result = JsonConvert.DeserializeObject<T>(content);
                return result;

            }
        }

        public static async Task<T> ApiDeleteAsync<T>(this string requestUri, object args)
        {
            using (var client = new HttpClient())
            {
                var qs = GetQueryString(args);
                var url = $"{ApiUrl}{requestUri}?{qs}";
                var response = await client.DeleteAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException(content);

                var result = JsonConvert.DeserializeObject<T>(content);
                return result;

            }
        }

        public static string GetQueryString(this object obj, string prefix = "")
        {
            var query = "";
            var vQueryString = (JsonConvert.SerializeObject(obj));

            var jObj = (JObject)JsonConvert.DeserializeObject(vQueryString);
            query = String.Join("&",
               jObj.Children().Cast<JProperty>()
               .Select(jp =>
               {
                   if (jp.Value.Type == JTokenType.Array)
                   {
                       var count = 0;
                       var arrValue = String.Join("&", jp.Value.ToList().Select<JToken, string>(p =>
                       {
                           var tmp = JsonConvert.DeserializeObject(p.ToString()).GetQueryString(jp.Name + HttpUtility.UrlEncode("[") + count++ + HttpUtility.UrlEncode("]"));
                           return tmp;
                       }));
                       return arrValue;
                   }
                   else
                       return (prefix.Length > 0 ? prefix + HttpUtility.UrlEncode("[") + jp.Name + HttpUtility.UrlEncode("]") : jp.Name) + "=" + HttpUtility.UrlEncode(jp.Value.ToString());
               }
               )) ?? "";

            return query;
        }
    }
}
