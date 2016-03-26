PRAGMA foreign_keys = off;

CREATE TABLE IF NOT EXISTS EnvironmentVariable (
    Id    INTEGER PRIMARY KEY AUTOINCREMENT
                  NOT NULL,
    [Key] TEXT    NOT NULL,
    Value TEXT    NOT NULL
);

CREATE TABLE IF NOT EXISTS LaunchGroup (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT
                     NOT NULL,
    ParentId INTEGER REFERENCES LaunchGroup (Id) ON DELETE CASCADE,
    Name     TEXT    NOT NULL
);

CREATE TABLE IF NOT EXISTS LaunchGroupVariables (
    LaunchGroupId         INTEGER NOT NULL
                                  REFERENCES LaunchGroup (Id) ON DELETE CASCADE,
    EnvironmentVariableId INTEGER NOT NULL
                                  REFERENCES EnvironmentVariable (Id) ON DELETE CASCADE,
    PRIMARY KEY (
        LaunchGroupId,
        EnvironmentVariableId
    )
);

CREATE TABLE IF NOT EXISTS LaunchItem (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT
                             NOT NULL,
    ParentId         INTEGER REFERENCES LaunchGroup (Id) ON DELETE CASCADE
                             NOT NULL,
    Name             TEXT    NOT NULL,
    Path             TEXT    NOT NULL,
    WorkingDirectory TEXT    NOT NULL
);

CREATE TABLE IF NOT EXISTS LaunchItemVariables (
    LaunchItemId          INTEGER NOT NULL
                          REFERENCES LaunchItem (Id) ON DELETE CASCADE,
    EnvironmentVariableId INTEGER NOT NULL
                          REFERENCES EnvironmentVariable (Id) ON DELETE CASCADE,
    PRIMARY KEY (
        LaunchItemId,
        EnvironmentVariableId
    )
);

CREATE TABLE IF NOT EXISTS DatabaseVersion (
    Version INTEGER PRIMARY KEY DESC NOT NULL
);

INSERT OR IGNORE INTO DatabaseVersion (Version) VALUES (0);

PRAGMA foreign_keys = on;
