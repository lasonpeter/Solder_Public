using System.Text.Json.Serialization;
using Solder.ServerInstanceManager.Core;
using Solder.Shared.DTOs.Solder.Settings;

namespace Solder.ServerInstanceManager.Serialization;

[JsonSerializable(typeof(StartupConfig))]
[JsonSerializable(typeof(GetSettingsRequest))]
[JsonSerializable(typeof(GetSettingsResponse))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}