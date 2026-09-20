# CodeArena Architecture & Data Flow

Bhai, yeh raha aapke CodeArena project ka complete Architecture Diagram. Isme clean architecture, WebSockets, aur AI integration ka pura flow dikhaya gaya hai.

```mermaid
flowchart TD
  subgraph Frontend ["🖥️ Angular 18 (Client App)"]
    UI[UI / PicoCSS Dashboard]
    Editor[Monaco Code Editor]
    API_Client[NSwag HTTP Client]
    SignalR_Client[SignalR WebSockets]
  end

  subgraph Backend ["⚙️ .NET 10 Backend (Clean Architecture)"]
    API[Web API Endpoints]
    CQRS[MediatR Command/Query]
    SignalR_Hub[Submission Hub]
    AI_Service[Semantic Kernel Service]
    EF[Entity Framework Core]
  end

  subgraph Database ["💾 Local Storage"]
    SQLite[(SQLite Database)]
  end

  subgraph External ["☁️ Cloud Services"]
    Groq[Groq API / Llama 3]
  end

  %% --- Connections ---
  UI -->|1. Type Code| Editor
  Editor -->|2. Click Submit| API_Client
  API_Client -->|3. HTTP POST Request| API
  
  API -->|4. Trigger Command| CQRS
  CQRS -->|5. Forward Code| AI_Service
  
  AI_Service -->|6. Push Status Updates| SignalR_Hub
  SignalR_Hub -.->|7. Live UI Updates| SignalR_Client
  
  AI_Service -->|8. Send Prompt + Code| Groq
  Groq -->|9. Return Verdict| AI_Service
  
  AI_Service -->|10. Pass Final Result| CQRS
  CQRS -->|11. Save Submission Data| EF
  EF -->|12. Read/Write| SQLite

  style Frontend fill:#1e1e1e,stroke:#00a3ff,stroke-width:2px,color:#fff
  style Backend fill:#1e1e1e,stroke:#8e44ad,stroke-width:2px,color:#fff
  style Database fill:#1e1e1e,stroke:#27ae60,stroke-width:2px,color:#fff
  style External fill:#1e1e1e,stroke:#f39c12,stroke-width:2px,color:#fff
```

### 🧠 Kaun Kisse Connect Karta Hai (The Flow)

Chalo isko ek real example (Code Submit karna) se samajhte hain:

1. **User Input (Angular & Monaco):** 
   User Monaco Editor (VS Code jaisa UI) mein apna solution likhta hai aur "Submit" dabata hai.
2. **Frontend to Backend (NSwag):** 
   Angular ka `NSwag` client automatically us code ko uthata hai, JSON mein pack karta hai, aur backend (C# API) ko ek `HTTP POST` request bhejta hai.
3. **Backend Processing (MediatR & Clean Architecture):** 
   Request .NET API par aati hai. Humara backend **MediatR** (CQRS pattern) use karta hai. API seedha `EvaluateSubmissionCommand` ko kaam de deti hai.
4. **Live Loading (SignalR WebSockets):** 
   Jaise hi command start hoti hai, Backend ka **SignalR Hub** ek two-way connection (WebSocket) open rakhta hai. Yeh frontend ko chupke se live messages bhejta hai ("Compiling...", "Analyzing with AI..."). Frontend ka `SignalR_Client` usko pakad ke screen par live loading text change karta hai.
5. **AI Magic (Semantic Kernel to Groq):** 
   Command code ko `SemanticKernelJudgeService` ko bhejti hai. Semantic Kernel us code ko ek prompt ke sath **Groq API** (Llama 3) par internet ke through HTTP request bhejta hai. Groq code ko check karke result wapas C# backend ko deta hai.
6. **Saving to DB (EF Core to SQLite):** 
   C# backend Regex lagakar Groq ke answer (Pass/Fail) ko nikalta hai. Phir **Entity Framework (EF) Core** ko order deta hai ki "Is result ko save karo". EF Core SQL query banakar usko **SQLite Database** (`CodeArena.db`) mein likh deta hai.
7. **Final UI Update:** 
   Backend frontend ko HTTP request ka final success response deta hai, aur user ke dashboard par green/red tick aa jata hai!

Bhai, yeh architecture bilkul enterprise-level companies (jaise Microsoft/Amazon) wala hai. Isme frontend, backend, aur database ek doosre se strongly "decoupled" (alag) hain!
