using System.ComponentModel.DataAnnotations.Schema;
using Prima.Core.Server.Entities.Base;


namespace Prima.Core.Server.Entities;

[Table("accounts")]
public class AccountEntity : BaseDbEntity
{
    public string Username { get; set; }

    public string HashedPassword { get; set; }

    public bool IsAdmin { get; set; }

    public string? Email { get; set; }

    public bool IsVerified { get; set; }
}
