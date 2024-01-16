using Deco.DataAccess.Data;
using Deco.DataAccess.Repository.IRepository;
using Deco.Models;
using Deco.Models.ViewModels;
using Deco.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DecoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController  : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManager(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;
            RoleManagerVM roleManagerVM = new RoleManagerVM()
            {
                ApplicationUser = _db.ApplicationUsers.Include(u => u.Hotel).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                HotelList = _db.Hotels.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            //Get role for user because it was not mapped in model
            roleManagerVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            return View(roleManagerVM);
        }

        [HttpPost]
        public IActionResult RoleManager(RoleManagerVM roleManagerVM)
        {
            string roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagerVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            //If new role is selected
            if(!(roleManagerVM.ApplicationUser.Role == oldRole))
            {
                //Get EF core tracked user
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagerVM.ApplicationUser.Id);
                if(roleManagerVM.ApplicationUser.Role == SD.Role_Hotel)
                {
                    applicationUser.HotelId = roleManagerVM.ApplicationUser.HotelId;
                }
                else
                {
                    applicationUser.HotelId = null;
                }

                _db.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagerVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.Include(u => u.Hotel).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            //Set hotel name to empty string for regular user so that they will be shown in view via dataTable.
            foreach(var user in userList)
            {
                //Map user to role name
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id);
                //Assuming all users have role in production
                user.Role = roles.FirstOrDefault(u => u.Id == roleId.RoleId).Name;
                //if (roleId != null)
                //{
                //    user.Role = roles.FirstOrDefault(u => u.Id == roleId.RoleId).Name;
                //}

                if(user.Hotel == null)
                {
                    user.Hotel = new()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new {data = userList});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            return Json(new { success = true, message = "Deleted successfully" });
        }
        #endregion
    }
}
