using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CM360.Data;
using CM360.Models;
using CM360.Authorization;
using CM360.Pages.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CM360.Pages.Expense
{
    public class DetailsModel : DI_BasePageModel
    {
        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public Models.Expense Expense { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Models.Expense? _expense = await Context.Expense.FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (_expense == null)
            {
                return NotFound();
            }
            Expense = _expense;

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
                               User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Expense.OwnerID
                && Expense.Status != ExpenseStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, ExpenseStatus status)
        {
            var _expense = await Context.Expense.FirstOrDefaultAsync(
                                                      m => m.ExpenseId == id);

            if (_expense == null)
            {
                return NotFound();
            }

            var contactOperation = (status == ExpenseStatus.Approved)
            ? ExpenseOperations.Approve
                                                       : ExpenseOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, _expense,
                                        contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            _expense.Status = status;
            Context.Expense.Update(_expense);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}