using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.HttpHandlers;

/// <summary>
/// A custom <see cref="HttpClientHandler"/> to perform public actions
/// </summary>
/// <param name="userAgent">The user agent to use to send the requests</param>
internal sealed class UnauthenticatedHttpClientHandler(string userAgent) : HttpClientHandler
{
    /// <summary>
    /// The user agent to use to send the requests
    /// </summary>
    private readonly string userAgent = userAgent;

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Headers setup
        request.Headers.Add("User-Agent", this.userAgent);

        // Send the request and handle errors
        return base.SendAsync(request, cancellationToken);
    }
}
