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
                AdsImages = _unitOfWork.AdsImage.GetAll().ToList()
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

        public IActionResult UpsertAds(int? id)
        {
            AdsImageVM adsImageVM = new()
            {
                AdsTypeList = _unitOfWork.AdsType.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),
                AdsImage = new AdsImage()
            };

            if (id == null || id == 0)
            {
                return View(adsImageVM);
            }
            else
            {
                //Fetch corresponding product by id and go to edit form (create form with editing product's value)
                adsImageVM.AdsImage = _unitOfWork.AdsImage.Get(u => u.Id == id, includeProperties: "AdsType");
                return View(adsImageVM);
            }
        }

        [HttpPost]
        public IActionResult UpsertAds(AdsImageVM adsImageVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //Uploaded image handling
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    //Preparing saving path
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string path = @"images\ads\type-" + adsImageVM.AdsImage.AdsTypeId;
                    string finalPath = Path.Combine(wwwRootPath, path);

                    //Create folder for corresponding product image
                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    //Save images to destination
                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    //Set image url
                    adsImageVM.AdsImage.ImageUrl = @"\" + path + @"\" + fileName;
                }

                //Separate create or update handling logic
                if (adsImageVM.AdsImage.Id == 0)
                {
                    _unitOfWork.AdsImage.Add(adsImageVM.AdsImage);

                    TempData["success"] = "Ads created successfully";
                }
                else
                {
                    _unitOfWork.AdsImage.Update(adsImageVM.AdsImage);

                    TempData["success"] = "Ads updated successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            else
            {
                adsImageVM.AdsTypeList = _unitOfWork.AdsType.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                return View(adsImageVM);
            }
        }

        public IActionResult DeleteAds(int? id)
        {
            var deleteAds = _unitOfWork.AdsImage.Get(u => u.Id == id);
            if (deleteAds == null)
            {
                return NotFound();
            }

            string path = @"images\ads\type-" + deleteAds.AdsTypeId;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, path);

            var deleteImagePath = Path.Combine(_webHostEnvironment.WebRootPath, deleteAds.ImageUrl.TrimStart('\\'));

            if (Directory.Exists(finalPath))
            {
                if (System.IO.File.Exists(deleteImagePath))
                {
                    System.IO.File.Delete(deleteImagePath);
                }

                if (Directory.GetFiles(finalPath).Count() < 1)
                {
                    Directory.Delete(finalPath);
                }
            }

            _unitOfWork.AdsImage.Remove(deleteAds);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

    }
}
