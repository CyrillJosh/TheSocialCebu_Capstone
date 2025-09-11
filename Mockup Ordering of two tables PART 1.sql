-- Comprehensive SQL mockup data
-- Step by Step insertion of mockup datas from Ordering to Payment

-- Initialize Table V1
	DECLARE @V1 NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID());
	DECLARE @V2 NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID());

-- Insert Table V1 on VIP location
	INSERT INTO [Table] (TableId, TableNumber, QRCodeImage, LocationId)
		VALUES (@V1, 'V1', NULL, (SELECT TOP 1 LocationId FROM [Location] WHERE LocationName = 'VIP')),
				(@V2, 'V2', NULL, (SELECT TOP 1 LocationId FROM [Location] WHERE LocationName = 'VIP'));

-- Insert Tables V3, B1, B3 on VIP and Back location respectively
	INSERT INTO [Table] (TableId, TableNumber, QRCodeImage, LocationId)
		SELECT CONVERT(NVARCHAR(50), NEWID()), TableNumber, NULL, LocationId
		FROM (
		
			SELECT 'V3' AS TableNumber, 'VIP' AS LocationName
			UNION ALL
			SELECT 'B1', 'Back'
			UNION ALL
			SELECT 'B3', 'Back'
		) AS TableNumbers
		JOIN [Location] L ON L.LocationName = TableNumbers.LocationName;

-- Declare variables to hold newly created IDs of V1's Orders
	DECLARE @V1OrderIds TABLE (OrderId NVARCHAR(50));
	DECLARE @V2OrderIds TABLE (OrderId NVARCHAR(50));

-- Insert 3 order V1 table with OrderStatusId = 1
INSERT INTO Orders (TableId, CreatedAt, OrderStatusId)
OUTPUT inserted.OrderId INTO @V1OrderIds
SELECT @V1, GETDATE(), 1
UNION ALL
SELECT @V1, GETDATE(), 1
UNION ALL
SELECT @V1, GETDATE(), 1;


-- Insert 2 order V2 table with OrderStatusId = 1
INSERT INTO Orders (TableId, CreatedAt, OrderStatusId)
OUTPUT inserted.OrderId INTO @V2OrderIds
SELECT @V2, GETDATE(), 1
UNION ALL
SELECT @V2, GETDATE(), 1
UNION ALL
SELECT @V2, GETDATE(), 1;


-- Insert order items for each order in Table V1:
-- First order (order #1)
INSERT INTO OrderItem(OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT
    o.OrderId,
    p.ProdId,
    qty,
    1
FROM
    (SELECT TOP 1 OrderId FROM @V1OrderIds ORDER BY OrderId) o
    CROSS APPLY (VALUES
        ((SELECT ProdId FROM Product WHERE ProdName = 'Vanilla Ice Cream'), 1),
        ((SELECT ProdId FROM Product WHERE ProdName = 'Cheesecake'), 1)
    ) AS p(ProdId, qty);

-- Second order (order #2)
INSERT INTO OrderItem (OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT
    o.OrderId,
    p.ProdId,
    qty,
    1
FROM
    (SELECT OrderId FROM @V1OrderIds ORDER BY OrderId OFFSET 1 ROWS FETCH NEXT 1 ROW ONLY) o
    CROSS APPLY (VALUES
        ((SELECT ProdId FROM Product WHERE ProdName = 'Fries'), 1),
        ((SELECT ProdId FROM Product WHERE ProdName = 'Vanilla Ice Cream'), 1)
    ) AS p(ProdId, qty);

-- Third order (order #3)
INSERT INTO OrderItem (OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT
    o.OrderId,
    p.ProdId,
    qty,
    1 
FROM
    (SELECT OrderId FROM @V1OrderIds ORDER BY OrderId OFFSET 2 ROWS FETCH NEXT 1 ROW ONLY) o
    CROSS APPLY (VALUES
        ((SELECT ProdId FROM Product WHERE ProdName = 'Peach Fizz'), 1),
        ((SELECT ProdId FROM Product WHERE ProdName = 'Cheesecake'), 2)
    ) AS p(ProdId, qty);


-- First order of V2 (order #1)
INSERT INTO OrderItem (OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT
    o.OrderId,
    p.ProdId,
    qty,
    1
FROM
    (SELECT OrderId FROM @V2OrderIds ORDER BY OrderId OFFSET 1 ROWS FETCH NEXT 1 ROW ONLY) o
    CROSS APPLY (VALUES
        ((SELECT ProdId FROM Product WHERE ProdName = 'Cheesecake'), 2)
    ) AS p(ProdId, qty);


-- Second order (order #2)
INSERT INTO OrderItem (OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT
    o.OrderId,
    p.ProdId,
    qty,
    1
FROM
    (SELECT OrderId FROM @V2OrderIds ORDER BY OrderId OFFSET 1 ROWS FETCH NEXT 1 ROW ONLY) o
    CROSS APPLY (VALUES
        ((SELECT ProdId FROM Product WHERE ProdName = 'Fries'), 1),
        ((SELECT ProdId FROM Product WHERE ProdName = 'Vanilla Ice Cream'), 1)
    ) AS p(ProdId, qty);

	
-- Third order (order #3)
INSERT INTO OrderItem (OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT
    o.OrderId,
    p.ProdId,
    qty,
    1
FROM
    (SELECT OrderId FROM @V2OrderIds ORDER BY OrderId OFFSET 1 ROWS FETCH NEXT 1 ROW ONLY) o
    CROSS APPLY (VALUES
    ((SELECT ProdId FROM Product WHERE ProdName = 'Social Pancakes'), 2)
    ) AS p(ProdId, qty);



-- Initialize Table B2
DECLARE @B2 NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID());

-- Insert Table B2 on Back Location
INSERT INTO [Table] (TableId, TableNumber, QRCodeImage, LocationId)
VALUES (@B2, 'B2', NULL, (SELECT TOP 1 LocationId FROM [Location] WHERE LocationName = 'Back'));

-- Variables to hold newly created IDs of B2's Orders
DECLARE @B2OrderIds TABLE (OrderId NVARCHAR(50), RowNum INT);

-- Insert 3 orders (Customer 1, Customer 2, Customer 1 again)
INSERT INTO Orders (TableId, CreatedAt, OrderStatusId)
OUTPUT inserted.OrderId INTO @B2OrderIds(OrderId)
SELECT @B2, GETDATE(), 1
UNION ALL
SELECT @B2, GETDATE(), 1
UNION ALL
SELECT @B2, GETDATE(), 1;

-- Update the table variable to assign row numbers
;WITH Ordered AS (
    SELECT OrderId, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS RowNum
    FROM @B2OrderIds
)
UPDATE b
SET RowNum = o.RowNum
FROM @B2OrderIds b
JOIN Ordered o ON b.OrderId = o.OrderId;
-- Order 1 (Customer 1)
INSERT INTO OrderItem(OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT o.OrderId, p.ProdId, qty, 1
FROM (SELECT OrderId FROM @B2OrderIds WHERE RowNum = 1) o
CROSS APPLY (VALUES
    ((SELECT ProdId FROM Product WHERE ProdName = 'Eggs Benedict'), 1),
    ((SELECT ProdId FROM Product WHERE ProdName = 'Social Pancakes'), 1)
) AS p(ProdId, qty);

-- Order 2 (Customer 2)
INSERT INTO OrderItem(OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT o.OrderId, p.ProdId, qty, 1
FROM (SELECT OrderId FROM @B2OrderIds WHERE RowNum = 2) o
CROSS APPLY (VALUES
    ((SELECT ProdId FROM Product WHERE ProdName = 'Social Pancakes'), 2)
) AS p(ProdId, qty);

-- Order 3 (Customer 1 again)
INSERT INTO OrderItem(OrderId, ProdId, Quantity, OrderItemStatusId)
SELECT o.OrderId, p.ProdId, qty, 1
FROM (SELECT OrderId FROM @B2OrderIds WHERE RowNum = 3) o
CROSS APPLY (VALUES
    ((SELECT ProdId FROM Product WHERE ProdName = 'Mango'), 1)
) AS p(ProdId, qty);


-- Update all orders to 'Confirmed'
UPDATE Orders
SET OrderStatusId = (SELECT OrderStatusId FROM OrderStatus WHERE StatusName = 'Confirmed');

---- Update all order items to 'Served', excluding those from the second order of the V1 table
--DECLARE @V1SecondOrderId NVARCHAR(50);

--SELECT @V1SecondOrderId = OrderId
--FROM (
--    SELECT OrderId,
--           ROW_NUMBER() OVER (ORDER BY CreatedAt) AS rn
--    FROM Orders
--) AS sub
--WHERE sub.rn = 2;

UPDATE OrderItem
SET OrderItemStatusId = (SELECT OrderItemStatusId FROM OrderItemStatus WHERE StatusName = 'Served')


-- Update all Status of Orders to complete; 
-- Excluding those Orders that their corresponding order items are not yet 'Served'
UPDATE Orders
SET OrderStatusId = (SELECT OrderStatusId FROM OrderStatus WHERE StatusName = 'Completed')
FROM Orders
Where OrderId in (SELECT OrderId FROM OrderItem GROUP BY OrderId HAVING MIN(OrderItemStatusId) = (SELECT OrderItemStatusId FROM OrderItemStatus WHERE StatusName = 'Served')
    AND MAX(OrderItemStatusId) = (SELECT OrderItemStatusId FROM OrderItemStatus WHERE StatusName = 'Served'))

SELECT  
    t.TableNumber,     
    o.OrderId,  
    os.StatusName AS OrderStatus,  
    sc.SubcategoryName,     
    p.ProdName,     
    oi.Quantity,  
    p.Price,
    (oi.Quantity * p.Price) AS Subtotal,
    ois.StatusName AS ItemStatus
FROM [Table] t 
JOIN Orders o ON o.TableId = t.TableId 
JOIN OrderStatus os ON o.OrderStatusId = os.OrderStatusId 
JOIN OrderItem oi ON oi.OrderId = o.OrderId 
JOIN OrderItemStatus ois ON oi.OrderItemStatusId = ois.OrderItemStatusId 
JOIN Product p ON p.ProdId = oi.ProdId 
JOIN SubCategory sc ON p.SubcategoryId = sc.SubcategoryId 
ORDER BY t.TableNumber;