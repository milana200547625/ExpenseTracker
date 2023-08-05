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
using CM360.Pages.Contacts;

namespace CM360.Pages.Contact
{
    public class IndexModel : DI_BasePageModel
    {
        private readonly CM360.Data.ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context, IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager)
        {
        }

        public IList<Models.Contact> Contact { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var contacts = from c in Context.Contact
                           select c;

            var isAuthorized = User.IsInRole(CM360.Authorization.Constants.ContactManagersRole) ||
                               User.IsInRole(CM360.Authorization.Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            // Only approved contacts are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                contacts = contacts.Where(c => c.Status == ContactStatus.Approved
                                            || c.OwnerID == currentUserId);
            }

            Contact = await contacts.ToListAsync();
        }
    }
}
