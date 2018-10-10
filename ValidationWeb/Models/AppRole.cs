using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb
{
    /// <summary>
    /// Role information and definitions are stored in the Minnesota SSO database. 
    /// </summary>
    [Serializable]
    public class AppRole
    {
        public static AppRole Unauthorized = CreateAppRole("Unauthorized");
        public static AppRole SummaryViewer = CreateAppRole("SummaryViewer");
        public static AppRole Viewer = CreateAppRole("Viewer");
        public static AppRole Launcher = CreateAppRole("Launcher");
        public static AppRole Administrator = CreateAppRole("Administrator");
        const bool CaseInsensitive = true;

        public static AppRole CreateAppRole(string name)
        {
            if (string.Compare(name, "SummaryViewer", CaseInsensitive) == 0)
            {
                return new AppRole { Name = "SummaryViewer" };
            }
            if (string.Compare(name, "Viewer", CaseInsensitive) == 0)
            {
                return new AppRole { Name = "Viewer" };
            }
            if (string.Compare(name, "Launcher", CaseInsensitive) == 0)
            {
                return new AppRole { Name = "Launcher" };
            }
            if (string.Compare(name, "Administrator", CaseInsensitive) == 0)
            {
                return new AppRole { Name = "Administrator" };
            }
            return new AppRole { Name = "Unauthorized" };
        }

        #region Operator Overloads
        public static bool operator >=(AppRole leftHandSide, AppRole rightHandSide)
        {
            return !(leftHandSide <= rightHandSide) || (leftHandSide.Name == rightHandSide.Name);
        }

        public static bool operator <=(AppRole leftHandSide, AppRole rightHandSide)
        {
            if (leftHandSide.Name == Administrator.Name)
            {
                return rightHandSide.Name == Administrator.Name;
            }
            if (leftHandSide.Name == Launcher.Name)
            {
                return rightHandSide.Name == Administrator.Name
                    || rightHandSide.Name == Launcher.Name;
            }
            if (leftHandSide.Name == Viewer.Name)
            {
                return rightHandSide.Name == Administrator.Name
                    || rightHandSide.Name == Launcher.Name
                    || rightHandSide.Name == Viewer.Name;
            }
            if (leftHandSide.Name == SummaryViewer.Name)
            {
                return rightHandSide.Name == Administrator.Name
                    || rightHandSide.Name == Launcher.Name
                    || rightHandSide.Name == Viewer.Name
                    || rightHandSide.Name == SummaryViewer.Name;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return (obj as AppRole)?.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        #endregion Operator Overloads

        public string Name { get; set; }
    }
}