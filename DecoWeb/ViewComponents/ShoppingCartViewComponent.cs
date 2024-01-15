using Deco.DataAccess.Repository.IRepository;
using Deco.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DecoWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Retriving shopping cart count
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //User logged in
            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    //Get count of item type in cart and store in session
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(
                            u => u.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
