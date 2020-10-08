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
    public class StudentHousingInfoViewModel
    {
        public int ID { get; set; } //TODO change type to string, since this is type used in studentProfileViewModel
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public char OnOffCampus { get; set; } //TODO change type to string, since this is type used in studentProfileViewModel
        public string BuildingDescription { get; set; }
        public string OnCampusRoom { get; set; }
        public char Gender { get; set; } //TODO change type to string, since this is type used in studentProfileViewModel
        public string Email { get; set; }
       
        public static implicit operator StudentHousingInfoViewModel(HousingInfo h)
        {
            StudentHousingInfoViewModel vm = new StudentHousingInfoViewModel
            {
                ID = h.ID,
                Title = h.Title,
                FirstName = h.FirstName,
                LastName = h.LastName,
                OnOffCampus = h.OnOffCampus,
                BuildingDescription = h.BuildingDescriptionID,
                OnCampusRoom = h.OnCampusRoom,
                Gender = h.Gender,
                Email = h.Email,
            };

            return vm;
        }
    }
}