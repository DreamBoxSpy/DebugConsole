using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebugConsole.Commands
{
    internal static class ExampleCommands
    {
        [ConsoleMethod("test", "", "msg")]
        public static void Echo(TextWriter writer, string message)
        {
            writer.WriteLine(message);
        }
        [ConsoleMethod("exit-example", "Exit the game.")]
        public static void ExitGame(TextWriter writer)
        {
            writer.WriteLine("Exiting game...");
            Environment.Exit(0);
        }
    }
}
