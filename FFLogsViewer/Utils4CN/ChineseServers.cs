using System.Collections.Generic;
using Dalamud.Data;
using Lumina.Excel.GeneratedSheets;

namespace Utils4CN
{
    public static class ChineseServers
    {
        /// <summary>
        /// Converts the provided romanized data center or world name into its Hanzi form.
        /// </summary>
        /// <param name="worldOrDc">The romanized name of the world or data center.</param>
        /// <returns>The Hanzi form of the name, or the input data if it is already in Hanzi or no mapping exists.</returns>
        public static string RomanizedToHanzi(string worldOrDc)
            => worldOrDc.ToLowerInvariant() switch
            {
                "luxingniao" => "陆行鸟",
                "moguli" => "莫古力",
                "maoxiaopang" => "猫小胖",
                "doudouchai" => "豆豆柴",

                "hongyuhai" => "红玉海",
                "shenyizhidi" => "神意之地",
                "lanuoxiya" => "拉诺西亚",
                "huanyingqundao" => "幻影群岛",
                "mengyachi" => "萌芽池",
                "yuzhouheyin" => "宇宙和音",
                "woxianxiran" => "沃仙曦染",
                "chenxiwangzuo" => "晨曦王座",

                "baiyinxiang" => "白银乡",
                "baijinhuanxiang" => "白金幻象",
                "shenquanhen" => "神拳痕",
                "chaofengting" => "潮风亭",
                "lvrenzhanqiao" => "旅人栈桥",
                "fuxiaozhijian" => "拂晓之间",
                "longchaoshendian" => "龙巢神殿",
                "mengyubaojing" => "梦羽宝境",

                "zishuizhanqiao" => "紫水栈桥",
                "yanxia" => "延夏",
                "jingyuzhuangyuan" => "静语庄园",
                "moduna" => "摩杜纳",
                "haimaochawu" => "海猫茶屋",
                "roufenghaiwan" => "柔风海湾",
                "hupoyuan" => "琥珀原",

                "hongchachuan2" => "红茶川",
                "yixiujiade2" => "伊修加德",
                "taiyanghaian2" => "太阳海岸",
                "yinleihu2" => "银泪湖",
                "shuijingta2" => "水晶塔",
                "saleian" => "萨雷安",
                "jialeima" => "加雷马",
                "yamawuluoti" => "亚马乌罗提",

                _ => worldOrDc,
            };

        /// <summary>
        /// Converts the provided Hanzi world or data center name into its romanized form.
        /// </summary>
        /// <param name="worldOrDc">The Hanzi name of the world or data center.</param>
        /// <returns>The romanized form of the name, or the input data if it is already romanized or no mapping exists.</returns>
        public static string HanziToRomanized(string worldOrDc)
            => worldOrDc switch
            {
                "陆行鸟" => "LuXingNiao",
                "莫古力" => "MoGuLi",
                "猫小胖" => "MaoXiaoPang",
                "豆豆柴" => "DouDouChai",

                "红玉海" => "HongYuHai",
                "神意之地" => "ShenYiZhiDi",
                "拉诺西亚" => "LaNuoXiYa",
                "幻影群岛" => "HuanYingQunDao",
                "萌芽池" => "MengYaChi",
                "宇宙和音" => "YuZhouHeYin",
                "沃仙曦染" => "WoXianXiRan",
                "晨曦王座" => "ChenXiWangZuo",

                "白银乡" => "BaiYinXiang",
                "白金幻象" => "BaiJinHuanXiang",
                "神拳痕" => "ShenQuanHen",
                "潮风亭" => "ChaoFengTing",
                "旅人栈桥" => "LvRenZhanQiao",
                "拂晓之间" => "FuXiaoZhiJian",
                "龙巢神殿" => "Longchaoshendian",
                "梦羽宝境" => "MengYuBaoJing",

                "紫水栈桥" => "ZiShuiZhanQiao",
                "延夏" => "YanXia",
                "静语庄园" => "JingYuZhuangYuan",
                "摩杜纳" => "MoDuNa",
                "海猫茶屋" => "HaiMaoChaWu",
                "柔风海湾" => "RouFengHaiWan",
                "琥珀原" => "HuPoYuan",

                "红茶川" => "HongChaChuan2",
                "伊修加德" => "YiXiuJiaDe2",
                "太阳海岸" => "TaiYangHaiAn2",
                "银泪湖" => "YinLeiHu2",
                "水晶塔" => "ShuiJingTa2",
                "萨雷安" => "SaLeiAn",
                "加雷马" => "JiaLeiMa",
                "亚马乌罗提" => "YaMaWuLuoTi",
                _ => worldOrDc,
            };

        public static Dictionary<uint, MyWorld> WorldMap = new Dictionary<uint, MyWorld> {
            { 1175, new MyWorld(1175, "晨曦王座", 1) },
            { 1174, new MyWorld(1174, "沃仙曦染", 1) },
            { 1173, new MyWorld(1173, "宇宙和音", 1) },
            { 1167, new MyWorld(1167, "红玉海", 1) },
            { 1060, new MyWorld(1060, "萌芽池", 1) },
            { 1081, new MyWorld(1081, "神意之地", 1) },
            { 1044, new MyWorld(1044, "幻影群岛", 1) },
            { 1042, new MyWorld(1042, "拉诺西亚", 1) },

            { 1121, new MyWorld(1121, "拂晓之间", 2) },
            { 1166, new MyWorld(1166, "龙巢神殿", 2) },
            { 1113, new MyWorld(1113, "旅人栈桥", 2) },
            { 1076, new MyWorld(1076, "白金幻象", 2) },
            { 1176, new MyWorld(1176, "梦羽宝境", 2) },
            { 1171, new MyWorld(1171, "神拳痕", 2) },
            { 1170, new MyWorld(1170, "潮风亭", 2) },
            { 1172, new MyWorld(1172, "白银乡", 2) },

            { 1179, new MyWorld(1179, "琥珀原", 3) },
            { 1178, new MyWorld(1178, "柔风海湾", 3) },
            { 1177, new MyWorld(1177, "海猫茶屋", 3) },
            { 1169, new MyWorld(1169, "延夏", 3) },
            { 1106, new MyWorld(1106, "静语庄园", 3) },
            { 1045, new MyWorld(1045, "摩杜纳", 3) },
            { 1043, new MyWorld(1043, "紫水栈桥", 3) },

            { 1201, new MyWorld(1201, "红茶川", 4) },
            { 1186, new MyWorld(1186, "伊修加德", 4) },
            { 1180, new MyWorld(1180, "太阳海岸", 4) },
            { 1183, new MyWorld(1183, "银泪湖", 4) },
            { 1192, new MyWorld(1192, "水晶塔", 4) },
            { 1202, new MyWorld(1202, "萨雷安", 4) },
            { 1203, new MyWorld(1203, "加雷马", 4) },
            { 1200, new MyWorld(1200, "亚马乌罗提", 4) },
        };

        public static World[] GetAllWorldsByDcRow(uint dcRow, DataManager dataManager)
        {
            var gameWorlds = dataManager.GetExcelSheet<World>()!;
            DataCenter dc;
            var worlds = new List<World>();
            if (DataCenterMap.TryGetValue(dcRow, out dc!)) {
                foreach (var wId in dc.WorldIds) {
                    var world = gameWorlds.GetRow(wId)!;
                    worlds.Add(world);
                }
            }

            return worlds.ToArray();
        }

        internal static Dictionary<uint, DataCenter> DataCenterMap = new Dictionary<uint, DataCenter> {
            { 1, new DataCenter { Id = 1, Name = "LuXingNiao", WorldIds = new uint[] { 1167, 1081, 1042, 1044, 1060, 1173, 1174, 1175 } } },
            { 2, new DataCenter { Id = 2, Name = "MoGuLi", WorldIds = new uint[] { 1076, 1171, 1170, 1113, 1121, 1166, 1176, 1172 } } },
            { 3, new DataCenter { Id = 3, Name = "MaoXiaoPang", WorldIds = new uint[] { 1043, 1169, 1106, 1045, 1177, 1178, 1179 } } },
            { 4, new DataCenter { Id = 4, Name = "DouDouChai", WorldIds = new uint[] { 1201, 1186, 1180, 1183, 1192 } } },
        };

        public class DataCenter
        {
            public uint Id;
            public string Name = string.Empty;
            public uint[] WorldIds = null!;
        }

        public class MyWorld
        {
            public uint Id;
            public string Name;
            public uint DataCenterId;
            public MyWorld(uint id, string name, uint dataCenterId)
            {
                this.Id = id;
                this.Name = name;
                this.DataCenterId = dataCenterId;
            }
        }
    }
}
