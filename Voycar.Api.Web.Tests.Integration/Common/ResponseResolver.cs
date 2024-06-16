namespace Voycar.Api.Web.Tests.Integration.Common;

using System.Text.Json;
using Generic;

public static class ResponseResolver
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Gets the content from the <paramref name="httpResponse"/> and parses it as JSON into a <c>TEntity</c>.
    /// </summary>
    /// <param name="httpResponse">The <c>HttpResponseMessage</c> to parse the content of</param>
    /// <typeparam name="TEntity">The expected type of the response content</typeparam>
    /// <returns>An instance of <c>TEntity</c> created from the response content</returns>
    /// <exception cref="ArgumentException">If the response content is not a type of <c>TEntity</c></exception>
    public static async Task<TEntity> ResolveResponse<TEntity>(HttpResponseMessage httpResponse)
        where TEntity : Entity
    {
        var responseMessage = await httpResponse.Content.ReadAsStringAsync();
        var responseEntity = JsonSerializer.Deserialize<TEntity>(responseMessage, Options);
        if (responseEntity is null)
        {
            throw new ArgumentException("The given HttpResponseMessage content can not be parsed into " +
                                        $"a {typeof(TEntity).Name} instance.");
        }

        return responseEntity;
    }
}
