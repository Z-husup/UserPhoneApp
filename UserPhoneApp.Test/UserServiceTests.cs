using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using UserPhoneApp.Data;
using UserPhoneApp.Exceptions;
using UserPhoneApp.Models;
using UserPhoneApp.Services;

[TestFixture]
public class UserServiceTests
{
    private AppDbContext _context = null!;
    private UserService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new UserService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    // ---------- ADD ----------

    [Test]
    public void Add_ShouldCreateUser_WhenValid()
    {
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        _service.Add(user);

        Assert.That(_context.Users.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Add_ShouldThrow_WhenNameEmpty()
    {
        var user = new User
        {
            Name = "",
            Email = "john@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        Assert.Throws<ValidationException>(() => _service.Add(user));
    }

    [Test]
    public void Add_ShouldThrow_WhenEmailEmpty()
    {
        var user = new User
        {
            Name = "John",
            Email = "",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        Assert.Throws<ValidationException>(() => _service.Add(user));
    }

    [Test]
    public void Add_ShouldThrow_WhenEmailDuplicate()
    {
        _context.Users.Add(new User
        {
            Name = "Existing",
            Email = "test@mail.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        });
        _context.SaveChanges();

        var user = new User
        {
            Name = "John",
            Email = "test@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        Assert.Throws<ValidationException>(() => _service.Add(user));
    }

    [Test]
    public void Add_ShouldThrow_WhenDateInFuture()
    {
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            DateOfBirth = DateTime.Today.AddDays(1)
        };

        Assert.Throws<ValidationException>(() => _service.Add(user));
    }

    // ---------- GET ----------

    [Test]
    public void GetById_ShouldReturnUser_WhenExists()
    {
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var result = _service.GetById(user.Id);

        Assert.That(result.Name, Is.EqualTo("John"));
    }

    [Test]
    public void GetById_ShouldThrow_WhenNotFound()
    {
        Assert.Throws<NotFoundException>(() => _service.GetById(999));
    }

    // ---------- UPDATE ----------

    [Test]
    public void Update_ShouldModifyUser_WhenValid()
    {
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        user.Name = "Updated";

        _service.Update(user);

        var updated = _context.Users.First();
        Assert.That(updated.Name, Is.EqualTo("Updated"));
    }

    [Test]
    public void Update_ShouldThrow_WhenUserNotFound()
    {
        var user = new User
        {
            Id = 999,
            Name = "John",
            Email = "john@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        Assert.Throws<NotFoundException>(() => _service.Update(user));
    }

    // ---------- DELETE ----------

    [Test]
    public void Delete_ShouldRemoveUser_WhenExists()
    {
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            DateOfBirth = new DateTime(2000, 1, 1)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        _service.Delete(user.Id);

        Assert.That(_context.Users.Count(), Is.EqualTo(0));
    }

    [Test]
    public void Delete_ShouldThrow_WhenNotFound()
    {
        Assert.Throws<NotFoundException>(() => _service.Delete(999));
    }
}
