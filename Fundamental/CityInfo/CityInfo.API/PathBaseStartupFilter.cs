namespace CityInfo.API
{
    public class PathBaseStartupFilter : IStartupFilter
    {
        private readonly string _pathBase;
        public PathBaseStartupFilter(string pathBase)
        {
            _pathBase = pathBase;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UsePathBase(_pathBase); // 👈 Adds the PathBaseMiddleware
                next(app);                  //     before the other middleware
            };
        }
    }
}
