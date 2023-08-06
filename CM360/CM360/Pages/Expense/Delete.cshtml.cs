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
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Models.Expense Expense { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Models.Expense? _expense = await Context.Expense.FirstOrDefaultAsync(
                                                 m => m.ExpenseId == id);

            if (_expense == null)
            {
                return NotFound();
            }
            Expense = _expense;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Expense,
                                                     ExpenseOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var contact = await Context
                .Expense.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ExpenseId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, contact,
                                                     ExpenseOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Expense.Remove(contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}