using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.PerformanceInsideCloud
{
    public class Common
    {
        static string ids = "'461549','453646','447176','452725','448274','454302','455040','451777','713510','448872','451954','791920','449791','450367','455288','449953','448675','727377','447764','461598','453959','450480','448178','450057','694417','446988','447249','455617','451611','449647','450601','448011','452529','451578','448273','447464','448403','450394','448364'";
                            //16       17      17          9       9       13         11      10        21       5        3         3      75        82       74       74       9         17       15      26        11      7        6        6       6          26     10        9       14        14      11        19        8       7         10      9       9         9         9
                            //446589 32, 446612 23, 447788 56, 447666 46, 448122 45, 446684 42, 448318 40, 
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
