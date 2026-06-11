using dc;
using IngameDebugConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace DebugConsole.Commands
{
    internal static class ExampleCommands
    {
        [ConsoleMethod("test", "", "msg")]
        public static void Echo(TextWriter writer, string message, int i)
        {
            writer.WriteLine(i);
            writer.WriteLine(message);
        }

        [ConsoleMethod("crash-game", "Crash the game.")]
        public static void CrashGame(TextWriter writer)
        {
            CrashGame(writer, 0);
        }

        [ConsoleMethod("crash-game", "Crash the game.")]
        public static unsafe void CrashGame(TextWriter writer, int kind)
        {
            if (kind == 0)
            {
                Hook_Boot.mainLoop += (_, _1) =>
                {
                    throw new Exception("Test");
                };
                return;
            }
            else if (kind == 1)
            {
                ((delegate*<void>)0x2222)();
            }

            Environment.FailFast("Test");
        }
    }
}
