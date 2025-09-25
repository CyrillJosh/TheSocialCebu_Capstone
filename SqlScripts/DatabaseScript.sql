USE MASTER
GO

ALTER DATABASE TheSocialCebu SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE TheSocialCebu

CREATE DATABASE TheSocialCebu
GO

USE TheSocialCebu
GO

--UserRoleAccess
--Role
CREATE Table [Role](
RoleId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
RoleName NVARCHAR(150) NOT NULL
--Admin (Manager) → CRUD menu, override tables/orders, approve discounts, manage accounts.
--Cashier → Generate bills, apply discounts, accept payments.
--Kitchen Staff → Approve/reject orders, mark ready to serve.
--Front Staff → Serve items, manage table delivery, assist customers.
)

-- Person
CREATE Table Person(
PersonId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
RoleId NVARCHAR(50) NOT NULL,
[Name] NVARCHAR(50) NOT NULL,
BirthDate Date NOT NULL,
HiredDate Date NOT NULL,
[Status] BIT DEFAULT 1 Not NULL,
Gender NVARCHAR(50) NOT NULL,
FOREIGN KEY (RoleId) REFERENCES [Role](RoleId)
)

--Account
CREATE Table Account(
AccountId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
PersonId NVARCHAR(50) NOT NULL,
Username NVARCHAR(50) UNIQUE NOT NULL,
[Password] NVARCHAR(50) NOT NULL,
FOREIGN KEY (PersonId) REFERENCES Person(PersonId))

--QR Integration
--Location
CREATE Table [Location](
LocationId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
LocationName nvarchar(50) UNIQUE NOT NULL
)
 INSERT INTO [Location] (LocationName)VALUES
 ('VIP'), ('Front'), ('Back'), ('Side')



-- Table Status Lookup Table (3NF)
CREATE TABLE TableStatus (
    TableStatusId INT PRIMARY KEY IDENTITY,
    StatusName NVARCHAR(50) UNIQUE NOT NULL -- 'Available', 'Occupied', 'Billing', 'Payment'
);
 INSERT INTO TableStatus VALUES
 ('Available'), ('Occupied'), ('Billing'), ('Payment'), ('Unavailable')

--[Table]
CREATE Table [Table](
TableId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
TableNumber NVARCHAR(50) Not Null,
QRCodeImage varbinary(max) NULL,
TableStatusId INT NOT NULL DEFAULT 1, -- Default to 'Available'
LocationId NVARCHAR(50) NOT NULL,
FOREIGN KEY (LocationId) REFERENCES [Location](LocationId),
FOREIGN KEY (TableStatusId) REFERENCES TableStatus(TableStatusId),
CONSTRAINT UQ_Table_TableNumber_Location UNIQUE (TableNumber, LocationId)
)

--Sessions
CREATE TABLE TableSession (
    SessionId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
    TableId NVARCHAR(50) NOT NULL,
    StartedAt DATETIME NOT NULL DEFAULT GETDATE(),
    EndedAt DATETIME NULL,
    FOREIGN KEY (TableId) REFERENCES [Table](TableId)
);

--Menu
--Category
CREATE TABLE Category(
    CategoryId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
    CategoryName NVARCHAR(100) NOT NULL
);

--SubCategory
CREATE TABLE SubCategory(
    SubcategoryId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
    SubcategoryName NVARCHAR(100) NOT NULL,
    CategoryId NVARCHAR(50) NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);

--Products 
CREATE TABLE [Product](
    ProdId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
    ProdName NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(MAX),
    Price DECIMAL(18, 2) NOT NULL,
    Availability BIT DEFAULT 1 Not NULL,
    ProdImage VARBINARY(MAX),
    SubcategoryId NVARCHAR(50) NOT NULL,
    FOREIGN KEY (SubcategoryId) REFERENCES SubCategory(SubcategoryId)
);

--Insertion
	-- Declare Category IDs
	DECLARE 
		@MainCourse NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@BigBites NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@LiteBites NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Sharable NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@AlcoholDrinks NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@NonAlcoholDrinks NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@DessertCat NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID());

	-- Insert Category
	INSERT INTO Category (CategoryId, CategoryName) VALUES
	(@MainCourse, 'MAIN COURSE'),
	(@BigBites, 'BIG BITES'),
	(@LiteBites, 'LITE BITES'),
	(@Sharable, 'Sharable'),
	(@AlcoholDrinks, 'Alcohol Drinks'),
	(@NonAlcoholDrinks, 'Drinks (Non-Alcoholic)'),
	(@DessertCat, 'Dessert');

	-- Declare Subcategory IDs
	DECLARE 
		@Breakfast NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Sizzlers NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Grill NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Pizza NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Pasta NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Burgers NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@CoffeeAndTea NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@SoftDrinks NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@ShakesJuices NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Frappes NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Mocktails NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Dessert NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Sandwiches NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@MeatSeafood NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@BarSnacks NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Sides NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Salads NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@Soup NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@MealPlatter NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@RicePlatter NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
		@BeerBuckets NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID())

	-- Insert SubCategory
	INSERT INTO SubCategory (SubcategoryId, SubcategoryName, CategoryId) VALUES
		--Main Course
		(@Breakfast, 'Breakfast', @MainCourse),
		(@Sizzlers, 'Sizzlers', @MainCourse),
		(@Grill, 'Grill', @MainCourse),
		(@Pizza, 'Pizza', @MainCourse),
		(@Pasta, 'Pasta', @MainCourse),
		-- BigBites
		(@Burgers, 'Burgers', @BigBites),
		(@Sandwiches, 'Sandwiches', @BigBites),
		(@MeatSeafood, 'Meat & Seafood', @BigBites),
		--LiteBites
		(@BarSnacks, 'Bar Snacks', @LiteBites),
		(@Sides, 'Sides', @LiteBites),
		(@Salads, 'Salads', @LiteBites),
		(@Soup, 'Soup', @LiteBites),
		--Shareable
		(@MealPlatter, 'Meal Platter', @Sharable),
		(@RicePlatter, 'Rice Platter', @Sharable),
		(@BeerBuckets, 'Beer Buckets', @Sharable),
		--Drinks (Non Alcoholic)
		(@CoffeeAndTea, 'Coffee & Tea', @NonAlcoholDrinks),
		(@SoftDrinks, 'Soft drinks', @NonAlcoholDrinks),
		(@Frappes, 'Frappes', @NonAlcoholDrinks),
		(@ShakesJuices, 'Shakes/Juices', @NonAlcoholDrinks),
		(@Mocktails, 'Mocktails', @NonAlcoholDrinks),

		--Dessert
		(@Dessert, 'Dessert', @DessertCat);
	
	-- Frappes
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Strawberry Banana', 240, 1,  @Frappes),
	('White Chocolate', 240, 1,  @Frappes),
	('Oreo Ice Cream', 240, 1, @Frappes);

	-- Shakes / Juices
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Mango', 210, 1, @ShakesJuices),
	('Pineapple', 210, 1, @ShakesJuices),
	('Watermelon', 210, 1, @ShakesJuices),
	('Banana', 210, 1, @ShakesJuices),
	('Orange', 210, 1, @ShakesJuices),
	('Green Apple', 210, 1, @ShakesJuices),
	('Two Mixed Fruit', 290, 1, @ShakesJuices);

	-- Breakfast
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Social Pancakes', 320, 1, @Breakfast),
	('Eggs Benedict', 549, 1, @Breakfast),
	('The Social Breakfast', 599, 1, @Breakfast),
	('Avocado Toast', 589, 1, @Breakfast);

	-- Sizzlers
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Squid Sisig', 439, 1, @Sizzlers),
	('Pork Sisig', 499, 1, @Sizzlers),
	('Shrimp w/ Veg', 569, 1, @Sizzlers),
	('Beef Salpicao', 729, 1, @Sizzlers);

	-- Grill
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Half Chicken', 679, 1, @Grill),
	('Sliced Pork Belly', 699, 1, @Grill),
	('Salmon Fillet', 799, 1, @Grill),
	('U.S Beef Steak', 1299, 1, @Grill),
	('Cab Prime Rib Eye', 3199, 1, @Grill);

	-- Pizza
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Margherita', 499, 1, @Pizza),
	('Hawaiian', 599, 1, @Pizza),
	('Mixed', 599, 1, @Pizza),
	('Pepperoni', 619, 1, @Pizza),
	('Cheesy Hungarian', 619, 1, @Pizza),
	('Seafood Overload', 649, 1, @Pizza);

	-- Pasta
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Aglio Olio', 349, 1, @Pasta),
	('Beef Bolognese', 619, 1, @Pasta),
	('Carbonara', 639, 1, @Pasta),
	('Seafood Marinara', 649, 1, @Pasta);

	-- Burgers
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Pulled Pork Sliders', 339, 1, @Burgers),
	('Mini Beef Sliders', 359, 1, @Burgers),
	('The Social Burger', 629, 1, @Burgers);

	-- Sandwiches
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Egg Tuna Sandwich', 319, 1, @Sandwiches),
	('Cheesy Pulled Pork Sandwich', 329, 1, @Sandwiches),
	('Bacon and Egg Sandwich', 349, 1, @Sandwiches),
	('Social Cheese Steak', 599, 1, @Sandwiches);

	-- Meat & Seafood
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Fish & Chips', 569, 1, @MeatSeafood),
	('Bangers and Mashed', 569, 1, @MeatSeafood),
	('Chicken Parmigiana', 609, 1, @MeatSeafood),
	('Pigged Out Chop', 879, 1, @MeatSeafood),
	('Texas Smoked Ribs', 909, 1, @MeatSeafood),
	('U.S Beef Brisket', 1099, 1, @MeatSeafood),
	('Crispy Pata', 1229, 1, @MeatSeafood);

	-- Bar Snacks
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Roasted Garlic Peanuts', 229, 1, @BarSnacks),
	('Bucket Fries', 329, 1, @BarSnacks),
	('Chicken Fajita Taco', 469, 1, @BarSnacks),
	('Calamari', 489, 1, @BarSnacks),
	('Prawn Fritters', 529, 1, @BarSnacks),
	('Chicken Wings', 539, 1, @BarSnacks),
	('Social Nachos', 819, 1, @BarSnacks);

	-- Sides
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Fries', 239, 1, @Sides),
	('Mashed Potato', 249, 1, @Sides),
	('Roasted Veggies', 249, 1, @Sides);

	-- Salads
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Caesar Salad', 559, 1, @Salads),
	('Chicken Oriental Salad', 569, 1, @Salads),
	('Greek Salad', 469, 1, @Salads),
	('House Salad', 250, 1, @Salads);

	-- Soup
	INSERT INTO Product (ProdName, Price, Availability, SubcategoryId) VALUES
	('Cream of Tomato', 269, 1, @Soup);

	-- Meal Platter
	INSERT INTO Product (ProdName, Price, SubcategoryId, Availability)
	VALUES 
	('Social Skewers', 1499, @MealPlatter, 1),
	('The Social', 1999, @MealPlatter, 1),
	('Surf and Turf', 2199, @MealPlatter, 1);

	-- Coffee & Tea
	INSERT INTO Product (ProdName, Price, SubcategoryId, Availability)
	VALUES 
	('Espresso', 110, @CoffeeAndTea, 1),
	('Americano', 140, @CoffeeAndTea, 1),
	('Cafe Latte', 160, @CoffeeAndTea, 1),
	('Cappucino', 160, @CoffeeAndTea, 1),
	('Cafe Mocha', 190, @CoffeeAndTea, 1),
	('Caramel Latte', 190, @CoffeeAndTea, 1),
	('White Chocolate Latte', 190, @CoffeeAndTea, 1),
	('Green Tea', 150, @CoffeeAndTea, 1),
	('English Breakfast', 150, @CoffeeAndTea, 1),
	('Camomile', 150, @CoffeeAndTea, 1),
	('Four Red Fruit', 150, @CoffeeAndTea, 1);

	-- Soft Drinks
	INSERT INTO Product (ProdName, Price, SubcategoryId, Availability)
	VALUES 
	('Pepsi', 99, @SoftDrinks, 1),
	('7Up', 99, @SoftDrinks, 1),
	('Pepsi Max', 99, @SoftDrinks, 1),
	('Mt Dew', 99, @SoftDrinks, 1),
	('Soda Water', 99, @SoftDrinks, 1),
	('Tonic Water', 99, @SoftDrinks, 1),
	('Bottled Water', 99, @SoftDrinks, 1),
	('Ginger Ale', 150, @SoftDrinks, 1),
	('Sparkling Water', 199, @SoftDrinks, 1),
	('RedBull', 199, @SoftDrinks, 1);

	-- Mocktails
	INSERT INTO Product (ProdName, Price, SubcategoryId, Availability)
	VALUES 
	('Watermelon Mojito', 240, @Mocktails, 1),
	('Minty Mango Peach', 240, @Mocktails, 1),
	('Cebu Sunset', 240, @Mocktails, 1),
	('Brewed Ice Tea', 240, @Mocktails, 1),
	('Peach Fizz', 240, @Mocktails, 1),
	('Green Apple Cucumber Fizz', 240, @Mocktails, 1);

	-- Dessert
	INSERT INTO Product (ProdName, Price, SubcategoryId, Availability)
	VALUES 
	('Cheesecake', 300, @Dessert, 1),
	('Mango Sticky Rice', 300, @Dessert, 1),
	('Moist Chocolate Cake', 319, @Dessert, 1),
	('Vanilla Ice Cream', 250, @Dessert, 1);

--Ordering
--OrderStatus
CREATE TABLE OrderStatus (
    OrderStatusId INT PRIMARY KEY IDENTITY,
    StatusName NVARCHAR(50) UNIQUE NOT NULL -- 'Active', 'Completed', 'Cancelled'
);

 INSERT INTO OrderStatus VALUES
 ('Pending'), ('Confirmed'), ('Completed') 

-- OrderItem Status Lookup Table (3NF)
CREATE TABLE OrderItemStatus (
    OrderItemStatusId INT PRIMARY KEY IDENTITY,
    StatusName NVARCHAR(50) UNIQUE NOT NULL -- 'Pending', 'Preparing', 'Served', 'Cancelled'
);

 INSERT INTO OrderItemStatus VALUES
 ('Pending'), ('Preparing'), ('Served')

--Orders
CREATE TABLE Orders(
OrderId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
SessionId NVARCHAR(50) NOT NULL, 
CreatedAt DATETIME,
OrderStatusId INT NOT NULL,
FOREIGN KEY (OrderStatusId) REFERENCES OrderStatus(OrderStatusId),
FOREIGN KEY (TableId) REFERENCES [Table](TableId)
)

--OrderItem
CREATE TABLE OrderItem(
OrderItemId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
OrderId NVARCHAR(50) NOT NULL,
ProdId NVARCHAR(50) NOT NULL,
Quantity INT,
Instructions NVARCHAR(100) NULL,
OrderItemStatusId INT NOT NULL, -- Changed from NVARCHAR
-- pending, preparing, serving, served, cancelled
FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
FOREIGN KEY (OrderItemStatusId) REFERENCES OrderItemStatus(OrderItemStatusId),
FOREIGN KEY (ProdId) REFERENCES [Product](ProdId)
)

--CREATE TABLE Cancellation(
--    CancellationId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
--    OrderItemId NVARCHAR(50) NOT NULL,
--    Reason NVARCHAR(200) NOT NULL,
--    CancelledBy NVARCHAR(50) NOT NULL,    -- staff who approved
--    CancelledAt DATETIME DEFAULT GETDATE(),
--    FOREIGN KEY (OrderItemId) REFERENCES OrderItem(OrderItemId),
--    FOREIGN KEY (CancelledBy) REFERENCES Person(PersonId)
--);

--Billing & Discounts
--Billing
CREATE TABLE Billing(
BillingId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
SessionId NVARCHAR(50) NOT NULL,
BillingTime DATETIME DEFAULT GETDATE(),
Subtotal DECIMAL (10,2),
VatAmount DECIMAL (10,2),
ServiceCharge DECIMAL (10,2),
GrandTotal DECIMAL (10,2),
FOREIGN KEY (SessionId) REFERENCES TableSession(SessionId)
)

--associative entity
CREATE TABLE BillingOrder (
    BillingOrderId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()), -- Unique ID for the association
    BillingId NVARCHAR(50) NOT NULL,         -- Foreign Key to Billing
    OrderId NVARCHAR(50) NOT NULL,           -- Foreign Key to Orders
    FOREIGN KEY (BillingId) REFERENCES Billing(BillingId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
)


--Payment
CREATE TABLE Payment(
PaymentId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
BillingId NVARCHAR(50) NOT NULL,
AmountPaid DECIMAL (10,2),
PaymentTime DATETIME,
FOREIGN KEY (BillingId) REFERENCES Billing(BillingId)
)

--Discounts
--DiscountsTypes
CREATE TABLE DiscountType(
    DiscountTypeId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
    DiscountName NVARCHAR(100) NOT NULL,
    Percentage DECIMAL(5,2) NOT NULL     
);

CREATE TABLE Discounts(
    DiscountId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
    BillingId NVARCHAR(50) NOT NULL,
    DiscountTypeId NVARCHAR(50) NOT NULL,
    ApprovedBy NVARCHAR(50) NULL,         -- Cashier/Admin who approved
    ApprovedAt DATETIME NULL,
    FOREIGN KEY (BillingId) REFERENCES Billing(BillingId),
    FOREIGN KEY (DiscountTypeId) REFERENCES DiscountType(DiscountTypeId),
    FOREIGN KEY (ApprovedBy) REFERENCES Person(PersonId)
)

--Feedback & Marketing
--Feedback
CREATE TABLE Feedback(
FeedbackId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
BillingId NVARCHAR(50) NOT NULL,
Rating INT CHECK (Rating IN (1,2,3,4,5)),
FOREIGN KEY (BillingId) REFERENCES Billing(BillingId)
)

--Marketing
CREATE TABLE Marketing(
EmailId NVARCHAR(50) PRIMARY KEY DEFAULT CONVERT(NVARCHAR(50), NEWID()),
Email NVARCHAR(50) UNIQUE NOT NULL
)