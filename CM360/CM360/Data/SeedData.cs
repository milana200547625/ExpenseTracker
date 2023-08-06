using CM360.Data;
using CM360.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace CM360.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@expenseteam.com");
                await EnsureRole(serviceProvider, adminID, Authorization.Constants.ContactAdministratorsRole);

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw, "manager@expenseteam.com");
                await EnsureRole(serviceProvider, managerID, Authorization.Constants.ContactManagersRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Expense.Any())
            {
                return;   // DB has been seeded
            }

            context.Expense.AddRange(
                new Expense
                {
                    Name = "Debra Garcia",
                    EmployeeNumber = 1001,
                    ExpenseType = ExpenseCategory.InternetBill,
                    Amount = (decimal?)35.00,
                    Email = "debra@gmail.com",
                    Status = ExpenseStatus.Approved,
                    OwnerID = adminID
                },
                new Expense
                {
                    Name = "Thorsten Weinrich",
                    EmployeeNumber = 1002,
                    ExpenseType = ExpenseCategory.TeamLunch,
                    Amount = (decimal?)800.00,
                    Email = "thorsten@yahoo.com",
                    Status = ExpenseStatus.Submitted,
                    OwnerID = adminID
                },
                new Expense
                {
                    Name = "Yuhong Li",
                    EmployeeNumber = 8923,
                    ExpenseType = ExpenseCategory.Travel,
                    Amount = (decimal?)800.00,
                    Email = "yuhong@yahoo.com",
                    Status = ExpenseStatus.Rejected,
                    OwnerID = adminID
                },
                new Expense
                {
                    Name = "Jon Orton",
                    EmployeeNumber = 4005,
                    ExpenseType = ExpenseCategory.Relocation,
                    Amount = (decimal?)500.00,
                    Email = "jon@ymail.com",
                    Status = ExpenseStatus.Submitted,
                    OwnerID = adminID
                },
                new Expense
                {
                    Name = "Diliana Alexieva-Bosseva",
                    EmployeeNumber = 7800,
                    ExpenseType = ExpenseCategory.InternetBill,
                    Amount = (decimal?)75.00,
                    Email = "diliana@gmail.com",
                    Status = ExpenseStatus.Submitted,
                    OwnerID = adminID
                });
            context.SaveChanges();
        }
    }
}
