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
    
    public partial class Application_Applicant
    {
        public long ID { get; set; }
        public long ApplicantID { get; set; }
        public long ApplicationID { get; set; }
        public System.DateTime SubmissionDate { get; set; }
        public int Status { get; set; }
        public int Step { get; set; }
        public string Location { get; set; }
    
        public virtual Applicant Applicant { get; set; }
        public virtual Application Application { get; set; }
    }
}