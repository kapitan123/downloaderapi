namespace TestSolution.Domain;

public class DocumentMeta
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public string Type { get; set; }

	public string PreviewAddress { get; set; }

	public DateTime UploadedOn { get; set; }

	public string ContentAddress { get; set; }
}
