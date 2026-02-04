
using dc.hxd;
using DebugConsole.External;
using IngameDebugConsole;
using ModCore.Events.Interfaces.Game;
using ModCore.Mods;
using ModCore.Utilities;
using Serilog;
using Serilog.Core;
using System.Reflection;

namespace DebugConsole
{
    internal class ModMain(ModInfo info) : ModBase(info),
        IOnFrameUpdate
    {

        public static ILogger Log { get; private set; } = Serilog.Log.Logger;
        public override int Priority => int.MaxValue;
       
        public override void Initialize()
        {
            Log = Logger;
            //Scan commands


            DebugLogConsole.ResetStatics();

            _ = new ConsoleInst();
        }

        void IOnFrameUpdate.OnFrameUpdate(double dt)
        {
            if(Key.Class.isPressed(0x71))
            {
                _ = new ConsoleInst();
            }
        }
    }
}
