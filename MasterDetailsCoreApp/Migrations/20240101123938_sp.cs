using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDetailsCoreApp.Migrations
{
    /// <inheritdoc />
    public partial class sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string spInsertCustomer = @"CREATE or ALTER PROCEDURE dbo.spInsertCustomer 
    @Name nvarchar(50),
    @Address nvarchar(100), 
	@ContactNo nvarchar(max),
	@IsPermanent bit,
	@Image nvarchar(max)
AS  

INSERT INTO [dbo].[Customers]
           ([Name]
           ,[Address]
           ,[ContactNo]
           ,[IsPermanent]
           ,[Image])
     VALUES
           (@Name, @Address,	@ContactNo,	@IsPermanent, @Image )
return @@identity
		   
GO";
            migrationBuilder.Sql(spInsertCustomer);


            string spInsertInvoice = @"CREATE or ALTER PROCEDURE dbo.spInsertInvoice 
@InvoiceDate datetime2(7),
@Quantity int,
@ItemsId int,
@CustomerId int

AS    
INSERT INTO [dbo].[InvoiceItems]
           ([InvoiceDate]
           ,[Quantity]
           ,[ItemsId]
           ,[CustomerId])
     VALUES
           (@InvoiceDate, @Quantity, @ItemsId, @CustomerId)
        return @@rowcount
GO";
            migrationBuilder.Sql(spInsertInvoice);


            string spUpdateCustomer = @"CREATE or ALTER PROCEDURE dbo.spUpdateCustomer 
  @CustomerId int,
  @Name nvarchar(50),
  @Address nvarchar(100),
  @ContactNo nvarchar(max),
  @IsPermanent bit,
  @Image nvarchar(max)


AS  

UPDATE [dbo].[Customers]
   SET [Name] = @Name
      ,[Address] = @Address
      ,[ContactNo] = @ContactNo
      ,[IsPermanent] = @IsPermanent
      ,[Image] = @Image
 WHERE Id = @CustomerId

 delete from InvoiceItems where CustomerId = @CustomerId
return @@rowcount
GO";
            migrationBuilder.Sql(spUpdateCustomer);


            string spDeleteCustomer = @"CREATE or ALTER PROCEDURE dbo.spDeleteCustomer 
   @CustomerId int
AS
DELETE FROM [dbo].InvoiceItems WHERE CustomerId = @CustomerId
DELETE FROM [dbo].[Customers] WHERE Id = @CustomerId
return @@rowcount
GO";
            migrationBuilder.Sql(spDeleteCustomer);

        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop proc spInsertCustomer");
            migrationBuilder.Sql("drop proc spInsertInvoice");
            migrationBuilder.Sql("drop proc spUpdateCustomer");
            migrationBuilder.Sql("drop proc spDeleteCustomer");
        }
    }
}
