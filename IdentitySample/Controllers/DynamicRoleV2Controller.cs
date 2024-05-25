using IdentitySample.Models.Context;
using IdentitySample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using IdentitySample.Repositories;
using IdentitySample.ViewModels.DynamicRoleV2;
using IdentitySample.Authorization.ClaimBasedAuthorization.MvcUserAccessClaims;

namespace IdentitySample.Controllers
{
    public class DynamicRoleV2Controller : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly AppDbContext _dbContext;

        public DynamicRoleV2Controller(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> EditUserAccess(string id)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == id)) return NotFound();

            var model = await PrepareEditUserAccessViewModelAsync(id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserAccess(EditUserAccessViewModel model)
        {
            if (!await _userManager.Users.AnyAsync(u => u.Id == model.UserId)) return NotFound();

            if (ModelState.IsValid)
            {
                if (!IsEditUserAccessModelValid(model))
                {
                    model = await PrepareEditUserAccessViewModelAsync(model.UserId);
                    ModelState.AddModelError("", "مقدایر دسترسی معتبر نمیباشد، لطفا این صفحه را دوباره باز بکنید.");
                    return View(model);
                }

                await EditUserAccessInternalAsync(model);
                await _userManager.UpdateSecurityStampAsync(await _userManager.FindByIdAsync(model.UserId));
            }

            model = await PrepareEditUserAccessViewModelAsync(model.UserId);
            return View(model);
        }

        #region Helpers

        private async Task<EditUserAccessViewModel> PrepareEditUserAccessViewModelAsync(string userId)
        {
            var userName = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .SingleAsync();

            var userClaimValues = await _dbContext.UserClaims
                .Where(c => c.UserId == userId && c.ClaimType == ClaimStore.UserAccess)
                .Select(c => c.ClaimValue)
                .ToListAsync();

            var model = new EditUserAccessViewModel
            {
                UserId = userId,
                UserName = userName,
                UserClaimValues = AllControllersClaimValues.AllClaimValues
                    .Select(c => new EditUserAccessClaimValuesDto
                    {
                        ClaimValue = c.claimValueEnglish,
                        ClaimValuePersian = c.claimValuePersian,
                        IsSelected = userClaimValues.Contains(c.claimValueEnglish)
                    }).ToList()
            };

            return model;
        }

        private async Task EditUserAccessInternalAsync(EditUserAccessViewModel model)
        {
            var userClaims = await _dbContext.UserClaims
                .Where(c => c.UserId == model.UserId && c.ClaimType == ClaimStore.UserAccess)
                .ToListAsync();

            _dbContext.UserClaims.RemoveRange(userClaims);

            await _dbContext.UserClaims.AddRangeAsync(model.UserClaimValues.Where(c => c.IsSelected)
                 .Select(c => new IdentityUserClaim<string>
                 {
                     UserId = model.UserId,
                     ClaimType = ClaimStore.UserAccess,
                     ClaimValue = c.ClaimValue
                 }));

            await _dbContext.SaveChangesAsync();
        }

        private bool IsEditUserAccessModelValid(EditUserAccessViewModel model)
        {
            return model.UserClaimValues.All(c =>
                AllControllersClaimValues.AllClaimValues.Any(validClaim =>
                    validClaim.claimValueEnglish == c.ClaimValue));
        }

        #endregion
    }

}
