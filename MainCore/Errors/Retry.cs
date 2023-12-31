using FluentResults;

namespace MainCore.Errors
{
    public class Retry : Error
    {
        public Retry(string message) : base($"{message}. Bot must retry")
        {
        }

        public static Retry ElementCannotClick => new("Element cannot click");
        public static Retry ElementNotFound => new("Element not found");

        public static Retry ButtonNotFound(string button) => new($"Button [{button}] not found");
    }
}