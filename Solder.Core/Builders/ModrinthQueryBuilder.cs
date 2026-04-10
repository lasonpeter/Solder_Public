namespace Solder.Infrastructure.Persistence.ModrinthAPI;

public class ModrinthQueryBuilder
{
    private readonly List<string> _parameters = new();

    public ModrinthQueryBuilder Add(string key, string value)
    {
        _parameters.Add($"{key}={Uri.EscapeDataString(value)}");
        return this;
    }

    /// <summary>
    ///     Handles Modrinth's specific facet syntax: [["facet1","facet2"],["facet3"]]
    /// </summary>
    public ModrinthQueryBuilder Take(int count)
    {
        // Modrinth max is 100
        return Add("limit", count.ToString());
    }

    public ModrinthQueryBuilder Skip(int count)
    {
        return Add("offset", count.ToString());
    }

    public ModrinthQueryBuilder AddFacets(IEnumerable<IEnumerable<string>> facetGroups)
    {
        // 1. Convert each inner group to ["item1","item2"]
        var groups = facetGroups.Select(group =>
            "[" + string.Join(",", group.Select(item => $"\"{item}\"")) + "]"
        );

        // 2. Wrap them all in an outer array: [["group1"],["group2"]]
        var jsonNestedArray = "[" + string.Join(",", groups) + "]";

        _parameters.Add($"facets={Uri.EscapeDataString(jsonNestedArray)}");
        return this;
    }

    public string Build()
    {
        return _parameters.Any() ? "?" + string.Join("&", _parameters) : "";
    }
}