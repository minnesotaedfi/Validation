﻿using System;
using System.Web.Http.ModelBinding;
using ValidationWeb.ApiControllers.ModelBinders;
// ReSharper disable StyleCop.SA1300
namespace ValidationWeb.Models
{
    [Serializable]
    [ModelBinder(typeof(ValidationErrorFilterModelBinder))]
    public class ValidationErrorFilter
    {
        public int reportDetailsId { get; set; }

        public string[] filterColumns { get; set; }

        public string[] filterTexts { get; set; }

        public string[] sortColumns { get; set; }

        public string[] sortDirections { get; set; }

        public int pageStartingOffset { get; set; }

        public int pageSize { get; set; }

        public string autocompleteColumn { get; set; }

        public string autocompleteText { get; set; }
    }
}