using Deco.DataAccess.Repository.IRepository;
using Deco.Models;
using Deco.Models.ViewModels;
using Deco.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace DecoWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
            foreach(var item in ShoppingCartVM.ShoppingCartList)
            {
                item.Product.ProductImages = productImages.Where(u => u.ProductId == item.ProductId).ToList();
                item.UnitPrice = GetPrice(item);
                ShoppingCartVM.OrderHeader.OrderTotal += (item.UnitPrice * item.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            //Fix repeated code later.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.FirstName = ShoppingCartVM.OrderHeader.ApplicationUser.FirstName;
            ShoppingCartVM.OrderHeader.LastName = ShoppingCartVM.OrderHeader.ApplicationUser.LastName;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.Province = ShoppingCartVM.OrderHeader.ApplicationUser.Province;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                item.UnitPrice = GetPrice(item);
                ShoppingCartVM.OrderHeader.OrderTotal += (item.UnitPrice * item.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			//Fix repeated code later.
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Populate these values again because view doesn't contain all of inputs.
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId, includeProperties: "Product");
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            //If assign nav property in updating obj, fk obj will be created in its table too.
            //To avoid creating same entity which process same id that lead to sql exception, use new var that not going to be updated instead.
            //ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            foreach (var item in ShoppingCartVM.ShoppingCartList)
			{
				item.UnitPrice = GetPrice(item);
				ShoppingCartVM.OrderHeader.OrderTotal += (item.UnitPrice * item.Count);
			}

            //HotelId can be null for regular customer.
            if (applicationUser.HotelId.GetValueOrDefault() == 0)
            {
                //Set payment status for regular customer
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.Payment_Pending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.Status_Pending;
            }
            else
            {
				//Set payment status for hotel
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.Payment_DelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.Status_Approved;
			}

            //Save first to generate OrderHeader.Id
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach(var item in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    OrderedPrice = item.UnitPrice,
                    Count = item.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                //Why save in loop?
                _unitOfWork.Save();
            }

			if (applicationUser.HotelId.GetValueOrDefault() == 0)
			{
                //Add this to appsetting later
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                //Create session obj using Stripe API.
                //Configure payment details in its obj.
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/orderconfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                //Add each buying item into a list
                foreach(var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            //Shifting decimal point by 2.
                            UnitAmount = (long)(item.UnitPrice * 100),
                            Currency = "thb",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                //Create session with configured options
                var service = new SessionService();
                Session session = service.Create(options);

                //Save session id to DB first.
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }

			return RedirectToAction(nameof(OrderConfirmation),new { id = ShoppingCartVM.OrderHeader.Id });
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

            //Update payment status for regular customer
            if(orderHeader.PaymentStatus != SD.Payment_DelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                //After payment is done, update payment status in DB
                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.Status_Approved, SD.Payment_Approved);
                    _unitOfWork.Save();
                }

                HttpContext.Session.Remove(SD.SessionCart);
            }

            //After payment succeed, remove items in cart
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
        }

		public IActionResult Increase(int itemId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == itemId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int itemId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == itemId, tracked: true);
            //Remove because only have one
            if(cartFromDb.Count <= 1)
            {
                //Decrease item in cart count by 1 for session cart  
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
                //Then remove cart item in DB
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int itemId)
        {
            /**Tracked = true for retriving entity (cartFromDb) 
             * because it is used somewhere before it interact db again**/
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == itemId, tracked: true);

            //Decrease item in cart count  by 1 for session cart
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private double GetPrice(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count < 10)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                return shoppingCart.Product.SetPrice;
            }
        }
    }
}
