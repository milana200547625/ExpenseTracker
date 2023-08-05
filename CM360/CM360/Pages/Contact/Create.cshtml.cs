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
using CM360.Pages.Contacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CM360.Pages.Contact
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
        public Models.Contact Contact { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Contact.OwnerID = UserManager.GetUserId(User);
            Contact.Status = ContactStatus.Submitted;

            var contacts = from c in Context.Contact
                           select c;

            contacts = contacts.Where(c => c.EmployeeNumber == Contact.EmployeeNumber
                                            || c.Email == Contact.Email);
            int contactCheck = Context.Contact.Where(c => c.EmployeeNumber == Contact.EmployeeNumber 
            || c.Email == Contact.Email).Select(v=>v.ContactId).FirstOrDefault();
            if (contactCheck == 0)
            {

                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                            User, Contact,
                                                            ContactOperations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

                Context.Contact.Add(Contact);
                await Context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
            else
            {
                ModelState.AddModelError("DuplicateContact", "Employee already exists!");
                return Page();
            }
        }
    }
 }
