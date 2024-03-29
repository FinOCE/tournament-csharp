﻿namespace API.Models.Users;

/// <summary>
/// A user of the service
/// </summary>
public class User : ISociable, IVerifiable
{
    public const string DefaultIcon = "1234567890123456"; // TODO: Create default icon path

    private ISociable _Sociable = new Sociable();
    private IVerifiable _Verifiable = new Verifiable();

    public string Id { get; init; }
    public string Username { get; private set; }
    public int Discriminator { get; private set; }
    public string Icon { get; private set; }
    public DateTime Timestamp { get; init; }
    public int Permissions { get; set; }
    public Dictionary<string, Social> Socials { get { return _Sociable.Socials; } }
    public bool Verified { get { return _Verifiable.Verified; } }

    public string Tag
    {
        get { return $"{Username}#{Discriminator:0000}"; }
    }

    /// <exception cref="ArgumentException"></exception>
    public User(
        string id,
        string username,
        int discriminator,
        string? icon = null,
        bool verified = false,
        Dictionary<string, Social>? socials = null,
        int permissions = 0)
    {
        // Validate arguments
        if (!Snowflake.Validate(id))
            throw new ArgumentException($"Invalid {nameof(id)} provided");

        if (!SetUsername(username))
            throw new ArgumentException($"Invalid {nameof(username)} provided");

        if (!SetDiscriminator(discriminator))
            throw new ArgumentException($"Invalid {nameof(discriminator)} provided");

        if (!SetIcon(icon))
            throw new ArgumentException($"Invalid {nameof(icon)} provided");

        // Assign arguments to user
        Id = id;
        Username = username;
        Icon = icon ?? DefaultIcon;
        Discriminator = discriminator;
        Timestamp = Snowflake.GetTimestamp(id);
        Permissions = permissions;

        if (socials is not null)
            SetSocials(socials);

        if (verified)
            Verify();
    }

    /// <summary>
    /// Generate a dummy user to run validation on
    /// </summary>
    /// <returns>A dummy user to validate with</returns>
    public static User Dummy()
    {
        return new User("1", "Dummy User", 1);
    }

    /// <summary>
    /// Update the user's username
    /// </summary>
    public bool SetUsername(string username)
    {
        return new StringValidator()
            .Trim()
            .SetMinimumLength(3)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w-. ]"))
            .OnSuccess((string? username) => Username = username!)
            .Test(username);
    }

    /// <summary>
    /// Update the user's discriminator
    /// </summary>
    public bool SetDiscriminator(int discriminator)
    {
        // Validate discriminator
        if (discriminator < 1 || discriminator > 9999)
            return false;

        // Update discriminator
        Discriminator = discriminator;
        return true;
    }

    /// <summary>
    /// Set whether the default icon or a user icon is used
    /// </summary>
    public bool SetIcon(string? icon)
    {
        return new StringValidator()
            .AllowNull()
            .SetMinimumLength(16)
            .SetMaximumLength(16)
            .SetInvalidRegex(new(@"[^\w]"))
            .OnSuccess(icon => Icon = icon ?? DefaultIcon)
            .Test(icon);
    }

    /// <summary>
    /// Check if the user has a permission
    /// </summary>
    public bool HasPermission(UserPermission permission)
    {
        return (int)permission == (Permissions & (int)permission);
    }

    /// <summary>
    /// Give a permission to the user
    /// </summary>
    public void AddPermission(UserPermission permission)
    {
        Permissions |= (int)permission;
    }

    /// <summary>
    /// Remove a permission from the user
    /// </summary>
    public void RemovePermission(UserPermission permission)
    {
        Permissions &= ~(int)permission;
    }

    public override bool Equals(object? obj)
    {
        if (obj is User)
            return ((User)obj).Id == Id;

        if (obj is string)
            return (string)obj == Id;
        
        return false;
    }

    public void SetSocials(Dictionary<string, Social> socials)
    {
        _Sociable.SetSocials(socials);
    }

    public bool AddSocial(Social social)
    {
        return _Sociable.AddSocial(social);
    }

    public bool RemoveSocial(string id)
    {
        return _Sociable.RemoveSocial(id);
    }

    public void Verify()
    {
        _Verifiable.Verify();
    }
}

public enum UserPermission
{
    Administrator = 1
}
