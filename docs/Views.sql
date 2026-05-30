CREATE VIEW ProjectCostSummary AS
SELECT
    p.ProjectID,
    p.ProjectName,
    p.TotalEstimatedBudget,
    ISNULL(pc.ProductionDirectCost, 0) AS ProductionDirectCost,
    ISNULL(sc.SiteDirectCost, 0) AS SiteDirectCost,
    ISNULL(pc.ProductionDirectCost, 0) + ISNULL(sc.SiteDirectCost, 0) AS TotalDirectCost
FROM Projects p
LEFT JOIN (
    SELECT
        ProjectID,
        SUM(LaborCost + MoldDepreciationCost) AS ProductionDirectCost
    FROM ProductionOrders
    GROUP BY ProjectID
) pc ON pc.ProjectID = p.ProjectID
LEFT JOIN (
    SELECT
        ProjectID,
        SUM(SupervisorLaborCost + DailyExpenses) AS SiteDirectCost
    FROM SiteOperations
    GROUP BY ProjectID
) sc ON sc.ProjectID = p.ProjectID;

GO

CREATE VIEW JournalEntryBalance AS
SELECT
    je.JournalEntryID,
    je.ReferenceType,
    je.ReferenceID,
    SUM(jel.Debit) AS TotalDebit,
    SUM(jel.Credit) AS TotalCredit,
    SUM(jel.Debit) - SUM(jel.Credit) AS BalanceDifference
FROM JournalEntries je
LEFT JOIN JournalEntryLines jel ON jel.JournalEntryID = je.JournalEntryID
GROUP BY je.JournalEntryID, je.ReferenceType, je.ReferenceID;
