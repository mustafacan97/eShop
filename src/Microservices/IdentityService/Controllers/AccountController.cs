using IdentityService.Entities;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        #region Constants and Fields

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        #endregion

        #region Constructors and Destructors

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion

        #region Public Methods

        [HttpPost(Name = "Register")]
        public async Task<bool> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)            
                return false;

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            return result.Succeeded;
        }

        [HttpPost(Name = "Login")]
        public async Task<bool> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return false;

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, true);
            return result.Succeeded;
        }

        public async Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        #endregion
    }
}
