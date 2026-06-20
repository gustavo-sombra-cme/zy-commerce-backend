namespace Ecommerce.Api.Assistant;

public sealed class AssistantSafetyPolicy
{
    private static readonly string[] UnsafeQuestionTerms =
    [
        "create order",
        "place order",
        "cancel",
        "delete",
        "update",
        "deactivate",
        "reactivate",
        "change price",
        "admin",
        "all users",
        "other users",
        "another user",
        "different user",
        "someone else",
        "cross-user",
        "user id",
        "buyer id",
        "owner id",
        "customer id",
        "sql",
        "database",
        "connection string",
        "token",
        "authorization",
        "auth header",
        "password",
        "exception",
        "stack trace",
        "internal prompt",
        "system prompt"
    ];

    private static readonly string[] ForbiddenArgumentNames =
    [
        "userId",
        "buyerId",
        "ownerId",
        "customerId",
        "subject",
        "sub",
        "authorization",
        "authorizationHeader",
        "token",
        "accessToken",
        "password",
        "sql",
        "connectionString"
    ];

    public bool IsUnsafeQuestion(string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return false;
        }

        var normalized = question.Trim().ToLowerInvariant();
        return UnsafeQuestionTerms.Any(term => normalized.Contains(term, StringComparison.Ordinal));
    }

    public bool IsForbiddenArgumentName(string argumentName) =>
        ForbiddenArgumentNames.Contains(argumentName, StringComparer.OrdinalIgnoreCase);
}
