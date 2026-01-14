# Hotel Booking System (Service-Oriented / Microservices)

This project is a service-oriented hotel booking system built with separate services and an API Gateway.
It includes a simple Client UI and Admin UI, and uses Azure SQL + Azure Service Bus for messaging.

---

## Architecture (High Level)

- **API Gateway** (YARP Reverse Proxy)
  - Routes:
    - `/hotel/*` → HotelService
    - `/notify/*` → NotificationService

- **HotelService**
  - Hotel search, availability, booking, pricing prediction
  - Publishes `reservation-created` messages to **Azure Service Bus**

- **NotificationService**
  - Consumes messages from **Azure Service Bus**
  - Exposes notification APIs

- **Database**
  - **Azure SQL Database** (SQLite is NOT used)

- **Queue / Messaging**
  - **Azure Service Bus Queue**

- **Scheduling**
  - **Azure Logic App** (scheduled calls via API Gateway)

- **UIs**
  - `hotel-client-ui` (Client UI)
  - `hotel-admin-ui` (Admin UI)

---

## Deployed Endpoints

> Update these URLs if re-deployed.

- **API Gateway**
  - `https://hotelbooking-gateway-unique-d4h8fffjgxa8fqet.francecentral-01.azurewebsites.net`

- **Gateway Health**
  - `https://hotelbooking-gateway-unique-d4h8fffjgxa8fqet.francecentral-01.azurewebsites.net/health`

- **Hotel APIs (via Gateway)**
  - `https://hotelbooking-gateway-unique-d4h8fffjgxa8fqet.francecentral-01.azurewebsites.net/hotel/api/v1/...`

- **Notification APIs (via Gateway)**
  - `https://hotelbooking-gateway-unique-d4h8fffjgxa8fqet.francecentral-01.azurewebsites.net/notify/api/v1/...`

- **Client UI**
  - `https://client-ui-evakced2ekgvdacf.francecentral-01.azurewebsites.net/`

- **Admin UI**
  - `https://admin-ui-gddpathveuewesg4.francecentral-01.azurewebsites.net/`

---

## Local Run (Quick Guide)

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- npm

---

### Backend Services (Local)

Run each service separately:
- API Gateway
- HotelService
- NotificationService

---

### Client UI

cd ui/hotel-client-ui
npm install
npm run dev

Admin UI

cd ui/hotel-admin-ui
npm install
npm run dev

Configuration (IMPORTANT)
AppSettings

⚠️ Real configuration files are NOT committed to Git

Each service uses:

    appsettings.example.json (committed)

    Real values are provided via Azure App Service → Configuration

Required Environment Variables
HotelService

    ConnectionStrings__DefaultConnection → Azure SQL connection string

    SERVICEBUS_CONNECTION → Azure Service Bus connection string

    SERVICEBUS_QUEUE_NAME → reservation-created

    NOTIFY_BASE_URL → https://hotelbooking-gateway-unique-d4h8fffjgxa8fqet.francecentral-01.azurewebsites.net/notify

NotificationService

    ConnectionStrings__DefaultConnection → Azure SQL connection string (if used)

    SERVICEBUS_CONNECTION → Azure Service Bus connection string

    SERVICEBUS_QUEUE_NAME → reservation-created

API Gateway

    Uses appsettings.json for reverse proxy routing

Client / Admin UI

    Admin: https://admin-ui-gddpathveuewesg4.francecentral-01.azurewebsites.net/
    Client: https://client-ui-evakced2ekgvdacf.francecentral-01.azurewebsites.net/

Messaging (Queue)

    Queue Name: reservation-created

    Producer: HotelService

    Consumer: NotificationService (QueueConsumerWorker)

    Implemented using Azure Service Bus

    Async messaging (NOT synchronous HTTP)

Scheduling

    Implemented with Azure Logic App

    Logic App triggers:

        HTTP calls to Notification endpoints via API Gateway

    Used to demonstrate scheduled cloud-based orchestration

Docker

    Dockerfiles are included for:

        API Gateway

        HotelService

        NotificationService

    ❌ Docker images are NOT committed

    Dockerfiles are provided to satisfy deployment requirements

Assumptions & Notes

    IAM (AWS Cognito / Azure AD) was NOT implemented due to time and risk constraints

    Simple authentication is simulated where needed

    Notification POST endpoint exists but is primarily used internally via queue

    Pagination is implemented where applicable (e.g., hotel search)

    In-memory cache is used for performance optimization

Technologies Used

    ASP.NET Core 8

    Azure App Services

    Azure SQL Database

    Azure Service Bus

    Azure Logic Apps

    YARP Reverse Proxy

    React + Vite

    Docker

Notes for Instructor

    All services are deployed separately

    All APIs are accessed through API Gateway

    Queue-based async communication is implemented

    Cloud database is used (SQLite is NOT used)

    Simple UI is provided as required
