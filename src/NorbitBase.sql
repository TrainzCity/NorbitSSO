USE [master]
GO
/****** Object:  Database [NorbitBase]    Script Date: 29.01.2026 15:49:43 ******/
CREATE DATABASE [NorbitBase]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'NorbitBase', FILENAME = N'/var/opt/mssql/data/NorbitBase.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'NorbitBase_log', FILENAME = N'/var/opt/mssql/data/NorbitBase_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 COLLATE Cyrillic_General_CI_AS
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [NorbitBase] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NorbitBase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NorbitBase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NorbitBase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NorbitBase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NorbitBase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NorbitBase] SET ARITHABORT OFF 
GO
ALTER DATABASE [NorbitBase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NorbitBase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NorbitBase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NorbitBase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NorbitBase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NorbitBase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NorbitBase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NorbitBase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NorbitBase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NorbitBase] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NorbitBase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NorbitBase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NorbitBase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NorbitBase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NorbitBase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NorbitBase] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NorbitBase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NorbitBase] SET RECOVERY FULL 
GO
ALTER DATABASE [NorbitBase] SET  MULTI_USER 
GO
ALTER DATABASE [NorbitBase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NorbitBase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NorbitBase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NorbitBase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [NorbitBase] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [NorbitBase] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'NorbitBase', N'ON'
GO
ALTER DATABASE [NorbitBase] SET QUERY_STORE = ON
GO
ALTER DATABASE [NorbitBase] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [ituser09]    Script Date: 29.01.2026 15:49:43 ******/
CREATE LOGIN [ituser09] WITH PASSWORD=N'vgyWrrBQQP9hTmQxhwOQeTAWO093cgSv1SSaYuW4r7A=', DEFAULT_DATABASE=[NorbitBase], DEFAULT_LANGUAGE=[русский], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
ALTER LOGIN [ituser09] DISABLE
GO
ALTER AUTHORIZATION ON DATABASE::[NorbitBase] TO [ituser09]
GO
USE [NorbitBase]
GO
/****** Object:  Table [dbo].[AuthStatus]    Script Date: 29.01.2026 15:49:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthStatus](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](120) COLLATE Cyrillic_General_CI_AS NOT NULL,
 CONSTRAINT [PK_AuthStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[AuthStatus] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Log]    Script Date: 29.01.2026 15:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Time] [datetime] NOT NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Log] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[RequestType]    Script Date: 29.01.2026 15:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) COLLATE Cyrillic_General_CI_AS NOT NULL,
 CONSTRAINT [PK_RequestType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[RequestType] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[User]    Script Date: 29.01.2026 15:49:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UUID] [uniqueidentifier] NOT NULL,
	[Surname] [nvarchar](50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[Name] [nvarchar](25) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[Patronymic] [nvarchar](30) COLLATE Cyrillic_General_CI_AS NULL,
	[Birthday] [date] NOT NULL,
	[Email] [nvarchar](120) COLLATE Cyrillic_General_CI_AS NULL,
	[Phone] [nchar](12) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[Login] [nvarchar](25) COLLATE Cyrillic_General_CI_AS NOT NULL,
	[Password] [varbinary](max) NOT NULL,
	[IsBlocked] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[User] TO  SCHEMA OWNER 
GO
INSERT [dbo].[AuthStatus] ([Id], [Name]) VALUES (1, N'Успех')
GO
INSERT [dbo].[AuthStatus] ([Id], [Name]) VALUES (2, N'Неверный логин или пароль')
GO
INSERT [dbo].[AuthStatus] ([Id], [Name]) VALUES (3, N'Ошибка проверки токена')
GO
INSERT [dbo].[AuthStatus] ([Id], [Name]) VALUES (4, N'Ошибка проверки устройства')
GO
INSERT [dbo].[AuthStatus] ([Id], [Name]) VALUES (5, N'Аккунт заблокирован')
GO
SET IDENTITY_INSERT [dbo].[Log] ON 
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (1, 1, 1, N'8c67251b-090e-4ff2-ae2a-268cab745f41', CAST(N'2026-01-27T22:08:57.000' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (2, 1, 1, N'58b542f1-be28-45a8-9cfd-6fdeeaddc6c9', CAST(N'2026-01-29T11:12:52.220' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (3, 1, 1, N'5c27c289-4b46-4577-a164-b8638c388f46', CAST(N'2026-01-29T11:31:03.930' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (4, 1, 4, N'58b542f1-be28-45a8-9cfd-6fdeeaddc6c9', CAST(N'2026-01-29T11:43:20.153' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (5, 2, 2, NULL, CAST(N'2026-01-29T11:52:46.893' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (6, 2, 2, NULL, CAST(N'2026-01-29T11:54:11.773' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (7, 2, 2, NULL, CAST(N'2026-01-29T11:54:36.187' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (8, 5, 2, N'58b542f1-be28-45a8-9cfd-6fdeeaddc6c9', CAST(N'2026-01-29T11:54:44.473' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (9, 2, 2, NULL, CAST(N'2026-01-29T11:55:26.053' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (10, 2, 2, NULL, CAST(N'2026-01-29T11:56:18.443' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (11, 1, 2, N'5c27c289-4b46-4577-a164-b8638c388f46', CAST(N'2026-01-29T11:56:25.963' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (12, 1, 5, NULL, CAST(N'2026-01-29T11:56:39.163' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (13, 1, 1, N'789a250f-4d01-436c-9929-8063100f7e2f', CAST(N'2026-01-29T12:14:00.013' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (14, 1, 2, N'789a250f-4d01-436c-9929-8063100f7e2f', CAST(N'2026-01-29T12:24:30.350' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (15, 1, 5, NULL, CAST(N'2026-01-29T12:24:51.907' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (16, 2, 2, NULL, CAST(N'2026-01-29T10:29:04.750' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (17, 2, 2, NULL, CAST(N'2026-01-29T10:29:27.580' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (18, 2, 2, NULL, CAST(N'2026-01-29T10:29:34.660' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (19, 2, 2, NULL, CAST(N'2026-01-29T10:30:12.900' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (20, 2, 2, NULL, CAST(N'2026-01-29T10:30:37.407' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (21, 2, 2, NULL, CAST(N'2026-01-29T10:30:42.703' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (22, 2, 2, NULL, CAST(N'2026-01-29T10:31:18.517' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (23, 2, 2, NULL, CAST(N'2026-01-29T10:31:58.337' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (24, 2, 2, NULL, CAST(N'2026-01-29T10:32:02.550' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (25, 2, 2, NULL, CAST(N'2026-01-29T10:32:06.980' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (26, 2, 2, NULL, CAST(N'2026-01-29T10:33:58.460' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (27, 1, 2, N'8c67251b-090e-4ff2-ae2a-268cab745f41', CAST(N'2026-01-29T10:34:56.417' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (28, 1, 2, N'8c67251b-090e-4ff2-ae2a-268cab745f41', CAST(N'2026-01-29T10:35:47.973' AS DateTime))
GO
INSERT [dbo].[Log] ([Id], [StatusId], [TypeId], [UserId], [Time]) VALUES (29, 1, 2, N'8c67251b-090e-4ff2-ae2a-268cab745f41', CAST(N'2026-01-29T12:19:33.090' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Log] OFF
GO
INSERT [dbo].[RequestType] ([Id], [Name]) VALUES (1, N'Регистрация')
GO
INSERT [dbo].[RequestType] ([Id], [Name]) VALUES (2, N'Запрос токена')
GO
INSERT [dbo].[RequestType] ([Id], [Name]) VALUES (3, N'Авторизация')
GO
INSERT [dbo].[RequestType] ([Id], [Name]) VALUES (4, N'Блокировка')
GO
INSERT [dbo].[RequestType] ([Id], [Name]) VALUES (5, N'Выход из системы')
GO
INSERT [dbo].[User] ([UUID], [Surname], [Name], [Patronymic], [Birthday], [Email], [Phone], [Login], [Password], [IsBlocked]) VALUES (N'8c67251b-090e-4ff2-ae2a-268cab745f41', N'Гребенщиков', N'Глеб', N'Романович', CAST(N'2007-05-27' AS Date), N'gleb@trainzcity.com', N'+79207000662', N'gleb', 0xA38EE049EFEBDACE56DF4EFC1AF614BDE4383FE6FC0826A99308E2547D9AB3D7, 0)
GO
INSERT [dbo].[User] ([UUID], [Surname], [Name], [Patronymic], [Birthday], [Email], [Phone], [Login], [Password], [IsBlocked]) VALUES (N'58b542f1-be28-45a8-9cfd-6fdeeaddc6c9', N'Александр', N'Гатауллин', N'Эдуардович', CAST(N'2007-02-09' AS Date), N'test@trainzcity.com', N'+79304095264', N'umshoow', 0xA38EE049EFEBDACE56DF4EFC1AF614BDE4383FE6FC0826A99308E2547D9AB3D7, 1)
GO
INSERT [dbo].[User] ([UUID], [Surname], [Name], [Patronymic], [Birthday], [Email], [Phone], [Login], [Password], [IsBlocked]) VALUES (N'789a250f-4d01-436c-9929-8063100f7e2f', N'Норбитский', N'Александр', N'Амбассадорович', CAST(N'2005-12-06' AS Date), N'ubangishari@trainzcity.com', N'+79254134957', N'ubangishari', 0x032F7EE076043C501D46732AEF14E52362DA1BCDE8F1931BEFF02E3AFA3D4BB9, 0)
GO
INSERT [dbo].[User] ([UUID], [Surname], [Name], [Patronymic], [Birthday], [Email], [Phone], [Login], [Password], [IsBlocked]) VALUES (N'5c27c289-4b46-4577-a164-b8638c388f46', N'Муратова', N'Злата', N'Юрьевна', CAST(N'2007-09-04' AS Date), N'zlatik@trainzcity.com', N'+78005553535', N'cinnamon_6un', 0x7D546DD184832C33DEA53D2F77D52DA85A686B8E7D34454E7C0FA3384B52D36F, 0)
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_UUID]  DEFAULT (newid()) FOR [UUID]
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_AuthStatus] FOREIGN KEY([StatusId])
REFERENCES [dbo].[AuthStatus] ([Id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_AuthStatus]
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_RequestType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[RequestType] ([Id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_RequestType]
GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UUID])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_User]
GO
USE [master]
GO
ALTER DATABASE [NorbitBase] SET  READ_WRITE 
GO
