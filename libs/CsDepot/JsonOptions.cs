//
// At least a portion of the code below was created using AI tool Claude Code.
//

using System.Text.Json;

namespace CsDepot;


/// <summary>
/// The serializer options shared by all JSON serialization in this repo.  Never
/// create another JsonSerializerOptions instance; configure host frameworks (for
/// example, ASP.NET Core) with these same settings.
/// </summary>
public static class JsonOptions
{
    //------------------------------------------------------------------------------
    // Static fields
    //------------------------------------------------------------------------------

    /// <summary>
    /// Web defaults (camelCase property names) plus strict handling of missing
    /// and null values.
    /// </summary>
    public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        // Accept JSON where "kind" is not the first property (for example, from non-.NET producers)
        AllowOutOfOrderMetadataProperties = true,

        // A missing constructor argument (for example, no "value") throws instead of becoming default
        RespectRequiredConstructorParameters = true,

        // An explicit null for a non-nullable member (for example, "value": null) throws
        RespectNullableAnnotations = true
    };
}
