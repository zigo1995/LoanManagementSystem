# نظام إدارة القروض الاحترافي
## Professional Loan Management System

### 🎯 مميزات النظام

✅ **إدارة العملاء** - Customer Management
✅ **إدارة القروض** - Loan Management
✅ **إدارة الأقساط** - Installment Schedule
✅ **معالجة الدفعات** - Payment Processing
✅ **إدارة الضمانات** - Guarantor Management
✅ **حساب العقوبات** - Late Payment Penalties
✅ **إدارة الخزينة** - Treasury Management
✅ **نظام الأدوار والصلاحيات** - RBAC
✅ **التدقيق والسجلات** - Audit Logging
✅ **التقارير والطباعة** - Reports & Printing
✅ **دعم كامل للعربية** - Full Arabic RTL Support
✅ **مظاهر احترافية** - Professional Themes

---

### 💻 المتطلبات

- Windows 10/11 أو أحدث
- .NET 8 SDK
- SQL Server 2019 أو أحدث
- Visual Studio 2022 Community

---

### 🚀 خطوات الإعداد

#### 1. تثبيت .NET 8
```bash
https://dotnet.microsoft.com/download
```

#### 2. تثبيت SQL Server
```bash
https://www.microsoft.com/sql-server
```

#### 3. استنساخ المستودع
```bash
git clone https://github.com/yourusername/LoanManagementSystem.git
cd LoanManagementSystem
```

#### 4. تشغيل قاعدة البيانات
```bash
# افتح SQL Server Management Studio
# شغّل Scripts في المجلد database/:
# 001_CreateDatabase.sql
# 002_SeedData.sql
```

#### 5. استعادة الحزم
```bash
dotnet restore
```

#### 6. تشغيل البرنامج
```bash
cd src/LoanManagementSystem.Presentation
dotnet run
```

---

### 🔑 بيانات الدخول

**Username:** `admin`
**Password:** `Admin@123`

---

### 🏗️ هيكل المشروع

```
LoanManagementSystem/
├── src/
│   ├── LoanManagementSystem.Domain/
│   ├── LoanManagementSystem.Application/
│   ├── LoanManagementSystem.Infrastructure/
│   └── LoanManagementSystem.Presentation/
├── database/
│   ├── 001_CreateDatabase.sql
│   └── 002_SeedData.sql
└── README.md
```

---

### 📚 التكنولوجيا المستخدمة

- **Language:** C# 12
- **Framework:** .NET 8
- **UI:** Windows Forms + Guna UI2
- **Database:** SQL Server
- **ORM:** Entity Framework Core 8
- **Security:** BCrypt.Net
- **Reporting:** ClosedXML

---

### 📄 الترخيص

MIT License

---

### 👨‍💻 المطورون

فريق تطوير نظام إدارة القروض

---

### 💬 الدعم

للدعم والاستفسارات:
📧 support@loansystem.com
