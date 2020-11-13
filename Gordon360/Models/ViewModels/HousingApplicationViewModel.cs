/********************************
 * This file is manually created and may be edited
 * The view model allows access to all of the model's data without
 * anything unnecessary. It prevents a self-referencing loop error
 * in models that need a category that need a model etc.
 * The implicit operator allows conversion between the model and the view model
 ********************************/

using System;

namespace Gordon360.Models.ViewModels
{
    public class HousingApplicationViewModel
    {
        public string username { get; set; }
        public string [] applicants { get; set; }
    }
}