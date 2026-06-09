using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

internal sealed class UploadDocumentNormalizer
{
    public ValidatedDocumentUpload Normalize(UploadDocumentCommand request)
    {
        var fileName = Path.GetFileName(request.FileName.Trim());

        return new ValidatedDocumentUpload(
            fileName,
            Path.GetExtension(fileName).Trim().ToLowerInvariant(),
            NormalizeAccessLevel(request.AccessLevel));
    }

    private static DocumentAccessLevel NormalizeAccessLevel(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DocumentAccessLevel.Private;
        }

        return value.Trim().Equals(nameof(DocumentAccessLevel.TenantPublic), StringComparison.OrdinalIgnoreCase)
            ? DocumentAccessLevel.TenantPublic
            : DocumentAccessLevel.Private;
    }
}
