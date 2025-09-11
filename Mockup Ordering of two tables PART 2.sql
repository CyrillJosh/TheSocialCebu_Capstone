-- =============================================
-- Part 2: Billing to Payment
-- Refined for SQL Server
-- =============================================
-- =============================================
-- Comprehensive SQL Mockup Data: Billing to Payment
-- =============================================

-- =============================================
-- Step 0: Declare Table IDs
-- =============================================
DECLARE 
    @V1TableId NVARCHAR(50) = (SELECT TableId FROM [Table] WHERE TableNumber = 'V1'),
    @V2TableId NVARCHAR(50) = (SELECT TableId FROM [Table] WHERE TableNumber = 'V2'),
    @B2TableId NVARCHAR(50) = (SELECT TableId FROM [Table] WHERE TableNumber = 'B2');

-- =============================================
-- Step 1: Billing for V1 (VAT-inclusive, simple)
-- =============================================
DECLARE 
    @V1BillingId NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
    @V1Subtotal DECIMAL(10,2),
    @V1VatSale DECIMAL(10,2),
    @V1VatAmount DECIMAL(10,2),
    @V1ServiceCharge DECIMAL(10,2),
    @V1GrandTotal DECIMAL(10,2);

SELECT @V1Subtotal = SUM(p.Price * oi.Quantity)
FROM Orders o
JOIN OrderItem oi ON o.OrderId = oi.OrderId
JOIN Product p ON oi.ProdId = p.ProdId
JOIN OrderItemStatus ois ON oi.OrderItemStatusId = ois.OrderItemStatusId
WHERE o.TableId = @V1TableId
  AND ois.StatusName = 'Served';

SET @V1VatSale = @V1Subtotal / 1.12;
SET @V1VatAmount = @V1Subtotal - @V1VatSale;
SET @V1ServiceCharge = @V1Subtotal * 0.10;
SET @V1GrandTotal = @V1Subtotal + @V1ServiceCharge;

INSERT INTO Billing (BillingId, TableId, BillingTime, Subtotal, VatableSale, VatExemptSale, VatAmount, ServiceCharge, GrandTotal)
VALUES (@V1BillingId, @V1TableId, GETDATE(), @V1Subtotal, @V1VatSale, 0, @V1VatAmount, @V1ServiceCharge, @V1GrandTotal);

INSERT INTO BillingOrder (BillingId, OrderId)
SELECT @V1BillingId, OrderId FROM Orders WHERE TableId = @V1TableId;

INSERT INTO Payment (BillingId, AmountPaid, PaymentTime)
VALUES (@V1BillingId, @V1GrandTotal, GETDATE());

---- =============================================
---- Step 2: Billing for V2 (PWD + Regular Customers)
---- =============================================
--DECLARE 
--    @V2BillingId NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
--    @V2Subtotal DECIMAL(10,2),
--    @V2Customers INT = 3,
--    @V2PWD INT = 1,
--    @V2Regular INT = 2,
--    @ServiceRate DECIMAL(5,2) = 0.10,
--    @PerPerson DECIMAL(10,2),
--    @PWDGross DECIMAL(10,2),
--    @PWDBase DECIMAL(10,2),
--    @PWDDiscounted DECIMAL(10,2),
--    @RegularGross DECIMAL(10,2),
--    @ServiceCharge DECIMAL(10,2),
--    @V2GrandTotal DECIMAL(10,2),
--    @V2VatSales DECIMAL(10,2),
--    @V2VatExemptSale DECIMAL(10,2),
--    @V2VatAmount DECIMAL(10,2);

---- Step 2a: Subtotal (gross, VAT-inclusive)
--SELECT @V2Subtotal = SUM(p.Price * oi.Quantity)
--FROM Orders o
--JOIN OrderItem oi ON o.OrderId = oi.OrderId
--JOIN Product p ON oi.ProdId = p.ProdId
--JOIN OrderItemStatus ois ON oi.OrderItemStatusId = ois.OrderItemStatusId
--WHERE o.TableId = @V2TableId
--  AND ois.StatusName = 'Served';

---- Step 2b: Per person gross share
--SET @PerPerson = @V2Subtotal / @V2Customers;

---- Step 2c: PWD share (remove VAT, apply 20% discount)
--SET @PWDGross = @PerPerson;
--SET @PWDBase = @PWDGross / 1.12;                 -- remove VAT
--SET @PWDDiscounted = @PWDBase * 0.80;            -- 20% off, VAT exempt

---- Step 2d: Regular share (gross, VAT-inclusive, no discount)
--SET @RegularGross = @PerPerson * @V2Regular;

---- Step 2e: Service charge = 10% of subtotal (always full subtotal)
--SET @ServiceCharge = @V2Subtotal * @ServiceRate;

---- Step 2f: Grand total = Regular (gross) + PWD (discounted base) + Service
--SET @V2GrandTotal = @RegularGross + @PWDDiscounted + @ServiceCharge;

---- Step 2g: Breakdown for reporting
--SET @V2VatSales = @RegularGross / 1.12;          -- vatable base for regulars
--SET @V2VatAmount = @RegularGross - @V2VatSales;  -- VAT portion
--SET @V2VatExemptSale = @PWDDiscounted;           -- PWD is exempt

---- Insert Billing
--INSERT INTO Billing (BillingId, TableId, BillingTime, Subtotal, VatableSale, VatExemptSale, VatAmount, ServiceCharge, GrandTotal)
--VALUES (@V2BillingId, @V2TableId, GETDATE(), @V2Subtotal, @V2VatSales, @V2VatExemptSale, @V2VatAmount, @ServiceCharge, @V2GrandTotal);

---- Link Orders
--INSERT INTO BillingOrder (BillingId, OrderId)
--SELECT @V2BillingId, OrderId FROM Orders WHERE TableId = @V2TableId;

---- Payment
--INSERT INTO Payment (BillingId, AmountPaid, PaymentTime)
--VALUES (@V2BillingId, @V2GrandTotal, GETDATE());

---- Record PWD Discount
--INSERT INTO Discounts (BillingId, DiscountTypeId, ApprovedBy, ApprovedAt)
--SELECT @V2BillingId, DiscountTypeId, NULL, GETDATE()
--FROM DiscountType WHERE DiscountName = 'PWD';


-- =============================================
-- Step 3: Billing for B2 (similar to V1 but with PWD 20%)
-- =============================================
DECLARE 
    @B2BillingId NVARCHAR(50) = CONVERT(NVARCHAR(50), NEWID()),
    @B2Subtotal DECIMAL(10,2),
    @B2VatSales DECIMAL(10,2),
    @B2VatAmount DECIMAL(10,2),
    @B2Discount DECIMAL(10,2),
    @B2Discounted DECIMAL(10,2),
    @B2ServiceCharge DECIMAL(10,2),
    @B2GrandTotal DECIMAL(10,2);

-- Subtotal
SELECT @B2Subtotal = SUM(p.Price * oi.Quantity)
FROM Orders o
JOIN OrderItem oi ON o.OrderId = oi.OrderId
JOIN Product p ON oi.ProdId = p.ProdId
JOIN OrderItemStatus ois ON oi.OrderItemStatusId = ois.OrderItemStatusId
WHERE o.TableId = @B2TableId
  AND ois.StatusName = 'Served';

-- VAT
SET @B2VatSales = @B2Subtotal / 1.12;
SET @B2VatAmount = @B2Subtotal - @B2VatSales;

-- PWD Discount 20%
SET @B2Discount = @B2VatSales * 0.20;
SET @B2Discounted = @B2VatSales - @B2Discount;

-- Service 10%
SET @B2ServiceCharge = @B2Discounted * 0.10;

-- Grand Total
SET @B2GrandTotal = @B2Discounted + @B2ServiceCharge;

-- Insert Billing
INSERT INTO Billing (BillingId, TableId, BillingTime, Subtotal, VatableSale, VatExemptSale, VatAmount, ServiceCharge, GrandTotal)
VALUES (@B2BillingId, @B2TableId, GETDATE(), @B2Subtotal, @B2VatSales, 0, @B2VatAmount, @B2ServiceCharge, @B2GrandTotal);

-- Link Orders
INSERT INTO BillingOrder (BillingId, OrderId)
SELECT @B2BillingId, OrderId FROM Orders WHERE TableId = @B2TableId;

-- Payment
INSERT INTO Payment (BillingId, AmountPaid, PaymentTime)
VALUES (@B2BillingId, @B2GrandTotal, GETDATE());

-- Record PWD Discount
INSERT INTO Discounts (BillingId, DiscountTypeId, ApprovedBy, ApprovedAt)
SELECT @B2BillingId, DiscountTypeId, NULL, GETDATE()
FROM DiscountType WHERE DiscountName = 'PWD';

-- =============================================
-- Step 4: Update Table Status to Available
-- =============================================
UPDATE [Table]
SET TableStatusId = (SELECT TableStatusId FROM TableStatus WHERE StatusName = 'Available')
WHERE TableId IN (@V1TableId, @V2TableId, @B2TableId);

-- =============================================
-- Step 5: Display Billing Table
-- =============================================
SELECT 
    BillingId,
    t.TableNumber,
    BillingTime,
    Subtotal,
    VatableSale,
    VatExemptSale,
    VatAmount,
    ServiceCharge,
    GrandTotal
FROM Billing b
JOIN [Table] t ON b.TableId = t.TableId
ORDER BY BillingTime;

-- =============================================
-- Step 6: Display Payment Table
-- =============================================
SELECT
    p.PaymentId,
    p.BillingId,
    AmountPaid AS AmountDue,
    AmountPaid - b.GrandTotal AS Change,
    p.PaymentTime
FROM Payment p
JOIN Billing b ON p.BillingId = b.BillingId
ORDER BY p.PaymentTime;

-- =============================================
-- Step 7: Display Discounts Table
-- =============================================
SELECT 
    d.DiscountId,
    dt.DiscountName,
    dt.[Percentage],
    b.BillingId,
    t.TableNumber,
    d.ApprovedBy,
    d.ApprovedAt
FROM Discounts d
JOIN DiscountType dt ON d.DiscountTypeId = dt.DiscountTypeId
JOIN Billing b ON d.BillingId = b.BillingId
JOIN [Table] t ON b.TableId = t.TableId
ORDER BY b.BillingId;
