using FluentAssertions.Execution;
using EPY.Services.Common.Service.Models;

namespace EPY.Services.UserWorkQuota.Test.Extensions
{
    public static class ServiceResultExtensions
    {
        private const string HaveStateFailMsg = "";

        public static void ShouldHaveState<T>(this ServiceResult<T> subject, string status, string because = HaveStateFailMsg, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(subject.Status == status)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected the status of the response to be {0}{reason} but found {1}", status, subject.Status);
        }
    }
}
