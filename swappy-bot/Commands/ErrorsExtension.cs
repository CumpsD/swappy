namespace SwappyBot.Commands
{
    using System.Linq;
    using FluentResults;

    public static class ErrorsExtension
    {
        public static string BuildError<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return string.Empty;

            var error = string.Join(", ", result.Errors.Select(e => e.Message));

            if (error.EndsWith('.'))
                error = error.TrimEnd('.');

            return error;
        }
    }
}