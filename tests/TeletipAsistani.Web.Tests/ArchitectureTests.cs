using System.Xml.Linq;

namespace TeletipAsistani.Web.Tests;

// AGENTS.md §5'teki bağımlılık yönünü kodla korur: biri yanlış referans eklerse bu test kırılır.
// .csproj dosyalarını okuyoruz; çünkü derlenmiş DLL, kullanılmayan referansı göstermez.
public class ArchitectureTests
{
    [Theory]
    [InlineData("TeletipAsistani.Domain")]
    [InlineData("TeletipAsistani.Application", "TeletipAsistani.Domain")]
    [InlineData("TeletipAsistani.Infrastructure", "TeletipAsistani.Application")]
    [InlineData("TeletipAsistani.Web", "TeletipAsistani.Application", "TeletipAsistani.Infrastructure")]
    public void Project_ReferencesOnlyAllowedLayers(string project, params string[] allowed)
    {
        var actual = ReadProjectReferences(project);

        Assert.Equal(allowed.Order(), actual.Order());
    }

    private static IEnumerable<string> ReadProjectReferences(string project)
    {
        var csproj = Path.Combine(FindSolutionRoot(), "src", project, $"{project}.csproj");

        return XDocument.Load(csproj)
            .Descendants("ProjectReference")
            .Select(reference => Path.GetFileNameWithoutExtension(
                reference.Attribute("Include")!.Value.Replace('\\', Path.DirectorySeparatorChar)));
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "TeletipAsistani.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("TeletipAsistani.sln bulunamadı.");
    }
}
