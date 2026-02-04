using dc.en;
using IngameDebugConsole;
using ModCore.Utilities;
using System.Collections.Generic;
using System.IO;

namespace DebugConsole.Commands.ChiuYi
{
    public static class GotolevelCommands
    {
        [ConsoleMethod("gotolvl", "前往编号对应关卡，关卡编号查看:gotolvl -help", "用法: gotolvl [关卡编号]")]
        public static void Gotolevel(TextWriter writer, string message = "")
        {
            getlvlid(writer);

            if (message == "-help")
            {
                for (int i = 0; i < allLevels.Count; i++)
                {
                    writer.Write($"{i + 1}. {allLevels[i]}");
                }
                writer.Write($"请输入 'gotolvl 编号' 来跳转到对应关卡");
                return;
            }

            Hero hero = ModCore.Modules.Game.Instance.HeroInstance!;
            if (hero == null)
            {
                writer.Write($"请保证细胞人实例存在！");
                return;
            }

            if (int.TryParse(message, out int levelIndex))
            {
                int zeroBasedIndex = levelIndex - 1;
                if (zeroBasedIndex >= 0 && zeroBasedIndex < allLevels.Count)
                {
                    string levelName = allLevels[zeroBasedIndex];
                    writer.Write($"正在前往到关卡: {levelIndex}. {levelName}");

                    dc.cine.LevelTransition.Class.@goto(levelName.AsHaxeString());


                }
                else
                {
                    writer.Write($"关卡编号无效。请输入 1 到 {allLevels.Count} 之间的数字。");
                }
            }
            else
            {
                writer.Write("参数必须是数字。请使用 'gotolvl -help' 查看所有可用关卡。");
            }
        }

        private static readonly List<string> allLevels = new List<string> { };


        private static void getlvlid(TextWriter writer)
        {
            var data = dc.Data.Class.level.all.get_length();
            for (int i = 0; i < data; i++)
            {
                var levelid = dc.Data.Class.level.all.array.getDyn(i).id;
                allLevels.Add(levelid.ToString());
            }

        }
    }
}