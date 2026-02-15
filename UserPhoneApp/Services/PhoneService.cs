using Microsoft.EntityFrameworkCore;
using UserPhoneApp.Data;
using UserPhoneApp.Exceptions;
using UserPhoneApp.Models;

namespace UserPhoneApp.Services;

/// <summary>
/// Service responsible for:
/// - Phone CRUD operations
/// - PhoneNumber Validation.
/// </summary>
public class PhoneService : IPhoneService
{
    private readonly AppDbContext _context;
    
    // Validation limits for PhoneNumber
    private const int MinPhoneLength = 7;
    private const int MaxPhoneLength = 15;

    public PhoneService(AppDbContext context)
    {
        _context = context;
    }


    public IEnumerable<Phone> GetAllWithUsers()
    {
        return _context.Phones
            .Include(p => p.User)
            .ToList();
    }

    public Phone GetById(int id)
    {
        return _context.Phones
            .Include(p => p.User)
            .FirstOrDefault(p => p.Id == id)
            ?? throw new NotFoundException("Phone not found.");
    }


    public void Add(Phone phone)
    {
        ValidatePhone(phone);

        phone.PhoneNumber = phone.PhoneNumber.Trim();

        _context.Phones.Add(phone);
        _context.SaveChanges();
    }


    public void Update(Phone phone)
    {
        var existing = _context.Phones.Find(phone.Id)
            ?? throw new NotFoundException("Phone not found.");

        ValidatePhone(phone, phone.Id);

        existing.PhoneNumber = phone.PhoneNumber.Trim();
        existing.UserId = phone.UserId;

        _context.SaveChanges();
    }


    public void Delete(int id)
    {
        var phone = _context.Phones.Find(id)
            ?? throw new NotFoundException("Phone not found.");

        _context.Phones.Remove(phone);
        _context.SaveChanges();
    }


    private void ValidatePhone(Phone phone, int? updatingId = null)
    {
        if (phone == null)
            throw new ValidationException(string.Empty, "Phone cannot be null.");

        if (!_context.Users.Any())
            throw new ValidationException(
                nameof(phone.UserId),
                "Cannot create phone because no users exist. Please create a user first."
            );

        if (string.IsNullOrWhiteSpace(phone.PhoneNumber))
            throw new ValidationException(
                nameof(phone.PhoneNumber),
                "Phone number cannot be empty."
            );

        var normalized = phone.PhoneNumber.Trim();

        if (normalized.Length < MinPhoneLength ||
            normalized.Length > MaxPhoneLength)
            throw new ValidationException(
                nameof(phone.PhoneNumber),
                $"Phone number length must be between {MinPhoneLength} and {MaxPhoneLength} characters."
            );

        if (!_context.Users.Any(u => u.Id == phone.UserId))
            throw new ValidationException(
                nameof(phone.UserId),
                "Selected user does not exist."
            );

        var duplicateExists = _context.Phones.Any(p =>
            p.PhoneNumber == normalized &&
            (!updatingId.HasValue || p.Id != updatingId.Value));

        if (duplicateExists)
            throw new ValidationException(
                nameof(phone.PhoneNumber),
                "Phone number already exists."
            );
    }

}
