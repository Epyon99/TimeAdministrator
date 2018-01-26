using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPY.Services.UserWorkQuota.Test.Extensions
{
    public static class HttpContextExtensions
    {
        public static HttpContext WithFakeContext(this Controller subject)
        {
            var fakeCtx = A.Fake<HttpContext>();

            var fakeResponse = A.Fake<HttpResponse>();
            A.CallTo(() => fakeCtx.Response).Returns(fakeResponse);

            var fakeRequest = A.Fake<HttpRequest>();
            A.CallTo(() => fakeCtx.Request).Returns(fakeRequest);

            subject.ControllerContext = new ControllerContext
            {
                HttpContext = fakeCtx
            };

            return fakeCtx;
        }

        public static HttpRequest WithMethod(this HttpRequest subject, string method)
        {
            A.CallTo(() => subject.Method).Returns(method);

            return subject;
        }

        public static HttpContext WithFakePrincipal(this HttpContext subject, string name)
        {
            var fakeIdentity = A.Fake<IIdentity>();
            A.CallTo(() => fakeIdentity.Name).Returns(name);

            var fakeUser = A.Fake<ClaimsPrincipal>();
            A.CallTo(() => fakeUser.Identity).Returns(fakeIdentity);

            A.CallTo(() => subject.User).Returns(fakeUser);

            return subject;
        }

        public static HttpContext WithClaim(this HttpContext subject, Action<IList<Claim>> claimFactory)
        {
            A.CallTo(() => subject.User.Claims).ReturnsLazily(
                () =>
                {
                    var claimsCol = new List<Claim>();
                    claimFactory(claimsCol);
                    return claimsCol;
                });

            return subject;
        }
    }
}