using System.Text.Json.Serialization;

namespace Solder.Core.DTOs.Modrinth;

public record ModrinthSearchResponse(
    [property: JsonPropertyName("hits")] List<ModpackHit> Hits,
    [property: JsonPropertyName("offset")] int Offset,
    [property: JsonPropertyName("limit")] int Limit,
    [property: JsonPropertyName("total_hits")]
    int TotalHits
);

public record ModpackHit(
    [property: JsonPropertyName("project_id")]
    string ProjectId,
    [property: JsonPropertyName("project_type")]
    string ProjectType,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("author")] string Author,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")]
    string Description,
    [property: JsonPropertyName("categories")]
    List<string> Categories,
    [property: JsonPropertyName("display_categories")]
    List<string> DisplayCategories,
    [property: JsonPropertyName("versions")]
    List<string> Versions,
    [property: JsonPropertyName("downloads")]
    int Downloads,
    [property: JsonPropertyName("follows")]
    int Follows,
    [property: JsonPropertyName("icon_url")]
    string? IconUrl,
    [property: JsonPropertyName("date_created")]
    DateTime DateCreated,
    [property: JsonPropertyName("date_modified")]
    DateTime DateModified,
    [property: JsonPropertyName("latest_version")]
    string LatestVersion,
    [property: JsonPropertyName("license")]
    string License,
    [property: JsonPropertyName("client_side")]
    string ClientSide,
    [property: JsonPropertyName("server_side")]
    string ServerSide,
    [property: JsonPropertyName("gallery")]
    List<string> Gallery,
    [property: JsonPropertyName("featured_gallery")]
    string? FeaturedGallery,
    [property: JsonPropertyName("color")] int? Color
);