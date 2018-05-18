using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ValidationWeb
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var mde = new EdOrg { Id = "MDE", Name = "MN Department of Education", Parent = null, Type = EdOrgType.State };
            var sRegion = new EdOrg { Id = "South", Name = "Southern Region", Parent = mde, Type = EdOrgType.Region };
            var d622 = new EdOrg { Id = "ISD 622", Name = "North St. Paul-Maplewood School District", Parent = sRegion, Type = EdOrgType.District };
            var d625 = new EdOrg { Id = "ISD 625", Name = "St. Paul School District", Parent = sRegion, Type = EdOrgType.District };
            var s6221 = new EdOrg { Id = "School 622-1", Name = "Eagle Point Elementary School", Parent = d622, Type = EdOrgType.School };
            var s6222 = new EdOrg { Id = "School 622-2", Name = "John Glenn Middle School", Parent = d622, Type = EdOrgType.School };

            var model = new HomeIndexViewModel
            {
                Announcements = new Announcement[]
                {
                    new Announcement
                    {
                        Priority = 0,
                        ContactInfo = "info@education.mn.gov",
                        IsEmergency = false,
                        LinkUrl = "https://education.mn.gov/",
                        Message = "You may know about the nice Department of Education web page. But have you been there lately?\r\nWhy don't you go have a look now?"
                    },
                    new Announcement
                    {
                        Priority = 1,
                        ContactInfo = "info@education.mn.gov",
                        IsEmergency = true,
                        LinkUrl = "https://education.mn.gov/",
                        Message = "A volcano erupted in Hawaii!"
                    }
                },
                YearsOpenForDataSubmission = new List<SchoolYear>
                {
                    new SchoolYear("2018", "2019"),
                    new SchoolYear("2019", "2020")
                },
                AuthorizedEdOrgs = new EdOrg[]
                {
                    mde, sRegion, d622, d625, s6221, s6222
                },
                FilteredEdOrgs = new EdOrg[]
                {
                    d622
                }
            };
            return View(model);
        }

        public ActionResult Announcements()
        {
            var model = new HomeAnnouncementsViewModel
            {
                Announcements = new Announcement[]
                {
                    new Announcement
                    {
                        Priority = 0,
                        ContactInfo = "info@education.mn.gov",
                        IsEmergency = false,
                        LinkUrl = "https://education.mn.gov/",
                        Message = "You may know about the nice Department of Education web page. But have you been there lately?\r\nWhy don't you go have a look now?"
                    },
                    new Announcement
                    {
                        Priority = 1,
                        ContactInfo = "info@education.mn.gov",
                        IsEmergency = true,
                        LinkUrl = "https://education.mn.gov/",
                        Message = "A volcano erupted in Hawaii!"
                    }
                }
            };
            return View(model);
        }
    }
}
