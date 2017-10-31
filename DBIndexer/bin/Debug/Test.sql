SELECT p1.ProductModelID 
    FROM Production.Product AS p1
    GROUP BY p1.ProductModelID
    HAVING MAX(p1.ListPrice) >= ALL
        (SELECT AVG(p2.ListPrice)
        FROM Production.Product AS p2
        WHERE p1.ProductModelID = p2.ProductModelID);


SELECT pp.LastName, pp.FirstName, e.JobTitle 
INTO dbo.EmployeeOne
FROM Person.Person AS pp JOIN HumanResources.Employee AS e
ON e.BusinessEntityID = pp.BusinessEntityID
WHERE pp.LastName = 'Johnson';

SELECT pp.LastName, pp.FirstName, e.JobTitle 
INTO dbo.EmployeeOne
FROM Person.Person AS pp JOIN HumanResources.Employee AS e
ON e.BusinessEntityID = pp.BusinessEntityID
WHERE pp.LastName = 'Johnson'
and exists (select p.FirstName from Person p where p.FirstName = 'Jayant');


