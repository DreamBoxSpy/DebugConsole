using DebugConsole.External;
using IngameDebugConsole;
using ModCore.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace DebugConsole
{
    internal class ConsoleInst : IDisposable
    {
        public AnonymousPipeServerStream Out { get; }
        public AnonymousPipeServerStream In { get; }
        public Process ConsoleProcess { get; }

        public TextWriter Writter { get; }


        private class ConsoleTextWriter(ConsoleInst inst) : TextWriter
        {
            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(string? value)
            {
                inst.SendMsg(value ?? "");
            }
            public override void Write(char value)
            {
                inst.SendMsg(value.ToString());
            }
        }

        public ConsoleInst()
        {
            Out = new(PipeDirection.Out, HandleInheritability.Inheritable);
            In = new(PipeDirection.In, HandleInheritability.Inheritable);

            Writter = new ConsoleTextWriter(this);

            ConsoleProcess = WorkerProcessUtils.StartWorkerProcess(typeof(ExternalMain).AssemblyQualifiedName!, nameof(ExternalMain.Main),
                new()
                {
                    EnvironmentVariables =
                    {
                        ["DCCM_DebugConsole_OutPipe"] = In.GetClientHandleAsString(),
                        ["DCCM_DebugConsole_InPipe"] = Out.GetClientHandleAsString()
                    }
                }, typeof(ExternalMain).Assembly.Location);

            Out.DisposeLocalCopyOfClientHandle();
            In.DisposeLocalCopyOfClientHandle();

            new Thread(WorkerThread)
            {
                IsBackground = true
            }.Start();
        }

        private bool IsClosed()
        {
            return !Out.CanWrite || !In.CanRead || ConsoleProcess.HasExited;
        }

        public void SendMsg(string str)
        {
            if (IsClosed())
            {
                return;
            }
            lock (Out)
            {
                ModMain.Log.Information(str);

                var bytes = Encoding.UTF8.GetBytes(str);
                Out.Write(BitConverter.GetBytes(bytes.Length));
                Out.Write(bytes);
                Out.Flush();
            }
        }

        private void HandleCommand(string str)
        {
            ModCore.Modules.Game.SynchronizationContext.Send(_ =>
            {
                DebugLogConsole.ExecuteCommand(str, Writter);
            }, null);
        }

        private void WorkerThread()
        {
            try
            {
                var commandEndSign = BitConverter.GetBytes(int.MaxValue);
                var lenBuffer = new byte[4];
                while (!IsClosed())
                {
                    In.ReadAtLeast(lenBuffer, 4, false);
                    var len = BitConverter.ToInt32(lenBuffer);
                    if (len <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    var buffer = new byte[len];
                    In.ReadAtLeast(buffer, buffer.Length, false);
                    var input = Encoding.UTF8.GetString(buffer);

                    HandleCommand(input);

                    Out.Write(commandEndSign);
                }
            }catch(IOException)
            { }
        }

        public void Dispose()
        {
            try
            {
                ConsoleProcess.Kill();
            }
            catch { }
            ConsoleProcess.Dispose();
            In.Dispose();
            Out.Dispose();
        }
    }
}
