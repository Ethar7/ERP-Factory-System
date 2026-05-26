# ERP Factory System API Design

## Overview

This document defines the proposed REST API design for the ERP Factory System. The APIs are grouped by module and cover project-based manufacturing from quotation and BOQ through molds, mix designs, production, inventory, delivery, site operations, accounting, and reports.

Base URL:

```text
/api/v1
```

Common success response:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {}
}
```

Common error response:

```json
{
  "success": false,
  "message": "Validation error",
  "errors": []
}
```

## 1. Projects and BOQ

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/projects` | List projects with optional filters |
| `POST` | `/projects` | Create a project; can support BOQ items in the same request |
| `GET` | `/projects/{projectId}` | Get project details with BOQ |
| `PUT` | `/projects/{projectId}` | Update project basic data |
| `PATCH` | `/projects/{projectId}/status` | Update project status |
| `GET` | `/projects/{projectId}/items` | List project BOQ items |
| `POST` | `/projects/{projectId}/items` | Add BOQ items to project |
| `PUT` | `/project-items/{projectItemId}` | Update BOQ item |
| `DELETE` | `/project-items/{projectItemId}` | Delete BOQ item |

Example create project request:

```json
{
  "projectName": "New Administrative Building",
  "customerId": 1,
  "startDate": "2026-06-01",
  "totalEstimatedBudget": 2500000,
  "items": [
    {
      "itemCode": "SEC-001",
      "itemName": "Concrete Facade Panel",
      "unit": "m2",
      "requiredQuantity": 500,
      "estimatedUnitPrice": 1200,
      "taxRate": 14
    }
  ]
}
```

## 2. Customers

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/customers` | List customers |
| `POST` | `/customers` | Create customer and optionally link to chart of accounts |
| `GET` | `/customers/{customerId}` | Get customer details |
| `PUT` | `/customers/{customerId}` | Update customer |

## 3. Molds Management

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/molds` | List molds and lifespan usage |
| `POST` | `/molds` | Create mold with build cost and expected lifespan |
| `GET` | `/molds/{moldId}` | Get mold details |
| `PUT` | `/molds/{moldId}` | Update mold data |
| `PATCH` | `/molds/{moldId}/status` | Update mold status |
| `GET` | `/projects/{projectId}/molds` | List molds allocated to a project |
| `POST` | `/projects/{projectId}/molds` | Allocate molds to project |
| `DELETE` | `/project-molds/{projectMoldId}` | Cancel mold allocation |

Example allocate mold request:

```json
{
  "moldId": 3,
  "allocQuantity": 5
}
```

## 4. Mix Designs and Ingredients

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/mix-designs` | List approved mix designs |
| `POST` | `/mix-designs` | Create mix design; can support ingredients in the same request |
| `GET` | `/mix-designs/{mixDesignId}` | Get mix design with ingredients |
| `PUT` | `/mix-designs/{mixDesignId}` | Update mix design basic data |
| `GET` | `/mix-designs/{mixDesignId}/ingredients` | List mix ingredients |
| `POST` | `/mix-designs/{mixDesignId}/ingredients` | Add raw material to mix |
| `PUT` | `/mix-ingredients/{ingredientId}` | Update ingredient quantity |
| `DELETE` | `/mix-ingredients/{ingredientId}` | Delete ingredient from mix |

Example create mix design request:

```json
{
  "mixName": "High Strength Mix",
  "targetStrength": "C40",
  "standardCostPerUnit": 950,
  "ingredients": [
    {
      "rawMaterialId": 10,
      "standardQtyPerUnit": 2.5
    }
  ]
}
```

## 5. Production Orders and Quality Control

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/production-orders` | List production orders |
| `POST` | `/production-orders` | Create production batch linked to project, item, mix, and mold |
| `GET` | `/production-orders/{productionOrderId}` | Get production order details with wastage |
| `PUT` | `/production-orders/{productionOrderId}` | Update production order while still in setup stage |
| `PATCH` | `/production-orders/{productionOrderId}/status` | Update production stage |
| `POST` | `/production-orders/{productionOrderId}/material-consumption` | Record actual raw material consumption |
| `GET` | `/production-orders/{productionOrderId}/material-consumption` | List actual vs standard material consumption |
| `PATCH` | `/production-orders/{productionOrderId}/quality-check` | Record produced, good, and rejected quantities |
| `POST` | `/production-orders/{productionOrderId}/post-accounting` | Post automatic accounting entries after production completion |

Example create production order request:

```json
{
  "projectId": 1,
  "projectItemId": 4,
  "mixDesignId": 2,
  "moldId": 3,
  "batchNumber": "B-2026-0001",
  "targetQuantity": 100,
  "laborCost": 5000,
  "moldDepreciationCost": 1200
}
```

Example quality check request:

```json
{
  "producedQuantity": 100,
  "goodQuantity": 95,
  "rejectedQuantity": 5,
  "laborCost": 5200,
  "moldDepreciationCost": 1200
}
```

## 6. Inventory Transactions

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/inventory/items` | List inventory items with current stock and average cost |
| `POST` | `/inventory/items` | Create inventory item |
| `GET` | `/inventory/items/{itemId}` | Get inventory item details |
| `PUT` | `/inventory/items/{itemId}` | Update inventory item |
| `GET` | `/inventory/transactions` | List inventory audit trail |
| `POST` | `/inventory/transactions` | Create inventory movement |
| `GET` | `/projects/{projectId}/inventory-transactions` | List project-related inventory movements |

Example inventory transaction request:

```json
{
  "itemId": 10,
  "projectId": 1,
  "transactionType": "ProductionIssue",
  "quantity": 150.75,
  "unitCost": 12.5,
  "referenceType": "ProductionOrder",
  "referenceId": 7,
  "notes": "Raw material issued for production batch"
}
```

## 7. Delivery Orders

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/delivery-orders` | List delivery orders |
| `POST` | `/delivery-orders` | Create delivery order with driver, vehicle, and ticket numbers |
| `GET` | `/delivery-orders/{deliveryOrderId}` | Get delivery order details and shipped quantities |
| `PUT` | `/delivery-orders/{deliveryOrderId}` | Update delivery order data |
| `PATCH` | `/delivery-orders/{deliveryOrderId}/status` | Update delivery status |
| `POST` | `/delivery-orders/{deliveryOrderId}/items` | Add products to delivery order |
| `PUT` | `/delivery-items/{deliveryItemId}` | Update delivery item quantities |
| `PATCH` | `/delivery-items/{deliveryItemId}/receive-confirmation` | Confirm received quantity and damaged quantity at site |
| `GET` | `/projects/{projectId}/delivery-summary` | Get delivered, damaged, and remaining quantities |

Example receive confirmation request:

```json
{
  "quantityReceived": 90,
  "quantityDamagedInTransit": 2
}
```

## 8. Site Operations

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/site-operations` | List site installation operations |
| `POST` | `/site-operations` | Record daily installed quantity, labor, and expenses |
| `GET` | `/site-operations/{siteOperationId}` | Get site operation details |
| `PUT` | `/site-operations/{siteOperationId}` | Update site operation |
| `POST` | `/site-operations/{siteOperationId}/consumption` | Record accessories, finishing materials, or consumables used on site |
| `GET` | `/site-operations/{siteOperationId}/consumption` | List site material consumption |
| `GET` | `/projects/{projectId}/installation-summary` | Get installed and remaining quantities |

Example site operation request:

```json
{
  "projectId": 1,
  "projectItemId": 4,
  "installedQuantity": 25,
  "supervisorLaborCost": 2000,
  "dailyExpenses": 750
}
```

## 9. Accounting

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/accounting/chart-of-accounts` | List chart of accounts |
| `POST` | `/accounting/chart-of-accounts` | Create account |
| `GET` | `/accounting/journal-entries` | List journal entries |
| `POST` | `/accounting/journal-entries` | Create manual journal entry |
| `GET` | `/accounting/journal-entries/{journalEntryId}` | Get journal entry with lines |
| `POST` | `/accounting/journal-entries/{journalEntryId}/lines` | Add journal entry line |
| `GET` | `/accounting/journal-entries/{journalEntryId}/balance-check` | Check journal entry debit and credit balance |

Example journal entry request:

```json
{
  "referenceType": "ProductionOrder",
  "referenceId": 7,
  "narration": "Production batch material and labor cost",
  "lines": [
    {
      "accountId": 501,
      "projectId": 1,
      "debit": 6200,
      "credit": 0
    },
    {
      "accountId": 101,
      "projectId": 1,
      "debit": 0,
      "credit": 6200
    }
  ]
}
```

## 10. Reports

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/reports/project-cost-summary` | List project cost summary for all projects |
| `GET` | `/reports/projects/{projectId}/cost-summary` | Project cost summary |
| `GET` | `/reports/projects/{projectId}/profitability` | Project profitability report |
| `GET` | `/reports/projects/{projectId}/production-progress` | Produced, rejected, and remaining quantities |
| `GET` | `/reports/projects/{projectId}/delivery-progress` | Delivered, damaged, and remaining quantities |
| `GET` | `/reports/projects/{projectId}/installation-progress` | Installed and remaining quantities |
| `GET` | `/reports/projects/{projectId}/material-variance` | Actual vs theoretical material consumption |
| `GET` | `/reports/projects/{projectId}/mold-cost` | Mold usage and cost allocation |
| `GET` | `/reports/journal-entry-balance` | List journal entry balance report |

Example project profitability response:

```json
{
  "success": true,
  "data": {
    "projectId": 1,
    "projectName": "New Administrative Building",
    "estimatedBudget": 2500000,
    "productionDirectCost": 850000,
    "siteDirectCost": 180000,
    "totalDirectCost": 1030000,
    "estimatedProfit": 1470000
  }
}
```

## 11. Suggested User Roles

| Role | Main Access |
| --- | --- |
| `Admin` | Full system access |
| `ProjectManager` | Projects, BOQ, molds, production, delivery, and reports |
| `InventoryUser` | Inventory items and inventory transactions |
| `Accountant` | Chart of accounts, journal entries, and cost reports |

## 12. Main Business Rules

- A project must be approved before creating production orders.
- Production orders must be linked to a project item, mix design, and mold.
- Mold allocation should affect mold availability and usage tracking.
- Actual material consumption should create inventory transactions.
- Finished production quantities should be received into inventory.
- Delivery quantities should not exceed available finished quantities.
- Site installation quantities should not exceed delivered quantities.
- Site material consumption should create inventory transactions.
- Journal entry lines must contain either debit or credit, not both.
- Journal entries should be balanced before final posting.
