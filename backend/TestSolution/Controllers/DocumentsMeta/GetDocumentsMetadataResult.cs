using DocumentStore.Domain.Documents;

namespace DocumentStore.Controllers.DocumentsMeta;

public class GetDocumentsMetadataResult
{
	public List<DocumentMeta> Data { get; set; }
}