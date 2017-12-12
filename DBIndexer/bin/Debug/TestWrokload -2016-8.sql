
SELECT ProductID, OrderQty, SUM(LineTotal) AS Total
FROM Sales.SalesOrderDetail
WHERE UnitPrice < $5.00
GROUP BY ProductID, OrderQty
ORDER BY ProductID, OrderQty
OPTION (HASH GROUP, FAST 10);
GO



SELECT *
FROM HumanResources.Employee AS e1
UNION
SELECT *
FROM HumanResources.Employee AS e2;
GO



-- Here is the simple union.
SELECT ProductModelID, Name
FROM Production.ProductModel
WHERE ProductModelID NOT IN (3, 4)
UNION






SELECT ProductModelID, Name
FROM Production.ProductModel
WHERE ProductModelID NOT IN (3, 4)





SELECT ProductModelID, Name
FROM Production.ProductModel
WHERE ProductModelID NOT IN (3, 4)
GO


SELECT [BusinessEntityID]
      ,[Name]
      ,[SalesPersonID]
      ,[Demographics]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[Store] where SalesPersonID='279';
  
 SELECT TOP (1000) [SpecialOfferID]
      ,[ProductID]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SpecialOfferProduct] where ProductID='680';
  
  SELECT TOP (1000) [SpecialOfferID]
      ,[ProductID]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SpecialOfferProduct];
  
  
SELECT [SpecialOfferID]
      ,[ProductID]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SpecialOfferProduct] where SpecialOfferID='1' ;
  
  
SELECT [SpecialOfferID]
      ,[Description]
      ,[DiscountPct]
      ,[Type]
      ,[Category]
      ,[StartDate]
      ,[EndDate]
      ,[MinQty]
      ,[MaxQty]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SpecialOffer];
  
  
  
SELECT [SpecialOfferID]
      ,[Description]
      ,[DiscountPct]
      ,[Type]
      ,[Category]
      ,[StartDate]
      ,[EndDate]
      ,[MinQty]
      ,[MaxQty]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SpecialOffer] where Type='Volume Discount';
  
  
SELECT [ShoppingCartItemID]
      ,[ShoppingCartID]
      ,[Quantity]
      ,[ProductID]
      ,[DateCreated]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[ShoppingCartItem] where Quantity>'3';
  
  
SELECT [BusinessEntityID]
      ,[TerritoryID]
      ,[StartDate]
      ,[EndDate]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesTerritoryHistory];
  
 
SELECT [BusinessEntityID]
      ,[TerritoryID]
      ,[StartDate]
      ,[EndDate]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesTerritoryHistory] where TerritoryID='2';
  
  
  SELECT [BusinessEntityID]
      ,[TerritoryID]
      ,[StartDate]
      ,[EndDate]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesTerritoryHistory] where TerritoryID>'3';
  
  
SELECT [TerritoryID]
      ,[Name]
      ,[CountryRegionCode]
      ,[Group]
      ,[SalesYTD]
      ,[SalesLastYear]
      ,[CostYTD]
      ,[CostLastYear]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesTerritory] where CountryRegionCode='US';
  
  
  SELECT  [SalesTaxRateID]
      ,[StateProvinceID]
      ,[TaxType]
      ,[TaxRate]
      ,[Name]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesTaxRate];
  
  /****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [SalesTaxRateID]
      ,[StateProvinceID]
      ,[TaxType]
      ,[TaxRate]
      ,[Name]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesTaxRate] where TaxRate>'7.00';
  
  
  SELECT  [SalesReasonID]
      ,[Name]
      ,[ReasonType]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesReason];
  
  
SELECT  [SalesReasonID]
      ,[Name]
      ,[ReasonType]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[SalesReason] where ReasonType='Other';
  
  
  
  SELECT  [CustomerID]
      ,[PersonID]
      ,[StoreID]
      ,[TerritoryID]
      ,[AccountNumber]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[Customer];
  
  
SELECT  [CustomerID]
      ,[PersonID]
      ,[StoreID]
      ,[TerritoryID]
      ,[AccountNumber]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[Customer] where TerritoryID='1';
  
  
  
SELECT  [CountryRegionCode]
      ,[CurrencyCode]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[CountryRegionCurrency]
  
  
  SELECT  [CountryRegionCode]
      ,[CurrencyCode]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Sales].[CountryRegionCurrency] where CountryRegionCode like 'A%';
  
  
  
SELECT  [PurchaseOrderID]
      ,[PurchaseOrderDetailID]
      ,[DueDate]
      ,[OrderQty]
      ,[ProductID]
      ,[UnitPrice]
      ,[LineTotal]
      ,[ReceivedQty]
      ,[RejectedQty]
      ,[StockedQty]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Purchasing].[PurchaseOrderDetail]
  
  
  
SELECT  [PurchaseOrderID]
      ,[PurchaseOrderDetailID]
      ,[DueDate]
      ,[OrderQty]
      ,[ProductID]
      ,[UnitPrice]
      ,[LineTotal]
      ,[ReceivedQty]
      ,[RejectedQty]
      ,[StockedQty]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Purchasing].[PurchaseOrderDetail] where ProductID='512';
  
  
  
  /****** Script for SelectTopNRows command from SSMS  ******/
SELECT  [WorkOrderID]
      ,[ProductID]
      ,[OperationSequence]
      ,[LocationID]
      ,[ScheduledStartDate]
      ,[ScheduledEndDate]
      ,[ActualStartDate]
      ,[ActualEndDate]
      ,[ActualResourceHrs]
      ,[PlannedCost]
      ,[ActualCost]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Production].[WorkOrderRouting];
  
  
  
SELECT  [WorkOrderID]
      ,[ProductID]
      ,[OperationSequence]
      ,[LocationID]
      ,[ScheduledStartDate]
      ,[ScheduledEndDate]
      ,[ActualStartDate]
      ,[ActualEndDate]
      ,[ActualResourceHrs]
      ,[PlannedCost]
      ,[ActualCost]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Production].[WorkOrderRouting] where LocationID='10' and ProductID='747';
  
  
  
  
SELECT [ProductModelID]
      ,[Name]
      ,[CatalogDescription]
      ,[Instructions]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[Production].[ProductModel] where Name='Classic Vest';
  
  
  
  SELECT TOP (1000) [BusinessEntityID]
      ,[DepartmentID]
      ,[ShiftID]
      ,[StartDate]
      ,[EndDate]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[HumanResources].[EmployeeDepartmentHistory]
  
  
  
  
SELECT TOP (1000) [BusinessEntityID]
      ,[DepartmentID]
      ,[ShiftID]
      ,[StartDate]
      ,[EndDate]
      ,[ModifiedDate]
  FROM [AdventureWorks2016].[HumanResources].[EmployeeDepartmentHistory] where DepartmentID='16';
  
  
  




