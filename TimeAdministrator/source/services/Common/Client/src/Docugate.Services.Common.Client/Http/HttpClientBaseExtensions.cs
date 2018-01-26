using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Docugate.Services.Common.Client.Http
{
    public static class HttpClientBaseExtensions
    {
        public static IHttpClientBase UseLoopThroughAuthorization(this IHttpClientBase subject, HttpContext context)
        {
            subject.Use(
                (req, _, next) =>
                {
                    var origAuth = context.Request.Headers["Authorization"];
                    req.Headers.Add("Authorization", origAuth.ToArray());
                    return next();
                });

            return subject;
        }

        public static IHttpClientBase UseBearerToken(this IHttpClientBase subject, string token)
        {
            subject.Use(
                (req, _, next) =>
                {
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    return next();
                });

            return subject;
        }

        public static IHttpClientBase UseAuthorizationHeader(this IHttpClientBase subject, AuthenticationHeaderValue authHeader)
        {
            subject.Use(
                (req, _, next) =>
                {
                    req.Headers.Authorization = authHeader;
                    return next();
                });

            return subject;
        }

        public static IHttpClientBase UseRequestAuthorizationForwarding(this IHttpClientBase subject, IServiceProvider services)
        {
            subject.Use(
                (req, _, next) =>
                {
                    var reqestSvc = services.GetService<IHttpContextAccessor>();
                    if (reqestSvc.HttpContext != null)
                    {
                        var headers = reqestSvc.HttpContext.Request.Headers;
                        if (headers.ContainsKey("Authorization"))
                        {
                            var auth = headers["Authorization"].ToString();
                            req.Headers.Authorization = AuthenticationHeaderValue.Parse(headers["Authorization"].ToString());
                        }
                    }
                    return next();
                });

            return subject;
        }
    }
}
