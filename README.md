# CQRS

Overview:
This project is a Purchase Order Management System implementing the CQRS pattern. The system handles the bulk validation and insertion of 30k-50k purchase order records from an Excel file into a SQL Server database using different bulk insert approaches and explored the best response time.

Features
- CQRS Pattern for separation of concerns.
- Bulk Validation & Bulk Insertion.
- Multiple Bulk Insert Methods:
  - LINQ
  - Table-Valued Parameters (TVP)
  - SqlBulkCopy
  - Dapper
  - Dapper Plus

Technologies Used
- .NET 8
- C#
- Entity Framework Core (for initial data operations)
- Dapper / Dapper Plus (for optimized bulk insert)
- SQL Server
- FluentValidation (for validation)


Future Enhancements
- Improve error handling & logging.
- Implement authentication & authorization.


