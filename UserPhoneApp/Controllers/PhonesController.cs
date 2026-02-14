using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserPhoneApp.Exceptions;
using UserPhoneApp.Models;
using UserPhoneApp.Services;

namespace UserPhoneApp.Controllers
{
    public class PhonesController : Controller
    {
        private readonly IPhoneService _phoneService;
        private readonly IUserService _userService;

        public PhonesController(
            IPhoneService phoneService,
            IUserService userService)
        {
            _phoneService = phoneService;
            _userService = userService;
        }

        // -------------------- INDEX --------------------

        public IActionResult Index()
        {
            var phones = _phoneService.GetAllWithUsers();
            return View(phones);
        }

        // -------------------- CREATE (GET) --------------------

        public IActionResult Create(int? userId, string? returnUrl)
        {
            ViewBag.Users = new SelectList(
                _userService.GetAll(),
                "Id",
                "Name",
                userId
            );

            ViewBag.HasUsers = _userService.GetAll().Any();
            ViewBag.ReturnUrl = returnUrl;

            return View(new Phone { UserId = userId ?? 0 });
        }

        // -------------------- CREATE (POST) --------------------

        [HttpPost]
        public IActionResult Create(Phone phone, string? returnUrl)
        {
            try
            {
                _phoneService.Add(phone);

                return string.IsNullOrEmpty(returnUrl)
                    ? RedirectToAction(nameof(Index))
                    : Redirect(returnUrl);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }

            PopulateUsers(phone.UserId);
            ViewBag.ReturnUrl = returnUrl;

            return View(phone);
        }

        private void PopulateUsers(int? selectedId = null)
        {
            ViewBag.Users = new SelectList(
                _userService.GetAll(),
                "Id",
                "Name",
                selectedId
            );
        }


        // -------------------- EDIT (GET) --------------------

        public IActionResult Edit(int id, string? returnUrl)
        {
            var phone = _phoneService.GetById(id);

            ViewBag.Users = new SelectList(
                _userService.GetAll(),
                "Id",
                "Name",
                phone.UserId
            );

            ViewBag.ReturnUrl = returnUrl;

            return View(phone);
        }

        // -------------------- EDIT (POST) --------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Phone phone, string? returnUrl)
        {
            if (id != phone.Id)
                return NotFound();

            try
            {
                _phoneService.Update(phone);

                return string.IsNullOrEmpty(returnUrl)
                    ? RedirectToAction(nameof(Index))
                    : Redirect(returnUrl);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.Users = new SelectList(
                    _userService.GetAll(),
                    "Id",
                    "Name",
                    phone.UserId
                );

                return View(phone);
            }
        }

        // -------------------- DELETE (GET) --------------------

        public IActionResult Delete(int id, string? returnUrl)
        {
            var phone = _phoneService.GetById(id);

            ViewBag.ReturnUrl = returnUrl;

            return View(phone);
        }

        // -------------------- DELETE (POST) --------------------

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id, string? returnUrl)
        {
            try
            {
                _phoneService.Delete(id);

                return string.IsNullOrEmpty(returnUrl)
                    ? RedirectToAction(nameof(Index))
                    : Redirect(returnUrl);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var phone = _phoneService.GetById(id);
                return View("Delete", phone);
            }
        }
    }
}
