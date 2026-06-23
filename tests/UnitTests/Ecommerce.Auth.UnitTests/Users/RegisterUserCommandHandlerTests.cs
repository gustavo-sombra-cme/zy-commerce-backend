using Ecommerce.Auth.Application.Abstractions;
using Ecommerce.Auth.Application.Security;
using Ecommerce.Auth.Application.Users;
using Ecommerce.Auth.Application.Users.RegisterUser;
using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_RegistersUser()
    {
        var repository = new FakeUserRepository();
        var unitOfWork = new FakeAuthUnitOfWork();
        var passwordHasher = new FakePasswordHasher();
        var handler = new RegisterUserCommandHandler(repository, unitOfWork, passwordHasher);

        var result = await handler.Handle(
            new RegisterUserCommand("USER@example.COM", "PlainPassword123"),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal("user@example.com", result.Email);
        Assert.True(passwordHasher.HashCalled);
        Assert.Equal("PlainPassword123", passwordHasher.LastPassword);
        Assert.Single(repository.Users);
        Assert.Equal("hashed::PlainPassword123", repository.Users[0].PasswordHash.Value);
        Assert.NotEqual("PlainPassword123", repository.Users[0].PasswordHash.Value);
        Assert.Equal(UserRole.Customer, repository.Users[0].Role);
        Assert.True(repository.Users[0].IsActive);
        Assert.False(repository.Users[0].IsEmailVerified);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsAndDoesNotSave()
    {
        var repository = new FakeUserRepository(existingEmail: "user@example.com");
        var unitOfWork = new FakeAuthUnitOfWork();
        var passwordHasher = new FakePasswordHasher();
        var handler = new RegisterUserCommandHandler(repository, unitOfWork, passwordHasher);

        var exception = await Assert.ThrowsAsync<DuplicateEmailException>(() => handler.Handle(
            new RegisterUserCommand("USER@example.COM", "PlainPassword123"),
            CancellationToken.None));

        Assert.Equal("user@example.com", exception.Email);
        Assert.Empty(repository.Users);
        Assert.False(passwordHasher.HashCalled);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsNormalizedEmail()
    {
        var repository = new FakeUserRepository();
        var unitOfWork = new FakeAuthUnitOfWork();
        var handler = new RegisterUserCommandHandler(repository, unitOfWork, new FakePasswordHasher());

        await handler.Handle(
            new RegisterUserCommand("  USER@example.COM  ", "PlainPassword123"),
            CancellationToken.None);

        Assert.Equal("user@example.com", repository.Users[0].Email.Value);
    }

    private sealed class FakeUserRepository(string? existingEmail = null) : IUserRepository
    {
        private readonly string? _existingEmail = existingEmail;

        public List<User> Users { get; } = [];

        public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken) =>
            Task.FromResult(_existingEmail == email.Value);

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAuthUnitOfWork : IAuthUnitOfWork
    {
        public bool SaveChangesCalled { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public bool HashCalled { get; private set; }

        public string? LastPassword { get; private set; }

        public string Hash(string password)
        {
            HashCalled = true;
            LastPassword = password;
            return $"hashed::{password}";
        }

        public bool Verify(string password, PasswordHash passwordHash) =>
            passwordHash.Value == $"hashed::{password}";
    }
}
