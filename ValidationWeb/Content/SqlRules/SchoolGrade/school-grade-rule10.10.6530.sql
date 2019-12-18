DECLARE @RuleId NVARCHAR(50) = '10.10.6530';
DECLARE @RuleDescription NVARCHAR(MAX) = '149. No students were reported in one or more grades for this school, but there exists a SCH/GRD record in the school file. Confirm that there are no students to be reported for this grade in this school.';
DECLARE @IsError BIT = 0;
DECLARE @DistrictId INT = 10625000;
DECLARE @RuleValidationId INT = 89;

WITH School (Id)
AS (
Select distinct IDS.schoolid from (
select SG.schoolid, SG.DistrictId, SG.SchoolGradeLevel
from rules.SchoolGrade SG where 
DistrictType  NOT IN ('01', '03', '06', '07', '08', '34', '50', '51', '52', '53', '61', '62', '70', '83', '99') AND 
SchoolClassification  IN ('00', '10', '20', '31', '32', '33', '40', '41', '42', '43', '46', '50', '55', '60', '70', '71', '72', '73', '74', '76', '77', '78', '79') AND
SchoolGradeLevel not in ('PS') AND
SG.DistrictId = @DistrictId
EXCEPT
select SE.schoolid, SE.DistrictId, SE.StudentGradeLevel
from rules.StudentEnrollment SE join
rules.SchoolGrade SG on 
SE.Schoolid = SG.SchoolId AND 
SE.studentGradelevel = SG.SchoolGradeLevel AND
SE.DistrictId = @DistrictId) IDS)

INSERT INTO
	rules.RuleValidationDetail (
		RuleValidationId,
		Id,
		RuleId,
		IsError,
		[Message]
		) 
SELECT
	@RuleValidationId,
	School.Id,
	@RuleId,
	@IsError,
	@RuleDescription
FROM 
	School

