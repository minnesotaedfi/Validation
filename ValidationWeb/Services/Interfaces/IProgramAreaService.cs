using System.Collections.Generic;

using Validation.DataModels;

namespace ValidationWeb.Services.Interfaces
{
    public interface IProgramAreaService
    {
        IList<ProgramArea> GetProgramAreas();
        ProgramArea GetProgramAreaById(int programAreaId);
    }
}