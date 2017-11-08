using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppFactory.MVC.Models
{
    public class UserSkill
    {
        public int SkillLevel { get; set; }
        public int SkillID { get; set; }
    }

    public class SkillModel
    {
        public int ApplicantID { get; set; }
        public int firstSkillID { get; set; }
        public int firstSkilllevel { get; set; }
        public IList<UserSkill> NewSkill { get; set; }

        public SkillModel()
        {

            NewSkill = new List<UserSkill>();
        }


    }
}