using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CM360.Data;
using CM360.Models;
using CM360.Authorization;
using CM360.Pages.Expense;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CM360.Pages.Expense
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.Expense Expense { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Expense.OwnerID = UserManager.GetUserId(User);
            Expense.Status = ExpenseStatus.Submitted;

            
            var expenseRecordCheck = Context.Expense.Where(c => c.EmployeeNumber == Expense.EmployeeNumber 
            || c.Email == Expense.Email).Select(v=>v.ExpenseId).FirstOrDefault();

            if (expenseRecordCheck == 0)
            {
                
                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                            User, Expense,
                                                            ExpenseOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                Context.Expense.Add(Expense);
                await Context.SaveChangesAsync();

                return RedirectToPage("./Index");
               
                
            }
            else
            {
                ModelState.AddModelError("DuplicateContact", "Employee ID/Email already exists!");
                return Page();
            }
        }
    }
 }
