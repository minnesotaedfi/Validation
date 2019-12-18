DECLARE @RuleId NVARCHAR(50) = '10.10.6643';
DECLARE @RuleDescription NVARCHAR(MAX) = '430. The MARSS Enrollment data reported for this school would indicate this is a Middle school (students are reported in grades 7 or above), but the grades registered in MDEORG conflict with this designation for accountability purposes. Check the grade levels that are authorized for this school in MDEORG.';
DECLARE @IsError BIT = 0;


WITH School (Id)
AS (
Select distinct IDS.schoolid from (
select SE.schoolid, SE.DistrictId, SE.studentGradelevel
from rules.StudentEnrollment SE where 
DistrictType  NOT IN ('01', '03', '06', '07', '08', '34', '50', '51', '52', '53', '61', '62', '70', '83', '99') AND 
SchoolClassification  IN ('00', '10', '20', '31', '32', '33', '40', '41', '42', '43', '46', '50', '55', '60', '70', '71', '72', '73', '74', '76', '77', '78', '79') AND 
StudentGradeLevel IN ('7', '8', '9', '10', '11', '12') AND 
StateAidCategory NOT IN ('02', '14', '16', '17', '18', '28', '46', '52', '98') AND SE.districtID = @districtId

EXCEPT
select SG.schoolid, SG.DistrictId, SG.schoolGradelevel
from rules.schoolGrade SG join
rules.StudentEnrollment SE on 
SE.Schoolid = SG.SchoolId AND
SE.studentGradelevel = SG.SchoolGradeLevel
AND SG.DistrictID = @DistrictId) IDS)

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

