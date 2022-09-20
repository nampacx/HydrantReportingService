using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.SymbolStore;

namespace HydrantReportingService.Library;
public class HydrantReportDTO
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonConverter(typeof(StringEnumConverter))] 
    [JsonProperty("documentType")]
    public DocumentType DocumentType { get; } = DocumentType.HydrantReport;

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty("type")]
    public HydrantType Type { get; set; }

    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("address")]
    public dynamic Address { get; set; }

    [JsonProperty("defect")]
    public bool Defect { get; set; }

    [JsonProperty("notes")]
    public string Notes { get; set; }

    [JsonProperty("approved")]
    public bool Approved { get; set; } = false;
}
