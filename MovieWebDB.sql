CREATE DATABASE MovieWebDB;
GO
USE MovieWebDB;
GO

-- Bảng người dùng
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User', -- Guest/User/Admin
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng thể loại phim
CREATE TABLE Genres (
    GenreId INT PRIMARY KEY IDENTITY(1,1),
    GenreName NVARCHAR(100) NOT NULL
);

-- Bảng phim
CREATE TABLE Movies (
    MovieId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    ReleaseYear INT,
    Duration INT, -- phút
    GenreId INT NOT NULL,
    PosterPath NVARCHAR(255), -- ảnh poster (Content/Posters)
    VideoPath NVARCHAR(255),  -- video (Content/Videos/demo.mp4)
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId)
);
INSERT INTO Users (FullName, Email, Username, PasswordHash, Role)
VALUES 
(N'Quản trị viên', N'admin@gmail.com', 'admin', '123456', 'Admin');
ALTER TABLE Movies ADD IsSeries BIT NOT NULL DEFAULT 0;
ALTER TABLE Movies
ADD ViewCount INT NOT NULL DEFAULT 0;
--Bảng Favorites
CREATE TABLE Favorites (
    FavoriteId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Favorites_Users FOREIGN KEY(UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Favorites_Movies FOREIGN KEY(MovieId) REFERENCES Movies(MovieId)
);

-- Tránh trùng lặp
CREATE UNIQUE INDEX IX_User_Movie ON Favorites(UserId, MovieId);
--bảng trung gian cho n-n thể loại
CREATE TABLE MovieGenres (
    MovieId INT NOT NULL,
    GenreId INT NOT NULL,
    PRIMARY KEY (MovieId, GenreId),
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE CASCADE,
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId) ON DELETE CASCADE
);



