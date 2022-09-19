using System.Diagnostics.SymbolStore;

namespace HydrantReportingService.Library;
public class HydrantReportDTO
{
    public string Id { get; set; }
    public DocumentType DocumentType { get; } = DocumentType.HydrantReport;

    public HydrantType Type { get; set; }

    public bool Defect { get; set; }

    public string Notes { get; set; }

    public bool Approved { get; set; } = false;
}
