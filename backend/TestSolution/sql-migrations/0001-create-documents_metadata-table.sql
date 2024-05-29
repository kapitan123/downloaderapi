CREATE TABLE documents_metadata (
   id UUID PRIMARY KEY,
   name VARCHAR(255),
   content_type VARCHAR(255),
   size bigint,
   uploaded_on TIMESTAMPTZ,
   uploaded_by VARCHAR(255),
   downloads_count integer
);