using Blazored.LocalStorage;

namespace Psycology.Space.Client.Auth;

public class AuthorizationMessageHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await localStorage.GetItemAsStringAsync("authToken");
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        catch
        {
            // Ignore - no token available (e.g., during prerender)
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
