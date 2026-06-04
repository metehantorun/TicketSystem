using Microsoft.AspNetCore.Identity;
using TicketSystem.Models;

namespace TicketSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Rolleri oluştur: Admin ve User (Müşteri)
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin kullanıcısı oluştur
            if (await userManager.FindByEmailAsync("admin@ticketsystem.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@ticketsystem.com",
                    Email = "admin@ticketsystem.com",
                    FullName = "Sistem Yöneticisi",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Demo müşteri kullanıcısı oluştur
            if (await userManager.FindByEmailAsync("musteri@ticketsystem.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "musteri@ticketsystem.com",
                    Email = "musteri@ticketsystem.com",
                    FullName = "Demo Müşteri",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "User123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
