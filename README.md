# 📦 .NET Web API for Invoice & Customer Management

This project is a lightweight **.NET Web API** built for managing **Invoices** and **Customers**, with a focus on **creating invoices** and performing **accounting operations**. It also includes **authentication & authorization** for secure access, and supports **file exports** in **PDF**, **Excel**, and other formats. The application uses **MongoDB** for data persistence.

---

## 🚀 Features

- 🔐 **Authentication & Authorization** implemented on all endpoints.
- 👥 Full **CRUD operations** for **Customer Management**.
- 🧾 **Invoice Creation** with built-in **accounting logic**.
- 📤 **Export capabilities**:
  - Export invoice/customer data to **PDF**
  - Export to **Excel (.xlsx)**
  - General file export (e.g. CSV, JSON)
- 📦 Built with **.NET Web API** and **MongoDB**.

---

## 🧰 Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | [.NET 8 Web API](https://dotnet.microsoft.com/) |
| Database | [MongoDB](https://www.mongodb.com/) |
| Auth | JWT-based Authentication and Role-based Authorization |
| Export Libraries | iTextSharp / EPPlus / ClosedXML (based on your choice for PDF/Excel export) |

---

## 🔐 Authentication & Authorization

- Secured using **JWT (JSON Web Tokens)**.
- All APIs require a valid token to access.
- **Role-based Authorization** can be applied to different endpoints if needed.
- Login/Register endpoints included for token generation.

---

## 📁 Modules Overview

### 🧾 Invoice Module
- **Create invoices** with accounting-related logic:
  - Subtotal, Tax, Discounts, Grand Total.
- Export invoices as:
  - **PDF** for printing or sharing.
  - **Excel** for record-keeping.
- (Note: Other CRUD operations like Update/Delete are not implemented.)

### 👥 Customer Module
- Create, Read customers.
- Link customers with invoices.
- Retrive customer for dropdown

---

## 📤 File Export Support

- PDF: Generate detailed invoice or customer reports using libraries like iTextSharp or DinkToPdf.
- Excel: Export lists or individual records with styles using EPPlus or ClosedXML.
- Generic file export: CSV or JSON download for external integrations or backup.

---

## 📦 Getting Started

### Prerequisites
- .NET 8
- MongoDB (local or Atlas)
- Visual Studio or VS Code

