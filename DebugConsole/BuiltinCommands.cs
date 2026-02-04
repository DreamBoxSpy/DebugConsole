using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebugConsole
{
    internal static class BuiltinCommands
    {
        [ConsoleMethod("echo", "", "msg")]
        public static void Echo(TextWriter writer, string message)
        {
            writer.WriteLine(message);
        }
        [ConsoleMethod("exit", "Exit the game.")]
        public static void ExitGame(TextWriter writer)
        {
            writer.WriteLine("Exiting game...");
            Environment.Exit(0);
        }
    }
}
