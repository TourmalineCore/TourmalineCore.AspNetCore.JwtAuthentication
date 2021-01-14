namespace JwtAuthentication.Core.Middlewares
{
    internal static class RouteMethodsStore
    {
        //private static readonly IDictionary<string, Func<LoginMiddleware, HttpContext, Task<AuthResponseModel>>> BaseMethods =
        //    new Dictionary<string, Func<LoginMiddleware, HttpContext, Task<AuthResponseModel>>>();

        //public static void InitBaseAuthRoutes()
        //{
        //    BaseMethods.Add("/auth/login", (x, context) => x.Login(context));
        //}

        //public static void ChangeLoginRoutes(string customRoute)
        //{
        //    var (route, action) = BaseMethods
        //        .Single(x => x.Key == "/auth/login");

        //    BaseMethods.Remove(route);
        //    BaseMethods.Add(customRoute, action);
        //}

        //public static IDictionary<string, Func<LoginMiddleware, HttpContext, Task<AuthResponseModel>>> GetMethods()
        //{
        //    return BaseMethods;
        //}

        //public static IEnumerable<string> GetAvailablePaths()
        //{
        //    return BaseMethods.Keys;
        //}
    }
}