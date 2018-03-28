CREATE TABLE IF NOT EXISTS usersettings (
	settingskey VARCHAR(50) NOT NULL,
	userid CHAR(36) NOT NULL,
	settingsvalue VARCHAR(50) NULL DEFAULT NULL
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
