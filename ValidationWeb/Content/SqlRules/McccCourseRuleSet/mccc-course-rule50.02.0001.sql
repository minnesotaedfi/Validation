/*

Error on: 

When Course grade level offered are within grades 9-12, and

If A is selected, then NOT B,E, G, X
If B is selected, then NOT A,C, D, E, G, X
If C is selected, then NOT B,E, G, X
If D is selected, then NOT B,E, G, X
If E is selected, then NOT A,B, C, D, G, X
If G is selected, then NOT A,B, C, D, E, X
If N is selected, then NOT X
If X is selected, then NOT A, B, C, D, E, G, N
P is not a valid choice for 9-12

*/

DECLARE @RuleId VARCHAR(32) = '50.02.0001';
DECLARE @Message NVARCHAR(MAX) = 'For course offered for grades 9-12, ';
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
 AND GradeLevel.CodeValue IN ('9','10','11','12')
GROUP BY
 CourseLevelType.EducationOrganizationId,
 CourseLevelType.CourseCode,
 CourseLevel.CodeValue

 ),
failed_rows AS (

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type A is selected for the course, B, E, G, or X cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'A'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('B','E','G','X')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type B is selected for the course, A, C, D, E, G, or X cannot also be selected on the same course.' [Message]
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
   AND inside.CourseLevelCodeValue IN ('A','C','D','E','G','X')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type D is selected for the course, B, E, G, or X cannot also be selected on the same course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'D'
 AND EXISTS (
  SELECT 1
  FROM
   course_level_types inside
  WHERE
   inside.EducationOrganizationId = course_level_types.EducationOrganizationId
   AND inside.CourseCode = course_level_types.CourseCode
   AND inside.CourseLevelCodeValue IN ('B','E','G','X')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type E is selected for the course, A, B, C, D, G, or X cannot also be selected on the same course.' [Message]
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
   AND inside.CourseLevelCodeValue IN ('A','B','C','D','G','X')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type G is selected for the course, A, B, C, D, E, or X cannot also be selected on the same course.' [Message]
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
   AND inside.CourseLevelCodeValue IN ('A','B','C','D','E','X')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type N is selected for the course, X cannot also be selected on the same course.' [Message]
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
   AND inside.CourseLevelCodeValue = 'X'
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'if course level type X is selected for the course, A, B, C, D, E, G, or N cannot also be selected on the same course.' [Message]
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
   AND inside.CourseLevelCodeValue IN ('A','B','C','D','E','G','N')
   )

UNION ALL

SELECT
 EducationOrganizationId,
 CourseCode,
 'P cannot be selected on the course.' [Message]
FROM
 course_level_types
WHERE
 CourseLevelCodeValue = 'P'

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
   ROOT ('failedRows')
  ) [Message]
FROM failed_rows;
