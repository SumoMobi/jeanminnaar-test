using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp1.Models;
using Microsoft.Graph;

namespace WebApp1.Controllers
{
    [Authorize]
    //[Authorize(Roles = ActiveDirectory.RoleGroup.Admins)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ClaimsPrincipal claimsPrincipal = ClaimsPrincipal.Current;
            List<string> groupIds = User.Identities.First().Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList();

            List<string> roles = ((ClaimsIdentity)User.Identity).Claims.Where(q => q.Type == ClaimTypes.GroupSid).Select(q => q.Value).ToList();

            foreach (string role in roles)
            {
                var name = new System.Security.Principal.SecurityIdentifier(role).Translate(typeof(System.Security.Principal.NTAccount)).ToString();
            }

            bool hasClaim = ((ClaimsIdentity)User.Identity).HasClaim("groups", "AdminPortalAccess");
            hasClaim = ((ClaimsIdentity)User.Identity).HasClaim("role", "AdminPortalAccess");
            //hasClaim = ((ClaimsIdentity)User.Identity).IsInRole("AdminPortalAccess");

            foreach (string groupId in groupIds)
            {
                hasClaim = ((ClaimsIdentity)User.Identity).HasClaim("groups", groupId);
                System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(groupId);
                string test = sid.Translate(typeof(System.Security.Principal.NTAccount)).ToString();
            }
            //            ClaimsIdentity userClaimsId = claimsPrincipal.Identity as ClaimsIdentity;

            //UserGroupsAndDirectoryRoles userGroupsAndDirectoryRoles = TokenHelper.GetUsersGroupsAsync(ClaimsPrincipal.Current).Result;

            //List<string> userGroupsAndId = userGroupsAndDirectoryRoles.GroupIds;

            //string userObjectId = Util.GetSignedInUsersObjectIdFromClaims();
            //userGroupsAndId.Add(userObjectId);


            //            Microsoft.Azure.ActiveDirectory.GraphClient.Group graphClient = new Microsoft.Azure.ActiveDirectory.GraphClient.Group()
            //MSGraphClient msGraphClient = new MSGraphClient(ConfigHelper.Authority, new ADALTokenCache(Util.GetSignedInUsersObjectIdFromClaims()));

            //User user = await msGraphClient.GetMeAsync();
            //UserGroupsAndDirectoryRoles userGroupsAndDirectoryRoles = await msGraphClient.GetCurrentUserGroupsAndRolesAsync();

            //IList<Group> groups = await msGraphClient.GetCurrentUserGroupsAsync();
            //IList<DirectoryRole> directoryRoles = await msGraphClient.GetCurrentUserDirectoryRolesAsync();

            //List<System.Security.Claims.Claim> claims = HttpContext.User.Claims.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
