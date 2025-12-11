using System.ComponentModel.DataAnnotations;
using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public record class UserRequest ([EmailAddress] string Email, [MinLength(2)] [MaxLength(100)] string Username, 
        [MinLength(6)] string Password, UserRole UserRole);
}
