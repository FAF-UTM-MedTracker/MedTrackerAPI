﻿using System.ComponentModel.DataAnnotations;

namespace MedTracker.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string UPassword { get; set; }
        public bool IsDoctor { get; set; }

        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }    
    }
}