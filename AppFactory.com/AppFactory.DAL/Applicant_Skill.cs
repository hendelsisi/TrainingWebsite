//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppFactory.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Applicant_Skill
    {
        public int ID { get; set; }
        public long ApplicantID { get; set; }
        public int SkillID { get; set; }
        public int SkillLevel { get; set; }
    
        public virtual Applicant Applicant { get; set; }
        public virtual Skill Skill { get; set; }
    }
}