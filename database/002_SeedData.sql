USE LoanManagementDB;
GO

-- Insert Roles
INSERT INTO [dbo].[Roles] ([RoleName], [Description], [IsActive])
VALUES 
    ('Admin', 'System Administrator', 1),
    ('Manager', 'Loan Manager', 1),
    ('Officer', 'Loan Officer', 1),
    ('Accountant', 'Accountant', 1),
    ('Viewer', 'View Only Access', 1);

-- Insert Permissions
INSERT INTO [dbo].[Permissions] ([PermissionCode], [PermissionName], [Description], [IsActive])
VALUES 
    ('USER_MANAGE', 'Manage Users', 'Create, Edit, Delete Users', 1),
    ('CUSTOMER_MANAGE', 'Manage Customers', 'Create, Edit, Delete Customers', 1),
    ('LOAN_CREATE', 'Create Loans', 'Create New Loans', 1),
    ('LOAN_EDIT', 'Edit Loans', 'Edit Existing Loans', 1),
    ('LOAN_DELETE', 'Delete Loans', 'Delete Loans', 1),
    ('PAYMENT_PROCESS', 'Process Payments', 'Process Loan Payments', 1),
    ('REPORT_VIEW', 'View Reports', 'View Financial Reports', 1),
    ('AUDIT_VIEW', 'View Audit Logs', 'View System Audit Logs', 1),
    ('BACKUP_MANAGE', 'Manage Backups', 'Create and Restore Backups', 1);

-- Insert Admin User (Password: Admin@123)
INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [FullName], [RoleId], [IsActive], [CreatedBy])
VALUES 
    ('admin', 'admin@loansystem.com', '$2a$12$R9h7cIPz0gi.URNNX3kh2OPST9/PgBkqquzi.Ee3j7eLZNqDH23f.', 'System Administrator', 1, 1, 'System');

GO