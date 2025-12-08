CREATE DATABASE MovieWebDB_New;
GO
USE MovieWebDB_New;
GO

-- =========================
-- 1. BẢNG USERS
-- =========================
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) DEFAULT 'User',
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =========================
-- 2. BẢNG GENRES
-- =========================
CREATE TABLE Genres (
    GenreId INT PRIMARY KEY IDENTITY(1,1),
    GenreName NVARCHAR(100) NOT NULL
);

-- =========================
-- 3. BẢNG MOVIES
-- =========================
CREATE TABLE Movies (
    MovieId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    GenreId INT NULL,
    ReleaseYear INT,
    PosterPath NVARCHAR(255),
    TrailerUrl NVARCHAR(255),
    Duration INT,
    ViewCount INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId) ON DELETE SET NULL
);

-- =========================
-- 4. BẢNG MOVIEGENRES (nhiều-nhiều)
-- =========================
CREATE TABLE MovieGenres (
    MovieId INT NOT NULL,
    GenreId INT NOT NULL,
    PRIMARY KEY (MovieId, GenreId),
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE CASCADE,
    FOREIGN KEY (GenreId) REFERENCES Genres(GenreId) ON DELETE NO ACTION
);

-- =========================
-- 5. BẢNG FAVORITES
-- =========================
CREATE TABLE Favorites (
    FavoriteId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE NO ACTION
);

-- =========================
-- 6. BẢNG COMMENTS
-- =========================
CREATE TABLE Comments (
    CommentId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE NO ACTION
);

-- =========================
-- 7. BẢNG RATINGS
-- =========================
CREATE TABLE Ratings (
    RatingId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    Score INT CHECK (Score BETWEEN 1 AND 5),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE NO ACTION
);

-- =========================
-- 8. BẢNG WATCHHISTORIES
-- =========================
CREATE TABLE WatchHistories (
    WatchHistoryId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    MovieId INT NOT NULL,
    WatchedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE NO ACTION
);

-- =========================
-- 9. BẢNG SERIES
-- =========================
CREATE TABLE Series (
    SeriesId INT PRIMARY KEY IDENTITY(1,1),
    MovieId INT NOT NULL,
    SeriesName NVARCHAR(200),
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE NO ACTION
);

-- =========================
-- 10. BẢNG EPISODES
-- =========================
CREATE TABLE Episodes (
    EpisodeId INT PRIMARY KEY IDENTITY(1,1),
    SeriesId INT NOT NULL,
    EpisodeNumber INT,
    Title NVARCHAR(200),
    VideoUrl NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SeriesId) REFERENCES Series(SeriesId) ON DELETE CASCADE
);

-- =========================
-- 11. BẢNG PERSONS
-- =========================
CREATE TABLE Persons (
    PersonId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) -- Actor, Director, etc.
);

-- =========================
-- 12. BẢNG MOVIEPERSONS (liên kết nhiều-nhiều)
-- =========================
CREATE TABLE MoviePersons (
    MovieId INT NOT NULL,
    PersonId INT NOT NULL,
    PRIMARY KEY (MovieId, PersonId),
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId) ON DELETE NO ACTION,
    FOREIGN KEY (PersonId) REFERENCES Persons(PersonId) ON DELETE CASCADE
);

-- =========================
-- 13. BẢNG ADMINLOGS
-- =========================
CREATE TABLE AdminLogs (
    AdminLogId INT PRIMARY KEY IDENTITY(1,1),
    AdminId INT,
    Action NVARCHAR(200),
    LogTime DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (AdminId) REFERENCES Users(UserId) ON DELETE SET NULL
);
INSERT INTO Users (FullName, Email, PasswordHash, Role, CreatedAt)
VALUES (N'Quản trị viên', 'admin@example.com', '123456', 'Admin', GETDATE());
INSERT INTO Users (FullName, Email, PasswordHash, Role)
VALUES (N'Quản trị viên', 'admin@gmail.com', '123456', 'Admin');
ALTER TABLE Movies
ADD VideoPath NVARCHAR(255) NULL,
    IsSeries BIT NOT NULL DEFAULT 0;


	CREATE TABLE AdsBanners (
    BannerId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200),
    ImagePath NVARCHAR(255) NOT NULL,
    TargetUrl NVARCHAR(255),
    Position NVARCHAR(50) NOT NULL,  -- HomeTop, HomeSide, MovieDetailTop...
    IsActive BIT DEFAULT 1,
    StartDate DATETIME NULL,
    EndDate DATETIME NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
ALTER TABLE Users ADD IsVip BIT NOT NULL DEFAULT 0, VipExpiredAt DATETIME NULL;




-- =========================
-- Funtion + Triger
-- =========================


--Hàm đếm tổng lượt xem phim_kha
CREATE FUNCTION fn_GetViewCount (@MovieId INT)
RETURNS INT
AS
BEGIN
    DECLARE @TotalViews INT;

    SELECT @TotalViews = COUNT(*)
    FROM WatchHistories
    WHERE MovieId = @MovieId;

    RETURN ISNULL(@TotalViews, 0);
END;
GO

SELECT dbo.fn_GetViewCount(1) AS ViewCount;

--Hàm tính điểm đánh giá trung bình của phim-duy
CREATE FUNCTION fn_GetKimetsuRating (@MovieId INT)
RETURNS FLOAT
AS
BEGIN
    DECLARE @AvgRating FLOAT;

    SELECT @AvgRating = AVG(CAST(Score AS FLOAT))
    FROM Ratings
    WHERE MovieId = @MovieId;

    RETURN ISNULL(@AvgRating, 0);
END;
GO

SELECT dbo.fn_GetKimetsuRating(1);

--Hàm kiểm tra User còn VIP hay không-Thien
CREATE FUNCTION fn_IsUserVip (@UserId INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsVip BIT;

    SELECT @IsVip =
        CASE 
            WHEN IsVip = 1 AND VipExpiredAt > GETDATE() THEN 1
            ELSE 0
        END
    FROM Users
    WHERE UserId = @UserId;

    RETURN @IsVip;
END;
GO

SELECT dbo.fn_IsUserVip(5);





----TRIGGER (KÍCH HOẠT TỰ ĐỘNG)


--Trigger: Khi người dùng xem phim → tăng ViewCount_Duy
CREATE TRIGGER trg_IncreaseViewCount
ON WatchHistories
AFTER INSERT
AS
BEGIN
    UPDATE Movies
    SET ViewCount = ViewCount + 1
    WHERE MovieId IN (SELECT MovieId FROM inserted);
END;
GO
 --Trigger: Tự động tắt VIP khi hết hạn-Thien
 CREATE TRIGGER trg_DisableExpiredVip
ON Users
AFTER UPDATE
AS
BEGIN
    UPDATE Users
    SET IsVip = 0
    WHERE VipExpiredAt IS NOT NULL
      AND VipExpiredAt < GETDATE();
END;
GO


--Trigger: Khi Admin cập nhật phim → ghi log vào AdminLogs_Kha
CREATE TRIGGER trg_LogAdminUpdateMovie
ON Movies
AFTER UPDATE
AS
BEGIN
    INSERT INTO AdminLogs (AdminId, Action, LogTime)
    SELECT NULL,
           N'Admin cập nhật thông tin phim: ' + Title,
           GETDATE()
    FROM inserted;
END;
GO
