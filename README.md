# ⚖️ LEX — Legal Services Platform API

LEX is an ASP.NET Core Web API that connects **customers** with **lawyers**. Customers browse verified lawyers and send them consultation or case requests; lawyers review and respond; and an **admin** vets every lawyer by reviewing their uploaded credentials before they go live.

Built with **C# · .NET 8 · Entity Framework Core · SQL Server · JWT**, organized in separate layers.

---

## ✨ Features

- 🔐 **JWT authentication** with three roles: `Admin`, `Customer`, `Lawyer`
- 📧 **Email OTP verification**, plus forgot/reset password flow
- 🪪 **Lawyer onboarding** — lawyers upload credentials (passport, license, syndicate card, graduation certificate, criminal record); an admin accepts or rejects them, with a rejection reason, and lawyers can **resubmit documents**
- 🔎 **Lawyer directory** — filter by specialization or search, view lawyer details
- 📂 **Case requests** — customers send a *Consultation* or *Case* request to a lawyer; the lawyer accepts or rejects it (`Pending → Accepted / Rejected → Completed`)
- ⭐ **Reviews** — customers add, update and delete reviews of lawyers
- 🔔 **Notifications** for status changes
- 📊 **Admin dashboard** and pending-lawyers queue
- 🗂️ **File uploads** for profile images and documents
- 🧪 **Swagger UI** with Bearer-token support (Development)

---

## 🏗️ Architecture

```
LEXSolution.sln
├── LEX.Domain               # Entities, enums, repository / unit-of-work contracts
├── LEX.Persistence          # EF Core DbContext, configurations, migrations, seeding, repositories
├── LEX.Service Abstraction  # Service interfaces
├── LEX.Service              # Business logic (auth, cases, lawyers, reviews, notifications, admin)
├── LEX.Shared               # DTOs and the ServiceResponse wrapper
└── LEX.Web                  # Controllers, Program.cs (composition root)
```

Patterns used: **Repository + Unit of Work**, **DTO mapping with AutoMapper**, **service layer behind interfaces**, **dependency injection**, password hashing with **BCrypt**.

---

## 🔌 API Overview

| Area | Base route | Access |
|---|---|---|
| Authentication | `/api/authentication` — `register`, `register-documents`, `login`, `verify-otp`, `forgot-password`, `reset-password`, `check-status` | Public |
| Lawyers | `/api/lawyers` — list/filter, `Details/{id}`, `categories` | Public |
| Case requests | `/api/caserequests` — create, my cases, lawyer inbox, handle (accept/reject), details | Customer / Lawyer |
| Reviews | `/api/reviews` — `Add`, `Update`, `Delete/{id}` | Customer |
| Documents | `/api/documents` — `my`, update, delete, `resubmit-documents` | Lawyer |
| Users | `/api/users/profile` — get / update | Authenticated |
| Notifications | `/api/notifications/my-notification` | Authenticated |
| Admin | `/api/admin` — `pending-lawyers`, `accept/{id}`, `reject/{id}`, `lawyer/{id}`, `dashboard` | Admin |

Full, interactive documentation is available in Swagger when running locally.

---

## 🚀 Getting Started

**Prerequisites:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and SQL Server (LocalDB works).

```bash
git clone https://github.com/makariosfaiz123/LEX.git
cd LEX

# apply migrations (creates the LEX database)
dotnet ef database update --project LEX.Persistence --startup-project LEX.Web

# run the API
dotnet run --project LEX.Web
```

Then open `/swagger` on the URL printed in the console.

- The connection string lives in `LEX.Web/appsettings.Development.json` (defaults to `(localdb)\MSSQLLocalDB`).
- Sample customers, lawyers, documents, case requests and reviews are seeded on startup, and an admin account is created on first run (see `LEX.Persistence/Data/DataSeed`).
- Outgoing email (OTP / password reset) is sent via SMTP from `LEX.Service/Authentication/EmailService.cs`.

---

## 🗺️ Roadmap

- Move secrets (JWT signing key, SMTP credentials) out of source into configuration / user-secrets
- Unit and integration tests
- Dockerfile and CI pipeline

---

## 👤 Author

**Makarios Faiz Fawzy** — Full Stack Developer (.NET | React)
[LinkedIn](https://www.linkedin.com/in/makarios-faiz-157750278/) · [GitHub](https://github.com/makariosfaiz123)
