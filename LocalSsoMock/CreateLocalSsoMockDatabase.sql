USE [master]
GO
/****** Object:  Database [ValidationPortalSsoMock]    Script Date: 3/28/2021 4:28:25 PM ******/
CREATE DATABASE [ValidationPortalSsoMock]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ValidationPortalSsoMock', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ValidationPortalSsoMock.mdf' , SIZE = 3264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ValidationPortalSsoMock_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ValidationPortalSsoMock_log.ldf' , SIZE = 2624KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ValidationPortalSsoMock] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ValidationPortalSsoMock].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ValidationPortalSsoMock] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET ARITHABORT OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET RECOVERY FULL 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET  MULTI_USER 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ValidationPortalSsoMock] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [ValidationPortalSsoMock] SET DELAYED_DURABILITY = DISABLED 
GO
USE [ValidationPortalSsoMock]
GO
/****** Object:  User [IIS APPPOOL\DefaultAppPool]    Script Date: 3/28/2021 4:28:25 PM ******/
CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\defaultapppool] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\DefaultAppPool]
GO
/****** Object:  Schema [apps]    Script Date: 3/28/2021 4:28:25 PM ******/
CREATE SCHEMA [apps]
GO
/****** Object:  Table [apps].[App]    Script Date: 3/28/2021 4:28:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [apps].[App](
	[AppId] [varchar](64) NOT NULL,
	[AppName] [varchar](64) NULL,
PRIMARY KEY CLUSTERED 
(
	[AppId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [apps].[AppAuthorization]    Script Date: 3/28/2021 4:28:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [apps].[AppAuthorization](
	[UserId] [varchar](64) NOT NULL,
	[RoleId] [varchar](64) NOT NULL,
	[StateOrganizationId] [varchar](64) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [apps].[AppRole]    Script Date: 3/28/2021 4:28:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [apps].[AppRole](
	[RoleId] [varchar](64) NOT NULL,
	[RoleDescription] [varchar](64) NULL,
	[AppId] [varchar](64) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [apps].[AppUser]    Script Date: 3/28/2021 4:28:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [apps].[AppUser](
	[UserId] [varchar](64) NOT NULL,
	[FirstName] [varchar](64) NULL,
	[MiddleName] [varchar](64) NULL,
	[LastName] [varchar](64) NULL,
	[FullName] [varchar](64) NULL,
	[Email] [varchar](64) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [apps].[District]    Script Date: 3/28/2021 4:28:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [apps].[District](
	[StateOrganizationId] [varchar](64) NOT NULL,
	[FormattedOrganizationId] [varchar](64) NULL,
	[DistrictNumber] [int] NULL,
	[DistrictType] [int] NULL,
	[OrganizationName] [varchar](64) NULL,
PRIMARY KEY CLUSTERED 
(
	[StateOrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[EDVP_MidmsEdiamUser_View]    Script Date: 3/28/2021 4:28:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[EDVP_MidmsEdiamUser_View] as 
	 select 
		au.[userId],
		au.[firstName],
		au.[middleName],
		au.[lastName],
		au.[fullName],
		au.[email],
		'Telephone' as [telephone],
		app.[AppId],
		[appName],
		ar.[RoleId],
		[roleDescription],
		aa.[stateOrganizationId],
		[formattedOrganizationId],
		[districtNumber],
		[districtType],
		'SchoolNumber' as [schoolNumber],
		'UnitMinor' as [unitMinor],
		[organizationName],
		'educationOrganizationId' as [educationOrganizationId]
	from apps.AppUser au
	join apps.AppAuthorization aa on aa.UserId=au.UserId
	join apps.AppRole ar on ar.RoleId=aa.RoleId
	join apps.App app on app.AppId = ar.AppId
	join apps.District d on d.StateOrganizationId = aa.StateOrganizationId
GO
INSERT [apps].[App] ([AppId], [AppName]) VALUES (N'BRKN', N'This app is all about pain')
GO
INSERT [apps].[App] ([AppId], [AppName]) VALUES (N'VAL', N'Validation Portal')
GO
INSERT [apps].[AppAuthorization] ([UserId], [RoleId], [StateOrganizationId]) VALUES (N'adminuser', N'EDVP-Admin', N'10001000000')
GO
INSERT [apps].[AppAuthorization] ([UserId], [RoleId], [StateOrganizationId]) VALUES (N'brokenuser', N'TEST-Broken', N'10625000000')
GO
INSERT [apps].[AppAuthorization] ([UserId], [RoleId], [StateOrganizationId]) VALUES (N'dataowner', N'EDVP-DataOwner', N'10625000000')
GO
INSERT [apps].[AppAuthorization] ([UserId], [RoleId], [StateOrganizationId]) VALUES (N'districtuser', N'EDVP-DistrictUser', N'10625000000')
GO
INSERT [apps].[AppAuthorization] ([UserId], [RoleId], [StateOrganizationId]) VALUES (N'helpdesk', N'EDVP-HelpDesk', N'10625000000')
GO
INSERT [apps].[AppRole] ([RoleId], [RoleDescription], [AppId]) VALUES (N'EDVP-Admin', N'Admin', N'VAL')
GO
INSERT [apps].[AppRole] ([RoleId], [RoleDescription], [AppId]) VALUES (N'EDVP-DataOwner', N'Data Owner', N'VAL')
GO
INSERT [apps].[AppRole] ([RoleId], [RoleDescription], [AppId]) VALUES (N'EDVP-DistrictUser', N'District User', N'VAL')
GO
INSERT [apps].[AppRole] ([RoleId], [RoleDescription], [AppId]) VALUES (N'EDVP-HelpDesk', N'Help Desk', N'VAL')
GO
INSERT [apps].[AppRole] ([RoleId], [RoleDescription], [AppId]) VALUES (N'TEST-Broken', N'Broken User, assign this role to break a user', N'BRKN')
GO
INSERT [apps].[AppUser] ([UserId], [FirstName], [MiddleName], [LastName], [FullName], [Email]) VALUES (N'adminuser', N'Admin', N'T.', N'User', N'Admin T. User', N'adminuser@wearedoubleline.com')
GO
INSERT [apps].[AppUser] ([UserId], [FirstName], [MiddleName], [LastName], [FullName], [Email]) VALUES (N'brokenuser', N'Broken', N'T.', N'User', N'Broken T. User', N'brokenuser@wearedoubleline.com')
GO
INSERT [apps].[AppUser] ([UserId], [FirstName], [MiddleName], [LastName], [FullName], [Email]) VALUES (N'dataowner', N'Data', N'T.', N'Owner', N'Data T. Onwer', N'dataowner@wearedoubleline.com')
GO
INSERT [apps].[AppUser] ([UserId], [FirstName], [MiddleName], [LastName], [FullName], [Email]) VALUES (N'districtuser', N'District', N'T.', N'User', N'District T. User', N'districtuser@wearedoubleline.com')
GO
INSERT [apps].[AppUser] ([UserId], [FirstName], [MiddleName], [LastName], [FullName], [Email]) VALUES (N'helpdesk', N'Help', N'T.', N'Desk', N'Help T. Desk', N'helpdesk@wearedoubleline.com')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'10001000000', N'10001000', 10001000, 1, N'Aitkin Public School District')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'10347000000', N'10347000', 10347000, 1, N'Willmar Public School District')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'10624000000', N'10624000', 10624000, 1, N'White Bear Lake School District')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'10625000000', N'10625000', 10625000, 1, N'St. Paul Public School District')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'520397000000', N'520397000000', 520397000, 1, N'Lake A.')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'526094000000', N'526094000000', 526094000, 1, N'Cannon Valley Special Education Cooperative')
GO
INSERT [apps].[District] ([StateOrganizationId], [FormattedOrganizationId], [DistrictNumber], [DistrictType], [OrganizationName]) VALUES (N'616026000000', N'616026000000', 616026000, 1, N'District With a big EdOrg Id')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [PK_AppAuthorization]    Script Date: 3/28/2021 4:28:26 PM ******/
ALTER TABLE [apps].[AppAuthorization] ADD  CONSTRAINT [PK_AppAuthorization] PRIMARY KEY NONCLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC,
	[StateOrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [apps].[AppAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_AppAuthorization_App] FOREIGN KEY([RoleId])
REFERENCES [apps].[AppRole] ([RoleId])
GO
ALTER TABLE [apps].[AppAuthorization] CHECK CONSTRAINT [FK_AppAuthorization_App]
GO
ALTER TABLE [apps].[AppAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_AppAuthorization_District] FOREIGN KEY([StateOrganizationId])
REFERENCES [apps].[District] ([StateOrganizationId])
GO
ALTER TABLE [apps].[AppAuthorization] CHECK CONSTRAINT [FK_AppAuthorization_District]
GO
ALTER TABLE [apps].[AppAuthorization]  WITH CHECK ADD  CONSTRAINT [FK_AppAuthorization_User] FOREIGN KEY([UserId])
REFERENCES [apps].[AppUser] ([UserId])
GO
ALTER TABLE [apps].[AppAuthorization] CHECK CONSTRAINT [FK_AppAuthorization_User]
GO
ALTER TABLE [apps].[AppRole]  WITH CHECK ADD  CONSTRAINT [FK_AppRole_App] FOREIGN KEY([AppId])
REFERENCES [apps].[App] ([AppId])
GO
ALTER TABLE [apps].[AppRole] CHECK CONSTRAINT [FK_AppRole_App]
GO
/****** Object:  StoredProcedure [apps].[GetAuthorizations]    Script Date: 3/28/2021 4:28:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [apps].[GetAuthorizations](@UserId varchar(64)) AS
BEGIN
  SELECT u.UserId, u.FirstName, u.MiddleName, u.LastName, u.FullName, u.Email,
         app.AppId, app.AppName, r.RoleId, r.RoleDescription, d.DistrictType, 
		 d.StateOrganizationId, d.FormattedOrganizationId, d.DistrictNumber, d.OrganizationName
  FROM apps.AppAuthorization aa
  INNER JOIN [apps].[AppUser] u ON u.UserId = aa.UserId
  INNER JOIN [apps].[AppRole] r ON r.RoleId = aa.RoleId
  INNER JOIN [apps].[App] app ON app.AppId = r.AppId
  INNER JOIN [apps].[District] d ON d.StateOrganizationId = aa.StateOrganizationId
  WHERE aa.UserId = @UserId
END



GO
USE [master]
GO
ALTER DATABASE [ValidationPortalSsoMock] SET  READ_WRITE 
GO
