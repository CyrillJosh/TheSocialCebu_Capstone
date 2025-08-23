-- Part 2 of the 'Comprehensive SQL mockup data'
-- Proceeding on billing to payment

-- Run this 'SELECT' command first before the others
SELECT
	t.TableNumber,
	ts.SessionId,
	ts.EndedAt,
    o.OrderId,
	os.StatusName,
	sc.SubcategoryName,
    p.ProdName,
    oi.Quantity,
	ois.StatusName
FROM [Table] t
JOIN TableSession ts ON ts.TableId = t.TableId
JOIN Orders o ON o.SessionId = ts.SessionId
JOIN OrderStatus os ON o.OrderStatusId = os.OrderStatusId
JOIN OrderItem oi ON oi.OrderId = o.OrderId
JOIN OrderItemStatus ois ON oi.OrderItemStatusId = ois.OrderItemStatusId
JOIN Product p ON p.ProdId = oi.ProdId
Join SubCategory sc on p.SubcategoryId = sc.SubcategoryId
ORDER BY t.TableNumber


-- Declare the variable to hold the TableId for V1
-- Set the TableId of 'V1' into scalar variable '@V1TableId'
DECLARE @V1TableId NVARCHAR(50);
SELECT @V1TableId = TableId FROM [Table] WHERE TableNumber = 'V1';

-- Check if Table V1 exists
-- Can be commented as well
IF @V1TableId IS NULL
BEGIN
    PRINT 'Table V1 not found. Aborting billing process.';
    RETURN;
END

-- Use a temporary table to store sessions that need to be billed
CREATE TABLE #SessionsToBill (
    SessionId NVARCHAR(50)
);

-- Find all active sessions for the specified table
INSERT INTO #SessionsToBill (SessionId)
SELECT SessionId
FROM TableSession
WHERE TableId = @V1TableId AND EndedAt IS NULL;

-- Check if any active sessions were found
IF NOT EXISTS (SELECT 1 FROM #SessionsToBill)
BEGIN
    PRINT 'No active sessions found for Table V1. No bill will be created.';
    DROP TABLE #SessionsToBill;
    RETURN;
END;

-- =======================================================
-- Step 1: Calculate the final bill totals for ALL sessions
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
        o.SessionId IN (SELECT SessionId FROM #SessionsToBill)
        AND ois.StatusName = 'Served'
)
SELECT
    TotalSubtotal,
    (TotalSubtotal * 0.12) AS VAT,
    (TotalSubtotal * 0.10) AS ServiceCharge,
    (TotalSubtotal + (TotalSubtotal * 0.12) + (TotalSubtotal * 0.10)) AS GrandTotal
INTO #BillSummary
FROM BillSubtotal;

-- Check if a bill summary was calculated
-- Can also be commented
IF NOT EXISTS (SELECT 1 FROM #BillSummary)
BEGIN
    PRINT 'No served items found for the active sessions. No bill created.';
    DROP TABLE #SessionsToBill;
    RETURN;
END

-- Declare variables for the calculated totals and a new ID for the combined bill
DECLARE @Subtotal DECIMAL(10,2),
        @VatAmount DECIMAL(10,2),
        @ServiceCharge DECIMAL(10,2),
        @GrandTotal DECIMAL(10,2),
        @NewBillingId NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID());

SELECT
    @Subtotal = TotalSubtotal,
    @VatAmount = VAT,
    @ServiceCharge = ServiceCharge,
    @GrandTotal = GrandTotal
FROM #BillSummary;

-- =======================================================
-- Step 2: Insert into the Billing table for the combined bill
-- =======================================================
INSERT INTO Billing (BillingId, SessionId, BillingTime, Subtotal, VatAmount, ServiceCharge, GrandTotal)
VALUES (@NewBillingId, (SELECT TOP 1 SessionId FROM #SessionsToBill), GETDATE(), @Subtotal, @VatAmount, @ServiceCharge, @GrandTotal);

-- =======================================================
-- Step 3: Insert into the BillingOrder table to link all orders to the new bill
-- =======================================================
INSERT INTO BillingOrder (BillingId, OrderId)
SELECT @NewBillingId, o.OrderId
FROM Orders o
WHERE o.SessionId IN (SELECT SessionId FROM #SessionsToBill);

-- =======================================================
-- Step 4: Insert into the Payment table (assuming full payment)
-- =======================================================
INSERT INTO Payment (BillingId, AmountPaid, PaymentTime)
VALUES (@NewBillingId, @GrandTotal, GETDATE());

-- =======================================================
-- Step 5: End all active table sessions
-- =======================================================
UPDATE TableSession
SET EndedAt = GETDATE()
WHERE TableId = @V1TableId AND EndedAt IS NULL;

-- =======================================================
-- Step 6: Update the table status to 'Available'
-- =======================================================
UPDATE [Table]
SET TableStatusId = (SELECT TableStatusId FROM TableStatus WHERE StatusName = 'Available')
WHERE TableId = @V1TableId;


