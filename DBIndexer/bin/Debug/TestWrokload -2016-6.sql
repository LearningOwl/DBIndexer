SELECT p1.ProductModelID
FROM Production.Product AS p1
GROUP BY p1.ProductModelID
HAVING MAX(p1.ListPrice) >= ALL
    (SELECT AVG(p2.ListPrice)
     FROM Production.Product AS p2
     WHERE p1.ProductModelID = p2.ProductModelID);





SELECT DISTINCT Name
FROM Production.Product
WHERE ProductModelID IN
    (SELECT ProductModelID 
     FROM Production.ProductModel
     WHERE Name LIKE 'Long-Sleeve Lo Jersey%');




SELECT DISTINCT p.LastName, p.FirstName 
FROM Person.Person AS p 
JOIN HumanResources.Employee AS e
    ON e.BusinessEntityID = p.BusinessEntityID WHERE 5000.00 IN
    (SELECT Bonus
     FROM Sales.SalesPerson AS sp
     WHERE e.BusinessEntityID = sp.BusinessEntityID);






SELECT p1.ProductModelID
FROM Production.Product AS p1
GROUP BY p1.ProductModelID
HAVING MAX(p1.ListPrice) >= ALL
    (SELECT AVG(p2.ListPrice)
     FROM Production.Product AS p2
     WHERE p1.ProductModelID = p2.ProductModelID);





SELECT DISTINCT Name
FROM Production.Product
WHERE ProductModelID IN
    (SELECT ProductModelID 
     FROM Production.ProductModel
     WHERE Name LIKE 'Long-Sleeve Lo Jersey%');




SELECT DISTINCT p.LastName, p.FirstName 
FROM Person.Person AS p 
JOIN HumanResources.Employee AS e
    ON e.BusinessEntityID = p.BusinessEntityID WHERE 5000.00 IN
    (SELECT Bonus
     FROM Sales.SalesPerson AS sp
     WHERE e.BusinessEntityID = sp.BusinessEntityID);
	 
	 

	SELECT ProductModelID, Name
	
	FROM Production.ProductModel
	WHERE ProductModelID IN (3, 4);
	


	SELECT ProductModelID, Name
	
	FROM Production.ProductModel
	WHERE ProductModelID NOT IN (3, 4)
	





	SELECT pp.LastName, pp.FirstName, e.JobTitle 
	FROM Person.Person AS pp JOIN HumanResources.Employee AS e
	ON e.BusinessEntityID = pp.BusinessEntityID
	WHERE LastName = 'Johnson';

	SELECT pp.LastName, pp.FirstName, e.JobTitle 
	
	FROM Person.Person AS pp JOIN HumanResources.Employee AS e
	ON e.BusinessEntityID = pp.BusinessEntityID
	WHERE LastName = 'Johnson';

	SELECT pp.LastName, pp.FirstName, e.JobTitle 
	
	FROM Person.Person AS pp JOIN HumanResources.Employee AS e
	ON e.BusinessEntityID = pp.BusinessEntityID
	WHERE LastName = 'Johnson';





	SELECT sp.BusinessEntityID, c.LastName, 
		sp.SalesYTD 
	FROM Sales.SalesPerson AS sp  
	INNER JOIN Person.Person AS c
		ON sp.BusinessEntityID = c.BusinessEntityID
	WHERE sp.BusinessEntityID LIKE '2%'
	ORDER BY sp.BusinessEntityID, c.LastName;






SELECT SalesQuota, SUM(SalesYTD) 'TotalSalesYTD', GROUPING(SalesQuota) AS 'Grouping'
FROM Sales.SalesPerson
GROUP BY SalesQuota WITH ROLLUP;
GO


SELECT D.Name
    ,CASE 
    WHEN GROUPING_ID(D.Name, E.JobTitle) = 0 THEN E.JobTitle
    WHEN GROUPING_ID(D.Name, E.JobTitle) = 1 THEN N'Total: ' + D.Name 
    WHEN GROUPING_ID(D.Name, E.JobTitle) = 3 THEN N'Company Total:'
        ELSE N'Unknown'
    END AS N'Job Title'
    ,COUNT(E.BusinessEntityID) AS N'Employee Count'
FROM HumanResources.Employee E
    INNER JOIN HumanResources.EmployeeDepartmentHistory DH
        ON E.BusinessEntityID = DH.BusinessEntityID
    INNER JOIN HumanResources.Department D
        ON D.DepartmentID = DH.DepartmentID     
WHERE DH.EndDate IS NULL
    AND D.DepartmentID IN (12,14)
GROUP BY ROLLUP(D.Name, E.JobTitle);


SELECT D.Name
    ,E.JobTitle
    ,GROUPING_ID(D.Name, E.JobTitle) AS 'Grouping Level'
    ,COUNT(E.BusinessEntityID) AS N'Employee Count'
FROM HumanResources.Employee AS E
    INNER JOIN HumanResources.EmployeeDepartmentHistory AS DH
        ON E.BusinessEntityID = DH.BusinessEntityID
    INNER JOIN HumanResources.Department AS D
        ON D.DepartmentID = DH.DepartmentID     
WHERE DH.EndDate IS NULL
    AND D.DepartmentID IN (12,14)
GROUP BY ROLLUP(D.Name, E.JobTitle)
HAVING GROUPING_ID(D.Name, E.JobTitle) = 0; --All titles

SELECT D.Name
    ,E.JobTitle
    ,GROUPING_ID(D.Name, E.JobTitle) AS 'Grouping Level'
    ,COUNT(E.BusinessEntityID) AS N'Employee Count'
FROM HumanResources.Employee AS E
    INNER JOIN HumanResources.EmployeeDepartmentHistory AS DH
        ON E.BusinessEntityID = DH.BusinessEntityID
    INNER JOIN HumanResources.Department AS D
        ON D.DepartmentID = DH.DepartmentID     
WHERE DH.EndDate IS NULL
    AND D.DepartmentID IN (12,14)
GROUP BY ROLLUP(D.Name, E.JobTitle)
HAVING GROUPING_ID(D.Name, E.JobTitle) = 1; --Group by Name;


SELECT CustomerID, OrderDate, SubTotal, TotalDue
FROM Sales.SalesOrderHeader
WHERE SalesPersonID = 35
ORDER BY OrderDate 

GO


SELECT SalesPersonID, CustomerID, OrderDate, SubTotal, TotalDue
FROM Sales.SalesOrderHeader
ORDER BY SalesPersonID, OrderDate 


SELECT *
FROM Production.Product
ORDER BY Name ASC;
-- Alternate way.
SELECT p.*
FROM Production.Product AS p
ORDER BY Name ASC;
GO



SELECT Name, ProductNumber, ListPrice AS Price
FROM Production.Product 
ORDER BY Name ASC;
GO


SELECT Name, ProductNumber, ListPrice AS Price
FROM Production.Product 
WHERE ProductLine = 'R' 
AND DaysToManufacture < 4
ORDER BY Name ASC;
GO



SELECT p.Name AS ProductName, 
NonDiscountSales = (OrderQty * UnitPrice),
Discounts = ((OrderQty * UnitPrice) * UnitPriceDiscount)
FROM Production.Product AS p 
INNER JOIN Sales.SalesOrderDetail AS sod
ON p.ProductID = sod.ProductID 
ORDER BY ProductName DESC;
GO



SELECT 'Total income is', ((OrderQty * UnitPrice) * (1.0 - UnitPriceDiscount)), ' for ',
p.Name AS ProductName 
FROM Production.Product AS p 
INNER JOIN Sales.SalesOrderDetail AS sod
ON p.ProductID = sod.ProductID 
ORDER BY ProductName ASC;
GO



SELECT DISTINCT JobTitle
FROM HumanResources.Employee
ORDER BY JobTitle;
GO
