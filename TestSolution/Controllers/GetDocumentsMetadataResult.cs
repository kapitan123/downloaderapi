using TestSolution.Domain;

namespace TestSolution.Controllers;

public class GetDocumentsMetadataResult
{
	// AK TODO separate representation
	public List<DocumentMeta> Data { get; set; }
}