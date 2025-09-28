using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Data;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Services;
using JEGASolutions.ExtraHours.Infrastructure.Repositories;
using JEGASolutions.ExtraHours.Core.Interfaces;

namespace JEGASolutions.ExtraHours.Tests
{
    /// <summary>
    /// Tests to verify multi-tenancy implementation in Extra-Hours module
    /// </summary>
    public class MultiTenancyTests
    {
        private AppDbContext _context;
        private ITenantContextService _tenantContextService;
        private IExtraHourRepository _extraHourRepository;
        private IExtraHourService _extraHourService;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _tenantContextService = new TenantContextService();
            _extraHourRepository = new ExtraHourRepository(_context, _tenantContextService);
            _extraHourService = new ExtraHourService(_extraHourRepository, null, _tenantContextService);

            // Seed test data
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTestData()
        {
            // Create test employees for different tenants
            var employee1 = new Employee
            {
                id = 1,
                name = "Employee 1",
                TenantId = 1
            };

            var employee2 = new Employee
            {
                id = 2,
                name = "Employee 2",
                TenantId = 2
            };

            _context.employees.AddRange(employee1, employee2);

            // Create test extra hours for different tenants
            var extraHour1 = new ExtraHour
            {
                registry = 1,
                id = 1,
                date = DateTime.Now,
                startTime = TimeSpan.FromHours(8),
                endTime = TimeSpan.FromHours(10),
                diurnal = 2.0,
                TenantId = 1
            };

            var extraHour2 = new ExtraHour
            {
                registry = 2,
                id = 2,
                date = DateTime.Now,
                startTime = TimeSpan.FromHours(8),
                endTime = TimeSpan.FromHours(10),
                diurnal = 2.0,
                TenantId = 2
            };

            _context.extraHours.AddRange(extraHour1, extraHour2);
            _context.SaveChanges();
        }

        [Test]
        public void TestTenantIsolation_ExtraHours()
        {
            // Test tenant 1
            _tenantContextService.SetCurrentTenantId(1);
            var tenant1ExtraHours = _extraHourService.GetAllAsync().Result;
            
            Assert.That(tenant1ExtraHours.Count(), Is.EqualTo(1));
            Assert.That(tenant1ExtraHours.First().TenantId, Is.EqualTo(1));

            // Test tenant 2
            _tenantContextService.SetCurrentTenantId(2);
            var tenant2ExtraHours = _extraHourService.GetAllAsync().Result;
            
            Assert.That(tenant2ExtraHours.Count(), Is.EqualTo(1));
            Assert.That(tenant2ExtraHours.First().TenantId, Is.EqualTo(2));
        }

        [Test]
        public void TestTenantContextService()
        {
            // Test setting and getting tenant ID
            _tenantContextService.SetCurrentTenantId(5);
            Assert.That(_tenantContextService.GetCurrentTenantId(), Is.EqualTo(5));
            Assert.That(_tenantContextService.HasTenantId(), Is.True);
        }

        [Test]
        public void TestTenantContextService_ThrowsException_WhenNoTenantSet()
        {
            // Test that exception is thrown when no tenant is set
            Assert.Throws<InvalidOperationException>(() => _tenantContextService.GetCurrentTenantId());
        }

        [Test]
        public void TestExtraHourService_ThrowsException_WhenNoTenantContext()
        {
            // Test that service throws exception when no tenant context is set
            Assert.Throws<InvalidOperationException>(() => _extraHourService.GetAllAsync().Result);
        }

        [Test]
        public void TestAddExtraHour_SetsTenantId()
        {
            // Set tenant context
            _tenantContextService.SetCurrentTenantId(3);

            var newExtraHour = new ExtraHour
            {
                id = 3,
                date = DateTime.Now,
                startTime = TimeSpan.FromHours(8),
                endTime = TimeSpan.FromHours(10),
                diurnal = 2.0
            };

            var savedExtraHour = _extraHourService.AddExtraHourAsync(newExtraHour).Result;

            Assert.That(savedExtraHour.TenantId, Is.EqualTo(3));
        }
    }
}
