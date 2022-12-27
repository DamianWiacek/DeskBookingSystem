using DeskBookingSystem.Exceptions;

namespace DeskBookingSystem.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
             
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadDateException e)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(e.Message);
            }
            catch (DeskNotAvaibleException e)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(e.Message);
            }
            catch (LocationNotEmptyException e)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(e.Message);
            }
            catch (DeskNotFoundException e)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(e.Message);
            }
            catch (LocationNotFoundException e)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(e.Message);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }

        }
    }
}