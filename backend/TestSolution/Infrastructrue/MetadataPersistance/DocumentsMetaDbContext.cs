﻿using Microsoft.EntityFrameworkCore;
using TestSolution.Domain;

namespace DocumentStore.Infrastructrue.MetadataPersistance;

public class DocumentsMetaDbContext(DbContextOptions<DocumentsMetaDbContext> options) : DbContext(options)
{
	// Generally I prefer a complete split between DB and Domain models and use a microORM like Dapper
	// So for production code I would always split them
	// here I jsut cut some corners skipping a translation step
	public DbSet<DocumentMeta> DocumentMetas { get; set; } // AK TODO do I even need this?

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
	}
}
