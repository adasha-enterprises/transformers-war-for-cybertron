USE [WarForCybertron]
GO

/****** Object:  StoredProcedure [dbo].[GetTransformerScore]    Script Date: 2021-02-15 4:27:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTransformerScore] @Id UNIQUEIDENTIFIER, @Score INT OUTPUT

AS 

BEGIN

SELECT @Score = Strength + Intelligence + Speed + Endurance + [Rank] + Courage + Firepower + Skill FROM Transformers WHERE Id = @Id 

END
GO