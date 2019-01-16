using System;
using System.Linq;
using Chushka.Services.Contracts;
using Chushka.Web.Controllers;
using Chushka.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Chushka.Common.Constants;

namespace Chushka.App.Controllers
{
    public class UsersController : BaseController
    {
	private readonly IUsersService usersService;

	public UsersController(IUsersService usersService)
	{
	    this.usersService = usersService;
	}

	[HttpGet]
	public IActionResult Login()
	{
	    if (User.Identity.IsAuthenticated)
	    {
		return RedirectToAction("Index", "Home");
	    }
	    return View();
	}

	[HttpPost]
	public IActionResult Login(LoginViewModel model)
	{
	    if (!ModelState.IsValid)
	    {
		ViewData[ErrorKey] = InvalidModelSateError;
		return View(model);
	    }
	    var userLoginTask = usersService.LogInUserAsync(model.Username, model.Password);
	    if (!userLoginTask.Result.Succeeded)
	    {
		ViewData[ErrorKey] = "Invalid credentials";
		return View();
	    }
	    return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	[Authorize]
	public IActionResult Logout()
	{
	    usersService.LogOutUserAsync().Wait();
	    return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	public IActionResult Register()
	{
	    if (User.Identity.IsAuthenticated)
	    {
		return RedirectToAction("Index", "Home");
	    }
	    return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Register(RegisterViewModel model)
	{
	    if (!ModelState.IsValid)
	    {
		ViewData[ErrorKey] = InvalidModelSateError;
		return View(model);
	    }
	    var userRegistrationTask = usersService
		.AddUserAsync(model.Username, model.Password, model.Email, model.FullName);
	    if (!userRegistrationTask.Result.Succeeded)
	    {
		ViewData[ErrorKey] = string.Join(Environment.NewLine,
		    userRegistrationTask.Result.Errors.Select(e => e.Description));
		return View();
	    }
	    return RedirectToAction("Login", "Users");
	}
    }
}
