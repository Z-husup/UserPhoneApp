using UserPhoneApp.Models;

namespace UserPhoneApp.Services;

/// <summary>
/// Provides operations for managing users
/// </summary>
public interface IUserService
{
    IEnumerable<User> GetAll();
    User? GetById(int id);
    void Add(User user);
    void Update(User user);
    void Delete(int id);
}