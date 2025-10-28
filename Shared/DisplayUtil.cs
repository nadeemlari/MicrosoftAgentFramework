namespace Shared;

public static class DisplayUtil
{
    private static bool _loading = true;
    public static void WriteLineError(Exception e)
    {
        WriteLineError(e.ToString());
    }

    public static void WriteLineError(string text)
    {
        WriteLine(text, ConsoleColor.Red);
    }

    public static void WriteLineYellow(string text)
    {
        WriteLine(text, ConsoleColor.Yellow);
    }
    
    public static void WriteLineDarkGray(string text)
    {
        WriteLine(text, ConsoleColor.DarkGray);
    }

    public static void WriteLineGreen(string text)
    {
        WriteLine(text, ConsoleColor.Green);
    }

    public static void WriteLineWarning(string text)
    {
        WriteLine(text, ConsoleColor.Yellow);
    }

    public static void WriteLineInformation(string text)
    {
        WriteLine(text, ConsoleColor.DarkGray);
    }

    public static void WriteLineSuccess(string text)
    {
        WriteLine(text, ConsoleColor.Green);
    }

    public static void WriteLine(string text, ConsoleColor color)
    {
        ConsoleColor orgColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        finally
        {
            Console.ForegroundColor = orgColor;
        }
    }

    public static void WriteError(Exception e)
    {
        WriteError(e.ToString());
    }

    public static void WriteError(string text)
    {
        Write(text, ConsoleColor.Red);
    }

    public static void WriteYellow(string text)
    {
        Write(text, ConsoleColor.Yellow);
    }

    public static void WriteWarning(string text)
    {
        Write(text, ConsoleColor.Yellow);
    }

    public static void WriteInformation(string text)
    {
        Write(text, ConsoleColor.DarkGray);
    }

    public static void WriteSuccess(string text)
    {
        Write(text, ConsoleColor.Green);
    }

    public static void Write(string text, ConsoleColor color)
    {
        ConsoleColor orgColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        finally
        {
            Console.ForegroundColor = orgColor;
        }
    }
    
    public static void Separator()
    {
        Console.WriteLine();
        WriteLine("".PadLeft(Console.WindowWidth, '-'), ConsoleColor.Gray);
        Console.WriteLine();
    }
    
    public static void StopLoading()
    {
        _loading = false;
        
    }
    public static void LoadingTask()
    {
        Task.Run(async () =>
        {
            string[] spinner = ["|", "/", "-", "\\"];
            var barLength = Console.WindowWidth-4;
            var count = 0;
            while (_loading)
            {
                var progress = (count % (barLength + 1));
                var bar = new string('█', progress) + new string(' ', barLength - progress);
                var spin = spinner[count % spinner.Length];

                // Save current color
                var prevColor = Console.ForegroundColor;

                // Write spinner and label in default color
                Console.Write($"\r{spin} [");

                // Set color for progress bar
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(bar);

                // Restore color and close bracket
                Console.ForegroundColor = prevColor;
                Console.Write("]");

                await Task.Delay(100);
                count++;
            }

            Console.Write("\r" + new string(' ', 120) + "\r"); // Clear the line
        });
    }
}