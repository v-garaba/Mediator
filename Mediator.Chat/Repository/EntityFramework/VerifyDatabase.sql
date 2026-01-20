-- Query to verify messages were stored in the database
-- Run this in SQL Server Management Studio or similar tool after running the application

USE ChatDb;
GO

-- Show all messages
SELECT
    Id,
    SenderId,
    Content,
    MessageType,
    TargetUserId,
    Timestamp
FROM Messages
ORDER BY Timestamp;

-- Show message count by type
SELECT
    CASE MessageType
        WHEN 0 THEN 'Public'
        WHEN 1 THEN 'Private'
        WHEN 2 THEN 'System'
    END AS MessageTypeDescription,
    COUNT(*) AS Count
FROM Messages
GROUP BY MessageType;

-- Show public messages
SELECT
    Content,
    Timestamp
FROM Messages
WHERE MessageType = 0
ORDER BY Timestamp;

-- Show private messages
SELECT
    Content,
    TargetUserId,
    Timestamp
FROM Messages
WHERE MessageType = 1
ORDER BY Timestamp;

-- Show system messages
SELECT
    Content,
    Timestamp
FROM Messages
WHERE MessageType = 2
ORDER BY Timestamp;
