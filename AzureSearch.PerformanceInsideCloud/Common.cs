using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.PerformanceInsideCloud
{
    public class Common
    {
        static string ids = "'454534','455095','462632','454395','447164','452269','449526','448552','447742','455530','453024','461549','453646','447176','452725','448274','454302','455040','451777','713510','448872','451954','791920','449791','450367','455288','449953','448675','727377','447764','461598','453959','450480','448178','450057','694417','446988','447249','455617','451611','449647','450601','448011','452529','451578','448273','447464','448403','450394','448364'";
        public static List<string> IdsList
        {
            get
            {
                string[] idsArray = ids.Split(',');
                List<string> idsList = new List<string>(idsArray.Length);
                for (int i = 0; i < 25; i++)
                {
                    idsList.Add(idsArray[i].Substring(1, idsArray[i].Length - 2));
                }
                return idsList; 
            }
        }
        public static string IdsInClause
        {
            get
            {
                string[] ids = IdsList.Select(i => "'" + i + "'").ToArray();
                string idsString = string.Join(",", ids);
                return idsString;
            }
        }
    }
}
