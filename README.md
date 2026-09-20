# CodeArena

Welcome to **CodeArena**, a modern, AI-powered coding platform designed for competitive programming and algorithm practice.

## Features

- **AI-Powered Judging**: Native integration with Large Language Models (LLMs) to intelligently analyze, test, and provide feedback on code submissions (Time & Space Complexity) in real-time.
- **Modern Workspace**: Built with Angular 18, featuring Monaco Editor integration for a true VS Code-like coding experience directly in the browser.
- **Real-Time Processing**: Powered by SignalR WebSockets for live evaluation status updates ("Compiling", "Testing", "Analyzing").
- **Clean Architecture**: Enterprise-grade .NET 10 Web API backend structured with Domain-Driven Design (DDD) and CQRS (MediatR).
- **Dockerized**: Fully containerized for simple one-command setup using Docker Compose.

## Getting Started

### Prerequisites
- Docker Desktop

### Run the Application

To launch CodeArena with all its services (Frontend, Backend API, SQLite Database), simply run:

`ash
docker compose up --build -d
`

Once the container is up and running, open your browser and navigate to the local environment URL provided in the Docker console.
