using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using backend.Seeders;
using backend.Persistences;
using backend.Model.AppEntity;
using backend.Model.IdEntity;
using backend.Identity;

namespace backend.Persistences
{
    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger, 
            AppDbContext context, 
            UserManager<ApplicationUser> userManager
            //, RoleManager<IdentityRole> roleManager
            )
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            //_roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                //if (_context.Database.IsSqlServer())
                //{
                await _context.Database.MigrateAsync();
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // Console.WriteLine("a");
            await AppDbContextSeeder.SeedParentTypeAsync(_context);
            await AppDbContextSeeder.SeedActuatorTypeAsync(_context);
            await AppDbContextSeeder.SeedPlantAsync(_context);
            await AppDbContextSeeder.SeedLandAsync(_context);
            await AppDbContextSeeder.SeedRegionAsync(_context);
            await AppDbContextSeeder.SeedIdIotAsync(_context);
            await AppDbContextSeeder.SeedMiniPcAsync(_context);
            await AppDbContextSeeder.SeedMicroAsync(_context);
            await AppDbContextSeeder.SeedSensorAsync(_context);
            await AppDbContextSeeder.SeedDataAsync(_context);
            await AppDbContextSeeder.SeedRoleAsync(_context);
            await AppDbContextSeeder.SeedDiseaseAsync(_context);
            await AppDbContextSeeder.SeedScheduleTagAsync(_context);
            //await AppIdentityDbContextSeed.SeedDefaultUserAsync(_userManager);
        }
    }
}