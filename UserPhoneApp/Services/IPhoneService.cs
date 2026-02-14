using UserPhoneApp.Models;

namespace UserPhoneApp.Services;

public interface IPhoneService
{
    IEnumerable<Phone> GetAllWithUsers();
    Phone? GetById(int id);
    void Add(Phone phone);
    void Update(Phone phone);
    void Delete(int id);
}