using Microsoft.AspNetCore.Mvc;
using UserPhoneApp.Exceptions;
using UserPhoneApp.Models;
using UserPhoneApp.Services;

namespace UserPhoneApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // -------------------- INDEX --------------------

        public IActionResult Index()
        {
            return View(_userService.GetAll());
        }

        // -------------------- DETAILS --------------------

        public IActionResult Details(int id)
        {
            try
            {
                return View(_userService.GetById(id));
            }
            catch (BusinessException)
            {
                return NotFound();
            }
        }

        // -------------------- CREATE (GET) --------------------

        public IActionResult Create()
        {
            return View();
        }

        // -------------------- CREATE (POST) --------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            try
            {
                _userService.Add(user);

                return RedirectToAction(
                    nameof(Details),
                    new { id = user.Id }
                );
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View(user);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(user);
            }
        }


        // -------------------- EDIT (GET) --------------------

        public IActionResult Edit(int id)
        {
            try
            {
                return View(_userService.GetById(id));
            }
            catch (BusinessException)
            {
                return NotFound();
            }
        }

        // -------------------- EDIT (POST) --------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.Id)
                return NotFound();

            try
            {
                _userService.Update(user);
                return RedirectToAction(nameof(Details), new { id = user.Id });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View(user);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(user);
            }
        }

        // -------------------- DELETE (GET) --------------------

        public IActionResult Delete(int id)
        {
            try
            {
                return View(_userService.GetById(id));
            }
            catch (BusinessException)
            {
                return NotFound();
            }
        }

        // -------------------- DELETE (POST) --------------------

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _userService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
