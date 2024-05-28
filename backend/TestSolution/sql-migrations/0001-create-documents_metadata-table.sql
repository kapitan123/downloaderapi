CREATE TABLE documents_metadata (
   id UUID PRIMARY KEY,
   name VARCHAR(255),
   downloaded_count integer,
   uploaded_on TIMESTAMPTZ,
   uploaded_by VARCHAR(255),
   content_type VARCHAR(255)
);