namespace CodeArena.Infrastructure.AI;

public class GroqEndpointHandler : System.Net.Http.DelegatingHandler
{
    public GroqEndpointHandler(System.Net.Http.HttpMessageHandler innerHandler) : base(innerHandler) { }
    protected override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        if (request.RequestUri != null && request.RequestUri.Host.Contains("openai.com"))
        {
            request.RequestUri = new System.Uri($"https://api.groq.com/openai{request.RequestUri.AbsolutePath}");
        }
        return base.SendAsync(request, cancellationToken);
    }
}


