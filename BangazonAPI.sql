-- USE MASTER
-- GO

-- IF NOT EXISTS (
--     SELECT [name]
--     FROM sys.databases
--     WHERE [name] = N'BangazonAPI'
-- )
-- CREATE DATABASE BangazonAPI
-- GO

-- USE BangazonAPI
-- GO

-- DELETE FROM OrderProduct;
-- DELETE FROM ComputerEmployee;
-- DELETE FROM EmployeeTraining;
-- DELETE FROM Employee;
-- DELETE FROM TrainingProgram;
-- DELETE FROM Computer;
-- DELETE FROM Department;
-- DELETE FROM [Order];
-- DELETE FROM PaymentType;
-- DELETE FROM Product;
-- DELETE FROM ProductType;
-- DELETE FROM Customer;


-- ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
-- ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
-- ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
-- ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
-- ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];
-- ALTER TABLE Product DROP CONSTRAINT [FK_Product_ProductType];
-- ALTER TABLE Product DROP CONSTRAINT [FK_Product_Customer];
-- ALTER TABLE PaymentType DROP CONSTRAINT [FK_PaymentType_Customer];
-- ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Customer];
-- ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Payment];
-- ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Product];
-- ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Order];


-- DROP TABLE IF EXISTS OrderProduct;
-- DROP TABLE IF EXISTS ComputerEmployee;
-- DROP TABLE IF EXISTS EmployeeTraining;
-- DROP TABLE IF EXISTS Employee;
-- DROP TABLE IF EXISTS TrainingProgram;
-- DROP TABLE IF EXISTS Computer;
-- DROP TABLE IF EXISTS Department;
-- DROP TABLE IF EXISTS [Order];
-- DROP TABLE IF EXISTS PaymentType;
-- DROP TABLE IF EXISTS Product;
-- DROP TABLE IF EXISTS ProductType;
-- DROP TABLE IF EXISTS Customer;


CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	INTEGER NOT NULL
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);

CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Manufacturer VARCHAR(55) NOT NULL
);

CREATE TABLE ComputerEmployee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	ComputerId INTEGER NOT NULL,
	AssignDate DATETIME NOT NULL,
	UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);


CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(255) NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price INTEGER NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);


CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber VARCHAR(55) NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);
