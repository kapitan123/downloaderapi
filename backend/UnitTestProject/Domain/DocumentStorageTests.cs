using DocumentStore.Domain.Documents;
using DocumentStore.Domain.PreviewGenerator;
using DocumentStore.Infrastructrue.MetadataPersistance;
using Microsoft.Extensions.Logging;
using Moq;
using TestSolution.Infrastructrue.Web;

namespace UnitTests.Domain
{
	// These tests are only for IDocumentStorage.GetFilteredByUserAsync method of DocumentStorage
	// Generally I prefer starting with tests, but writng all the tests takes plenty of time
	// I assume test for one method are enough to get an idea of my approach
	public class DocumentStorageTests
	{
		private readonly Mock<IPreviewGenerator> _previewGenMock = new();
		private readonly Mock<IDocuementContentStore> _storeMock = new();
		private readonly Mock<IMetadataRepository> _metaRepoMock = new();
		private readonly Mock<ILogger<DocumentStorage>> _loggerMock = new();

		private readonly DocumentStorage _sut;

		public DocumentStorageTests()
		{
			_sut = new DocumentStorage(_previewGenMock.Object, _storeMock.Object, _metaRepoMock.Object, _loggerMock.Object);
		}

		[Fact]
		public async Task Should_Return_Documents_Metadata_And_Content_When_File_Is_Accessed_Not_An_Owner()
		{
			var documentId = Guid.NewGuid();
			var user = "user123";
			var token = CancellationToken.None;

			var documentMeta = new DocumentMeta { Id = documentId, UploadedBy = user };
			var documentContent = new MemoryStream();

			_metaRepoMock.Setup(m => m.GetAsync(documentId, token)).ReturnsAsync(documentMeta);
			_storeMock.Setup(s => s.ReadDocumentAsync(documentId, token)).ReturnsAsync(documentContent);

			// Act
			var (meta, content) = await _sut.GetFilteredByUserAsync(documentId, user, token);

			Assert.Equal(documentMeta, meta);
			Assert.Equal(documentContent, content);

			_metaRepoMock.Verify(m => m.GetAsync(documentId, token), Times.Once);
			_storeMock.Verify(s => s.ReadDocumentAsync(documentId, token), Times.Once);
			_metaRepoMock.Verify(m => m.IncrementDownloadsAsync(documentId, token), Times.Once);
		}

		// The naming I prefer contains no method name, and describes a usecase
		[Fact]
		public async Task Should_Throw_Exception_When_File_Is_Accessed_Not_By_An_Owner()
		{
			var documentId = Guid.NewGuid();
			var user = "user123";
			var token = CancellationToken.None;

			var documentMeta = new DocumentMeta { Id = documentId, UploadedBy = "anotherUser" };

			_metaRepoMock.Setup(m => m.GetAsync(documentId, token)).ReturnsAsync(documentMeta);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.GetFilteredByUserAsync(documentId, user, token));

			// Genreally I prefer not to test an exception message
			// But in this case it contains important data
			Assert.Equal($"User {user} tried to access a file {documentId} which belongs to {documentMeta.UploadedBy}", exception.Message);

			_metaRepoMock.Verify(m => m.GetAsync(documentId, token), Times.Once);
			_storeMock.Verify(s => s.ReadDocumentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
			_metaRepoMock.Verify(m => m.IncrementDownloadsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}