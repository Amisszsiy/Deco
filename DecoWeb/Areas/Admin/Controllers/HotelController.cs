using Deco.DataAccess.Repository.IRepository;
using Deco.Models;
using Deco.Models.ViewModels;
using Deco.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DecoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HotelController  : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HotelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Hotel> Hotels = _unitOfWork.Hotel.GetAll().ToList();

            return View(Hotels);
        }

        public IActionResult Upsert(int? id)
        {
            //Check if it's create or update
            if(id == null || id == 0)
            {
                //Go to create form
                return View(new Hotel());
            }
            else
            {
                //Fetch corresponding Hotel by id and go to edit form (create form with editing Hotel's value)
                Hotel hotelObj = _unitOfWork.Hotel.Get(u => u.Id == id);
                return View(hotelObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Hotel hotelObj)
        {
            if (ModelState.IsValid)
            {

                //Separate create or update handling logic
                if(hotelObj.Id == 0)
                {
                    _unitOfWork.Hotel.Add(hotelObj);
                }
                else
                {
                    _unitOfWork.Hotel.Update(hotelObj);
                }

                _unitOfWork.Save();
                //Fix tempdata create and update later
                TempData["success"] = "Hotel created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(hotelObj);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Hotel> Hotels = _unitOfWork.Hotel.GetAll().ToList();
            return Json(new {data = Hotels});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var deleteHotel = _unitOfWork.Hotel.Get(u => u.Id == id);
            if(deleteHotel == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Hotel.Remove(deleteHotel);
            _unitOfWork.Save();

            List<Hotel> Hotels = _unitOfWork.Hotel.GetAll(includeProperties: "Category").ToList();
            return Json(new { success = true, message = "Deleted successfully" });
        }
        #endregion
    }
}
