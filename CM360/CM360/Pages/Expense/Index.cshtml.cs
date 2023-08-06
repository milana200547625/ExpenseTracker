using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CM360.Data;
using CM360.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CM360.Pages.Expense;

namespace CM360.Pages.Expense
{
    public class IndexModel : DI_BasePageModel
    {
        private readonly CM360.Data.ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context, IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager)
        {
        }
        public string CurrentFilter { get; set; }
        public IList<Models.Expense> Expense { get;set; } = default!;

        public async Task OnGetAsync(string searchString)
        {
            CurrentFilter = searchString;

            var expenses = from c in Context.Expense
                           select c;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                               
                ExpenseStatus search = ExpenseStatus.Approved;
                bool isValid = true;

                if(searchString == ExpenseStatus.Submitted.ToString())
                {
                    search = ExpenseStatus.Submitted;
                }else if (searchString == ExpenseStatus.Rejected.ToString())
                {
                    search = ExpenseStatus.Rejected;
                }else if(search.ToString() != searchString)
                {
                   ModelState.AddModelError("InvalidStatus", "Invalid status");
                    isValid = false;
                  
                }
                expenses = isValid? expenses.Where(s => s.Status == search) : expenses;
            }
           

            var isAuthorized = User.IsInRole(CM360.Authorization.Constants.ContactManagersRole) ||
                               User.IsInRole(CM360.Authorization.Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            // Only approved contacts are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                expenses = expenses.Where(c => c.Status == ExpenseStatus.Approved
                                            || c.OwnerID == currentUserId);
            }

            Expense = await expenses.ToListAsync();
        }
    }
}
