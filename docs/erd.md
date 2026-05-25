erDiagram

%% ==========================================
%% 1. FINANCIAL & CHART OF ACCOUNTS
%% ==========================================
ChartOfAccounts {
    int AccountID PK
    nvarchar AccountCode UK
    nvarchar AccountName
    nvarchar AccountType
}

Customers {
    int CustomerID PK
    nvarchar CustomerName UK
    nvarchar ContactPerson "NULL"
    nvarchar Phone "NULL"
    nvarchar Email "NULL"
    nvarchar Address "NULL"
    int AccountID FK "ON DELETE SET NULL"
    datetime CreatedAt "DEFAULT GETDATE()"
}

%% ==========================================
%% 2. PROJECT MANAGEMENT & BOQ
%% ==========================================
Projects {
    int ProjectID PK
    nvarchar ProjectName
    int CustomerID FK "ON DELETE NO ACTION"
    datetime StartDate "DEFAULT GETDATE()"
    datetime EndDate "NULL"
    nvarchar ProjectStatus "DEFAULT 'Draft'"
    decimal TotalEstimatedBudget "DEFAULT 0.00"
    datetime CreatedAt "DEFAULT GETDATE()"
}

ProjectItems {
    int ProjectItemID PK
    int ProjectID FK "ON DELETE CASCADE"
    nvarchar ItemCode
    nvarchar ItemName
    nvarchar Unit
    decimal RequiredQuantity
    decimal EstimatedUnitPrice
    decimal TaxRate "DEFAULT 0.00"
    decimal TaxAmount AS "RequiredQty*EstimatedPrice*(TaxRate/100)"
    decimal TotalPrice AS "RequiredQty*EstimatedPrice"
}

%% ==========================================
%% 3. INVENTORY & RAW MATERIALS
%% ==========================================
InventoryItems {
    int ItemID PK
    nvarchar ItemName
    nvarchar ItemType
    nvarchar Unit
    decimal CurrentStock "DEFAULT 0.0000"
    decimal AverageCost "DEFAULT 0.00"
}

%% ==========================================
%% 4. MOLDS & MIX DESIGNS
%% ==========================================
Molds {
    int MoldID PK
    nvarchar MoldName
    decimal CostToBuild
    int ExpectedLifespanUses
    int CurrentUsesCount "DEFAULT 0"
    nvarchar MoldStatus "DEFAULT 'Available'"
}

ProjectMolds {
    int ProjectMoldID PK
    int ProjectID FK "ON DELETE CASCADE"
    int MoldID FK "ON DELETE CASCADE"
    int AllocQuantity
}

MixDesigns {
    int MixDesignID PK
    nvarchar MixName
    nvarchar TargetStrength
    decimal StandardCostPerUnit "DEFAULT 0.00"
}

MixIngredients {
    int IngredientID PK
    int MixDesignID FK "ON DELETE CASCADE"
    int RawMaterialID FK "ON DELETE NO ACTION"
    decimal StandardQtyPerUnit
}

%% ==========================================
%% 5. SHOP FLOOR PRODUCTION
%% ==========================================
ProductionOrders {
    int ProductionOrderID PK
    int ProjectID FK "ON DELETE NO ACTION"
    int ProjectItemID FK "ON DELETE NO ACTION"
    int MixDesignID FK "ON DELETE NO ACTION"
    int MoldID FK "ON DELETE NO ACTION"
    datetime OrderDate "DEFAULT GETDATE()"
    nvarchar BatchNumber
    decimal TargetQuantity
    decimal ProducedQuantity
    decimal GoodQuantity "DEFAULT 0.00"
    decimal RejectedQuantity "DEFAULT 0.00"
    decimal LaborCost "DEFAULT 0.00"
    decimal MoldDepreciationCost "DEFAULT 0.00"
    nvarchar ProductionStatus "DEFAULT 'Setup'"
    bit IsAccountingPosted "DEFAULT 0"
}

ProductionMaterialConsumption {
    int ConsumptionID PK
    int ProductionOrderID FK "ON DELETE CASCADE"
    int MaterialID FK "ON DELETE NO ACTION"
    decimal ActualQtyConsumed
    decimal StandardQtyExpected
    decimal WastageQty AS "ActualQtyConsumed-StandardQtyExpected"
}

%% ==========================================
%% 6. LOGISTICS & DELIVERY
%% ==========================================
DeliveryOrders {
    int DeliveryOrderID PK
    int ProjectID FK "ON DELETE NO ACTION"
    datetime DeliveryDate "DEFAULT GETDATE()"
    nvarchar DriverName
    nvarchar VehicleNumber "NULL"
    nvarchar LoadingTicketNumber
    nvarchar DeliveryTicketNumber
    nvarchar DeliveryStatus "DEFAULT 'InTransit'"
    bit IsInvoiced "DEFAULT 0"
}

DeliveryItems {
    int DeliveryItemID PK
    int DeliveryOrderID FK "ON DELETE CASCADE"
    int ProjectItemID FK "ON DELETE NO ACTION"
    decimal QuantityShipped
    decimal QuantityReceived "NULL"
    decimal QuantityDamagedInTransit "DEFAULT 0.00"
}

%% ==========================================
%% 7. SITE OPERATIONS
%% ==========================================
SiteOperations {
    int SiteOperationID PK
    int ProjectID FK "ON DELETE NO ACTION"
    int ProjectItemID FK "ON DELETE NO ACTION"
    datetime OperationDate "DEFAULT GETDATE()"
    decimal InstalledQuantity
    decimal SupervisorLaborCost "DEFAULT 0.00"
    decimal DailyExpenses "DEFAULT 0.00"
}

SiteMaterialConsumption {
    int SiteConsumptionID PK
    int SiteOperationID FK "ON DELETE CASCADE"
    int MaterialID FK "ON DELETE NO ACTION"
    decimal QuantityConsumed
}

%% ==========================================
%% 8. GENERAL LEDGER (ACCOUNTING)
%% ==========================================
JournalEntries {
    int JournalEntryID PK
    nvarchar ReferenceType
    int ReferenceID
    datetime TransactionDate "DEFAULT GETDATE()"
    nvarchar Narration
}

JournalEntryLines {
    int JournalLineID PK
    int JournalEntryID FK "ON DELETE CASCADE"
    int AccountID FK "ON DELETE NO ACTION"
    int ProjectID FK "ON DELETE SET NULL"
    decimal Debit "DEFAULT 0.00"
    decimal Credit "DEFAULT 0.00"
}

%% ==========================================
%% RELATIONSHIPS (علاقات الجداول)
%% ==========================================

ChartOfAccounts ||--o{ Customers : "financial_account"
ChartOfAccounts ||--o{ JournalEntryLines : "booked_to"

Customers ||--o{ Projects : "orders"

Projects ||--o{ ProjectItems : "contains"
Projects ||--o{ ProjectMolds : "allocates"
Projects ||--o{ ProductionOrders : "tracks_production"
Projects ||--o{ DeliveryOrders : "ships_to"
Projects ||--o{ SiteOperations : "manages_installations"
Projects ||--o{ JournalEntryLines : "acts_as_cost_center"

ProjectItems ||--o{ ProductionOrders : "defines_product"
ProjectItems ||--o{ DeliveryItems : "shipped_as"
ProjectItems ||--o{ SiteOperations : "installed_as"

Molds ||--o{ ProjectMolds : "assigned_to"
Molds ||--o{ ProductionOrders : "shapes_production"

MixDesigns ||--o{ MixIngredients : "formulates"
MixDesigns ||--o{ ProductionOrders : "specifies_mix"

InventoryItems ||--o{ MixIngredients : "used_as_ingredient"
InventoryItems ||--o{ ProductionMaterialConsumption : "withdrawn_for_factory"
InventoryItems ||--o{ SiteMaterialConsumption : "withdrawn_for_site"

ProductionOrders ||--o{ ProductionMaterialConsumption : "requires_materials"

DeliveryOrders ||--o{ DeliveryItems : "includes"

SiteOperations ||--o{ SiteMaterialConsumption : "requires_site_materials"

JournalEntries ||--o{ JournalEntryLines : "has_lines"
