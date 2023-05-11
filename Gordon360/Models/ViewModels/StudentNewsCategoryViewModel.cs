﻿using Gordon360.Models.MyGordon;
using System;

namespace Gordon360.Models.ViewModels
{
    public class StudentNewsCategoryViewModel
    {
        public int categoryID { get; set; }
        public string categoryName { get; set; }
        public Nullable<int> SortOrder { get; set; }

        public static implicit operator StudentNewsCategoryViewModel(StudentNewsCategory c)
        {
            StudentNewsCategoryViewModel vm = new StudentNewsCategoryViewModel
            {
                categoryID = c.categoryID,
                categoryName = c.categoryName,
                SortOrder = c.SortOrder,
            };

            return vm;
        }
    }
}