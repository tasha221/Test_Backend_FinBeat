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

Задача 2

Написать запрос, который возвращает наименование клиентов и кол-во контактов клиентов

select 
	c.ClientName, 
	Count = count(c.Id) 
from dbo.Clients c
left join dbo.ClientContacts cc on c.Id = cc.ClientId
group by c.ClientName

Написать запрос, который возвращает список клиентов, у которых есть более 2 контактов

select 
	c.ClientName, 
	Count = count(c.Id) 
from dbo.Clients c
left join dbo.ClientContacts cc on c.Id = cc.ClientId
group by c.ClientName
having count(c.Id) > 2;

Задача 3 
Написать запрос, который возвращает интервалы для одинаковых Id. 

select
    d1.Id,
    d1.Dt AS Sd,
    (
        select min(d2.Dt)
        from Dates d2
        where d2.Id = d1.Id and d2.Dt > d1.Dt 
    ) as Ed
from
    Dates d1
where 
    (
        select min(d2.Dt)
        from Dates d2
        where d2.Id = d1.Id and d2.Dt > d1.Dt
    ) is not null
order by
    d1.Id, d1.Dt;
