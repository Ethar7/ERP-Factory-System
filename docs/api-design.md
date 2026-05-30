# 📘 API Documentation

> Complete reference for all available API endpoints, organized by module.

---

## 📑 Table of Contents

- [🔐 Auth](#-auth)
- [👥 Customers](#-customers)
- [📂 Projects](#-projects)
- [📦 Project Items](#-project-items)
- [🏭 Production Orders](#-production-orders)
- [🎨 Mix Designs](#-mix-designs)
- [🧱 Molds](#-molds)
- [🚚 Delivery Orders](#-delivery-orders)
- [🏗️ Site Operations](#️-site-operations)
- [📋 Inventory](#-inventory)
- [💰 Accounting](#-accounting)
- [📊 Reports](#-reports)
- [👤 Admin Users](#-admin-users)

---

## 🔐 Auth

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/Auth/register` | Register a new user |
| `POST` | `/api/Auth/login` | Login and obtain token |

---

## 👥 Customers

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Customers` | Get all customers |
| `POST` | `/api/Customers` | Create a new customer |
| `GET` | `/api/Customers/{customerId}` | Get customer by ID |
| `PUT` | `/api/Customers/{customerId}` | Update customer by ID |
| `GET` | `/api/Customers/{customerId}/projects` | Get all projects for a customer |

---

## 📂 Projects

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Projects` | Get all projects |
| `POST` | `/api/Projects` | Create a new project |
| `GET` | `/api/Projects/{projectId}` | Get project by ID |
| `PUT` | `/api/Projects/{projectId}` | Update project by ID |
| `PATCH` | `/api/Projects/{projectId}/status` | Update project status |
| `GET` | `/api/Projects/{projectId}/items` | Get all items in a project |
| `POST` | `/api/Projects/{projectId}/items` | Add item to a project |
| `GET` | `/api/Projects/{projectId}/summary` | Get project summary |
| `GET` | `/api/Projects/{projectId}/dashboard` | Get project dashboard data |

---

## 📦 Project Items

| Method | Endpoint | Description |
|--------|----------|-------------|
| `PUT` | `/api/ProjectItems/{projectItemId}` | Update a project item |
| `DELETE` | `/api/ProjectItems/{projectItemId}` | Delete a project item |

---

## 🏭 Production Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/ProductionOrders` | Get all production orders |
| `POST` | `/api/ProductionOrders` | Create a new production order |
| `GET` | `/api/ProductionOrders/{productionOrderId}` | Get production order by ID |
| `PUT` | `/api/ProductionOrders/{productionOrderId}` | Update production order |
| `PATCH` | `/api/ProductionOrders/{productionOrderId}/status` | Update production order status |
| `PATCH` | `/api/ProductionOrders/{productionOrderId}/quality-check` | Submit quality check result |
| `GET` | `/api/ProductionOrders/{productionOrderId}/material-consumption` | Get material consumption |
| `POST` | `/api/ProductionOrders/{productionOrderId}/material-consumption` | Log material consumption |
| `POST` | `/api/ProductionOrders/{productionOrderId}/post-accounting` | Post accounting entries |
| `POST` | `/api/ProductionOrders/{productionOrderId}/inventory-posting` | Post inventory update |
| `GET` | `/api/ProductionOrders/{productionOrderId}/progress` | Get production progress |
| `GET` | `/api/ProductionOrders/in-progress` | Get all in-progress orders |
| `GET` | `/api/ProductionOrders/completed` | Get all completed orders |

---

## 🎨 Mix Designs

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/MixDesigns` | Get all mix designs |
| `POST` | `/api/MixDesigns` | Create a new mix design |
| `GET` | `/api/MixDesigns/{mixDesignId}` | Get mix design by ID |
| `PUT` | `/api/MixDesigns/{mixDesignId}` | Update mix design |
| `GET` | `/api/MixDesigns/{mixDesignId}/ingredients` | Get mix design ingredients |
| `POST` | `/api/MixDesigns/{mixDesignId}/ingredients` | Add ingredient to mix design |
| `GET` | `/api/MixDesigns/{mixDesignId}/cost-analysis` | Get cost analysis for mix design |

---

## 🧱 Molds

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Molds` | Get all molds |
| `POST` | `/api/Molds` | Create a new mold |
| `GET` | `/api/Molds/{moldId}` | Get mold by ID |
| `PUT` | `/api/Molds/{moldId}` | Update mold |
| `PATCH` | `/api/Molds/{moldId}/status` | Update mold status |
| `GET` | `/api/Molds/{moldId}/usage-history` | Get mold usage history |
| `GET` | `/api/Molds/{moldId}/cost-analysis` | Get mold cost analysis |

---

## 🚚 Delivery Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/DeliveryOrders` | Get all delivery orders |
| `POST` | `/api/DeliveryOrders` | Create a new delivery order |
| `GET` | `/api/DeliveryOrders/{deliveryOrderId}` | Get delivery order by ID |
| `PUT` | `/api/DeliveryOrders/{deliveryOrderId}` | Update delivery order |
| `PATCH` | `/api/DeliveryOrders/{deliveryOrderId}/status` | Update delivery order status |
| `POST` | `/api/DeliveryOrders/{deliveryOrderId}/items` | Add items to delivery order |
| `GET` | `/api/DeliveryOrders/{deliveryOrderId}/tracking` | Get delivery tracking info |
| `PATCH` | `/api/DeliveryOrders/{deliveryOrderId}/items/{deliveryItemId}/receive-confirmation` | Confirm receipt of a delivery item |

---

## 🏗️ Site Operations

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/SiteOperations` | Get all site operations |
| `POST` | `/api/SiteOperations` | Create a new site operation |
| `GET` | `/api/SiteOperations/{siteOperationId}` | Get site operation by ID |
| `PUT` | `/api/SiteOperations/{siteOperationId}` | Update site operation |
| `GET` | `/api/SiteOperations/{siteOperationId}/consumption` | Get resource consumption |
| `POST` | `/api/SiteOperations/{siteOperationId}/consumption` | Log resource consumption |
| `GET` | `/api/SiteOperations/project/{projectId}/cost-summary` | Get project cost summary for site ops |

---

## 📋 Inventory

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Inventory/items` | Get all inventory items |
| `POST` | `/api/Inventory/items` | Create a new inventory item |
| `GET` | `/api/Inventory/items/{itemId}` | Get inventory item by ID |
| `PUT` | `/api/Inventory/items/{itemId}` | Update inventory item |
| `GET` | `/api/Inventory/transactions` | Get all inventory transactions |
| `POST` | `/api/Inventory/transactions` | Create an inventory transaction |
| `GET` | `/api/Inventory/items/{itemId}/balance` | Get current balance for an item |
| `GET` | `/api/Inventory/items/{itemId}/transactions` | Get transaction history for an item |
| `GET` | `/api/Inventory/low-stock` | Get all low-stock items |

---

## 💰 Accounting

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Accounting/chart-of-accounts` | Get chart of accounts |
| `POST` | `/api/Accounting/chart-of-accounts` | Create a new account |
| `GET` | `/api/Accounting/journal-entries` | Get all journal entries |
| `POST` | `/api/Accounting/journal-entries` | Create a journal entry |
| `GET` | `/api/Accounting/journal-entries/{journalEntryId}` | Get journal entry by ID |

---

## 📊 Reports

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Reports/project-cost-summary` | Get cost summary across all projects |
| `GET` | `/api/Reports/projects/{projectId}/cost-summary` | Get cost summary for a project |
| `GET` | `/api/Reports/projects/{projectId}/profitability` | Get profitability report for a project |
| `GET` | `/api/Reports/projects/{projectId}/production-progress` | Get production progress report |
| `GET` | `/api/Reports/projects/{projectId}/delivery-progress` | Get delivery progress report |
| `GET` | `/api/Reports/projects/{projectId}/installation-progress` | Get installation progress report |
| `GET` | `/api/Reports/projects/{projectId}/material-variance` | Get material variance report |
| `GET` | `/api/Reports/projects/{projectId}/mold-cost` | Get mold cost report |
| `GET` | `/api/Reports/journal-entry-balance` | Get journal entry balance report |

---

## 👤 Admin Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/admin/users` | Get all users |
| `PUT` | `/api/admin/users/{userId}/role` | Update user role |

---

<div align="center">

### HTTP Methods Legend

| Badge | Method | Usage |
|-------|--------|-------|
| `GET` | GET | Retrieve data |
| `POST` | POST | Create new resource |
| `PUT` | PUT | Replace/update resource |
| `PATCH` | PATCH | Partial update |
| `DELETE` | DELETE | Delete resource |

</div>