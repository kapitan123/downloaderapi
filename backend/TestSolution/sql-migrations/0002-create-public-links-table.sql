CREATE TABLE public_links (
   id VARCHAR(255) PRIMARY KEY,
   document_id UUID,
   created_on TIMESTAMPTZ,
   expires_by VARCHAR(255)
);