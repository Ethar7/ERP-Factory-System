# ERP Factory System ERD

```mermaid
erDiagram

ChartOfAccounts {
    int AccountID PK
    string AccountCode
    string AccountName
    string AccountType
}

Customers {
    int CustomerID PK
    string CustomerName
    string ContactPerson
    string Phone
    string Email
    string Address
    int AccountID FK
    datetime CreatedAt
}

Projects {
    int ProjectID PK
    string ProjectName
    int CustomerID FK
    datetime StartDate
    datetime EndDate
    string ProjectStatus
    decimal TotalEstimatedBudget
    datetime CreatedAt
}

ProjectItems {
    int ProjectItemID PK
    int ProjectID FK
    string ItemCode
    string ItemName
    string Unit
    decimal RequiredQuantity
    decimal EstimatedUnitPrice
    decimal TaxRate
    decimal TaxAmount
    decimal TotalPrice
    decimal TotalPriceWithTax
}

InventoryItems {
    int ItemID PK
    string ItemName
    string ItemType
    string Unit
    decimal CurrentStock
    decimal AverageCost
}

InventoryTransactions {
    int TransactionID PK
    int ItemID FK
    int ProjectID FK
    string TransactionType
    decimal Quantity
    decimal UnitCost
    datetime TransactionDate
    string ReferenceType
    int ReferenceID
    string Notes
}

Molds {
    int MoldID PK
    string MoldName
    decimal CostToBuild
    int ExpectedLifespanUses
    int CurrentUsesCount
    string MoldStatus
}

MixDesigns {
    int MixDesignID PK
    string MixName
    string TargetStrength
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
    string BatchNumber
    decimal TargetQuantity
    decimal ProducedQuantity
    decimal GoodQuantity
    decimal RejectedQuantity
    decimal LaborCost
    decimal MoldDepreciationCost
    string ProductionStatus
    bool IsAccountingPosted
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
    string DriverName
    string VehicleNumber
    string LoadingTicketNumber
    string DeliveryTicketNumber
    string DeliveryStatus
    bool IsInvoiced
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
    string ReferenceType
    int ReferenceID
    datetime TransactionDate
    string Narration
}

JournalEntryLines {
    int JournalLineID PK
    int JournalEntryID FK
    int AccountID FK
    int ProjectID FK
    decimal Debit
    decimal Credit
}

ChartOfAccounts ||--o{ Customers : linked_account
ChartOfAccounts ||--o{ JournalEntryLines : accounting_lines

Customers ||--o{ Projects : owns

Projects ||--o{ ProjectItems : contains
Projects ||--o{ InventoryTransactions : stock_movements
Projects ||--o{ ProductionOrders : production
Projects ||--o{ DeliveryOrders : deliveries
Projects ||--o{ SiteOperations : site_work
Projects ||--o{ JournalEntryLines : cost_center

ProjectItems ||--o{ ProductionOrders : produced_as
ProjectItems ||--o{ DeliveryItems : delivered_as
ProjectItems ||--o{ SiteOperations : installed_as

InventoryItems ||--o{ InventoryTransactions : tracked_by
InventoryItems ||--o{ MixIngredients : raw_material
InventoryItems ||--o{ ProductionMaterialConsumption : consumed_in_factory
InventoryItems ||--o{ SiteMaterialConsumption : consumed_on_site

Molds ||--o{ ProductionOrders : used_in

MixDesigns ||--o{ MixIngredients : contains
MixDesigns ||--o{ ProductionOrders : applied_in

ProductionOrders ||--o{ ProductionMaterialConsumption : material_usage

DeliveryOrders ||--o{ DeliveryItems : delivery_lines

SiteOperations ||--o{ SiteMaterialConsumption : material_usage

JournalEntries ||--o{ JournalEntryLines : entry_lines
```

## Reporting Views

- `ProjectCostSummary`: summarizes project estimated budget, production direct cost, site direct cost, and total direct cost.
- `JournalEntryBalance`: summarizes debit, credit, and balance difference for each journal entry.
