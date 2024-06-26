﻿using DocumentStore.Domain.Documents;

namespace DocumentStore.Controllers.Documents;

public class UploadDocumentRequest
{
	public IFormFile File { get; set; }

	public string UploadedBy { get; set; } // I put user data inside the request to not overlycomplicate task with authentication
}

public static class UploadDocumentRequestExtensions
{
	public static DocumentMeta ToMetaData(this UploadDocumentRequest req)
	{
		return new DocumentMeta
		{
			Id = Guid.NewGuid(),
			Name = req.File.FileName,
			UploadedOn = DateTime.UtcNow,
			ContentType = req.File.ContentType,
			Size = req.File.Length,
			UploadedBy = req.UploadedBy
		};
	}
}