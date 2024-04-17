using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.HttpHandlers;

/// <summary>
/// A custom <see cref="HttpClientHandler"/> to perform public actions
/// </summary>
internal sealed class UnauthenticatedHttpClientHandler : HttpClientHandler
{
    /// <summary>
    /// The user agent to use to send the requests
    /// </summary>
    private readonly string UserAgent;

    /// <summary>
    /// Creates a new <see cref="UnauthenticatedHttpClientHandler"/> instance with the specified parameters
    /// </summary>
    /// <param name="userAgent">The user agent to use to send the requests</param>
    public UnauthenticatedHttpClientHandler(string userAgent)
    {
        this.UserAgent = userAgent;
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Headers setup
        request.Headers.Add("User-Agent", this.UserAgent);

        // Send the request and handle errors
        return base.SendAsync(request, cancellationToken);
    }
}
