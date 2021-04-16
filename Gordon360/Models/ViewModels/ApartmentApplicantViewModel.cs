﻿using Gordon360.Repositories;
using Gordon360.Static.Data;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    // The view model used to send/receive apartment applicant data to/from the frontend
    public class ApartmentApplicantViewModel
    {
        public int ApplicationID { get; set; }
        public PublicStudentProfileViewModel Profile { get; set; }
        private string _username;
        public string Username
        {
            get => Profile?.AD_Username ?? _username;
            set => _username = value;
        }
        public DateTime? BirthDate { get; set; }
        public int? Age { get; set; } // To be calculated from the birthday
        public string Class => Profile?.Class ?? null;
        public string OffCampusProgram { get; set; }
        public bool Probation { get; set; }
        public int Points { get; set; }

        public static implicit operator ApartmentApplicantViewModel(GET_AA_APPLICANTS_BY_APPID_Result applicantDBModel)
        {
            ApartmentApplicantViewModel applicantModel = new ApartmentApplicantViewModel
            {
                ApplicationID = applicantDBModel.AprtAppID,
                Username = applicantDBModel.Username,
                // search username in cached data
                Profile = (StudentProfileViewModel)Data.StudentData.FirstOrDefault(x => x.AD_Username.ToLower() == applicantDBModel.Username.ToLower()),
                OffCampusProgram = applicantDBModel.AprtProgram,
                Probation = false, // Initialize to false. The actual value is determine and set in HousingService iff the user is housing admin
                Points = 0, // Initialize to zero. The point actual points are calculated in HousingService
            };

            return applicantModel;
        }
    }
}