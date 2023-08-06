using CM360.Models;
using CM360.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace CM360.Authorization
{
    public class ExpenseIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Expense>
    {
        UserManager<IdentityUser> _userManager;

        public ExpenseIsOwnerAuthorizationHandler(UserManager<IdentityUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Expense resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != CM360.Authorization.Constants.CreateOperationName &&
                requirement.Name != CM360.Authorization.Constants.ReadOperationName &&
                requirement.Name != CM360.Authorization.Constants.UpdateOperationName &&
                requirement.Name != CM360.Authorization.Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}