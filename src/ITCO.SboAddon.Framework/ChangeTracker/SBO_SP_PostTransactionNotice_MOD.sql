-- // T-SQL //

-- Filter Object Types
IF @object_type IN('2', '4', '17', '15') -- BP, Items, Orders, Deliveries
BEGIN
    DECLARE @timestamp integer, @expired integer, @startdate date = '2015-01-01', @key nvarchar(30)

    SET @timestamp = DATEDIFF(SECOND, @startdate, GETDATE())
    SET @expired = DATEDIFF(SECOND, @startdate, DATEADD(DAY, -7, GETDATE()))
    SET @key = CAST(@list_of_cols_val_tab_del AS nvarchar(30));

	-- Cleanup old records, expired or same key/object
    DELETE FROM [@ITCO_CHANGETRACKER] WHERE([U_ITCO_CT_Key] = @key AND [U_ITCO_CT_Obj] = CAST(@object_type AS nvarchar(30))) OR CAST([Code] AS int) < @expired

	-- Since [Code] is unique key, we have to ensure its unique
	WHILE EXISTS(SELECT 1 FROM[@ITCO_CHANGETRACKER] WHERE [Code] = @timestamp)
		SET @timestamp = @timestamp + 1

    INSERT INTO [@ITCO_CHANGETRACKER] VALUES(@timestamp, @timestamp, @key, CAST(@object_type AS nvarchar(30)))
END


-- // HANA //
BEGIN
DECLARE trackedObjectType int;
SELECT COUNT(1) INTO trackedObjectType FROM "DUMMY" WHERE object_type IN('2', '4', '17', '15'); -- BP, Items, Orders, Deliveries

IF :trackedObjectType > 0
THEN
	DECLARE startdate date = '2015-01-01';
    DECLARE timestampValue int = SECONDS_BETWEEN(:startdate, NOW());
    DECLARE timestampValueExists int = 0;
    DECLARE expired int = SECONDS_BETWEEN(:startdate, ADD_DAYS(NOW(), -7) );
	DECLARE entityKey nvarchar(30)= CAST(:list_of_cols_val_tab_del AS nvarchar(30));

	-- Cleanup old records, expired or same key/object
    DELETE FROM "@ITCO_CHANGETRACKER" WHERE ("U_ITCO_CT_Key" = :entityKey AND "U_ITCO_CT_Obj" = CAST(:object_type AS nvarchar(30))) OR CAST("Code" AS int) < :expired;
    
	-- Since "Code" is unique key, we have to ensure its unique
	SELECT COUNT(1) INTO timestampValueExists FROM "@ITCO_CHANGETRACKER" WHERE "Code" = :timestampValue;
	WHILE :timestampValueExists > 0 DO
		timestampValue := timestampValue + 1;
		SELECT COUNT(1) INTO timestampValueExists FROM "@ITCO_CHANGETRACKER" WHERE "Code" = :timestampValue;
	END WHILE;

    INSERT INTO "@ITCO_CHANGETRACKER" VALUES(:timestampValue, :timestampValue, :entityKey, CAST(:object_type AS nvarchar(30)));
    
END IF;
END;