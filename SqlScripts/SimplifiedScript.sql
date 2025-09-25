--Simplified Billing to Payment
--U only Need to know the exact SessionId

select * from TableSession
select * from Orders

-- Confirmed All Orders
UPDATE Orders
SET OrderStatusId = (
    SELECT OrderStatusId 
    FROM OrderStatus 
    WHERE StatusName = 'Confirmed'
)
-- Get;Set; SessionId 
-- Change All OrderItem to Served
-- based on the SessionId
WHERE SessionId = 'F89FFD84-744D-4028-A26A-C76414B9D1EA';
UPDATE OrderItem
SET OrderItemStatusId = (
    SELECT OrderItemStatusId 
    FROM OrderItemStatus 
    WHERE StatusName = 'Served'
)
WHERE OrderId IN (
    SELECT OrderId 
    FROM Orders 
    WHERE SessionId = 'F89FFD84-744D-4028-A26A-C76414B9D1EA'
);

-- Define the SessionId to be processed
DECLARE @SessionId NVARCHAR(50) = 'F89FFD84-744D-4028-A26A-C76414B9D1EA';

-- =======================================================
-- Step 1: Calculate the final bill totals
-- =======================================================
WITH BillSubtotal AS (
    SELECT
        SUM(p.Price * oi.Quantity) AS TotalSubtotal
    FROM
        Orders AS o
    JOIN
        OrderItem AS oi ON o.OrderId = oi.OrderId
    JOIN
        Product AS p ON oi.ProdId = p.ProdId
    JOIN
        OrderItemStatus AS ois ON oi.OrderItemStatusId = ois.OrderItemStatusId
    WHERE
        o.SessionId = @SessionId
        AND ois.StatusName = 'Served'
)
SELECT
    TotalSubtotal,
    (TotalSubtotal * 0.12) AS VAT,
    (TotalSubtotal * 0.10) AS ServiceCharge,
    (TotalSubtotal + (TotalSubtotal * 0.12) + (TotalSubtotal * 0.10)) AS GrandTotal
INTO #BillSummary
FROM BillSubtotal;

-- Declare variables for the calculated totals and new IDs
DECLARE @Subtotal DECIMAL(10,2),
        @VatAmount DECIMAL(10,2),
        @ServiceCharge DECIMAL(10,2),
        @GrandTotal DECIMAL(10,2),
        @NewBillingId NVARCHAR(50);

SELECT
    @Subtotal = TotalSubtotal,
    @VatAmount = VAT,
    @ServiceCharge = ServiceCharge,
    @GrandTotal = GrandTotal
FROM #BillSummary;


-- =======================================================
-- Step 2: Insert into the Billing table
-- =======================================================
SET @NewBillingId = CONVERT(NVARCHAR(50), NEWID());
INSERT INTO Billing (BillingId, SessionId, BillingTime, Subtotal, VatAmount, ServiceCharge, GrandTotal)
VALUES (@NewBillingId, @SessionId, GETDATE(), @Subtotal, @VatAmount, @ServiceCharge, @GrandTotal);


-- =======================================================
-- Step 3: Insert into the Payment table (assuming full payment)
-- =======================================================
INSERT INTO Payment (BillingId, AmountPaid, PaymentTime)
VALUES (@NewBillingId, @GrandTotal, GETDATE());


-- =======================================================
-- Step 4: End the table session
-- =======================================================
UPDATE TableSession
SET EndedAt = GETDATE()
WHERE SessionId = @SessionId;


-- =======================================================
-- Step 5: Update the table status to 'Available'
-- =======================================================
UPDATE T
SET T.TableStatusId = (SELECT TableStatusId FROM TableStatus WHERE StatusName = 'Available')
FROM [Table] AS T
JOIN TableSession AS TS ON T.TableId = TS.TableId
WHERE TS.SessionId = @SessionId;

-- Display Billing & Payment
SELECT * FROM Billing
SELECT * FROM BillingOrder
SELECT * FROM Payment

--delete FROM  OrderItem
--delete FROM  Orders
--delete from Payment
--delete from Billing
--delete FROM  TableSession