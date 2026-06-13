using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Marketplace.Catalog.FunctionalTests;

/// <summary>
/// Exercises the real HTTP auth pipeline: JWT validation + RBAC on a write endpoint. Categories
/// create is Admin-only, so it cleanly separates 401 (no token), 403 (wrong role), and authorized.
/// </summary>
public sealed class AuthorizationTests(CatalogApiFactory factory) : IClassFixture<CatalogApiFactory>
{
    private static readonly object CategoryBody = new { name = "Gadgets" };

    private static string Token(params string[] roles)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(CatalogApiFactory.SigningKey),
            SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()) };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private HttpClient Client(string? bearer = null)
    {
        var client = factory.CreateClient();
        if (bearer is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        return client;
    }

    [Fact]
    public async Task Creating_a_category_without_a_token_is_unauthorized()
    {
        var response = await Client().PostAsJsonAsync("/api/v1/categories", CategoryBody);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task A_seller_cannot_create_a_category_which_is_admin_only()
    {
        var response = await Client(Token("Seller")).PostAsJsonAsync("/api/v1/categories", CategoryBody);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task An_admin_passes_authentication_and_authorization()
    {
        var response = await Client(Token("Admin")).PostAsJsonAsync("/api/v1/categories", CategoryBody);

        // Past authentication (not 401) and authorization (not 403); whatever happens downstream
        // depends on the database, which this no-DB test deliberately does not exercise.
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }
}
