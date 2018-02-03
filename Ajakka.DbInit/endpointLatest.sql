CREATE TABLE endpoint_latest (
  mac char(12),
  ip varchar(39),
  hostname varchar(255),
  lastseen DATETIME,
  PRIMARY KEY (mac)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;