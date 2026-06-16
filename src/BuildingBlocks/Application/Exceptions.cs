namespace Marketplace.Application;

/// <summary>A requested resource does not exist. Mapped to HTTP 404 by the API (API-4).</summary>
public sealed class NotFoundException(string message) : Exception(message);

/// <summary>A uniqueness or concurrency conflict. Mapped to HTTP 409 by the API.</summary>
public sealed class ConflictException(string message) : Exception(message);

/// <summary>The caller is authenticated but not permitted to act on this resource. Mapped to HTTP 403 (SEC-6).</summary>
public sealed class ForbiddenException(string message) : Exception(message);
