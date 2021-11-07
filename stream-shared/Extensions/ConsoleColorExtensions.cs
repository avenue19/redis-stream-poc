using System;

namespace stream_shared.Extensions
{
    public static class ConsoleColorExtensions
    {
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <example>
        /// ConsoleColor.Green.WriteLine("The task executed successfully");
        /// </example>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteLine(this ConsoleColor foregroundColor, string value)
        {
            var originalForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(value);

            Console.ForegroundColor = originalForegroundColor;
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <example>
        /// ConsoleColor.Green.Write("Performing task ... ");
        /// </example>
        /// <param name="foregroundColor">Foreground color.</param>
        /// <param name="value">The value to write.</param>
        public static void Write(this ConsoleColor foregroundColor, string value)
        {
            var originalForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.Write(value);

            Console.ForegroundColor = originalForegroundColor;
        }

    }
}
