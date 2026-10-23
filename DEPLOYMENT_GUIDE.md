# CA Chartered Campus — Production Deployment & Go-Live Guide

This document outlines the applied fixes and instructions to launch the **CA Chartered Campus (CAPortal)** website publicly.

---

## 🚀 Summary of Fixes Applied

1. **Security & Package CVE Patches:**
   - Upgraded EntityFrameworkCore packages to `8.0.11`, completely resolving high-severity vulnerabilities (`Azure.Identity`, `Microsoft.Data.SqlClient`).
   - Project now builds with **0 errors and 0 warnings**.

2. **Database-Driven Admin Authentication:**
   - Moved admin user management into the database (`AdminUsers` table).
   - Removed hardcoded credentials dependency from `AdminAuthService.cs`.
   - Added `ChangePassword` API in `ProfileController` to allow admins to safely update credentials anytime.
   - Initial seed now sets credentials via configuration (`AdminCredentials:DefaultUsername` / `AdminCredentials:DefaultPassword`) or environment variables.

3. **Email Notification Service (`IEmailService`):**
   - Implemented `SmtpEmailService` supporting standard SMTP providers (SendGrid, Brevo, AWS SES, Gmail).
   - Graceful fallback: If SMTP is not configured, logs locally without breaking client checkout or form submissions.
   - Integrated with:
     - Contact Form inquiries (`/contact/submit`)
     - Expert CA consultation bookings (`/expert/book`)
     - Service page consultation requests (`/Home/SubmitClientRequest` and `/Home/PostConsult`)
     - Newsletter subscriptions (`/newsletter/subscribe`)

4. **Missing Endpoints Implemented (Fixed Silent Failures):**
   - Implemented `/Home/SubmitClientRequest` (used by the Service Detail modal).
   - Implemented `/Home/PostConsult` (used by the sidebar callback request form).
   - Leads are now reliably stored in `ClientRequests` and trigger email alerts.

5. **Spam & Bot Protection:**
   - Added silent honeypot fields to public contact and consultation forms. Bots filling hidden fields are immediately neutralized without bothering genuine users.

6. **Dynamic XML Sitemap (`/sitemap.xml`):**
   - Upgraded `/sitemap.xml` to dynamically query and output XML entries for:
     - All published blog articles (`/blog/{slug}`)
     - All active services (`/service/{id}`)
     - All legal & information pages (`/page/{slug}`)
     - Core index and directory pages

7. **Comprehensive Legal & Compliance Pages:**
   - Replaced placeholder 2-line text with Indian-law compliant policies in `DbInitializer`:
     - **About Us** (`/page/about-us`)
     - **Terms & Conditions** (`/page/terms-and-conditions`)
     - **Privacy Policy** (`/page/privacy-policy`)
     - **Cancellation & Refund Policy** (`/page/cancellation-and-refund-policy`)
     - **Contact Us** (`/page/contact-us`)
   *(Required by payment gateways such as Razorpay, Cashfree, or PhonePe for merchant KYC approval).*

---

## 🛠️ Step-by-Step Go-Live Checklist

### 1. Database Setup (Production)
Do **not** use free-tier Somee for public traffic. Recommended options:
- **Azure SQL Database** (Serverless tier costs ~$5/month).
- **AWS RDS SQL Server Express** (Free tier eligible).
- **Ubuntu/Windows VPS with SQL Server Developer/Express** ($5-10/month on Hetzner, DigitalOcean, or Hostinger).

Set the connection string via Environment Variable:
```bash
ConnectionStrings__DefaultConnection="Server=YOUR_SERVER;Database=cacampus;User Id=YOUR_USER;Password=YOUR_STRONG_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true;"
```

---

### 2. Configure Transactional Emails (SMTP)
Use **Brevo** (300 free emails/day) or **SendGrid** (100 free emails/day):
Set environment variables:
```bash
SmtpSettings__Host="smtp-relay.brevo.com"
SmtpSettings__Port="587"
SmtpSettings__Username="your-login@domain.com"
SmtpSettings__Password="your-smtp-master-key"
SmtpSettings__SenderEmail="support@yourdomain.com"
SmtpSettings__SenderName="CA Chartered Campus"
```

---

### 3. Publishing the Application
To publish for a production server (Self-Contained deployment so server doesn't need .NET 10 preview installed):

#### For Windows Server / IIS:
```powershell
dotnet publish -c Release -r win-x64 --self-contained -o ./bin/Release/publish
```

#### For Linux (Ubuntu + Nginx + Systemd):
```powershell
dotnet publish -c Release -r linux-x64 --self-contained -o ./bin/Release/publish-linux
```

---

### 4. Domain & SSL Setup
1. Point your domain (e.g. `cacampus.in`) DNS `A` records to your server IP.
2. Route DNS through **Cloudflare** for:
   - Free SSL/TLS (HTTPS)
   - DDoS protection & bot filtering
   - Edge caching of static CSS, JS, and SVG files
3. In Cloudflare SSL settings, choose **Full (Strict)**.

---

### 5. Payment Gateway Integration (When Ready)
To accept direct online payments for consultation fees (₹499 - ₹1,099) and packages:
1. Register on **Razorpay** or **Cashfree**.
2. Submit your business KYC along with website links (`/page/terms-and-conditions`, `/page/privacy-policy`, `/page/cancellation-and-refund-policy`).
3. Add Razorpay Checkout script to the booking modal and verify signature in a webhook.
