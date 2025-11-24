
CREATE TABLE Countries (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Code NVARCHAR(10) NOT NULL UNIQUE -- e.g., 'SG', 'MM'
);
GO

INSERT INTO Countries (Name, Code) VALUES ('Singapore', 'SG');
INSERT INTO Countries (Name, Code) VALUES ('Myanmar', 'MM');
INSERT INTO Countries (Name, Code) VALUES ('Thailand', 'TH');
GO

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash VARBINARY(MAX) NOT NULL,
    PasswordSalt VARBINARY(MAX) NOT NULL,
    IsEmailVerified BIT NOT NULL DEFAULT 0,
    ResetToken NVARCHAR(MAX) NULL,
    ResetTokenExpiry DATETIME2 NULL
);
GO

CREATE TABLE Packages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    InitialCredits INT NOT NULL,
    CountryId INT NOT NULL,
    
    FOREIGN KEY (CountryId) REFERENCES Countries(Id)
);
GO

INSERT INTO Packages (Name, Price, InitialCredits, CountryId) VALUES
('Starter Kit US', 49.99, 5, 1),
('Premium Monthly CA', 99.99, 5, 2);
GO

CREATE TABLE UserPackages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    PackageId INT NOT NULL,
    RemainingCredits INT NOT NULL,
    PurchaseDate DATETIME2 NOT NULL,
    ExpiryDate DATETIME2 NOT NULL,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (PackageId) REFERENCES Packages(Id)
);
GO

CREATE TABLE ClassSchedules (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    StartTime DATETIME2 NOT NULL,
    EndTime DATETIME2 NOT NULL,
    RequiredCredits INT NOT NULL,
    MaxCapacity INT NOT NULL,
    CountryId INT NOT NULL,
    
    FOREIGN KEY (CountryId) REFERENCES Countries(Id),
    INDEX IX_ScheduleTime (StartTime, EndTime)
);
GO

INSERT INTO ClassSchedules (Name, StartTime, EndTime, RequiredCredits, MaxCapacity, CountryId) VALUES
('Morning Yoga Flow (SG)', '2025-11-25 09:00:00', '2025-11-25 10:00:00', 3, 20, 1),
('Evening Spin Class (MM)', '2025-11-25 18:30:00', '2025-11-25 19:30:00', 5, 15, 2);
GO

CREATE TABLE Bookings (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ClassScheduleId INT NOT NULL,
    CreditsDeducted INT NOT NULL,
    Status NVARCHAR(50) NOT NULL CHECK (Status IN ('Confirmed', 'Canceled', 'CheckedIn')), 
    BookingDate DATETIME2 NOT NULL,
    CheckedIn BIT NOT NULL DEFAULT 0,
    UserPackageId INT NULL,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ClassScheduleId) REFERENCES ClassSchedules(Id),
    FOREIGN KEY (UserPackageId) REFERENCES UserPackages(Id) 
);
GO

CREATE TABLE Waitlists (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ClassScheduleId INT NOT NULL,
    WaitlistDate DATETIME2 NOT NULL,
    [Order] INT NOT NULL,
    CreditsDeducted INT NOT NULL,
    IsCreditRefunded BIT NOT NULL DEFAULT 0,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ClassScheduleId) REFERENCES ClassSchedules(Id),
    
    UNIQUE (UserId, ClassScheduleId),
    UNIQUE (ClassScheduleId, [Order])
);
GO
