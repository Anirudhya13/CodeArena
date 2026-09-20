# CodeArena Architecture & Data Flow

Welcome to the architectural documentation for CodeArena. This document outlines the application's clean architecture, WebSockets integration, and AI evaluation pipeline.

`mermaid
flowchart TD
  subgraph Frontend ["??? Angular 18 (Client App)"]
    UI[UI / PicoCSS Dashboard]
    Editor[Monaco Code Editor]
    API_Client[NSwag HTTP Client]
    SignalR_Client[SignalR WebSockets]
  end

  subgraph Backend ["?? .NET 10 Backend (Clean Architecture)"]
    API[Web API Endpoints]
    CQRS[MediatR Command/Query]
    SignalR_Hub[Submission Hub]
    AI_Service[Semantic Kernel Service]
    EF[Entity Framework Core]
  end

  subgraph Database ["?? Local Storage"]
    SQLite[(SQLite Database)]
  end

  subgraph External ["?? Cloud Services"]
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
`

### ?? Component Flow & Interactions

Here is a step-by-step breakdown of how the components interact when a user submits code:

1. **User Input (Angular & Monaco):** 
   The user writes their algorithmic solution inside the Monaco Editor (the same core engine powering VS Code) and clicks "Submit".
2. **Frontend to Backend (NSwag):** 
   The auto-generated Angular NSwag client bundles the code into a JSON payload and dispatches an HTTP POST request to the .NET Backend API.
3. **Backend Processing (MediatR & Clean Architecture):** 
   The .NET Web API receives the request. Following the CQRS pattern via **MediatR**, the API immediately routes the payload to the EvaluateSubmissionCommand.
4. **Real-Time Updates (SignalR WebSockets):** 
   As the evaluation begins, the backend's **SignalR Hub** utilizes a persistent WebSocket connection to push live status messages ("Compiling...", "Analyzing with AI...") to the frontend, providing a seamless user experience.
5. **AI Evaluation (Semantic Kernel to Groq):** 
   The command delegates the actual evaluation to the SemanticKernelJudgeService. Semantic Kernel wraps the user's code inside a highly specific system prompt and transmits it to the **Groq API** (running Llama 3) for lightning-fast inference.
6. **Persistence (EF Core to SQLite):** 
   Upon receiving the AI's response, the C# backend parses the final verdict (Pass/Fail) and Time/Space complexities. It then instructs **Entity Framework (EF) Core** to persist the result. EF Core generates the necessary SQL queries and commits the record to the **SQLite Database** (CodeArena.db).
7. **Final UI Update:** 
   The backend returns the final HTTP success response to the frontend, updating the user's dashboard with the final verdict and performance metrics.

This enterprise-grade architecture ensures that the frontend, backend, database, and external services are strictly decoupled, allowing for high scalability and maintainability.
