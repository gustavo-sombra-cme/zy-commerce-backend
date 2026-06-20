using System.Reflection;
using System.Xml.Linq;
using NetArchTest.Rules;

namespace Ecommerce.ArchitectureTests;

public sealed class DependencyRuleTests
{
    [Fact]
    public void DomainProjects_ShouldNotReference_ApplicationInfrastructureContractsOrApi()
    {
        var forbiddenFragments = new[]
        {
            ".Application",
            ".Infrastructure",
            ".Contracts",
            "Ecommerce.Api"
        };

        foreach (var projectName in ProjectGraph.ProjectNames.Where(name => name.EndsWith(".Domain", StringComparison.Ordinal)))
        {
            AssertNoProjectReferenceContains(projectName, forbiddenFragments);
        }
    }

    [Fact]
    public void ApplicationProjects_ShouldNotReference_InfrastructureOrApi()
    {
        var forbiddenFragments = new[]
        {
            ".Infrastructure",
            "Ecommerce.Api"
        };

        foreach (var projectName in ProjectGraph.ProjectNames.Where(name => name.EndsWith(".Application", StringComparison.Ordinal)))
        {
            AssertNoProjectReferenceContains(projectName, forbiddenFragments);
        }
    }

    [Fact]
    public void InfrastructureProjects_ShouldNotReference_Api()
    {
        foreach (var projectName in ProjectGraph.ProjectNames.Where(name => name.EndsWith(".Infrastructure", StringComparison.Ordinal)))
        {
            AssertNoProjectReferenceContains(projectName, new[] { "Ecommerce.Api" });
        }
    }

    [Fact]
    public void BuildingBlocksProjects_ShouldNotReference_ModuleProjects()
    {
        foreach (var projectName in ProjectGraph.ProjectNames.Where(name => name.StartsWith("Ecommerce.BuildingBlocks.", StringComparison.Ordinal)))
        {
            AssertNoProjectReferenceContains(projectName, new[] { "Ecommerce.Auth.", "Ecommerce.Catalog.", "Ecommerce.Orders." });
        }
    }

    [Fact]
    public void ModuleProjects_ShouldNotReference_OtherModuleProjects()
    {
        var modules = new[] { "Auth", "Catalog", "Orders" };

        foreach (var module in modules)
        {
            foreach (var projectName in ProjectGraph.ProjectNames.Where(name => name.StartsWith($"Ecommerce.{module}.", StringComparison.Ordinal)))
            {
                var forbiddenModuleReferences = modules
                    .Where(otherModule => otherModule != module)
                    .Select(otherModule => $"Ecommerce.{otherModule}.")
                    .ToArray();

                AssertNoProjectReferenceContains(projectName, forbiddenModuleReferences);
            }
        }
    }

    [Fact]
    public void ProductionTypes_ShouldNotDependOn_ApiAssembly()
    {
        var productionAssemblies = ProjectGraph.ProjectNames
            .Where(name => name.StartsWith("Ecommerce.BuildingBlocks.", StringComparison.Ordinal)
                || name.StartsWith("Ecommerce.Auth.", StringComparison.Ordinal)
                || name.StartsWith("Ecommerce.Catalog.", StringComparison.Ordinal)
                || name.StartsWith("Ecommerce.Orders.", StringComparison.Ordinal))
            .Where(name => !name.EndsWith(".UnitTests", StringComparison.Ordinal))
            .Where(name => !name.EndsWith(".ArchitectureTests", StringComparison.Ordinal))
            .Select(Assembly.Load);

        foreach (var assembly in productionAssemblies)
        {
            var result = Types.InAssembly(assembly)
                .Should()
                .NotHaveDependencyOn("Ecommerce.Api")
                .GetResult();

            Assert.True(result.IsSuccessful, $"{assembly.GetName().Name} has a dependency on Ecommerce.Api.");
        }
    }

    private static void AssertNoProjectReferenceContains(string projectName, IReadOnlyCollection<string> forbiddenFragments)
    {
        var references = ProjectGraph.GetReferencedProjectNames(projectName);
        var violations = references
            .Where(reference => forbiddenFragments.Any(fragment => reference.Contains(fragment, StringComparison.Ordinal)))
            .OrderBy(reference => reference)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            $"{projectName} has forbidden project references: {string.Join(", ", violations)}");
    }
}

internal static class ProjectGraph
{
    private static readonly string Root = FindRepositoryRoot();

    public static IReadOnlyCollection<string> ProjectNames =>
        ProjectFiles.Keys.OrderBy(name => name).ToArray();

    private static IReadOnlyDictionary<string, string> ProjectFiles =>
        Directory
            .EnumerateFiles(Root, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                && !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToDictionary(
                path => Path.GetFileNameWithoutExtension(path),
                path => path,
                StringComparer.Ordinal);

    public static IReadOnlyCollection<string> GetReferencedProjectNames(string projectName)
    {
        var projectPath = ProjectFiles[projectName];
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var document = XDocument.Load(projectPath);

        return document
            .Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(include => !string.IsNullOrWhiteSpace(include))
            .Select(include => Path.GetFullPath(Path.Combine(projectDirectory, include!)))
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name)
            .ToArray()!;
    }

    public static IReadOnlyCollection<string> GetSolutionProjectNames()
    {
        var solutionPath = Path.Combine(Root, "Ecommerce.sln");

        return File
            .ReadLines(solutionPath)
            .Where(line => line.StartsWith("Project(", StringComparison.Ordinal))
            .Select(line => line.Split('=', 2)[1].Trim())
            .Select(line => line.Split(',', 2)[0].Trim().Trim('"'))
            .Where(name => name.StartsWith("Ecommerce.", StringComparison.Ordinal))
            .OrderBy(name => name)
            .ToArray();
    }

    public static string GetRootPath() => Root;

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Ecommerce.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root containing Ecommerce.sln.");
    }
}
