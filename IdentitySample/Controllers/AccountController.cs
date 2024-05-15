﻿using IdentitySample.Models;
using IdentitySample.Repositories;
using IdentitySample.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMessageSender _messageSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMessageSender messageSender)
        {
            _userManager = userManager;
            _signInManager = signInManager; 
            _messageSender = messageSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegsiterViewModel regsiter)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = regsiter.UserName,
                    Email = regsiter.Email,
                    City = "Mahabad"
                };

                var result = await _userManager.CreateAsync(user, regsiter.Password);

                if (result.Succeeded)
                {
                    #region Email Confirma 

                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var emailMessage = Url.Action("ConfirmEmail", "Account", new { username = user.UserName, token = emailConfirmationToken }, Request.Scheme);

                    await _messageSender.SendEmailAsync(regsiter.Email, "Email confirmation", emailMessage);

                    #endregion

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(regsiter);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };


            ViewData["returnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            login.ReturnUrl = returnUrl;

            login.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ViewData["returnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ViewData["ErrorMessage"] = "اکانت شما به دلیل پنج بار ورود ناموفق به مدت پنج دقیقه قفل شده است";
                    return View(login);
                }

                ModelState.AddModelError("", "رمزعبور یا نام کاربری اشتباه است");
            }
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }

            return Json("ایمیل وارد شده از قبل موجود است");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsUserNameInUse(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return Json(true);
            }

            return Json("نام کاربری وارد شده از قبل موجود است");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userName, string token)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return Content(result.Succeeded ? "Email Confirmed" : "Email Not Confirmed");
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            ViewData["returnUrl"] = returnUrl;

            returnUrl = (returnUrl != null && Url.IsLocalUrl(returnUrl)) ? returnUrl : Url.Content("~/");

            var loginViewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError("", $"Error : {remoteError}");
                return View("Login", loginViewModel);
            }

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                ModelState.AddModelError("ErrorLoadingExternalLoginInfo", $"مشکلی پیش آمد");
                return View("Login", loginViewModel);
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            //first time login 

            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    //var userName = email.Split('@')[0];

                    //user = new ApplicationUser()
                    //{
                    //    UserName = (userName.Length <= 10 ? userName : userName.Substring(0, 10)),
                    //    Email = email,
                    //    EmailConfirmed = true,
                    //    City = "Mahabad"
                    //};

                    //await _userManager.CreateAsync(user);

                    return View();
                }

                await _userManager.AddLoginAsync(user, externalLoginInfo);

                await _signInManager.SignInAsync(user, false);

                return Redirect(returnUrl);
            }

            ViewData["ErrorMessage"] = $"دریافت کرد {externalLoginInfo.LoginProvider} نمیتوان اطلاعاتی از";

            return View("Login", loginViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ExternalLoginCallBack(ExternalLoginCallBackViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var loginViewModel = new LoginViewModel()
                {
                    ReturnUrl = returnUrl,
                    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                };

                var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

                if (externalLoginInfo?.Principal.FindFirstValue(ClaimTypes.Email) == null)
                {
                    ModelState.AddModelError("ErrorLoadingExternalLoginInfo", $"مشکلی پیش آمد");
                    return View("Login", loginViewModel);
                }

                var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

                var user = await _userManager.FindByEmailAsync(email);

                var result = new IdentityResult();

                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        Email = email,
                        UserName = model.UserName,
                        EmailConfirmed = true
                    };

                    if (!string.IsNullOrWhiteSpace(model.Password))
                    {
                        result = await _userManager.CreateAsync(user, model.Password);
                    }
                    else
                    {
                        result = await _userManager.CreateAsync(user);
                    }
                }

                if (result.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, externalLoginInfo);

                    await _signInManager.SignInAsync(user, false);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

    }
}
