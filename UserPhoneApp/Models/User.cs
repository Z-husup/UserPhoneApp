using System.ComponentModel.DataAnnotations;

namespace UserPhoneApp.Models;

/// <summary>
/// Represents user in the system
/// User can have multiple phone numbers
/// </summary>
public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    
    public ICollection<Phone> Phones { get; set; } =  new List<Phone>();
}