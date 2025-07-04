-- LOCATION TABLE
CREATE TABLE Location (
    Location_Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    Location_Name NVARCHAR(100) NOT NULL
);

-- TABLES TABLE
CREATE TABLE [Table] (
    Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    QRCodeImage VARBINARY(MAX),
    TableNumber NVARCHAR(50) NOT NULL,
    Location_Id NVARCHAR(50) NOT NULL,
    FOREIGN KEY (Location_Id) REFERENCES Location(Location_Id)
);

-- CATEGORY TABLE
CREATE TABLE Category (
    Category_Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    Category_Name NVARCHAR(100) NOT NULL
);

-- SUBCATEGORY TABLE
CREATE TABLE SubCategory (
    Subcategory_Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    Subcategory_Name NVARCHAR(100) NOT NULL,
    Category_Id NVARCHAR(50) NOT NULL,
    FOREIGN KEY (Category_Id) REFERENCES Category(Category_Id)
);

-- PRODUCT TABLE
CREATE TABLE Product (
    Prod_Id NVARCHAR(50) PRIMARY KEY DEFAULT NEWID(),
    Prod_Name NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    Category_Id NVARCHAR(50) NOT NULL,
    Subcategory_Id NVARCHAR(50) NOT NULL,
    Availability BIT NOT NULL DEFAULT 1,
    Prod_Image VARBINARY(MAX),
    FOREIGN KEY (Category_Id) REFERENCES Category(Category_Id),
    FOREIGN KEY (Subcategory_Id) REFERENCES SubCategory(Subcategory_Id)
);


Scaffold-DbContext "Data Source=LAPTOP-K56S2BSD\SQLEXPRESS;Initial Catalog=TheSocialCebu;Integrated Security=True;Trust Server Certificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Context -Context MyDBContext -f
