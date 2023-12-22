using System.Net;

namespace EventScheduler.Data.Model
{
    public class ExternalDependencyException: Exception
    {
        public ExternalDependencyException(HttpStatusCode statusCode, object? value = null)
        {
            StatusCode = statusCode;
            Value = value;
        }

        public HttpStatusCode StatusCode { get; set; }
        public object? Value { get; set; }
    }
}
