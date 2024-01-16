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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManager(string userId)
        {
            RoleManagerVM roleManagerVM = new RoleManagerVM()
            {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties:"Hotel"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                HotelList = _unitOfWork.Hotel.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            //Get role for user because it was not mapped in model
            roleManagerVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser
                .Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();

            return View(roleManagerVM);
        }

        [HttpPost]
        public IActionResult RoleManager(RoleManagerVM roleManagerVM)
        {
            string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser
                .Get(u => u.Id == roleManagerVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagerVM.ApplicationUser.Id);

            //If new role is selected
            if (!(roleManagerVM.ApplicationUser.Role == oldRole))
            {
                if(roleManagerVM.ApplicationUser.Role == SD.Role_Hotel)
                {
                    applicationUser.HotelId = roleManagerVM.ApplicationUser.HotelId;
                }
                else
                {
                    applicationUser.HotelId = null;
                }

                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagerVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                //If change only hotel
                if(oldRole == SD.Role_Hotel && applicationUser.HotelId != roleManagerVM.ApplicationUser.HotelId)
                {
                    applicationUser.HotelId = roleManagerVM.ApplicationUser.HotelId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Hotel").ToList();

            //Set hotel name to empty string for regular user so that they will be shown in view via dataTable.
            foreach(var user in userList)
            {
                //Assuming all users have role in production
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

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
        #endregion
    }
}
