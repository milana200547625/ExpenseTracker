using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CM360.Data;
using CM360.Models;
using CM360.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CM360.Pages.Expense;

namespace CM360.Pages.Expense
{
    public class EditModel : DI_BasePageModel
{
    public EditModel(
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
                                                  ExpenseOperations.Update);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Fetch Expense from DB to get OwnerID.
        var expense = await Context
            .Expense.AsNoTracking()
            .FirstOrDefaultAsync(m => m.ExpenseId == id);

        if (expense == null)
        {
            return NotFound();
        }

        var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                 User, expense,
                                                 ExpenseOperations.Update);
        if (!isAuthorized.Succeeded)
        {
            return Forbid();
        }

        Expense.OwnerID = expense.OwnerID;

        Context.Attach(Expense).State = EntityState.Modified;

        if (Expense.Status == ExpenseStatus.Approved)
        {
            // If the contact is updated after approval, 
            // and the user cannot approve,
            // set the status back to submitted so the update can be
            // checked and approved.
            var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                    Expense,
                                    ExpenseOperations.Approve);

            if (!canApprove.Succeeded)
            {
                Expense.Status = ExpenseStatus.Submitted;
            }
        }

        await Context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
}
