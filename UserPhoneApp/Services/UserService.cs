using Microsoft.EntityFrameworkCore;
using UserPhoneApp.Data;
using UserPhoneApp.Exceptions;
using UserPhoneApp.Models;
using UserPhoneApp.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    private const int MinNameLength = 2;
    private const int MaxNameLength = 50;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }


    public IEnumerable<User> GetAll()
    {
        return _context.Users
            .Include(u => u.Phones)
            .ToList();
    }

    public User GetById(int id)
    {
        return _context.Users
            .Include(u => u.Phones)
            .FirstOrDefault(u => u.Id == id)
            ?? throw new NotFoundException("User not found.");
    }


    public void Add(User user)
    {
        ValidateUser(user);

        _context.Users.Add(user);
        _context.SaveChanges();
    }


    public void Update(User user)
    {
        var existing = _context.Users.Find(user.Id)
            ?? throw new NotFoundException("User not found.");

        ValidateUser(user, user.Id);

        existing.Name = user.Name;
        existing.Email = user.Email;

        _context.SaveChanges();
    }


    public void Delete(int id)
    {
        var user = _context.Users
            .Include(u => u.Phones)
            .FirstOrDefault(u => u.Id == id)
            ?? throw new NotFoundException("User not found.");

        _context.Users.Remove(user);
        _context.SaveChanges();
    }


    private void ValidateUser(User user, int? updatingId = null)
    {
        if (user == null)
            throw new ValidationException(string.Empty, "User cannot be null.");

        ValidateName(user);
        ValidateEmail(user, updatingId);
        ValidateDate(user);
    }

    private void ValidateName(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
            throw new ValidationException(nameof(user.Name), "Name cannot be empty.");

        var normalized = user.Name.Trim();

        if (normalized.Length < MinNameLength || normalized.Length > MaxNameLength)
            throw new ValidationException(
                nameof(user.Name),
                $"Name length must be between {MinNameLength} and {MaxNameLength} characters."
            );

        user.Name = normalized;
    }

    private void ValidateEmail(User user, int? updatingId)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ValidationException(nameof(user.Email), "Email cannot be empty.");

        var normalized = user.Email.Trim().ToLowerInvariant();

        var exists = _context.Users.Any(u =>
            u.Email.ToLower() == normalized &&
            (!updatingId.HasValue || u.Id != updatingId.Value));

        if (exists)
            throw new ValidationException(nameof(user.Email), "User with this email already exists.");

        user.Email = normalized;
    }
    
    private void ValidateDate(User user)
    {
        if (user.DateOfBirth == default)
            throw new ValidationException(nameof(user.DateOfBirth), "Date of birth is required.");

        if (user.DateOfBirth > DateTime.Today)
            throw new ValidationException(nameof(user.DateOfBirth), "Date of birth cannot be in the future.");
    }
}
