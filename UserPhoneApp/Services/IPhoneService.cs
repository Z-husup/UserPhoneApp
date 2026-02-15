using UserPhoneApp.Models;

namespace UserPhoneApp.Services;

/// <summary>
/// Provides operations for managing phones
/// </summary>
public interface IPhoneService
{
    IEnumerable<Phone> GetAllWithUsers();
    Phone? GetById(int id);
    void Add(Phone phone);
    void Update(Phone phone);
    void Delete(int id);
}