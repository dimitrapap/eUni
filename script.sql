USE [master]
GO
/****** Object:  Database [eUniDB]    Script Date: 16-Jun-24 12:58:05 PM ******/
CREATE DATABASE [eUniDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'eUniDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\eUniDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'eUniDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\eUniDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [eUniDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [eUniDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [eUniDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [eUniDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [eUniDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [eUniDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [eUniDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [eUniDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [eUniDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [eUniDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [eUniDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [eUniDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [eUniDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [eUniDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [eUniDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [eUniDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [eUniDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [eUniDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [eUniDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [eUniDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [eUniDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [eUniDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [eUniDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [eUniDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [eUniDB] SET RECOVERY FULL 
GO
ALTER DATABASE [eUniDB] SET  MULTI_USER 
GO
ALTER DATABASE [eUniDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [eUniDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [eUniDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [eUniDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [eUniDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [eUniDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'eUniDB', N'ON'
GO
ALTER DATABASE [eUniDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [eUniDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [eUniDB]
GO
/****** Object:  Table [dbo].[Announcements]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Announcements](
	[AnnouncementId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Author] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AnnouncementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachments](
	[AttachmentId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[ContentType] [nvarchar](100) NOT NULL,
	[CourseId] [uniqueidentifier] NULL,
	[FileName] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseAnnouncement]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseAnnouncement](
	[CourseAnnouncementId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[CourseId] [uniqueidentifier] NULL,
	[CreatedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CourseAnnouncementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[CourseId] [uniqueidentifier] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[TeachersId] [uniqueidentifier] NOT NULL,
	[Semester] [int] NOT NULL,
	[Base] [int] NOT NULL,
	[CodeCourse] [nvarchar](250) NOT NULL,
	[CourseType] [nvarchar](250) NOT NULL,
	[Department] [nvarchar](250) NOT NULL,
	[Credits] [int] NOT NULL,
	[CourseDetails] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DilwmenaCourses]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DilwmenaCourses](
	[StudentsUserId] [uniqueidentifier] NOT NULL,
	[CourseId] [uniqueidentifier] NOT NULL,
	[Grade] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestReports]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestReports](
	[RequestId] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ApplicantsRN] [nvarchar](255) NOT NULL,
	[Completed] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentsId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PhoneNumber] [nvarchar](255) NOT NULL,
	[RegistrationNumber] [nvarchar](255) NOT NULL,
	[FathersName] [nvarchar](255) NOT NULL,
	[MothersName] [nvarchar](255) NOT NULL,
	[YearOfAdmission] [int] NOT NULL,
	[CurrentSemester] [int] NOT NULL,
	[Department] [nvarchar](255) NOT NULL,
	[Specialization] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[StudentsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teachers]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teachers](
	[TeachersId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PhoneNumber] [nvarchar](255) NOT NULL,
	[RegistrationNumber] [nvarchar](255) NOT NULL,
	[FathersName] [nvarchar](255) NOT NULL,
	[MothersName] [nvarchar](255) NOT NULL,
	[YearOfAdmission] [int] NOT NULL,
	[Department] [nvarchar](255) NOT NULL,
	[NumberOfCourses] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TeachersId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 16-Jun-24 12:58:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[PhoneNumber] [nvarchar](255) NOT NULL,
	[RegistrationNumber] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Role] [nvarchar](255) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Attachments]  WITH CHECK ADD FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[CourseAnnouncement]  WITH CHECK ADD FOREIGN KEY([CourseId])
REFERENCES [dbo].[Courses] ([CourseId])
GO
ALTER TABLE [dbo].[Courses]  WITH CHECK ADD FOREIGN KEY([TeachersId])
REFERENCES [dbo].[Teachers] ([TeachersId])
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Teachers]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
USE [master]
GO
ALTER DATABASE [eUniDB] SET  READ_WRITE 
GO
