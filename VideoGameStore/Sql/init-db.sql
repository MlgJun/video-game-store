------------------------------------------------------------
-- ЖАНРЫ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Genres])
BEGIN
    INSERT INTO [dbo].[Genres] ([Title]) VALUES
        (N'Action'),
        (N'RPG'),
        (N'Adventure'),
        (N'Indie'),
        (N'Strategy');
END;

------------------------------------------------------------
-- ПОЛЬЗОВАТЕЛИ (Customers / Sellers)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers])
BEGIN
    INSERT INTO [dbo].[Customers] ([CreatedAt]) VALUES
        (SYSUTCDATETIME()),
        (SYSUTCDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Sellers])
BEGIN
    INSERT INTO [dbo].[Sellers] ([CreatedAt]) VALUES
        (SYSUTCDATETIME()),
        (SYSUTCDATETIME());
END;

------------------------------------------------------------
-- ИГРЫ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Games])
BEGIN
    INSERT INTO [dbo].[Games]
        ([Price], [Title], [Description], [SellerId],
         [DeveloperTitle], [PublisherTitle], [CreatedAt])
    VALUES
        (19.99, N'Space Shooter', N'Аркадный космический шутер',
         3, N'StarDev Studio', N'Galaxy Publishing', SYSUTCDATETIME()),
        (39.99, N'Dungeon RPG', N'Пошаговая RPG в подземельях',
         3, N'DeepDungeons', N'OldSchool Games', SYSUTCDATETIME()),
        (9.99, N'Indie Pixel', N'Инди-платформер в пиксельной графике',
         4, N'IndieBrothers', N'IndieBrothers', SYSUTCDATETIME());
END;

------------------------------------------------------------
-- СВЯЗИ ИГРА-ЖАНРЫ
------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[GameGenres] gg
    JOIN [dbo].[Games] g ON g.Id = gg.GamesId AND g.Title = N'Space Shooter'
)
BEGIN
    DECLARE @GameId1 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Space Shooter');
    DECLARE @ActionId BIGINT = (SELECT TOP 1 Id FROM [dbo].[Genres] WHERE Title = N'Action');
    DECLARE @IndieId  BIGINT = (SELECT TOP 1 Id FROM [dbo].[Genres] WHERE Title = N'Indie');

    IF @GameId1 IS NOT NULL AND @ActionId IS NOT NULL
        INSERT INTO [dbo].[GameGenres] ([GamesId], [GenresId]) VALUES (@GameId1, @ActionId);

    IF @GameId1 IS NOT NULL AND @IndieId IS NOT NULL
        INSERT INTO [dbo].[GameGenres] ([GamesId], [GenresId]) VALUES (@GameId1, @IndieId);
END;

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[GameGenres] gg
    JOIN [dbo].[Games] g ON g.Id = gg.GamesId AND g.Title = N'Dungeon RPG'
)
BEGIN
    DECLARE @GameId2 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Dungeon RPG');
    DECLARE @RpgId   BIGINT = (SELECT TOP 1 Id FROM [dbo].[Genres] WHERE Title = N'RPG');

    IF @GameId2 IS NOT NULL AND @RpgId IS NOT NULL
        INSERT INTO [dbo].[GameGenres] ([GamesId], [GenresId]) VALUES (@GameId2, @RpgId);
END;

------------------------------------------------------------
-- КЛЮЧИ ДЛЯ ИГР
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Keys])
BEGIN
    DECLARE @GameId_Space  BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Space Shooter');
    DECLARE @GameId_Dungeon BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Dungeon RPG');

    IF @GameId_Space IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Keys] ([Value], [GameId]) VALUES
            (N'SPACE-KEY-1111-AAAA', @GameId_Space),
            (N'SPACE-KEY-2222-BBBB', @GameId_Space),
            (N'SPACE-KEY-3333-CCCC', @GameId_Space);
    END;

    IF @GameId_Dungeon IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Keys] ([Value], [GameId]) VALUES
            (N'DUNGEON-KEY-1111-DDDD', @GameId_Dungeon),
            (N'DUNGEON-KEY-2222-EEEE', @GameId_Dungeon);
    END;
END;

------------------------------------------------------------
-- КОРЗИНЫ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Carts])
BEGIN
    DECLARE @Customer1 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Customers] ORDER BY Id);

    IF @Customer1 IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Carts] ([CustomerId]) VALUES (@Customer1);
    END;
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[CartItems])
BEGIN
    DECLARE @CartId1 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Carts] ORDER BY Id);
    DECLARE @GameIdSpace BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Space Shooter');

    IF @CartId1 IS NOT NULL AND @GameIdSpace IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[CartItems] ([GameId], [Quantity], [CartId])
        VALUES (@GameIdSpace, 1, @CartId1);
    END;
END;

------------------------------------------------------------
-- ЗАКАЗ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Orders])
BEGIN
    DECLARE @Customer1Id BIGINT = (SELECT TOP 1 Id FROM [dbo].[Customers] ORDER BY Id);
    DECLARE @GameIdSpace2 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Space Shooter');
    DECLARE @PriceSpace DECIMAL(10,2) = (SELECT TOP 1 Price FROM [dbo].[Games] WHERE Id = @GameIdSpace2);

    IF @Customer1Id IS NOT NULL AND @GameIdSpace2 IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Orders] ([CustomerId], [TotalAmount], [CreatedAt])
        VALUES (@Customer1Id, @PriceSpace, SYSUTCDATETIME());

        DECLARE @OrderId1 BIGINT = SCOPE_IDENTITY();

        INSERT INTO [dbo].[OrderItems] ([GameId], [Quantity], [Price], [OrderId], [Key])
        VALUES (@GameIdSpace2, 1, @PriceSpace, @OrderId1, N'SPACE-ORDER-KEY-DEMO');
    END;
END;
