using WeConnect.Domain.Common;
using WeConnect.Domain.Enums;

namespace WeConnect.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? AzureAdObjectId { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Member;
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; private set; }

    private User() { }
    public static User Create(
        string firstName, string lastName, string email,
        string? azureAdObjectId = null, UserRole role = UserRole.Member)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new User
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            AzureAdObjectId = azureAdObjectId,
            Role = role
        };
    }

    public void UpdateProfile(string firstName, string lastName, string? profilePictureUrl)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ProfilePictureUrl = profilePictureUrl;
        MarkUpdated();
    }

    public void RecordLogin() { LastLoginAt = DateTime.UtcNow; MarkUpdated(); }
    public void Deactivate() { Status = UserStatus.Inactive; MarkUpdated(); }

    public string FullName => $"{FirstName} {LastName}";
}