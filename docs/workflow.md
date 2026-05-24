# ERP Manufacturing Workflow (Ionic Factory)

## 📌 Overview
This document explains the full end-to-end workflow of the ERP system for a project-based manufacturing factory. It seamlessly connects all modules: Projects, Production, Inventory, Delivery, Site Operations, and Accounting under a unified Cost Center system, mapping perfectly to the database schema.

---

## 📑 1. Workflow Description

### 1. Quotation & Project Creation
* Customer request/tender is received.
* Technical quotation is prepared based on engineering specs.
* Project is created in the system with a unique **Project ID** acting as a **Cost Center**.
* **BOQ (Project Items)** is defined and itemized in `ProjectItems` with specific `ItemCode`, quantities, and estimated prices to avoid any miscommunication between departments.

### 2. Project Approval & Activation
* Project is reviewed, contract terms are finalized, and the client signs off.
* Total project budget is confirmed and locked in the system.
* Project status shifts from **Draft** to **Approved/Active**, opening the financial period for this project to receive transactions.

### 3. Mold Preparation & Allocation
* Required molds are assigned from the current available inventory (`Molds`) or marked to be custom-built (`CostToBuild`).
* Molds are linked to the project via `ProjectMolds` to track their exact utilization, current uses count, and remaining lifespan.

### 4. Mix Design Selection (BOM)
* Appropriate standard mix design is selected from `MixDesigns` based on target strength requirements (e.g., 350 Kg/cm²).
* Standard material ratios (Bill of Materials - BOM) are pulled from `MixIngredients` to define theoretical raw material weights needed per unit.

### 5. Production & Quality Control Execution
* Production orders are created under a unique `BatchNumber` to track quality.
* Production status tracks stages in real-time: **Setup**, **Pouring/Curing (WIP)**, and **Finished**.
* Direct labor costs (`LaborCost`) and automated mold depreciation costs (`MoldDepreciationCost`) are recorded per batch.
* **Quality Control Check:** The total produced quantity is split into **GoodQuantity** (passed QC) and **RejectedQuantity** (scrapped or moved for crushing/recycling).

### 6. Inventory Update & Waste Analysis
* Upon batch closure, raw materials and accessories are automatically deducted from `InventoryItems` based on **Actual** consumption.
* Finished goods (`GoodQuantity`) are added to stock, automatically reserved under the specific `ProjectID`.
* **Variance Tracking:** System automatically calculates `WastageQty` (`ActualQtyConsumed` - `StandardQtyExpected`) to detect material deviation.
* System triggers low-stock alerts if raw materials drop below the predefined **Reorder Point**.

### 7. Delivery & Logistics
* Delivery orders are created in `DeliveryOrders` based on approved loading tickets.
* Finished goods are shipped to the site, tracking driver details and vehicle numbers.
* **Transit Check:** Quantities received at the site are matched against shipped amounts (`QuantityReceived` vs `QuantityShipped`). Any damages during transit are logged into `QuantityDamagedInTransit` to isolate shipping losses financially.

### 8. Site Operations & Installation
* Finished items are installed at the client's site, logging daily progress via `InstalledQuantity` in `SiteOperations`.
* Daily installation labor, equipment rentals, and site expenses are recorded.
* Site-specific materials (installation accessories like anchors/screws and finishing chemicals) are consumed via `SiteMaterialConsumption`, deducting them directly from the main inventory.

### 9. Real-Time Accounting Integration
* Every operational movement automatically triggers balanced **Journal Entries** (Debit/Credit) into `JournalEntries` and `JournalEntryLines`.
* All financial lines are automatically tagged with the **Project ID** to act as a **Cost Center**.
* The `IsAccountingPosted` flag is set to `1` upon success to secure data and prevent duplicate entries.

### 10. Project Completion & Evaluation
* Once all BOQ items are installed and verified, the project status shifts to **Completed**.
* The system aggregates all general ledger lines under the project's cost center to evaluate total actual spending against the estimated budget.
* Final project variance reports are generated for timelines, material waste, and overall profitability.

---

## 📊 2. System Outputs & Dashboards
* **Total Project Cost:** Full financial transparency across all operational stages.
* **Material Consumption Variance:** Precision comparison between Theoretical (BOM) vs. Actual raw material consumption.
* **Waste & Scrap Analysis:** Monitoring the percentage of wastage during manufacturing vs. transit damages.
* **Mold Efficiency:** Tracking the exact production yield, degradation, and financial depreciation cost per mold asset.
* **Project Profitability Report:** Instant ROI calculation ($\text{Quotation Price} - \text{Actual Accumulated Cost}$).
