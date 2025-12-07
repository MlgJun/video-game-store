using System.ComponentModel.DataAnnotations;
using VideoGameStore.Entities;

namespace VideoGameStore.Dtos
{
    public record class UserRequest ([EmailAddress] string Login, [MinLength(6)] string Password, UserRole UserRole);
}
