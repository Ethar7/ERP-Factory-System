```mermaid
graph TD
    %% تعاريف الألوان والتنسيق الاستايلنج
    classDef project fill:#2b5c8f,stroke:#333,stroke-width:2px,color:#fff;
    classDef prep fill:#e67e22,stroke:#333,stroke-width:2px,color:#fff;
    classDef prod fill:#27ae60,stroke:#333,stroke-width:2px,color:#fff;
    classDef delivery fill:#2980b9,stroke:#333,stroke-width:2px,color:#fff;
    classDef finance fill:#c0392b,stroke:#333,stroke-width:2px,color:#fff;

    %% 1. موديول المشاريع
    subgraph Module 1: Projects & BOQ
        A[1. Project Creation & BOQ] -->|Saves to Projects & ProjectItems| B(2. Review & Approve Budget)
        B -->|Status = Approved/Active| C{Project Activated?}
    end
    class A,B,C project;

    %% 2. موديول التحضير
    subgraph Module 2: Manufacturing Prep
        C -->|Yes| D[3. Allocate Molds]
        C -->|Yes| E[4. Select Mix Design & BOM]
        D -->|Links to ProjectMolds| F[Ready for Production]
        E -->|Standard Qty in MixIngredients| F
    end
    class D,E,F prep;

    %% 3. موديول الإنتاج والمخازن
    subgraph Module 3: Production & Inventory Control
        F --> G[5. Create Production Order]
        G -->|Batch Closing| H[8. Quality Control Station]
        H -->|Good Quantity| I[Add to InventoryItems - Finished Goods]
        H -->|Rejected Quantity| J[Log Waste & Scrap Analysis]
        
        %% الخصم التلقائي
        G -.->|Auto Deduct Raw Materials| K[InventoryItems - Raw Goods]
        G -.->|Calculate Variance| L[ProductionMaterialConsumption - WastageQty]
    end
    class G,H,I,J,K,L prod;

    %% 4. موديول التوريد والموقع
    subgraph Module 4: Logistics & Site Operations
        I --> M[9. Delivery Orders & Logistics]
        M -->|Shipped vs Received| N[10. Site Installation Progress]
        M -->|Transit Damage| O[Log Shipping Losses]
        N -->|Daily Labor & Expenses| P[SiteOperations & SiteMaterialConsumption]
    end
    class M,N,O,P delivery;

    %% 5. موديول الحسابات والتكاليف
    subgraph Module 5: Financial Accounting & Cost Centers
        G ==>|Trigger Balanced Ledger Entries| Q((11. Real-Time Accounting Integration))
        M ==>|Trigger Auto Journal Entries| Q
        P ==>|Trigger Daily Site Expenses| Q
        
        Q -->|Post Debit/Credit via ProjectID| R[JournalEntries & JournalEntryLines]
        R -->|Final Output| S[12. Dynamic Profitability Dashboard]
'''
    end
    class Q,R,S finance;
