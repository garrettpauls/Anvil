PRAGMA foreign_keys = off;

CREATE TABLE EnvironmentVariable (
    Id    INTEGER PRIMARY KEY AUTOINCREMENT
                  NOT NULL,
    [Key] TEXT    NOT NULL,
    Value TEXT    NOT NULL
);

CREATE TABLE LaunchGroup (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT
                     NOT NULL,
    ParentId INTEGER REFERENCES LaunchGroup (Id) ON DELETE CASCADE,
    Name     TEXT    NOT NULL
);

CREATE TABLE LaunchGroupVariables (
    LaunchGroupId         INTEGER NOT NULL
                                  REFERENCES LaunchGroup (Id) ON DELETE CASCADE,
    EnvironmentVariableId INTEGER NOT NULL
                                  REFERENCES EnvironmentVariable (Id) ON DELETE CASCADE,
    PRIMARY KEY (
        LaunchGroupId,
        EnvironmentVariableId
    )
);

CREATE TABLE LaunchItem (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT
                             NOT NULL,
    ParentId         INTEGER REFERENCES LaunchGroup (Id) ON DELETE CASCADE
                             NOT NULL,
    Name             TEXT    NOT NULL,
    Path             TEXT    NOT NULL,
    WorkingDirectory TEXT    NOT NULL
);

CREATE TABLE LaunchItemVariables (
    LaunchItemId          INTEGER NOT NULL
                          REFERENCES LaunchItem (Id) ON DELETE CASCADE,
    EnvironmentVariableId INTEGER NOT NULL
                          REFERENCES EnvironmentVariable (Id) ON DELETE CASCADE,
    PRIMARY KEY (
        LaunchItemId,
        EnvironmentVariableId
    )
);

PRAGMA foreign_keys = on;