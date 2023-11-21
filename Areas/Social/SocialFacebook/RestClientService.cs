using FeedHiveAuth.Data.Extensions;
using RestSharp;

namespace FeedHiveAuth.Areas.Social.SocialFacebook
{
    public class RestClientService
    {
        protected readonly RestClient client;

        public RestClientService(string baseUrl)
        {
            client = new RestClient(baseUrl);
        }

        public IRestResponse Get(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, string> headers = null, Dictionary<string, object> query = null)
        {
            return ExecuteRequest(endpoint, parms: parms, headers: headers, query: query);
        }

        public IRestResponse<T> Get<T>(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, string> headers = null, Dictionary<string, object> query = null) where T : class, new()
        {
            return ExecuteRequest<T>(endpoint, parms: parms, headers: headers, query: query);
        }

        public IRestResponse Post(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, string> headers = null, bool multipart = false, object body = null, Dictionary<string, object> query = null)
        {
            return ExecuteRequest(endpoint, Method.POST, parms, query, headers, multipart, body);
        }

        public IRestResponse<T> Post<T>(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, string> headers = null, bool multipart = false, object body = null, Dictionary<string, object> query = null) where T : class, new()
        {
            return ExecuteRequest<T>(endpoint, Method.POST, parms, query, headers, multipart, body);
        }

        public IRestResponse Delete(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, string> headers = null)
        {
            return ExecuteRequest(endpoint, Method.DELETE, parms, headers: headers);
        }

        public IRestResponse<T> Delete<T>(string endpoint, Dictionary<string, object> parms = null, Dictionary<string, string> headers = null) where T : class, new()
        {
            return ExecuteRequest<T>(endpoint, Method.DELETE, parms, headers: headers);
        }

        private IRestResponse ExecuteRequest(string endpoint, Method method = Method.GET, Dictionary<string, object> parms = null, Dictionary<string, object> query = null, Dictionary<string, string> headers = null, bool multipart = false, object body = null)
        {
            var request = GetRequest(endpoint, method, parms, query, headers, multipart, body);
            return client.Execute(request);
        }

        private IRestResponse<T> ExecuteRequest<T>(string endpoint, Method method = Method.GET, Dictionary<string, object> parms = null, Dictionary<string, object> query = null, Dictionary<string, string> headers = null, bool multipart = false, object body = null) where T : class, new()
        {
            var request = GetRequest(endpoint, method, parms, query, headers, multipart, body);
            return client.Execute<T>(request);
        }

        private RestRequest GetRequest(string endpoint, Method method = Method.GET, Dictionary<string, object> parms = null, Dictionary<string, object> query = null, Dictionary<string, string> headers = null, bool multipart = false, object body = null)
        {
            var request = new RestRequest(endpoint, method)
            {
                AlwaysMultipartFormData = multipart,
            };
            parms = parms ?? new Dictionary<string, object>();
            query = query ?? new Dictionary<string, object>();
            headers = headers ?? new Dictionary<string, string>();

            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body.Serialize());
            }

            foreach (var parm in parms)
            {
                request.AddParameter(parm.Key, parm.Value, ParameterType.GetOrPost);
            }

            foreach (var q in query)
            {
                request.AddParameter(q.Key, q.Value, ParameterType.QueryString);
            }

            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }

            return request;
        }
    }
}
