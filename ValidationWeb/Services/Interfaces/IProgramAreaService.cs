using System.Collections.Generic;

using Validation.DataModels;

namespace ValidationWeb.Services.Interfaces
{
    public interface IProgramAreaService
    {
        IList<ProgramAreaLookup> GetProgramAreas();
        ProgramAreaLookup GetProgramAreaById(int programAreaId);
    }
}