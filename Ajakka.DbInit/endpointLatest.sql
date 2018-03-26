CREATE TABLE IF NOT EXISTS endpoint_latest (
  mac char(12),
  ip varchar(39),
  hostname varchar(255),
  lastseen DATETIME,
  detectedby VARCHAR(20) NULL DEFAULT NULL,
  PRIMARY KEY (mac)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;