erDiagram

ChartOfAccounts {
    int AccountID PK
    nvarchar AccountCode UK
    nvarchar AccountName
    nvarchar AccountType
}

Customers {
    int CustomerID PK
    nvarchar CustomerName UK
    nvarchar ContactPerson
    nvarchar Phone
    nvarchar Email
    nvarchar Address
    int AccountID FK
    datetime CreatedAt
}

Projects {
    int ProjectID PK
    nvarchar ProjectName
    int CustomerID FK
    datetime StartDate
    datetime EndDate
    nvarchar ProjectStatus
    decimal TotalEstimatedBudget
    datetime CreatedAt
}

ProjectItems {
    int ProjectItemID PK
    int ProjectID FK
    nvarchar ItemCode
    nvarchar ItemName
    nvarchar Unit
    decimal RequiredQuantity
    decimal EstimatedUnitPrice
    decimal TaxRate
    decimal TaxAmount
    decimal TotalPrice
}

InventoryItems {
    int ItemID PK
    nvarchar ItemName
    nvarchar ItemType
    nvarchar Unit
    decimal CurrentStock
    decimal AverageCost
}

Molds {
    int MoldID PK
    nvarchar MoldName
    decimal CostToBuild
    int ExpectedLifespanUses
    int CurrentUsesCount
    nvarchar MoldStatus
}

ProjectMolds {
    int ProjectMoldID PK
    int ProjectID FK
    int MoldID FK
    int AllocQuantity
}

MixDesigns {
    int MixDesignID PK
    nvarchar MixName
    nvarchar TargetStrength
    decimal StandardCostPerUnit
}

MixIngredients {
    int IngredientID PK
    int MixDesignID FK
    int RawMaterialID FK
    decimal StandardQtyPerUnit
}

ProductionOrders {
    int ProductionOrderID PK
    int ProjectID FK
    int ProjectItemID FK
    int MixDesignID FK
    int MoldID FK
    datetime OrderDate
    nvarchar BatchNumber
    decimal TargetQuantity
    decimal ProducedQuantity
    decimal GoodQuantity
    decimal RejectedQuantity
    decimal LaborCost
    decimal MoldDepreciationCost
    nvarchar ProductionStatus
    bit IsAccountingPosted
}

ProductionMaterialConsumption {
    int ConsumptionID PK
    int ProductionOrderID FK
    int MaterialID FK
    decimal ActualQtyConsumed
    decimal StandardQtyExpected
    decimal WastageQty
}

DeliveryOrders {
    int DeliveryOrderID PK
    int ProjectID FK
    datetime DeliveryDate
    nvarchar DriverName
    nvarchar VehicleNumber
    nvarchar LoadingTicketNumber
    nvarchar DeliveryTicketNumber
    nvarchar DeliveryStatus
    bit IsInvoiced
}

DeliveryItems {
    int DeliveryItemID PK
    int DeliveryOrderID FK
    int ProjectItemID FK
    decimal QuantityShipped
    decimal QuantityReceived
    decimal QuantityDamagedInTransit
}

SiteOperations {
    int SiteOperationID PK
    int ProjectID FK
    int ProjectItemID FK
    datetime OperationDate
    decimal InstalledQuantity
    decimal SupervisorLaborCost
    decimal DailyExpenses
}

SiteMaterialConsumption {
    int SiteConsumptionID PK
    int SiteOperationID FK
    int MaterialID FK
    decimal QuantityConsumed
}

JournalEntries {
    int JournalEntryID PK
    nvarchar ReferenceType
    int ReferenceID
    datetime TransactionDate
    nvarchar Narration
}

JournalEntryLines {
    int JournalLineID PK
    int JournalEntryID FK
    int AccountID FK
    int ProjectID FK
    decimal Debit
    decimal Credit
}

ChartOfAccounts ||--o{ Customers : financial_account
ChartOfAccounts ||--o{ JournalEntryLines : booked_to

Customers ||--o{ Projects : orders

Projects ||--o{ ProjectItems : contains
Projects ||--o{ ProjectMolds : allocates
Projects ||--o{ ProductionOrders : tracks_production
Projects ||--o{ DeliveryOrders : ships_to
Projects ||--o{ SiteOperations : manages_installations
Projects ||--o{ JournalEntryLines : cost_center

ProjectItems ||--o{ ProductionOrders : defines_product
ProjectItems ||--o{ DeliveryItems : shipped_as
ProjectItems ||--o{ SiteOperations : installed_as

Molds ||--o{ ProjectMolds : assigned_to
Molds ||--o{ ProductionOrders : shapes_production

MixDesigns ||--o{ MixIngredients : formulates
MixDesigns ||--o{ ProductionOrders : specifies_mix

InventoryItems ||--o{ MixIngredients : used_as_ingredient
InventoryItems ||--o{ ProductionMaterialConsumption : factory_material
InventoryItems ||--o{ SiteMaterialConsumption : site_material

ProductionOrders ||--o{ ProductionMaterialConsumption : requires_materials

DeliveryOrders ||--o{ DeliveryItems : includes

SiteOperations ||--o{ SiteMaterialConsumption : requires_site_materials

JournalEntries ||--o{ JournalEntryLines : has_lines
