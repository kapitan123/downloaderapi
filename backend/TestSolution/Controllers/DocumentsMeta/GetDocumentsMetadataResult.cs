using TestSolution.Domain;

namespace DocumentStore.Controllers.DocumentsMeta;

public class GetDocumentsMetadataResult
{
	// AK TODO separate representation
	public List<DocumentMeta> Data { get; set; }
}