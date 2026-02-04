using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;

namespace DebugConsole.External
{
    internal class ExternalMain
    {
        private static AnonymousPipeClientStream In { get; set; }
        private static AnonymousPipeClientStream Out { get; set; }
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        private static void RecvMsgLoop()
        {
            var lenBuffer = new byte[4];
            var inStream = new BinaryReader(In);
            while (true)
            {
                In.ReadAtLeast(lenBuffer, 4, false);
                var len = BitConverter.ToInt32(lenBuffer);
                if(len <= 0)
                {
                    continue;
                }
                if(len == int.MaxValue)
                {
                    return;
                }
                var buffer = new byte[len];
                In.ReadAtLeast(buffer, len, false);
                var str = Encoding.UTF8.GetString(buffer);

                SimpleHtmlFormatter.PrintHtml(str);
            }
        }

        public static void Main()
        {
            FreeConsole();
            AllocConsole();

            Console.Title = "Dead Cells Debug Console";

            In = new(PipeDirection.In, Environment.GetEnvironmentVariable("DCCM_DebugConsole_InPipe")!);
            Out = new(PipeDirection.Out, Environment.GetEnvironmentVariable("DCCM_DebugConsole_OutPipe")!);

            var outStream = new BinaryWriter(Out);
            while(true)
            {
                Console.WriteLine();
                Console.Write("DCCM> ");
                var str = Console.ReadLine();
                if(string.IsNullOrEmpty(str))
                {
                    continue;
                }
                var bytes = Encoding.UTF8.GetBytes(str);
                outStream.Write(bytes.Length);
                outStream.Write(bytes);

                RecvMsgLoop();
            }
        }
    }
}
