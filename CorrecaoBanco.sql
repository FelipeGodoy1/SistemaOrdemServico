ALTER TABLE cadFunc
DROP COLUMN endere�o;

ALTER TABLE cadClientForn
DROP COLUMN endere�o;

ALTER TABLE cadFunc
ADD bairro nvarchar(40),
    numero nvarchar(20),
	rua nvarchar(40);

ALTER TABLE cadClientForn
ADD rua nvarchar(40);