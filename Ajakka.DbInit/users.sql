CREATE TABLE IF NOT EXISTS users (
  id char(36) NOT NULL DEFAULT '',
  name varchar(255) NOT NULL DEFAULT '',
  pwdHash varchar(255) DEFAULT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;