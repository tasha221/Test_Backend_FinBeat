Для Задачи 1 таблицы в MsSql

CREATE TABLE dbo.fb_values (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code INT NOT NULL,
    Value VARCHAR(MAX) NULL
);

CREATE TABLE dbo.fb_requestLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Method NVARCHAR(50) NOT NULL,
    Path NVARCHAR(200) NOT NULL,
    QueryString NVARCHAR(MAX),
    RequestBody NVARCHAR(MAX),
    ResponseBody NVARCHAR(MAX),
    StatusCode INT,
    Timestamp DATETIME NOT NULL
);

