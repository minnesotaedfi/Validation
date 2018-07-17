using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValidationWeb.Services
{
    public interface IRulesEngineService
    {
        ValidationReportSummary RunEngine(string fourDigitOdsDbYear, string collectionId);
        List<Collection> GetCollections();
    }
}