CREATE TABLE IF NOT EXISTS users (
  id char(36) NOT NULL DEFAULT '',
  name varchar(255) NOT NULL DEFAULT '',
  showVendorLogos INT NOT NULL DEFAULT '0',
  pwdHash varchar(255) DEFAULT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO users (id,name,pwdHash) values ('admin','admin',NULL);