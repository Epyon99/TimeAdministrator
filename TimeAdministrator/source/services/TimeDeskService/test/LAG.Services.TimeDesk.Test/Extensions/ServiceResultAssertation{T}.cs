using FluentAssertions.Primitives;
using EPY.Services.Common.Service.Models;

namespace EPY.LogTiempo.Service.Test.Extensions
{
    public class ServiceResultAssertation<T> : ReferenceTypeAssertions<ServiceResult<T>, ServiceResultAssertation<T>>
    {
        protected override string Context => nameof(ServiceResult<T>);
    }
}