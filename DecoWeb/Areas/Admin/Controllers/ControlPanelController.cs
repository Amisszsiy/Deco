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
    public class ControlPanelController  : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ControlPanelController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            ControlPanelVM controlPanelVM = new()
            {
                AdsType = _unitOfWork.AdsType.GetAll().ToList(),
                //AdsImages = _unitOfWork.AdsImage.GetAll().ToList()
            };

            return View(controlPanelVM);
        }

        public IActionResult UpsertType(int? id)
        {
            AdsType adsType = new();
            //Check if it's create or update
            if (id == null || id == 0)
            {
                //Go to create form
                return View(adsType);
            }
            else
            {
                //Fetch corresponding product by id and go to edit form (create form with editing product's value)
                adsType = _unitOfWork.AdsType.Get(u => u.Id == id);
                return View(adsType);
            }
        }

        [HttpPost]
        public IActionResult UpsertType(AdsType adsType)
        {
            if (ModelState.IsValid)
            {
                //Separate create or update handling logic
                if (adsType.Id == 0)
                {
                    _unitOfWork.AdsType.Add(adsType);
                }
                else
                {
                    _unitOfWork.AdsType.Update(adsType);
                }

                _unitOfWork.Save();
                
                //Fix tempdata create and update later
                TempData["success"] = "type created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {                
                return View(adsType);
            }
        }

        public IActionResult DeleteType(int? id)
        {
            AdsType? adsType = _unitOfWork.AdsType.Get(u => u.Id == id);
            if (adsType == null)
            {
                return NotFound();
            }
            _unitOfWork.AdsType.Remove(adsType);
            _unitOfWork.Save();
            TempData["success"] = "Ads deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
