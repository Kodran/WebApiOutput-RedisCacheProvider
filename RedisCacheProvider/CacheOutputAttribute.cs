using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace RedisCacheProvider
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CacheOutputAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        // cache repository
        protected readonly RedisCacheApi _cache = new RedisCacheApi();
        
        /// <summary>
        /// How long response should be cached on the server side (in seconds)
        /// </summary>
        public int ServerExpirationTime { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            var requestParameters = actionContext.Request.RequestUri.AbsolutePath 
                + GetParametersFromRequest(actionContext);

            if (string.IsNullOrEmpty(requestParameters))
            {
                requestParameters = actionContext.Request.RequestUri.AbsolutePath;
            }
           
            var cachekey = requestParameters;

            var cacheKeyExists = _cache.SearchKeys(cachekey);
            if (!cacheKeyExists)
                return;

            var cachedData = _cache.Get(cachekey);
            if (cachedData == null)
                return;

            actionContext.Response = actionContext.Request.CreateResponse();
            actionContext.Response.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(cachedData + " FROM CACHE"));

            actionContext.Response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.ActionContext.Response == null || !actionExecutedContext.ActionContext.Response.IsSuccessStatusCode)
                return;

            var responseContent = actionExecutedContext.Response.Content as ObjectContent;
            if (responseContent != null)
            {
                var contentType = responseContent.Headers.ContentType;
         
                responseContent.Headers.Remove("Content-Length");

                var requestParameters = actionExecutedContext.ActionContext.Request.RequestUri.AbsolutePath 
                    + GetParametersFromRequest(actionExecutedContext.ActionContext);

                if (string.IsNullOrEmpty(requestParameters))
                {
                    requestParameters = actionExecutedContext.ActionContext.Request.RequestUri.AbsolutePath;
                }

                var cachekey = requestParameters;

                _cache.Add(cachekey, Newtonsoft.Json.JsonConvert.SerializeObject(responseContent.Value), 
                    ServerExpirationTime != 0 ? ServerExpirationTime : 30);
            }
        }

        private string GetParametersFromRequest(HttpActionContext actionContext)
        {
            switch (actionContext.Request.Method.Method)
            {
                case "GET":
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(
                            actionContext.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value));
                    }                    
                case "POST":
                    {
                        string body;
                        using (var stream = actionContext.Request.Content.ReadAsStreamAsync().Result)
                        {
                            if (stream.CanSeek)
                            {
                                stream.Position = 0;
                            }
                            body = actionContext.Request.Content.ReadAsStringAsync().Result;                                                                                    
                        }
                        NameValueCollection values = HttpUtility.ParseQueryString(body);
                        return  Newtonsoft.Json.JsonConvert.SerializeObject(ToDictionary(values));
                    }                   
            }
            return string.Empty;
        }
     
        IDictionary<string, string> ToDictionary(NameValueCollection col)
        {
            var dict = new Dictionary<string, string>();

            foreach (var key in col.Keys)
            {
                dict.Add(key.ToString(), col[key.ToString()]);
            }

            return dict;
        }
    }
}
