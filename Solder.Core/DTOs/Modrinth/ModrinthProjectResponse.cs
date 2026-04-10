using System.Text.Json.Serialization;

namespace Solder.Core.DTOs.Modrinth;

public record ModrinthProjectResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("project_type")]
    string ProjectType,
    [property: JsonPropertyName("team")] string Team,
    [property: JsonPropertyName("organization")]
    string? Organization,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")]
    string Description,
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("body_url")]
    string? BodyUrl,
    [property: JsonPropertyName("published")]
    DateTime Published,
    [property: JsonPropertyName("updated")]
    DateTime Updated,
    [property: JsonPropertyName("approved")]
    DateTime? Approved,
    [property: JsonPropertyName("queued")] DateTime? Queued,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("requested_status")]
    string? RequestedStatus,
    [property: JsonPropertyName("moderator_message")]
    string? ModeratorMessage,
    [property: JsonPropertyName("license")]
    License License,
    [property: JsonPropertyName("client_side")]
    string ClientSide,
    [property: JsonPropertyName("server_side")]
    string ServerSide,
    [property: JsonPropertyName("downloads")]
    long Downloads,
    [property: JsonPropertyName("followers")]
    int Followers,
    [property: JsonPropertyName("categories")]
    List<string> Categories,
    [property: JsonPropertyName("additional_categories")]
    List<string> AdditionalCategories,
    [property: JsonPropertyName("game_versions")]
    List<string> GameVersions,
    [property: JsonPropertyName("loaders")]
    List<string> Loaders,
    [property: JsonPropertyName("versions")]
    List<string> Versions,
    [property: JsonPropertyName("icon_url")]
    string? IconUrl,
    [property: JsonPropertyName("issues_url")]
    string? IssuesUrl,
    [property: JsonPropertyName("source_url")]
    string? SourceUrl,
    [property: JsonPropertyName("wiki_url")]
    string? WikiUrl,
    [property: JsonPropertyName("discord_url")]
    string? DiscordUrl,
    [property: JsonPropertyName("donation_urls")]
    List<DonationUrl> DonationUrls,
    [property: JsonPropertyName("gallery")]
    List<GalleryItem> Gallery,
    [property: JsonPropertyName("color")] int? Color,
    [property: JsonPropertyName("thread_id")]
    string? ThreadId,
    [property: JsonPropertyName("monetization_status")]
    string? MonetizationStatus
);

public record License(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string? Url
);

public record DonationUrl(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("platform")]
    string Platform,
    [property: JsonPropertyName("url")] string Url
);

public record GalleryItem(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("raw_url")]
    string RawUrl,
    [property: JsonPropertyName("featured")]
    bool Featured,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("created")]
    DateTime Created,
    [property: JsonPropertyName("ordering")]
    int Ordering
);