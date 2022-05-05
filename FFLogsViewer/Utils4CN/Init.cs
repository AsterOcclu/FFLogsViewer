using System.Linq;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using Lumina.Text;

namespace Utils4CN
{
    public class Init
    {
        public static bool IsCN()
        {
            var lang = DalamudApi.DataManager.Language;
            return (uint)lang == 4 || lang.ToString() == "ChineseSimplified";
        }

        public DalamudPluginInterface Pi;

        public Init(DalamudPluginInterface pi) {
            this.Pi = pi;
            this.Pi.Create<DalamudApi>();
        }

        public void ReplaceDataIfCN()
        {
            if (IsCN()) {
                this.ReplaceDcAndWorlds();
            }
        }

        public void ReplaceDcAndWorlds()
        {
            var gameDCs = DalamudApi.DataManager.GetExcelSheet<WorldDCGroupType>()!;
            var gameWorlds = DalamudApi.DataManager.GetExcelSheet<World>()!;
            foreach (var item in gameWorlds.Where(w => w.RowId < 1000 && w.IsPublic && w.DataCenter.Value?.RowId is >=1 and <= 3).ToArray())
            {
                item.IsPublic = false;
            }

            foreach (var mydc in ChineseServers.DataCenterMap.Values)
            {
                var dc = gameDCs.GetRow(mydc.Id);

                if (dc != null) {
                    dc.Name = new SeString(mydc.Name);
                    dc.Region = (byte)4;
                }

                foreach (var wid in mydc.WorldIds)
                {
                    var myWorld = ChineseServers.WorldMap[wid];
                    var world = gameWorlds.GetRow(wid)!;
                    world.Name = new SeString(myWorld.Name);
                    world.IsPublic = true;
                    world.DataCenter = new Lumina.Excel.LazyRow<WorldDCGroupType>(DalamudApi.DataManager.GameData, mydc.Id);
                }
            }
        }
    }
}
