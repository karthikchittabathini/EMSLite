using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Models
{
    [Table("Employee")]
    public partial class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string FirstName { get; set; } = null!;
        [StringLength(20)]
        [Unicode(false)]
        public string LastName { get; set; } = null!;
        [StringLength(20)]
        [Unicode(false)]
        public string Designation { get; set; } = null!;
        [StringLength(20)]
        [Unicode(false)]
        public string OfficeLocation { get; set; } = null!;
        [StringLength(20)]
        [Unicode(false)]
        public string MobileNumber { get; set; } = null!;
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string Gender { get; set; } = null!;
        [StringLength(30)]
        [Unicode(false)]
        public string Manager { get; set; } = null!;
    }
}
