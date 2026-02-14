using Microsoft.EntityFrameworkCore;
using UserPhoneApp.Data;
using UserPhoneApp.Exceptions;
using UserPhoneApp.Models;
using UserPhoneApp.Services;

[TestFixture]
public class PhoneServiceTests
{
    private AppDbContext _context = null!;
    private PhoneService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new PhoneService(_context);

        // Добавляем одного пользователя для тестов
        _context.Users.Add(new User
        {
            Name = "John",
            Email = "john@mail.com"
        });

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    // ---------- ADD ----------

    [Test]
    public void Add_ShouldCreatePhone_WhenValid()
    {
        var userId = _context.Users.First().Id;

        var phone = new Phone
        {
            PhoneNumber = "1234567",
            UserId = userId
        };

        _service.Add(phone);

        Assert.That(_context.Phones.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Add_ShouldThrow_WhenPhoneEmpty()
    {
        var userId = _context.Users.First().Id;

        var phone = new Phone
        {
            PhoneNumber = "",
            UserId = userId
        };

        Assert.Throws<ValidationException>(() => _service.Add(phone));
    }

    [Test]
    public void Add_ShouldThrow_WhenPhoneTooShort()
    {
        var userId = _context.Users.First().Id;

        var phone = new Phone
        {
            PhoneNumber = "123",
            UserId = userId
        };

        Assert.Throws<ValidationException>(() => _service.Add(phone));
    }

    [Test]
    public void Add_ShouldThrow_WhenUserNotExists()
    {
        var phone = new Phone
        {
            PhoneNumber = "1234567",
            UserId = 999
        };

        Assert.Throws<ValidationException>(() => _service.Add(phone));
    }

    [Test]
    public void Add_ShouldThrow_WhenDuplicatePhone()
    {
        var userId = _context.Users.First().Id;

        _context.Phones.Add(new Phone
        {
            PhoneNumber = "1234567",
            UserId = userId
        });
        _context.SaveChanges();

        var phone = new Phone
        {
            PhoneNumber = "1234567",
            UserId = userId
        };

        Assert.Throws<ValidationException>(() => _service.Add(phone));
    }

    // ---------- GET ----------

    [Test]
    public void GetById_ShouldReturnPhone_WhenExists()
    {
        var userId = _context.Users.First().Id;

        var phone = new Phone
        {
            PhoneNumber = "1234567",
            UserId = userId
        };

        _context.Phones.Add(phone);
        _context.SaveChanges();

        var result = _service.GetById(phone.Id);

        Assert.That(result.PhoneNumber, Is.EqualTo("1234567"));
    }

    [Test]
    public void GetById_ShouldThrow_WhenNotFound()
    {
        Assert.Throws<NotFoundException>(() => _service.GetById(999));
    }

    // ---------- UPDATE ----------

    [Test]
    public void Update_ShouldModifyPhone_WhenValid()
    {
        var userId = _context.Users.First().Id;

        var phone = new Phone
        {
            PhoneNumber = "1234567",
            UserId = userId
        };

        _context.Phones.Add(phone);
        _context.SaveChanges();

        phone.PhoneNumber = "7654321";

        _service.Update(phone);

        var updated = _context.Phones.First();
        Assert.That(updated.PhoneNumber, Is.EqualTo("7654321"));
    }

    [Test]
    public void Update_ShouldThrow_WhenNotFound()
    {
        var phone = new Phone
        {
            Id = 999,
            PhoneNumber = "1234567",
            UserId = 1
        };

        Assert.Throws<NotFoundException>(() => _service.Update(phone));
    }

    // ---------- DELETE ----------

    [Test]
    public void Delete_ShouldRemovePhone_WhenExists()
    {
        var userId = _context.Users.First().Id;

        var phone = new Phone
        {
            PhoneNumber = "1234567",
            UserId = userId
        };

        _context.Phones.Add(phone);
        _context.SaveChanges();

        _service.Delete(phone.Id);

        Assert.That(_context.Phones.Count(), Is.EqualTo(0));
    }

    [Test]
    public void Delete_ShouldThrow_WhenNotFound()
    {
        Assert.Throws<NotFoundException>(() => _service.Delete(999));
    }
}
