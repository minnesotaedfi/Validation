/*

Error on: 

When Course level types for grades K-8

If B is selected, then NOT E, G, X, A, C, D
If E is selected, then NOT B, G, X, A, C, D
If G is selected, then NOT B, E, X, A, C, D
IF N is selected, then NOT X, A, C, D
If X is selected, then NOT B, E, G, N, A, C, D
A is NOT a valid choice for K-8
C is NOT a valid choice for K-8

*/

DECLARE @RuleId VARCHAR(32) = '50.02.0002';
DECLARE @Message NVARCHAR(MAX) = 'Course level type combination error detected for course offered for grades K-8.';
DECLARE @IsError BIT = 1;

WITH 
course_level_types AS (

SELECT
 CourseLevelType.EducationOrganizationId,
 CourseLevelType.CourseCode,
 CourseLevel.CodeValue AS CourseLevelCodeValue
FROM 
 mn.CourseLevelType
 INNER JOIN 
 edfi.Descriptor CourseLevel
  ON CourseLevelTypeDescriptorId = CourseLevel.DescriptorId
 INNER JOIN
 edfi.CourseOfferedGradeLevel
  ON CourseOfferedGradeLevel.EducationOrganizationId = CourseLevelType.EducationOrganizationId
   AND CourseOfferedGradeLevel.CourseCode = CourseLevelType.CourseCode
 INNER JOIN
 edfi.GradeLevelDescriptor
  ON GradeLevelDescriptor.GradeLevelDescriptorId = CourseOfferedGradeLevel.GradeLevelDescriptorId
 INNER JOIN
 edfi.Descriptor GradeLevel
  ON GradeLevel.DescriptorId = GradeLevelDescriptor.GradeLevelDescriptorId
WHERE
 CourseLevelType.EducationOrganizationId = @DistrictId
 AND GradeLevel.CodeValue IN ('HK','KA','KB','KC','KD','KE','KF','KG','KI','KJ','KK','1','2','3','4','5','6','7','8')
GROUP BY
 CourseLevelType.EducationOrganizationId,
 CourseLevelType.CourseCode,
 CourseLevel.CodeValue

 ),
failed_rows AS (

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type B is selected for the course, E, G, X, A, C, or D cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'B'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('E','G','X','A','C','D')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type E is selected for the course, B, G, X, A, C, or D cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'E'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('B','G','X','A','C','D')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type G is selected for the course,B, E, X, A, C, or D cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'G'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('B','E','X','A','C','D')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type N is selected for the course, X, A, C, or D cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'N'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('X','A','C','D')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type X is selected for the course, B, E, G, N, A, C, or D cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'X'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('B','E','G','N','A','C','D')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'A cannot be selected on the course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'A'

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'C cannot be selected on the course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'C'

)
INSERT INTO 
 rules.RuleValidationDetail (RuleValidationId, Id, RuleId, IsError, [Message])
SELECT TOP 1
 @RuleValidationId, @DistrictId, @RuleId RuleId, @IsError IsError, 
 @Message + CHAR(13)+CHAR(10)+ (
  SELECT TOP 100 
   [Message],
   CourseCode
  FROM failed_rows [xml] 
  FOR XML RAW,
   ROOT ('Details')
  ) [Message]
FROM failed_rows;
