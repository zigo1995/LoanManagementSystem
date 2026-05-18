-- Create Database
CREATE DATABASE IF NOT EXISTS LoanManagementDB;
GO

USE LoanManagementDB;
GO

-- Users Table
CREATE TABLE [dbo].[Users] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [FullName] NVARCHAR(100) NOT NULL,
    [RoleId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastLogin] DATETIME NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(50) NULL,
    [UpdatedBy] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- Roles Table
CREATE TABLE [dbo].[Roles] (
    [RoleId] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleName] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(200) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
);

-- Permissions Table
CREATE TABLE [dbo].[Permissions] (
    [PermissionId] INT IDENTITY(1,1) PRIMARY KEY,
    [PermissionCode] NVARCHAR(50) NOT NULL UNIQUE,
    [PermissionName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(200) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1
);

-- Role Permissions Table
CREATE TABLE [dbo].[RolePermissions] (
    [RolePermissionId] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleId] INT NOT NULL,
    [PermissionId] INT NOT NULL,
    FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([RoleId]) ON DELETE CASCADE,
    FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([PermissionId]) ON DELETE CASCADE,
    UNIQUE([RoleId], [PermissionId])
);

-- Customers Table
CREATE TABLE [dbo].[Customers] (
    [CustomerId] INT IDENTITY(1,1) PRIMARY KEY,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [NationalId] NVARCHAR(20) NOT NULL UNIQUE,
    [PhoneNumber] NVARCHAR(20) NOT NULL,
    [Email] NVARCHAR(100) NULL,
    [Address] NVARCHAR(200) NOT NULL,
    [City] NVARCHAR(50) NOT NULL,
    [Country] NVARCHAR(50) NOT NULL,
    [DateOfBirth] DATE NULL,
    [Occupation] NVARCHAR(100) NULL,
    [MonthlyIncome] DECIMAL(12,2) NULL,
    [CreditScore] INT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(50) NOT NULL,
    [UpdatedBy] NVARCHAR(50) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0
);

-- Loans Table
CREATE TABLE [dbo].[Loans] (
    [LoanId] INT IDENTITY(1,1) PRIMARY KEY,
    [LoanNumber] NVARCHAR(20) NOT NULL UNIQUE,
    [CustomerId] INT NOT NULL,
    [LoanAmount] DECIMAL(12,2) NOT NULL,
    [InterestRate] DECIMAL(5,2) NOT NULL,
    [LoanTermMonths] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [LoanType] NVARCHAR(50) NOT NULL,
    [Purpose] NVARCHAR(200) NULL,
    [BalanceAmount] DECIMAL(12,2) NOT NULL,
    [TotalInterest] DECIMAL(12,2) NOT NULL,
    [TotalPaid] DECIMAL(12,2) NOT NULL DEFAULT 0,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] NVARCHAR(50) NOT NULL,
    [UpdatedBy] NVARCHAR(50) NULL,
    FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers]([CustomerId]) ON DELETE CASCADE
);

-- Installments Table
CREATE TABLE [dbo].[Installments] (
    [InstallmentId] INT IDENTITY(1,1) PRIMARY KEY,
    [LoanId] INT NOT NULL,
    [InstallmentNumber] INT NOT NULL,
    [DueDate] DATE NOT NULL,
    [AmountDue] DECIMAL(12,2) NOT NULL,
    [PrincipalAmount] DECIMAL(12,2) NOT NULL,
    [InterestAmount] DECIMAL(12,2) NOT NULL,
    [AmountPaid] DECIMAL(12,2) NOT NULL DEFAULT 0,
    [Status] NVARCHAR(20) NOT NULL,
    [PaidDate] DATE NULL,
    [LateCharges] DECIMAL(12,2) NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([LoanId]) REFERENCES [dbo].[Loans]([LoanId]) ON DELETE CASCADE,
    UNIQUE([LoanId], [InstallmentNumber])
);

-- Payments Table
CREATE TABLE [dbo].[Payments] (
    [PaymentId] INT IDENTITY(1,1) PRIMARY KEY,
    [InstallmentId] INT NOT NULL,
    [LoanId] INT NOT NULL,
    [PaymentDate] DATETIME NOT NULL,
    [Amount] DECIMAL(12,2) NOT NULL,
    [PaymentMethod] NVARCHAR(50) NOT NULL,
    [ReferenceNumber] NVARCHAR(100) NULL,
    [Notes] NVARCHAR(200) NULL,
    [ProcessedBy] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([InstallmentId]) REFERENCES [dbo].[Installments]([InstallmentId]),
    FOREIGN KEY ([LoanId]) REFERENCES [dbo].[Loans]([LoanId]) ON DELETE CASCADE
);

-- Guarantors Table
CREATE TABLE [dbo].[Guarantors] (
    [GuarantorId] INT IDENTITY(1,1) PRIMARY KEY,
    [LoanId] INT NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName] NVARCHAR(50) NOT NULL,
    [NationalId] NVARCHAR(20) NOT NULL,
    [PhoneNumber] NVARCHAR(20) NOT NULL,
    [Address] NVARCHAR(200) NOT NULL,
    [Relationship] NVARCHAR(50) NOT NULL,
    [GuaranteeAmount] DECIMAL(12,2) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([LoanId]) REFERENCES [dbo].[Loans]([LoanId]) ON DELETE CASCADE
);

-- Treasury Table
CREATE TABLE [dbo].[TreasuryTransactions] (
    [TreasuryId] INT IDENTITY(1,1) PRIMARY KEY,
    [TransactionDate] DATETIME NOT NULL,
    [TransactionType] NVARCHAR(20) NOT NULL,
    [Amount] DECIMAL(12,2) NOT NULL,
    [Description] NVARCHAR(200) NOT NULL,
    [RelatedLoanId] INT NULL,
    [RelatedPaymentId] INT NULL,
    [ProcessedBy] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([RelatedLoanId]) REFERENCES [dbo].[Loans]([LoanId]),
    FOREIGN KEY ([RelatedPaymentId]) REFERENCES [dbo].[Payments]([PaymentId])
);

-- Audit Logs Table
CREATE TABLE [dbo].[AuditLogs] (
    [AuditId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [TableName] NVARCHAR(50) NOT NULL,
    [RecordId] INT NOT NULL,
    [OldValues] NVARCHAR(MAX) NULL,
    [NewValues] NVARCHAR(MAX) NULL,
    [IPAddress] NVARCHAR(50) NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);

-- Late Payment Penalties Table
CREATE TABLE [dbo].[LatePaymentPenalties] (
    [PenaltyId] INT IDENTITY(1,1) PRIMARY KEY,
    [InstallmentId] INT NOT NULL,
    [PenaltyPercentage] DECIMAL(5,2) NOT NULL,
    [PenaltyAmount] DECIMAL(12,2) NOT NULL,
    [DaysOverdue] INT NOT NULL,
    [AppliedDate] DATETIME NOT NULL,
    [IsPaid] BIT NOT NULL DEFAULT 0,
    [PaidDate] DATETIME NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([InstallmentId]) REFERENCES [dbo].[Installments]([InstallmentId]) ON DELETE CASCADE
);

-- Create Indexes
CREATE INDEX [IX_Users_RoleId] ON [dbo].[Users]([RoleId]);
CREATE INDEX [IX_Loans_CustomerId] ON [dbo].[Loans]([CustomerId]);
CREATE INDEX [IX_Loans_Status] ON [dbo].[Loans]([Status]);
CREATE INDEX [IX_Installments_LoanId] ON [dbo].[Installments]([LoanId]);
CREATE INDEX [IX_Installments_Status] ON [dbo].[Installments]([Status]);
CREATE INDEX [IX_Payments_LoanId] ON [dbo].[Payments]([LoanId]);
CREATE INDEX [IX_Payments_PaymentDate] ON [dbo].[Payments]([PaymentDate]);
CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs]([UserId]);
CREATE INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs]([CreatedAt]);

GO