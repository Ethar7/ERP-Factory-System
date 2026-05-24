# ERP System Modules

## Overview
This ERP system is designed for a project-based cement manufacturing factory.  
The system is divided into functional modules, each responsible for a specific part of the workflow from estimation to accounting.

---

# 1. Project Management Module
Responsible for managing all projects in the system.

### Features:
- Create / Update / Delete Projects
- Assign Customer to Project
- Define Project Timeline
- Track Project Status

### Related Tables:
- Projects
- Customers
- ProjectItems

---

# 2. Estimation & BOQ Module
Handles project breakdown and quantity estimation.

### Features:
- Define Project Items (BOQ)
- Set required quantities
- Define unit prices
- Calculate total project value

### Related Tables:
- ProjectItems

---

# 3. Mold Management Module
Manages all molds used in production.

### Features:
- Register molds
- Track mold usage cycles
- Assign molds to projects
- Maintenance tracking

### Related Tables:
- Molds
- ProjectMolds

---

# 4. Mix Design Module
Defines concrete/cement mix recipes.

### Features:
- Create mix designs
- Define material ratios
- Calculate cost per unit
- Link mix to production

### Related Tables:
- MixDesigns
- MixIngredients

---

# 5. Production Management Module
Core manufacturing module.

### Features:
- Create production orders
- Track production stages
- Record good vs rejected quantities
- Calculate labor & mold cost
- Consume raw materials automatically

### Related Tables:
- ProductionOrders
- ProductionMaterialConsumption

---

# 6. Inventory Management Module
Controls all warehouse operations.

### Features:
- Add inventory items
- Track stock levels
- Record all stock movements
- Monitor raw materials + finished goods

### Related Tables:
- InventoryItems
- InventoryTransactions

---

# 7. Delivery Management Module
Handles shipment from factory to site.

### Features:
- Create delivery orders
- Track shipped quantities
- Record damaged goods
- Confirm site receipt

### Related Tables:
- DeliveryOrders
- DeliveryItems

---

# 8. Site Operations Module
Tracks installation and site execution.

### Features:
- Record installed quantities
- Track daily site work
- Record site expenses
- Track installation progress per item

### Related Tables:
- SiteOperations
- SiteMaterialConsumption

---

# 9. Accounting Module
Handles all financial operations and postings.

### Features:
- Chart of Accounts
- Journal Entries
- Automatic posting from production/delivery/site
- Cost center tracking per project

### Related Tables:
- ChartOfAccounts
- JournalEntries
- JournalEntryLines

---

# 10. Costing & Analytics Module (Implicit Module)
Responsible for financial analysis and profitability.

### Features:
- Project cost calculation
- Material wastage analysis
- Labor cost tracking
- Mold depreciation calculation
- Profitability reports

### Data Source:
- All system modules

---

# System Flow Summary

Quotation → Project → BOQ → Mold Setup → Mix Design → Production → Inventory → Delivery → Site Installation → Accounting → Reporting
