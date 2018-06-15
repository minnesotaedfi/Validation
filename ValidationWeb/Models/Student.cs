using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    [Table("validation.Student")]
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The StudentUniqueId is not globally unique, but unique just to one ODS database.
        /// </summary>
        public string StudentUniqueId { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }

        public string GetFullName()
        {
            var hasMiddleName = string.IsNullOrWhiteSpace(MiddleName);
            return hasMiddleName
                ? $"{FirstName} {LastName}"
                : $"{FirstName} {MiddleName} {LastName}";
        }

        public string GetFullNameSuffix()
        {
            var hasSuffix = string.IsNullOrWhiteSpace(Suffix);
            var is2nd3rd4th = hasSuffix && Suffix.StartsWith("I");
            var commaSuffix = (hasSuffix && !(is2nd3rd4th)) ? $", {Suffix}" : (hasSuffix ? $" Suffix" : string.Empty);
            return $"{GetFullName()}{commaSuffix}";
        }

        public string GetTitleFullNameSuffix()
        {
            var title = (string.IsNullOrWhiteSpace(Title)) ? string.Empty : $"{Title} ";
            return $"{GetFullNameSuffix()}{title}";
        }
    }
}