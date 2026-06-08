namespace Ecommerce.ArchitectureTests;

public sealed class ProjectStructureTests
{
    private static readonly string[] ApprovedProjects =
    {
        "Ecommerce.Api",
        "Ecommerce.ArchitectureTests",
        "Ecommerce.Auth.Application",
        "Ecommerce.Auth.Contracts",
        "Ecommerce.Auth.Domain",
        "Ecommerce.Auth.Infrastructure",
        "Ecommerce.Auth.UnitTests",
        "Ecommerce.BuildingBlocks.Application",
        "Ecommerce.BuildingBlocks.Domain",
        "Ecommerce.BuildingBlocks.Infrastructure",
        "Ecommerce.Catalog.Application",
        "Ecommerce.Catalog.Contracts",
        "Ecommerce.Catalog.Domain",
        "Ecommerce.Catalog.Infrastructure",
        "Ecommerce.Catalog.UnitTests"
    };

    [Fact]
    public void Solution_ShouldContainOnly_ApprovedProjects()
    {
        var actualProjects = ProjectGraph.GetSolutionProjectNames();

        Assert.Equal(ApprovedProjects.OrderBy(name => name), actualProjects);
    }

    [Fact]
    public void Repository_ShouldContainOnly_ApprovedProjectFiles()
    {
        var actualProjects = ProjectGraph.ProjectNames;

        Assert.Equal(ApprovedProjects.OrderBy(name => name), actualProjects);
    }

    [Fact]
    public void Repository_ShouldNotContain_BootstrapperOrSharedProjects()
    {
        var forbiddenProjects = ProjectGraph.ProjectNames
            .Where(name => name.Contains("Bootstrapper", StringComparison.Ordinal)
                || name.Contains("Shared", StringComparison.Ordinal))
            .OrderBy(name => name)
            .ToArray();

        Assert.True(
            forbiddenProjects.Length == 0,
            $"Forbidden projects found: {string.Join(", ", forbiddenProjects)}");
    }

    [Fact]
    public void ModulesDirectory_ShouldContainOnly_ApprovedModules()
    {
        var modulesPath = Path.Combine(ProjectGraph.GetRootPath(), "src", "Modules");
        var modules = Directory.Exists(modulesPath)
            ? Directory.GetDirectories(modulesPath).Select(Path.GetFileName).OrderBy(name => name).ToArray()
            : Array.Empty<string>();

        Assert.Equal(new[] { "Auth", "Catalog" }, modules);
    }
}
