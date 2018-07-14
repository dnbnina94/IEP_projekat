namespace UserTablesPrimer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Name = c.String(),
                        Surname = c.String(),
                        Tokens = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Message = c.String(),
                        UserId = c.String(maxLength: 128),
                        Read = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Auction",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        Img = c.Binary(),
                        Length = c.Int(nullable: false),
                        StartPrice = c.Single(nullable: false),
                        CurrentPrice = c.Single(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        OpenDate = c.DateTime(),
                        CloseDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        CurrencyId = c.String(maxLength: 128),
                        WinnerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.WinnerId)
                .Index(t => t.UserId)
                .Index(t => t.CurrencyId)
                .Index(t => t.WinnerId);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Bid",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(maxLength: 128),
                        AuctionId = c.String(maxLength: 128),
                        Bids = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auction", t => t.AuctionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(maxLength: 128),
                        Tokens = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                        Status = c.Int(nullable: false),
                        CurrencyId = c.String(maxLength: 128),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CurrencyId);
            
            CreateTable(
                "dbo.Parameters",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Order", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Order", "CurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.Bid", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Bid", "AuctionId", "dbo.Auction");
            DropForeignKey("dbo.Auction", "WinnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Auction", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Auction", "CurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Notification", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.Order", new[] { "CurrencyId" });
            DropIndex("dbo.Order", new[] { "UserId" });
            DropIndex("dbo.Bid", new[] { "AuctionId" });
            DropIndex("dbo.Bid", new[] { "UserId" });
            DropIndex("dbo.Auction", new[] { "WinnerId" });
            DropIndex("dbo.Auction", new[] { "CurrencyId" });
            DropIndex("dbo.Auction", new[] { "UserId" });
            DropIndex("dbo.Notification", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Parameters");
            DropTable("dbo.Order");
            DropTable("dbo.Bid");
            DropTable("dbo.Currencies");
            DropTable("dbo.Auction");
            DropTable("dbo.Notification");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
        }
    }
}
