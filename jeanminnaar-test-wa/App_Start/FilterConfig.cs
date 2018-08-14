using System.Web;
using System.Web.Mvc;

namespace jeanminnaar_test_wa
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
