namespace WebApiEventos.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseHttpMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ResponseHttpMiddleware>();
        }
    }
    public class ResponseHttpMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<ResponseHttpMiddleware> logger;

        public ResponseHttpMiddleware(RequestDelegate siguiente,
            ILogger<ResponseHttpMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

    
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {    
                var bodyOriginal = context.Response.Body;
                context.Response.Body = ms;
                await siguiente(context);
                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(bodyOriginal);
                context.Response.Body = bodyOriginal;
                logger.LogInformation(response);
            }
        }
    }
}
