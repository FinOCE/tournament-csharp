namespace API.Tests.Unit.Models.Users;

[TestClass]
public class UserTest
{
    [TestMethod]
    public void TagTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user1 = new(snowflake.ToString(), "Username", 1234);
        Assert.AreEqual("Username#1234", user1.Tag);

        User user2 = new(snowflake.ToString(), "Username", 1);
        Assert.AreEqual("Username#0001", user2.Tag);
    }

    [TestMethod]
    public void ConstructorTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();
        
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new("", "", 1234);
        });
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new("", "Username", 1234);
        });
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new(snowflake.ToString(), "", 1234);
        });
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new(snowflake.ToString(), "Username", 0);
        });
        Assert.ThrowsException<ArgumentException>(() => {
            User user = new(snowflake.ToString(), "Username", 0, "invalid.icon");
        });
        Assert.IsInstanceOfType(new User(snowflake.ToString(), "Username", 1234), typeof(User));
    }

    [TestMethod]
    public void DummyTest()
    {
        // Act
        User dummy = User.Dummy();

        // Assert
        Assert.IsInstanceOfType(dummy, typeof(User), "A dummy user should be generated");
    }

    [TestMethod]
    public void SetUsernameTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", 1234);

        Assert.IsFalse(user.SetUsername(""));
        Assert.IsFalse(user.SetUsername("1"));
        Assert.IsFalse(user.SetUsername("     "));
        Assert.IsFalse(user.SetUsername(" Us "));
        Assert.IsFalse(user.SetUsername("User,"));
        Assert.IsTrue(user.SetUsername("U.s.e.r."));
        Assert.IsTrue(user.SetUsername("    User  "));
        Assert.IsTrue(user.SetUsername("12345"));
        Assert.IsTrue(user.SetUsername("User."));
    }

    [TestMethod]
    public void SetDiscriminatorTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", 1234);

        Assert.IsTrue(user.SetDiscriminator(1337));
        Assert.AreEqual(1337, user.Discriminator);

        Assert.IsTrue(user.SetDiscriminator(1));
        Assert.IsTrue(user.SetDiscriminator(9999));
        Assert.IsFalse(user.SetDiscriminator(-1));
        Assert.IsFalse(user.SetDiscriminator(0));
        Assert.IsFalse(user.SetDiscriminator(10000));
    }

    [TestMethod]
    public void SetIconTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "User", 1234);

        Assert.AreEqual(User.DefaultIcon, user.Icon);

        string validIconName = "1234567890123456";
        Assert.IsTrue(user.SetIcon(validIconName));
        Assert.AreEqual(validIconName, user.Icon);

        Assert.IsFalse(user.SetIcon(""));
        Assert.IsFalse(user.SetIcon("InvalidLength"));
        Assert.IsFalse(user.SetIcon("Has a space"));
        Assert.IsFalse(user.SetIcon("Some!invalid_chars"));
        Assert.AreEqual(validIconName, user.Icon);

        Assert.IsTrue(user.SetIcon(null));
        Assert.AreEqual(User.DefaultIcon, user.Icon);
    }

    [TestMethod]
    public void HasPermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", 1234);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));

        user.AddPermission(UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));

        User user3 = new(snowflake.ToString(), "Username", 1234, null, false, permissions: (int)UserPermission.Administrator);
        Assert.IsTrue(user3.HasPermission(UserPermission.Administrator));
    }

    [TestMethod]
    public void AddPermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", 1234);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));

        user.AddPermission(UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));

        user.AddPermission(UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));
    }

    [TestMethod]
    public void RemovePermissionTest()
    {
        SnowflakeService snowflakeService = new();
        Snowflake snowflake = snowflakeService.Generate();

        User user = new(snowflake.ToString(), "Username", 1234, null, false, permissions: (int)UserPermission.Administrator);
        Assert.IsTrue(user.HasPermission(UserPermission.Administrator));

        user.RemovePermission(UserPermission.Administrator);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));

        user.RemovePermission(UserPermission.Administrator);
        Assert.IsFalse(user.HasPermission(UserPermission.Administrator));
    }

    [TestMethod]
    public void EqualsTest()
    {
        // Arrange
        User user = new("123", "User", 1234);
        User matchingUser = new("123", "User", 1234);
        User notMatchingUser = new("1234", "User", 123);
        string id = "123";
        
        // Act
        bool matching = user.Equals(matchingUser);
        bool notMatching = user.Equals(notMatchingUser);
        bool matchesId = user.Equals(id);
        bool matchesNull = user.Equals(null);

        // Assert
        Assert.IsTrue(matching, "Matching user IDs should be accepted");
        Assert.IsFalse(notMatching, "Not matching user IDs should not be accepted");
        Assert.IsTrue(matchesId, "Matching a user with a string ID should work");
        Assert.IsFalse(matchesNull, "A user should not match null");
    }
}
