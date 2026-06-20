using Ecommerce.Auth.Application.Security;
using Ecommerce.Auth.Application.Users;
using Ecommerce.Auth.Application.Users.LoginUser;
using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsUser()
    {
        var user = CreateUser("USER@example.COM", "stored-hash");
        var repository = new FakeUserRepository(user);
        var passwordHasher = new FakePasswordHasher(verifyResult: true);
        var accessTokenGenerator = new FakeAccessTokenGenerator();
        var handler = new LoginUserCommandHandler(repository, passwordHasher, accessTokenGenerator);

        var result = await handler.Handle(
            new LoginUserCommand(" user@example.com ", "PlainPassword123"),
            CancellationToken.None);

        Assert.Equal(user.Id.Value, result.UserId);
        Assert.Equal("user@example.com", result.Email);
        Assert.True(passwordHasher.VerifyCalled);
        Assert.Equal("PlainPassword123", passwordHasher.LastPassword);
        Assert.Equal(user.PasswordHash, passwordHasher.LastPasswordHash);
        Assert.True(accessTokenGenerator.GenerateCalled);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("Bearer", result.TokenType);
        Assert.True(result.ExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Handle_WithMissingUser_ThrowsInvalidCredentials()
    {
        var passwordHasher = new FakePasswordHasher(verifyResult: true);
        var accessTokenGenerator = new FakeAccessTokenGenerator();
        var handler = new LoginUserCommandHandler(new FakeUserRepository(user: null), passwordHasher, accessTokenGenerator);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(
            new LoginUserCommand("user@example.com", "PlainPassword123"),
            CancellationToken.None));

        Assert.False(passwordHasher.VerifyCalled);
        Assert.False(accessTokenGenerator.GenerateCalled);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsInvalidCredentials()
    {
        var user = CreateUser("user@example.com", "stored-hash");
        var passwordHasher = new FakePasswordHasher(verifyResult: false);
        var accessTokenGenerator = new FakeAccessTokenGenerator();
        var handler = new LoginUserCommandHandler(new FakeUserRepository(user), passwordHasher, accessTokenGenerator);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(
            new LoginUserCommand("user@example.com", "WrongPassword"),
            CancellationToken.None));

        Assert.True(passwordHasher.VerifyCalled);
        Assert.False(accessTokenGenerator.GenerateCalled);
    }

    [Fact]
    public async Task Handle_WithInactiveUser_ThrowsInactiveUserAndDoesNotVerifyPassword()
    {
        var user = CreateUser("user@example.com", "stored-hash");
        user.Deactivate(DateTimeOffset.UtcNow.AddMinutes(1));
        var passwordHasher = new FakePasswordHasher(verifyResult: true);
        var accessTokenGenerator = new FakeAccessTokenGenerator();
        var handler = new LoginUserCommandHandler(new FakeUserRepository(user), passwordHasher, accessTokenGenerator);

        var exception = await Assert.ThrowsAsync<InactiveUserException>(() => handler.Handle(
            new LoginUserCommand("user@example.com", "PlainPassword123"),
            CancellationToken.None));

        Assert.Equal("user@example.com", exception.Email);
        Assert.False(passwordHasher.VerifyCalled);
        Assert.False(accessTokenGenerator.GenerateCalled);
    }

    [Fact]
    public async Task Handle_WithUppercaseEmail_LooksUpNormalizedEmail()
    {
        var user = CreateUser("user@example.com", "stored-hash");
        var repository = new FakeUserRepository(user);
        var handler = new LoginUserCommandHandler(
            repository,
            new FakePasswordHasher(verifyResult: true),
            new FakeAccessTokenGenerator());

        await handler.Handle(
            new LoginUserCommand("USER@example.COM", "PlainPassword123"),
            CancellationToken.None);

        Assert.Equal("user@example.com", repository.LastEmail?.Value);
    }

    private static User CreateUser(string email, string passwordHash) =>
        User.Register(
            UserId.New(),
            Email.Create(email),
            PasswordHash.Create(passwordHash),
            DateTimeOffset.UtcNow);

    private sealed class FakeUserRepository(User? user) : IUserRepository
    {
        public Email? LastEmail { get; private set; }

        public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken) =>
            Task.FromResult(user is not null && user.Email == email);

        public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
        {
            LastEmail = email;
            return Task.FromResult(user is not null && user.Email == email ? user : null);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class FakePasswordHasher(bool verifyResult) : IPasswordHasher
    {
        public bool VerifyCalled { get; private set; }

        public string? LastPassword { get; private set; }

        public PasswordHash? LastPasswordHash { get; private set; }

        public string Hash(string password) => $"hashed::{password}";

        public bool Verify(string password, PasswordHash passwordHash)
        {
            VerifyCalled = true;
            LastPassword = password;
            LastPasswordHash = passwordHash;
            return verifyResult;
        }
    }

    private sealed class FakeAccessTokenGenerator : IAccessTokenGenerator
    {
        public bool GenerateCalled { get; private set; }

        public AccessTokenResult Generate(User user)
        {
            GenerateCalled = true;
            return new AccessTokenResult("access-token", "Bearer", DateTimeOffset.UtcNow.AddMinutes(15));
        }
    }
}
