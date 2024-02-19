namespace CinemeBooking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ghe",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        MaGhe = c.String(maxLength: 50),
                        GiaGhe = c.Decimal(storeType: "money"),
                        TrangThai = c.Int(nullable: false),
                        isCouple = c.Boolean(),
                        PhongChieu_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.PhongChieu", t => t.PhongChieu_id)
                .Index(t => t.PhongChieu_id);
            
            CreateTable(
                "dbo.PhongChieu",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TenPhong = c.String(nullable: false, maxLength: 100),
                        SoChoNgoi = c.Int(nullable: false),
                        TinhTrang = c.Boolean(nullable: false),
                        LoaiManHinh_id = c.Int(),
                        RapPhim_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.LoaiManHinh", t => t.LoaiManHinh_id)
                .ForeignKey("dbo.RapPhim", t => t.RapPhim_id)
                .Index(t => t.LoaiManHinh_id)
                .Index(t => t.RapPhim_id);
            
            CreateTable(
                "dbo.LichChieu",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        NgayChieu = c.DateTime(nullable: false, storeType: "date"),
                        GioBatDau = c.String(nullable: false, maxLength: 50),
                        PhuPhi = c.Decimal(nullable: false, storeType: "money"),
                        TrangThai = c.Boolean(nullable: false),
                        Phim_id = c.Int(),
                        PhongChieu_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Phim", t => t.Phim_id)
                .ForeignKey("dbo.PhongChieu", t => t.PhongChieu_id)
                .Index(t => t.Phim_id)
                .Index(t => t.PhongChieu_id);
            
            CreateTable(
                "dbo.Phim",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TenPhim = c.String(nullable: false, maxLength: 100),
                        MoTa = c.String(maxLength: 1000),
                        ThoiLuong = c.Int(nullable: false),
                        NgayKhoiChieu = c.DateTime(nullable: false, storeType: "date"),
                        NgayKetThuc = c.DateTime(nullable: false, storeType: "date"),
                        QuocGia = c.String(nullable: false, maxLength: 50),
                        DaoDien = c.String(nullable: false, maxLength: 100),
                        DienVien = c.String(),
                        NamSX = c.Int(nullable: false),
                        ApPhich = c.String(),
                        TrangThai = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.TheLoai",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TenTheLoai = c.String(nullable: false, maxLength: 100),
                        MoTa = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Ve",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        LoaiVe = c.Int(nullable: false),
                        idTaiKhoan = c.String(maxLength: 128),
                        NgayDat = c.DateTime(nullable: false),
                        TienBanVe = c.Decimal(nullable: false, storeType: "money"),
                        Ghe_id = c.Int(),
                        LichChieu_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ghe", t => t.Ghe_id)
                .ForeignKey("dbo.LichChieu", t => t.LichChieu_id)
                .Index(t => t.Ghe_id)
                .Index(t => t.LichChieu_id);
            
            CreateTable(
                "dbo.LoaiManHinh",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TenMH = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.RapPhim",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        TenRap = c.String(),
                        TongSoPhong = c.Int(),
                        ThanhPho = c.String(),
                        QuanHuyen = c.String(),
                        PhuongXa = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Phone = c.String(),
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
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
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
                "dbo.TheLoaiPhims",
                c => new
                    {
                        TheLoai_id = c.Int(nullable: false),
                        Phim_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TheLoai_id, t.Phim_id })
                .ForeignKey("dbo.TheLoai", t => t.TheLoai_id, cascadeDelete: true)
                .ForeignKey("dbo.Phim", t => t.Phim_id, cascadeDelete: true)
                .Index(t => t.TheLoai_id)
                .Index(t => t.Phim_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PhongChieu", "RapPhim_id", "dbo.RapPhim");
            DropForeignKey("dbo.PhongChieu", "LoaiManHinh_id", "dbo.LoaiManHinh");
            DropForeignKey("dbo.Ve", "LichChieu_id", "dbo.LichChieu");
            DropForeignKey("dbo.Ve", "Ghe_id", "dbo.Ghe");
            DropForeignKey("dbo.LichChieu", "PhongChieu_id", "dbo.PhongChieu");
            DropForeignKey("dbo.TheLoaiPhims", "Phim_id", "dbo.Phim");
            DropForeignKey("dbo.TheLoaiPhims", "TheLoai_id", "dbo.TheLoai");
            DropForeignKey("dbo.LichChieu", "Phim_id", "dbo.Phim");
            DropForeignKey("dbo.Ghe", "PhongChieu_id", "dbo.PhongChieu");
            DropIndex("dbo.TheLoaiPhims", new[] { "Phim_id" });
            DropIndex("dbo.TheLoaiPhims", new[] { "TheLoai_id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Ve", new[] { "LichChieu_id" });
            DropIndex("dbo.Ve", new[] { "Ghe_id" });
            DropIndex("dbo.LichChieu", new[] { "PhongChieu_id" });
            DropIndex("dbo.LichChieu", new[] { "Phim_id" });
            DropIndex("dbo.PhongChieu", new[] { "RapPhim_id" });
            DropIndex("dbo.PhongChieu", new[] { "LoaiManHinh_id" });
            DropIndex("dbo.Ghe", new[] { "PhongChieu_id" });
            DropTable("dbo.TheLoaiPhims");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RapPhim");
            DropTable("dbo.LoaiManHinh");
            DropTable("dbo.Ve");
            DropTable("dbo.TheLoai");
            DropTable("dbo.Phim");
            DropTable("dbo.LichChieu");
            DropTable("dbo.PhongChieu");
            DropTable("dbo.Ghe");
        }
    }
}
