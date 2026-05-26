CREATE TABLE ChartOfAccounts (
    AccountID INT IDENTITY(1,1) PRIMARY KEY,
    AccountCode NVARCHAR(50) NOT NULL UNIQUE,
    AccountName NVARCHAR(150) NOT NULL,
    AccountType NVARCHAR(50) NOT NULL,
    CONSTRAINT CK_ChartOfAccounts_AccountType CHECK (AccountType IN ('Asset', 'Liability', 'Equity', 'Revenue', 'Expense'))
);

CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(150) NOT NULL UNIQUE,
    ContactPerson NVARCHAR(100) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(250) NULL,
    AccountID INT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Customers_Account FOREIGN KEY (AccountID)
        REFERENCES ChartOfAccounts(AccountID) ON DELETE SET NULL
);

CREATE TABLE Projects (
    ProjectID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectName NVARCHAR(150) NOT NULL,
    CustomerID INT NOT NULL,
    StartDate DATETIME NOT NULL DEFAULT GETDATE(),
    EndDate DATETIME NULL,
    ProjectStatus NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    TotalEstimatedBudget DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Projects_Customer FOREIGN KEY (CustomerID)
        REFERENCES Customers(CustomerID) ON DELETE NO ACTION,
    CONSTRAINT CK_Projects_Status CHECK (ProjectStatus IN ('Draft', 'Approved', 'Active', 'Completed', 'Cancelled')),
    CONSTRAINT CK_Projects_Budget CHECK (TotalEstimatedBudget >= 0)
);

CREATE TABLE ProjectItems (
    ProjectItemID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    ItemCode NVARCHAR(50) NOT NULL,
    ItemName NVARCHAR(200) NOT NULL,
    Unit NVARCHAR(50) NOT NULL,
    RequiredQuantity DECIMAL(18,2) NOT NULL,
    EstimatedUnitPrice DECIMAL(18,2) NOT NULL,
    TaxRate DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    TaxAmount AS (RequiredQuantity * EstimatedUnitPrice * (TaxRate / 100)),
    TotalPrice AS (RequiredQuantity * EstimatedUnitPrice),
    TotalPriceWithTax AS ((RequiredQuantity * EstimatedUnitPrice) + (RequiredQuantity * EstimatedUnitPrice * (TaxRate / 100))),
    CONSTRAINT FK_ProjectItems_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE CASCADE,
    CONSTRAINT UQ_ProjectItems_Project_ItemCode UNIQUE (ProjectID, ItemCode),
    CONSTRAINT CK_ProjectItems_Quantity CHECK (RequiredQuantity > 0),
    CONSTRAINT CK_ProjectItems_UnitPrice CHECK (EstimatedUnitPrice >= 0),
    CONSTRAINT CK_ProjectItems_TaxRate CHECK (TaxRate >= 0)
);

CREATE TABLE InventoryItems (
    ItemID INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(150) NOT NULL,
    ItemType NVARCHAR(50) NOT NULL,
    Unit NVARCHAR(50) NOT NULL,
    CurrentStock DECIMAL(18,4) NOT NULL DEFAULT 0.0000,
    AverageCost DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT CK_InventoryItems_Type CHECK (ItemType IN ('RawMaterial', 'FinishedGood', 'Accessory', 'Consumable')),
    CONSTRAINT CK_InventoryItems_Stock CHECK (CurrentStock >= 0),
    CONSTRAINT CK_InventoryItems_AverageCost CHECK (AverageCost >= 0)
);

CREATE TABLE InventoryTransactions (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    ItemID INT NOT NULL,
    ProjectID INT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    Quantity DECIMAL(18,4) NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
    ReferenceType NVARCHAR(50) NULL,
    ReferenceID INT NULL,
    Notes NVARCHAR(250) NULL,
    CONSTRAINT FK_InventoryTransactions_Item FOREIGN KEY (ItemID)
        REFERENCES InventoryItems(ItemID) ON DELETE NO ACTION,
    CONSTRAINT FK_InventoryTransactions_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE SET NULL,
    CONSTRAINT CK_InventoryTransactions_Type CHECK (TransactionType IN ('PurchaseIn', 'ProductionIssue', 'ProductionReceipt', 'DeliveryOut', 'SiteIssue', 'AdjustmentIn', 'AdjustmentOut', 'Waste')),
    CONSTRAINT CK_InventoryTransactions_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_InventoryTransactions_UnitCost CHECK (UnitCost >= 0)
);

CREATE TABLE Molds (
    MoldID INT IDENTITY(1,1) PRIMARY KEY,
    MoldName NVARCHAR(100) NOT NULL,
    CostToBuild DECIMAL(18,2) NOT NULL,
    ExpectedLifespanUses INT NOT NULL,
    CurrentUsesCount INT NOT NULL DEFAULT 0,
    MoldStatus NVARCHAR(50) NOT NULL DEFAULT 'Available',
    CONSTRAINT CK_Molds_Cost CHECK (CostToBuild >= 0),
    CONSTRAINT CK_Molds_Lifespan CHECK (ExpectedLifespanUses > 0),
    CONSTRAINT CK_Molds_CurrentUses CHECK (CurrentUsesCount >= 0),
    CONSTRAINT CK_Molds_Status CHECK (MoldStatus IN ('Available', 'Allocated', 'Maintenance', 'Retired'))
);

CREATE TABLE ProjectMolds (
    ProjectMoldID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    MoldID INT NOT NULL,
    AllocQuantity INT NOT NULL,
    AllocatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_ProjectMolds_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE CASCADE,
    CONSTRAINT FK_ProjectMolds_Mold FOREIGN KEY (MoldID)
        REFERENCES Molds(MoldID) ON DELETE NO ACTION,
    CONSTRAINT UQ_ProjectMolds_Project_Mold UNIQUE (ProjectID, MoldID),
    CONSTRAINT CK_ProjectMolds_Quantity CHECK (AllocQuantity > 0)
);

CREATE TABLE MixDesigns (
    MixDesignID INT IDENTITY(1,1) PRIMARY KEY,
    MixName NVARCHAR(100) NOT NULL UNIQUE,
    TargetStrength NVARCHAR(50) NULL,
    StandardCostPerUnit DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT CK_MixDesigns_Cost CHECK (StandardCostPerUnit >= 0)
);

CREATE TABLE MixIngredients (
    IngredientID INT IDENTITY(1,1) PRIMARY KEY,
    MixDesignID INT NOT NULL,
    RawMaterialID INT NOT NULL,
    StandardQtyPerUnit DECIMAL(18,4) NOT NULL,
    CONSTRAINT FK_MixIngredients_MixDesign FOREIGN KEY (MixDesignID)
        REFERENCES MixDesigns(MixDesignID) ON DELETE CASCADE,
    CONSTRAINT FK_MixIngredients_RawMaterial FOREIGN KEY (RawMaterialID)
        REFERENCES InventoryItems(ItemID) ON DELETE NO ACTION,
    CONSTRAINT UQ_MixIngredients_Mix_RawMaterial UNIQUE (MixDesignID, RawMaterialID),
    CONSTRAINT CK_MixIngredients_StandardQty CHECK (StandardQtyPerUnit > 0)
);

CREATE TABLE ProductionOrders (
    ProductionOrderID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    ProjectItemID INT NOT NULL,
    MixDesignID INT NOT NULL,
    MoldID INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    BatchNumber NVARCHAR(50) NOT NULL,
    TargetQuantity DECIMAL(18,2) NOT NULL,
    ProducedQuantity DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    GoodQuantity DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    RejectedQuantity DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    LaborCost DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    MoldDepreciationCost DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    ProductionStatus NVARCHAR(50) NOT NULL DEFAULT 'Setup',
    IsAccountingPosted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ProductionOrders_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE NO ACTION,
    CONSTRAINT FK_ProductionOrders_ProjectItem FOREIGN KEY (ProjectItemID)
        REFERENCES ProjectItems(ProjectItemID) ON DELETE NO ACTION,
    CONSTRAINT FK_ProductionOrders_MixDesign FOREIGN KEY (MixDesignID)
        REFERENCES MixDesigns(MixDesignID) ON DELETE NO ACTION,
    CONSTRAINT FK_ProductionOrders_Mold FOREIGN KEY (MoldID)
        REFERENCES Molds(MoldID) ON DELETE NO ACTION,
    CONSTRAINT UQ_ProductionOrders_BatchNumber UNIQUE (BatchNumber),
    CONSTRAINT CK_ProductionOrders_Status CHECK (ProductionStatus IN ('Setup', 'InProgress', 'QualityCheck', 'Completed', 'Cancelled')),
    CONSTRAINT CK_ProductionOrders_Target CHECK (TargetQuantity > 0),
    CONSTRAINT CK_ProductionOrders_Quantities CHECK (
        ProducedQuantity >= 0
        AND GoodQuantity >= 0
        AND RejectedQuantity >= 0
        AND GoodQuantity + RejectedQuantity <= ProducedQuantity
    ),
    CONSTRAINT CK_ProductionOrders_Costs CHECK (LaborCost >= 0 AND MoldDepreciationCost >= 0)
);

CREATE TABLE ProductionMaterialConsumption (
    ConsumptionID INT IDENTITY(1,1) PRIMARY KEY,
    ProductionOrderID INT NOT NULL,
    MaterialID INT NOT NULL,
    ActualQtyConsumed DECIMAL(18,4) NOT NULL,
    StandardQtyExpected DECIMAL(18,4) NOT NULL,
    WastageQty AS (ActualQtyConsumed - StandardQtyExpected),
    CONSTRAINT FK_ProductionMaterialConsumption_Order FOREIGN KEY (ProductionOrderID)
        REFERENCES ProductionOrders(ProductionOrderID) ON DELETE CASCADE,
    CONSTRAINT FK_ProductionMaterialConsumption_Material FOREIGN KEY (MaterialID)
        REFERENCES InventoryItems(ItemID) ON DELETE NO ACTION,
    CONSTRAINT UQ_ProductionMaterialConsumption_Order_Material UNIQUE (ProductionOrderID, MaterialID),
    CONSTRAINT CK_ProductionMaterialConsumption_Qty CHECK (ActualQtyConsumed >= 0 AND StandardQtyExpected >= 0)
);

CREATE TABLE DeliveryOrders (
    DeliveryOrderID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    DeliveryDate DATETIME NOT NULL DEFAULT GETDATE(),
    DriverName NVARCHAR(100) NULL,
    VehicleNumber NVARCHAR(50) NULL,
    LoadingTicketNumber NVARCHAR(50) NULL,
    DeliveryTicketNumber NVARCHAR(50) NULL,
    DeliveryStatus NVARCHAR(50) NOT NULL DEFAULT 'InTransit',
    IsInvoiced BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_DeliveryOrders_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE NO ACTION,
    CONSTRAINT CK_DeliveryOrders_Status CHECK (DeliveryStatus IN ('Draft', 'InTransit', 'Delivered', 'PartiallyDelivered', 'Cancelled'))
);

CREATE TABLE DeliveryItems (
    DeliveryItemID INT IDENTITY(1,1) PRIMARY KEY,
    DeliveryOrderID INT NOT NULL,
    ProjectItemID INT NOT NULL,
    QuantityShipped DECIMAL(18,2) NOT NULL,
    QuantityReceived DECIMAL(18,2) NULL,
    QuantityDamagedInTransit DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_DeliveryItems_Order FOREIGN KEY (DeliveryOrderID)
        REFERENCES DeliveryOrders(DeliveryOrderID) ON DELETE CASCADE,
    CONSTRAINT FK_DeliveryItems_ProjectItem FOREIGN KEY (ProjectItemID)
        REFERENCES ProjectItems(ProjectItemID) ON DELETE NO ACTION,
    CONSTRAINT UQ_DeliveryItems_Order_ProjectItem UNIQUE (DeliveryOrderID, ProjectItemID),
    CONSTRAINT CK_DeliveryItems_Quantities CHECK (
        QuantityShipped > 0
        AND (QuantityReceived IS NULL OR QuantityReceived >= 0)
        AND QuantityDamagedInTransit >= 0
        AND (QuantityReceived IS NULL OR QuantityReceived + QuantityDamagedInTransit <= QuantityShipped)
    )
);

CREATE TABLE SiteOperations (
    SiteOperationID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectID INT NOT NULL,
    ProjectItemID INT NOT NULL,
    OperationDate DATETIME NOT NULL DEFAULT GETDATE(),
    InstalledQuantity DECIMAL(18,2) NOT NULL,
    SupervisorLaborCost DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    DailyExpenses DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_SiteOperations_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE NO ACTION,
    CONSTRAINT FK_SiteOperations_ProjectItem FOREIGN KEY (ProjectItemID)
        REFERENCES ProjectItems(ProjectItemID) ON DELETE NO ACTION,
    CONSTRAINT CK_SiteOperations_Quantity CHECK (InstalledQuantity > 0),
    CONSTRAINT CK_SiteOperations_Costs CHECK (SupervisorLaborCost >= 0 AND DailyExpenses >= 0)
);

CREATE TABLE SiteMaterialConsumption (
    SiteConsumptionID INT IDENTITY(1,1) PRIMARY KEY,
    SiteOperationID INT NOT NULL,
    MaterialID INT NOT NULL,
    QuantityConsumed DECIMAL(18,4) NOT NULL,
    CONSTRAINT FK_SiteMaterialConsumption_Operation FOREIGN KEY (SiteOperationID)
        REFERENCES SiteOperations(SiteOperationID) ON DELETE CASCADE,
    CONSTRAINT FK_SiteMaterialConsumption_Material FOREIGN KEY (MaterialID)
        REFERENCES InventoryItems(ItemID) ON DELETE NO ACTION,
    CONSTRAINT UQ_SiteMaterialConsumption_Operation_Material UNIQUE (SiteOperationID, MaterialID),
    CONSTRAINT CK_SiteMaterialConsumption_Quantity CHECK (QuantityConsumed > 0)
);

CREATE TABLE JournalEntries (
    JournalEntryID INT IDENTITY(1,1) PRIMARY KEY,
    ReferenceType NVARCHAR(50) NOT NULL,
    ReferenceID INT NOT NULL,
    TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
    Narration NVARCHAR(500) NOT NULL
);

CREATE TABLE JournalEntryLines (
    JournalLineID INT IDENTITY(1,1) PRIMARY KEY,
    JournalEntryID INT NOT NULL,
    AccountID INT NOT NULL,
    ProjectID INT NULL,
    Debit DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    Credit DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_JournalEntryLines_JournalEntry FOREIGN KEY (JournalEntryID)
        REFERENCES JournalEntries(JournalEntryID) ON DELETE CASCADE,
    CONSTRAINT FK_JournalEntryLines_Account FOREIGN KEY (AccountID)
        REFERENCES ChartOfAccounts(AccountID) ON DELETE NO ACTION,
    CONSTRAINT FK_JournalEntryLines_Project FOREIGN KEY (ProjectID)
        REFERENCES Projects(ProjectID) ON DELETE SET NULL,
    CONSTRAINT CK_JournalEntryLines_Amounts CHECK (
        Debit >= 0
        AND Credit >= 0
        AND (
            (Debit > 0 AND Credit = 0)
            OR (Credit > 0 AND Debit = 0)
        )
    )
);

GO

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
