using DocumentStore.Domain.Documents;
using DocumentStore.Domain.ShareabaleUrls;
using Microsoft.EntityFrameworkCore;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

public class DocumentsStoreDbContext(DbContextOptions<DocumentsStoreDbContext> options) : DbContext(options)
{
	// Generally I prefer a complete split between DB and Domain models and use a microORM like Dapper
	// So for production code I would always split them
	// here I jsut cut some corners skipping a translation step
	public DbSet<DocumentMeta> DocumentMetas { get; set; }

	public DbSet<PublicLink> PublicLinks { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<DocumentMeta>()
			.ToTable("documents_metadata");

		modelBuilder.Entity<DocumentMeta>()
			.HasKey(k => k.Id);

		modelBuilder.Entity<DocumentMeta>(
			b =>
			{
				b.Property(p => p.Id).HasColumnName("id");
				b.Property(p => p.Name).HasColumnName("name");
				b.Property(p => p.ContentType).HasColumnName("content_type");
				b.Property(p => p.Size).HasColumnName("size");
				b.Property(p => p.UploadedOn).HasColumnName("uploaded_on");
				b.Property(p => p.UploadedBy).HasColumnName("uploaded_by");
				b.Property(p => p.DownloadsCount).HasColumnName("downloads_count");
			});

		modelBuilder.Entity<PublicLink>()
			.ToTable("public_link");

		modelBuilder.Entity<PublicLink>()
			.HasKey(k => k.Id);

		modelBuilder.Entity<PublicLink>(
			b =>
			{
				b.Property(p => p.Id).HasColumnName("id");
				b.Property(p => p.DocumentId).HasColumnName("document_id");
				b.Property(p => p.CreatedOn).HasColumnName("created_on");
				b.Property(p => p.ExpiresOn).HasColumnName("expires_on");
			});
	}
}
