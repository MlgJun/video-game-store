------------------------------------------------------------
-- ЖАНРЫ (дополнены для реальных игр)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Genres])
BEGIN
    INSERT INTO [dbo].[Genres] ([Title]) VALUES
        (N'Шутер от первого лица'),
        (N'Шутер от третьего лица'),
        (N'Стратегии и тактические ролевые'),
        (N'Симуляторы строительства и автоматизации'),
        (N'Симуляторы хобби и работы'),
        (N'Казуальные'),
        (N'Рогалики'),
        (N'Карточные и настольные'),
        (N'Пошаговые стратегии'),
        (N'Научная фантастика'),
        (N'Головоломки'),
        (N'Спортивные симуляторы'),
        (N'Хорроры'),
        (N'Гонки'),
        (N'Выживание'),
        (N'Башенная защита'),
        (N'Платформеры'),
        (N'Приключения'),
        (N'Песочницы'),
        (N'MMO');
END;

------------------------------------------------------------
-- ПОЛЬЗОВАТЕЛИ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Customers] WHERE Id = 1000)
BEGIN
    INSERT INTO [dbo].[Customers] ([Id],[CreatedAt]) VALUES
        (1000, SYSUTCDATETIME()),
        (2000, SYSUTCDATETIME());
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Sellers] WHERE Id = 3000)
BEGIN
    INSERT INTO [dbo].[Sellers] ([Id], [CreatedAt]) VALUES
        (3000, SYSUTCDATETIME()), -- Valve / Classic Devs
        (4000, SYSUTCDATETIME()), -- Indie Studios
        (5000, SYSUTCDATETIME()), -- AAA Publishers
        (6000, SYSUTCDATETIME()); -- Multi-Genre / Hybrid
END;

------------------------------------------------------------
-- НАСТОЯЩИЕ ИГРЫ ИЗ STEAM
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Games] WHERE Title = N'Half-Life 2')
BEGIN
    INSERT INTO [dbo].[Games] ([Price], [Title], [Description], [SellerId], [DeveloperTitle], [PublisherTitle], [CreatedAt], [ImageUrl])
    VALUES
    -- Seller 3000: Valve / Legendary Devs
    (90.99, N'Half-Life 2', N'Революционный шутер от Valve, изменивший индустрию.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/halflife2.jpg'),
    (190.99, N'Portal 2', N'Гениальные физические головоломки в мире Aperture Science.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/portal2.jpg'),
    (40.99, N'Team Fortress 2', N'Классический командный шутер с юмором и стилем.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/tf2.jpg'),
    (190.99, N'Left 4 Dead 2', N'Кооперативный зомби-шутер с динамичным ИИ.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/l4d2.jpg'),
    (390.99, N'Counter-Strike 2', N'Легендарный тактический шутер, обновлённый для нового поколения.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/cs2.jpg'),
    (290.99, N'Dota 2', N'Королева MOBA с киберспортивной сценой мирового уровня.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/dota2.jpg'),
    (140.99, N'Day of Defeat: Source', N'Тактический шутер времён Второй мировой войны.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/dod.jpg'),
    (90.99, N'Deathmatch Classic', N'Ностальгический шутер в стиле Quake.', 3000, N'Valve', N'Valve', SYSUTCDATETIME(), 'games/cscz.jpg'),

    -- Seller 4000: Indie Studios
    (140.99, N'Hollow Knight', N'Метроидвания с атмосферным миром и сложными боями.', 4000, N'Team Cherry', N'Team Cherry', SYSUTCDATETIME(), 'games/hk.jpg'),
    (190.99, N'Stardew Valley', N'Уютный фермерский симулятор с глубокой системой взаимодействий.', 4000, N'ConcernedApe', N'ConcernedApe', SYSUTCDATETIME(), 'games/sv.jpg'),
    (240.99, N'Hades', N'Динамичный рогалик с богатым сюжетом и персонажами.', 4000, N'Supergiant Games', N'Supergiant Games', SYSUTCDATETIME(), 'games/hades.jpg'),
    (190.99, N'Celeste', N'Точный платформер с эмоциональной историей.', 4000, N'Maddy Makes Games', N'Maddy Makes Games', SYSUTCDATETIME(), 'games/celeste.jpg'),
    (140.99, N'Undertale', N'Ролевая игра, где милота скрывает глубокие моральные выборы.', 4000, N'tobyfox', N'tobyfox', SYSUTCDATETIME(), 'games/undertale.jpg'),
    (190.99, N'Dead Cells', N'Безумно быстрый рогалик-платформер с прокачкой.', 4000, N'Motion Twin', N'Motion Twin', SYSUTCDATETIME(), 'games/dc.jpg'),
    (290.99, N'Slay the Spire', N'Карточная стратегия с элементами рогалика.', 4000, N'MegaCrit', N'MegaCrit', SYSUTCDATETIME(), 'games/sts.jpg'),
    (70.99, N'Papers, Please', N'Симулятор пограничника в диктатуре — этические дилеммы каждый день.', 4000, N'Lucas Pope', N'3909', SYSUTCDATETIME(), 'games/pp.jpg'),

    -- Seller 5000: AAA Studios
    (590.99, N'Red Dead Redemption 2', N'Эпическая история на закате Дикого Запада.', 5000, N'Rockstar Games', N'Rockstar Games', SYSUTCDATETIME(), 'games/rdr2.jpg'),
    (390.99, N'Doom Eternal', N'Безумный шутер с ритмичными боями и демонами.', 5000, N'id Software', N'Bethesda', SYSUTCDATETIME(), 'games/doom.jpg'),
    (590.99, N'BioShock Infinite', N'Путешествие в летающий город с философским подтекстом.', 5000, N'Irrational Games', N'2K', SYSUTCDATETIME(), 'games/bioshock.jpg'),
    (390.99, N'Control', N'Психологический экшен с сверхспособностями в странном здании.', 5000, N'Remedy Entertainment', N'505 Games', SYSUTCDATETIME(), 'games/control.jpg'),
    (490.99, N'Dishonored 2', N'Стелс-экшен с магией и альтернативными путями.', 5000, N'Arkane Studios', N'Bethesda', SYSUTCDATETIME(), 'games/dishonored2.jpg'),
    (590.99, N'Cyberpunk 2077', N'Открытый мир будущего с RPG-глубиной (пост-патч).', 5000, N'CD PROJEKT RED', N'CD PROJEKT RED', SYSUTCDATETIME(), 'games/cyberpunk.jpg'),
    (390.99, N'Prey (2017)', N'Хоррор-песочница на космической станции.', 5000, N'Arkane Studios', N'Bethesda', SYSUTCDATETIME(), 'games/prey.jpg'),
    (290.99, N'Wolfenstein: The New Order', N'Альтернативная история с боем против нацистов.', 5000, N'MachineGames', N'Bethesda', SYSUTCDATETIME(), 'games/wolf.jpg'),

    -- Seller 6000: Multi-Genre / Strategy / Sim
    (390.99, N'Sid Meier’s Civilization VI', N'Построй империю и прославься в веках.', 6000, N'Firaxis Games', N'2K', SYSUTCDATETIME(), 'games/civ6.jpg'),
    (440.99, N'Cities: Skylines', N'Городской симулятор для архитекторов и планировщиков.', 6000, N'Colossal Order', N'Paradox Interactive', SYSUTCDATETIME(), 'games/cities.jpg'),
    (290.99, N'Terraria', N'2D-песочница с исследованием, боем и крафтом.', 6000, N'Re-Logic', N'Re-Logic', SYSUTCDATETIME(), 'games/terraria.jpg'),
    (240.99, N'Oxygen Not Included', N'Симулятор колонии на экзопланете с физикой и химией.', 6000, N'Klei Entertainment', N'Klei Entertainment', SYSUTCDATETIME(), 'games/oni.jpg'),
    (340.99, N'Factorio', N'Автоматизируй производство и защищай базу от чужих.', 6000, N'Wube Software', N'Wube Software', SYSUTCDATETIME(), 'games/factorio.jpg'),
    (190.99, N'Subnautica', N'Выживание и исследование подводного мира чужой планеты.', 6000, N'Unknown Worlds', N'Unknown Worlds', SYSUTCDATETIME(), 'games/subnautica.jpg'),
    (290.99, N'Among Us', N'Социальная детективная игра для друзей.', 6000, N'Innersloth', N'Innersloth', SYSUTCDATETIME(), 'games/amongus.jpg'),
    (390.99, N'Outer Wilds', N'Космическая головоломка с временной петлёй.', 6000, N'Mobius Digital', N'Annapurna Interactive', SYSUTCDATETIME(), 'games/ow.jpg');
END;

------------------------------------------------------------
-- СВЯЗИ ИГРА-ЖАНРЫ (для всех игр)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[GameGenres])
BEGIN
    DECLARE @Genres TABLE (Title NVARCHAR(100), Id BIGINT);
    INSERT INTO @Genres SELECT Title, Id FROM [dbo].[Genres];

    DECLARE @Games TABLE (Title NVARCHAR(100), Id BIGINT);
    INSERT INTO @Games SELECT Title, Id FROM [dbo].[Games];

    -- Valve
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Half-Life 2' AND ge.Title IN (N'Шутер от первого лица', N'Научная фантастика', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Portal 2' AND ge.Title IN (N'Головоломки', N'Приключения', N'Научная фантастика');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Team Fortress 2' AND ge.Title IN (N'Шутер от первого лица', N'Казуальные');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Left 4 Dead 2' AND ge.Title IN (N'Шутер от первого лица', N'Хорроры', N'Выживание');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Counter-Strike 2' AND ge.Title IN (N'Шутер от первого лица', N'Казуальные');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Dota 2' AND ge.Title IN (N'MMO', N'Стратегии и тактические ролевые');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Day of Defeat: Source' AND ge.Title IN (N'Шутер от первого лица', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Deathmatch Classic' AND ge.Title IN (N'Шутер от первого лица');

    -- Indie
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Hollow Knight' AND ge.Title IN (N'Платформеры', N'Приключения', N'Рогалики');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Stardew Valley' AND ge.Title IN (N'Симуляторы хобби и работы', N'Песочницы', N'Казуальные');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Hades' AND ge.Title IN (N'Рогалики', N'Платформеры', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Celeste' AND ge.Title IN (N'Платформеры', N'Приключения', N'Казуальные');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Undertale' AND ge.Title IN (N'Ролевые', N'Головоломки', N'Казуальные');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Dead Cells' AND ge.Title IN (N'Рогалики', N'Платформеры', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Slay the Spire' AND ge.Title IN (N'Карточные и настольные', N'Рогалики', N'Стратегии и тактические ролевые');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Papers, Please' AND ge.Title IN (N'Симуляторы хобби и работы', N'Приключения', N'Казуальные');

    -- AAA
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Red Dead Redemption 2' AND ge.Title IN (N'Шутер от третьего лица', N'Приключения', N'Песочницы');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Doom Eternal' AND ge.Title IN (N'Шутер от первого лица', N'Научная фантастика', N'Хорроры');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'BioShock Infinite' AND ge.Title IN (N'Шутер от первого лица', N'Научная фантастика', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Control' AND ge.Title IN (N'Шутер от третьего лица', N'Научная фантастика', N'Хорроры');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Dishonored 2' AND ge.Title IN (N'Шутер от первого лица', N'Стелс', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Cyberpunk 2077' AND ge.Title IN (N'Шутер от первого лица', N'Научная фантастика', N'Песочницы');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Prey (2017)' AND ge.Title IN (N'Шутер от первого лица', N'Научная фантастика', N'Хорроры');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Wolfenstein: The New Order' AND ge.Title IN (N'Шутер от первого лица', N'Научная фантастика', N'Приключения');

    -- Multi-Genre
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Sid Meier’s Civilization VI' AND ge.Title IN (N'Пошаговые стратегии', N'Стратегии и тактические ролевые');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Cities: Skylines' AND ge.Title IN (N'Симуляторы строительства и автоматизации', N'Стратегии и тактические ролевые');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Terraria' AND ge.Title IN (N'Песочницы', N'Платформеры', N'Выживание');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Oxygen Not Included' AND ge.Title IN (N'Симуляторы строительства и автоматизации', N'Выживание');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Factorio' AND ge.Title IN (N'Симуляторы строительства и автоматизации', N'Стратегии и тактические ролевые');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Subnautica' AND ge.Title IN (N'Выживание', N'Приключения', N'Научная фантастика');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Among Us' AND ge.Title IN (N'Казуальные', N'Приключения');
    INSERT INTO [dbo].[GameGenres] SELECT g.Id, ge.Id FROM @Games g CROSS JOIN @Genres ge WHERE g.Title = N'Outer Wilds' AND ge.Title IN (N'Головоломки', N'Приключения', N'Научная фантастика');
END;

------------------------------------------------------------
-- КЛЮЧИ (для первых 10 игр)
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Keys])
BEGIN
    DECLARE @GameIds TABLE (Id BIGINT);
    INSERT INTO @GameIds
    SELECT TOP 10 Id FROM [dbo].[Games] ORDER BY Id;

    INSERT INTO [dbo].[Keys] ([Value], [GameId])
    SELECT 
        CONCAT('KEY-', g.Id, '-', FORMAT(nums.n, '000')),
        g.Id
    FROM @GameIds g
    CROSS JOIN (SELECT TOP 8 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) n FROM sys.objects) nums;
END;

------------------------------------------------------------
-- КОРЗИНЫ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Carts])
BEGIN
    DECLARE @CustomerId1 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Customers]);
    IF @CustomerId1 IS NOT NULL
        INSERT INTO [dbo].[Carts] ([CustomerId]) VALUES (@CustomerId1);
END;

IF NOT EXISTS (SELECT 1 FROM [dbo].[CartItems])
BEGIN
    DECLARE @CartId1 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Carts]);
    DECLARE @GameId1 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Hollow Knight');
    IF @CartId1 IS NOT NULL AND @GameId1 IS NOT NULL
        INSERT INTO [dbo].[CartItems] ([GameId], [Quantity], [CartId]) VALUES (@GameId1, 1, @CartId1);
END;

------------------------------------------------------------
-- ЗАКАЗ
------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM [dbo].[Orders])
BEGIN
    DECLARE @CustomerId2 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Customers]);
    DECLARE @GameId2 BIGINT = (SELECT TOP 1 Id FROM [dbo].[Games] WHERE Title = N'Stardew Valley');
    DECLARE @Price2 DECIMAL(10,2) = (SELECT Price FROM [dbo].[Games] WHERE Id = @GameId2);
    
    IF @CustomerId2 IS NOT NULL AND @GameId2 IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[Orders] ([CustomerId], [TotalAmount], [CreatedAt])
        VALUES (@CustomerId2, @Price2, SYSUTCDATETIME());

        DECLARE @OrderId2 BIGINT = SCOPE_IDENTITY();
        INSERT INTO [dbo].[OrderItems] ([GameId], [Quantity], [Price], [OrderId], [Keys])
        VALUES (@GameId2, 1, @Price2, @OrderId2, N'["KEY-2-001"]');
    END;
END;