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
    
    public partial class Applicant_Language
    {
        public int ID { get; set; }
        public long ApplicantID { get; set; }
        public int LanguageID { get; set; }
        public int SpokenLevel { get; set; }
        public int WrittenLevel { get; set; }
    
        public virtual Applicant Applicant { get; set; }
        public virtual Language Language { get; set; }
    }
}
