using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiEventos.Filtros
{
    public class FiltroDeExcepcion : ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroDeExcepcion> log;

        public FiltroDeExcepcion(ILogger<FiltroDeExcepcion> log)
        {
            this.log = log;
        }

        public override void OnException(ExceptionContext context)
        {
            log.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }
    }
}
