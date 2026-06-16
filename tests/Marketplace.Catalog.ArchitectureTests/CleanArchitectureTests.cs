using System.Reflection;
using FluentAssertions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Domain.Products;
using NetArchTest.Rules;
using Xunit;

namespace Marketplace.Catalog.ArchitectureTests;

/// <summary>
/// Enforces the Clean Architecture dependency rule as a build/CI gate (ADR-0004, BE-2, BE-16).
/// A boundary violation fails the test — and therefore the pipeline.
/// </summary>
public class CleanArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(Product).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(IUnitOfWork).Assembly;

    private const string Application = "Marketplace.Catalog.Application";
    private const string Infrastructure = "Marketplace.Catalog.Infrastructure";
    private const string Api = "Marketplace.Catalog.Api";

    [Fact]
    public void Domain_should_not_depend_on_outer_layers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Application, Infrastructure, Api)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(Describe(result));
    }

    [Fact]
    public void Domain_should_be_free_of_infrastructure_frameworks()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "MassTransit", "Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(Describe(result));
    }

    [Fact]
    public void Application_should_not_depend_on_infrastructure_or_api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(Infrastructure, Api)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(Describe(result));
    }

    [Fact]
    public void Application_should_be_persistence_ignorant()
    {
        // The Application layer talks to ports, never EF Core or the broker directly (BE-2, BE-4).
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "MassTransit")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(Describe(result));
    }

    private static string Describe(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Violating types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
