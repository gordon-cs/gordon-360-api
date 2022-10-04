﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Gordon360.Models.CCT;

namespace Gordon360.Models.CCT.Context
{
    public partial class CCTContext : DbContext
    {
        public CCTContext()
        {
        }

        public CCTContext(DbContextOptions<CCTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ACCOUNT> ACCOUNT { get; set; }
        public virtual DbSet<ACT_INFO> ACT_INFO { get; set; }
        public virtual DbSet<ADMIN> ADMIN { get; set; }
        public virtual DbSet<Alumni> Alumni { get; set; }
        public virtual DbSet<Birthdays> Birthdays { get; set; }
        public virtual DbSet<Buildings> Buildings { get; set; }
        public virtual DbSet<CM_SESSION_MSTR> CM_SESSION_MSTR { get; set; }
        public virtual DbSet<CUSTOM_PROFILE> CUSTOM_PROFILE { get; set; }
        public virtual DbSet<ChapelEvent> ChapelEvent { get; set; }
        public virtual DbSet<Clifton_Strengths> Clifton_Strengths { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<DiningInfo> DiningInfo { get; set; }
        public virtual DbSet<Dining_Meal_Choice_Desc> Dining_Meal_Choice_Desc { get; set; }
        public virtual DbSet<Dining_Meal_Plan_Change_History> Dining_Meal_Plan_Change_History { get; set; }
        public virtual DbSet<Dining_Meal_Plan_Id_Mapping> Dining_Meal_Plan_Id_Mapping { get; set; }
        public virtual DbSet<Dining_Mealplans> Dining_Mealplans { get; set; }
        public virtual DbSet<Dining_Student_Meal_Choice> Dining_Student_Meal_Choice { get; set; }
        public virtual DbSet<ERROR_LOG> ERROR_LOG { get; set; }
        public virtual DbSet<EmergencyContact> EmergencyContact { get; set; }
        public virtual DbSet<FacStaff> FacStaff { get; set; }
        public virtual DbSet<Graduation> Graduation { get; set; }
        public virtual DbSet<Health_Question> Health_Question { get; set; }
        public virtual DbSet<Health_Status> Health_Status { get; set; }
        public virtual DbSet<Health_Status_CTRL> Health_Status_CTRL { get; set; }
        public virtual DbSet<Housing_Admins> Housing_Admins { get; set; }
        public virtual DbSet<Housing_Applicants> Housing_Applicants { get; set; }
        public virtual DbSet<Housing_Applications> Housing_Applications { get; set; }
        public virtual DbSet<Housing_HallChoices> Housing_HallChoices { get; set; }
        public virtual DbSet<Housing_Halls> Housing_Halls { get; set; }
        public virtual DbSet<Information_Change_Request> Information_Change_Request { get; set; }
        public virtual DbSet<Internships_as_Involvements> Internships_as_Involvements { get; set; }
        public virtual DbSet<JENZ_ACT_CLUB_DEF> JENZ_ACT_CLUB_DEF { get; set; }
        public virtual DbSet<JNZB_ACTIVITIES> JNZB_ACTIVITIES { get; set; }
        public virtual DbSet<MEMBERSHIP> MEMBERSHIP { get; set; }
        public virtual DbSet<MYSCHEDULE> MYSCHEDULE { get; set; }
        public virtual DbSet<Mailboxes> Mailboxes { get; set; }
        public virtual DbSet<Majors> Majors { get; set; }
        public virtual DbSet<Message_Rooms> Message_Rooms { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<PART_DEF> PART_DEF { get; set; }
        public virtual DbSet<Police> Police { get; set; }
        public virtual DbSet<REQUEST> REQUEST { get; set; }
        public virtual DbSet<RoomAssign> RoomAssign { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<Save_Bookings> Save_Bookings { get; set; }
        public virtual DbSet<Save_Rides> Save_Rides { get; set; }
        public virtual DbSet<Schedule_Control> Schedule_Control { get; set; }
        public virtual DbSet<Slider_Images> Slider_Images { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentNewsExpiration> StudentNewsExpiration { get; set; }
        public virtual DbSet<Timesheets_Clock_In_Out> Timesheets_Clock_In_Out { get; set; }
        public virtual DbSet<User_Connection_Ids> User_Connection_Ids { get; set; }
        public virtual DbSet<User_Rooms> User_Rooms { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<_360_SLIDER> _360_SLIDER { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACCOUNT>(entity =>
            {
                entity.ToView("ACCOUNT");
            });

            modelBuilder.Entity<ACT_INFO>(entity =>
            {
                entity.HasKey(e => e.ACT_CDE)
                    .HasName("PK_Activity_Info");

                entity.Property(e => e.ACT_CDE).IsFixedLength();

                entity.Property(e => e.ACT_DESC).IsFixedLength();

                entity.Property(e => e.ACT_TYPE).IsFixedLength();

                entity.Property(e => e.ACT_TYPE_DESC).IsFixedLength();
            });

            modelBuilder.Entity<ADMIN>(entity =>
            {
                entity.HasKey(e => e.ADMIN_ID)
                    .HasName("PK_Admin");
            });

            modelBuilder.Entity<Alumni>(entity =>
            {
                entity.ToView("Alumni");

                entity.Property(e => e.grad_student).IsFixedLength();
            });

            modelBuilder.Entity<Birthdays>(entity =>
            {
                entity.ToView("Birthdays");
            });

            modelBuilder.Entity<Buildings>(entity =>
            {
                entity.ToView("Buildings");

                entity.Property(e => e.BLDG_CDE).IsFixedLength();

                entity.Property(e => e.BUILDING_DESC).IsFixedLength();
            });

            modelBuilder.Entity<CM_SESSION_MSTR>(entity =>
            {
                entity.ToView("CM_SESSION_MSTR");

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<ChapelEvent>(entity =>
            {
                entity.ToView("ChapelEvent");
            });

            modelBuilder.Entity<Clifton_Strengths>(entity =>
            {
                entity.HasKey(e => new { e.ID_NUM, e.ACCESS_CODE })
                    .HasName("PK_CliftonStrengths");

                entity.Property(e => e.Private).HasComment("Whether the user wants their strengths to be private (not shown to other users)");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.ToView("Countries");
            });

            modelBuilder.Entity<DiningInfo>(entity =>
            {
                entity.ToView("DiningInfo");

                entity.Property(e => e.ChoiceDescription).IsFixedLength();

                entity.Property(e => e.SessionCode).IsFixedLength();
            });

            modelBuilder.Entity<Dining_Meal_Choice_Desc>(entity =>
            {
                entity.ToView("Dining_Meal_Choice_Desc");

                entity.Property(e => e.Meal_Choice_Desc).IsFixedLength();

                entity.Property(e => e.Meal_Choice_Id).IsFixedLength();
            });

            modelBuilder.Entity<Dining_Meal_Plan_Change_History>(entity =>
            {
                entity.ToView("Dining_Meal_Plan_Change_History");

                entity.Property(e => e.OLD_PLAN_DESC).IsFixedLength();
            });

            modelBuilder.Entity<Dining_Meal_Plan_Id_Mapping>(entity =>
            {
                entity.ToView("Dining_Meal_Plan_Id_Mapping");
            });

            modelBuilder.Entity<Dining_Mealplans>(entity =>
            {
                entity.ToView("Dining_Mealplans");
            });

            modelBuilder.Entity<Dining_Student_Meal_Choice>(entity =>
            {
                entity.ToView("Dining_Student_Meal_Choice");

                entity.Property(e => e.MEAL_CHOICE_ID).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<EmergencyContact>(entity =>
            {
                entity.ToView("EmergencyContact");

                entity.Property(e => e.AddressAddrCode).IsFixedLength();

                entity.Property(e => e.ApprowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.EmailAddrCode).IsFixedLength();

                entity.Property(e => e.HomeAddrCode).IsFixedLength();

                entity.Property(e => e.HomeExt).IsFixedLength();

                entity.Property(e => e.HomePhone).IsFixedLength();

                entity.Property(e => e.MobileAddrCode).IsFixedLength();

                entity.Property(e => e.MobileExt).IsFixedLength();

                entity.Property(e => e.MobilePhone).IsFixedLength();

                entity.Property(e => e.WorkAddrCode).IsFixedLength();

                entity.Property(e => e.WorkExr).IsFixedLength();

                entity.Property(e => e.WorkPhone).IsFixedLength();
            });

            modelBuilder.Entity<FacStaff>(entity =>
            {
                entity.ToView("FacStaff");

                entity.Property(e => e.BuildingDescription).IsFixedLength();
            });

            modelBuilder.Entity<Graduation>(entity =>
            {
                entity.ToView("Graduation");
            });

            modelBuilder.Entity<Health_Status>(entity =>
            {
                entity.HasKey(e => new { e.Created, e.ID_Num })
                    .HasName("PK__Health_S__6CC83B6815B808A2");

                entity.HasOne(d => d.HealthStatus)
                    .WithMany(p => p.Health_Status)
                    .HasForeignKey(d => d.HealthStatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Health_Status_HealthStatus_CTRL");
            });

            modelBuilder.Entity<Housing_Applicants>(entity =>
            {
                entity.HasKey(e => new { e.HousingAppID, e.Username });

                entity.Property(e => e.SESS_CDE).IsFixedLength();

                entity.HasOne(d => d.HousingApp)
                    .WithMany(p => p.Housing_Applicants)
                    .HasForeignKey(d => d.HousingAppID)
                    .HasConstraintName("FK_Applicants_HousingAppID");
            });

            modelBuilder.Entity<Housing_HallChoices>(entity =>
            {
                entity.HasOne(d => d.HousingApp)
                    .WithMany()
                    .HasForeignKey(d => d.HousingAppID)
                    .HasConstraintName("FK_HallChoices_HousingAppID");
            });

            modelBuilder.Entity<Information_Change_Request>(entity =>
            {
                entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Internships_as_Involvements>(entity =>
            {
                entity.ToView("Internships_as_Involvements");

                entity.Property(e => e.SESS_CDE).IsFixedLength();

                entity.Property(e => e.TRM_CDE).IsFixedLength();

                entity.Property(e => e.YR_CDE).IsFixedLength();
            });

            modelBuilder.Entity<JENZ_ACT_CLUB_DEF>(entity =>
            {
                entity.ToView("JENZ_ACT_CLUB_DEF");

                entity.Property(e => e.ACT_CDE).IsFixedLength();

                entity.Property(e => e.ACT_DESC).IsFixedLength();

                entity.Property(e => e.ACT_TYPE).IsFixedLength();

                entity.Property(e => e.ACT_TYPE_DESC).IsFixedLength();
            });

            modelBuilder.Entity<JNZB_ACTIVITIES>(entity =>
            {
                entity.Property(e => e.ACT_CDE).IsFixedLength();

                entity.Property(e => e.INCL_PROFILE_RPT).IsFixedLength();

                entity.Property(e => e.JOB_NAME).IsFixedLength();

                entity.Property(e => e.MEMBERSHIP_STS).IsFixedLength();

                entity.Property(e => e.PART_CDE).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();

                entity.Property(e => e.TRACK_MTG_ATTEND).IsFixedLength();

                entity.Property(e => e.USER_NAME).IsFixedLength();
            });

            modelBuilder.Entity<MEMBERSHIP>(entity =>
            {
                entity.HasKey(e => e.MEMBERSHIP_ID)
                    .HasName("PK_Membership");

                entity.Property(e => e.ACT_CDE).IsFixedLength();

                entity.Property(e => e.JOB_NAME).IsFixedLength();

                entity.Property(e => e.PART_CDE).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();

                entity.Property(e => e.USER_NAME).IsFixedLength();
            });

            modelBuilder.Entity<MYSCHEDULE>(entity =>
            {
                entity.HasKey(e => new { e.EVENT_ID, e.GORDON_ID });
            });

            modelBuilder.Entity<Mailboxes>(entity =>
            {
                entity.ToView("Mailboxes");
            });

            modelBuilder.Entity<Majors>(entity =>
            {
                entity.ToView("Majors");
            });

            modelBuilder.Entity<Message_Rooms>(entity =>
            {
                entity.HasKey(e => e.room_id)
                    .HasName("PK__Message___19675A8A3E781488");
            });

            modelBuilder.Entity<PART_DEF>(entity =>
            {
                entity.ToView("PART_DEF");

                entity.Property(e => e.PART_CDE).IsFixedLength();

                entity.Property(e => e.PART_DESC).IsFixedLength();
            });

            modelBuilder.Entity<Police>(entity =>
            {
                entity.ToView("Police");
            });

            modelBuilder.Entity<REQUEST>(entity =>
            {
                entity.HasKey(e => e.REQUEST_ID)
                    .HasName("PK_Request");

                entity.Property(e => e.ACT_CDE).IsFixedLength();

                entity.Property(e => e.PART_CDE).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<RoomAssign>(entity =>
            {
                entity.ToView("RoomAssign");

                entity.Property(e => e.BLDG_CDE).IsFixedLength();

                entity.Property(e => e.BLDG_LOC_CDE).IsFixedLength();

                entity.Property(e => e.ROOM_ASSIGN_STS).IsFixedLength();

                entity.Property(e => e.ROOM_CDE).IsFixedLength();

                entity.Property(e => e.ROOM_TYPE).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<Save_Bookings>(entity =>
            {
                entity.HasKey(e => new { e.ID, e.rideID });

                entity.HasOne(d => d.ride)
                    .WithMany(p => p.Save_Bookings)
                    .HasForeignKey(d => d.rideID)
                    .HasConstraintName("FK_booking_rides");
            });

            modelBuilder.Entity<Schedule_Control>(entity =>
            {
                entity.Property(e => e.IsSchedulePrivate).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.ToView("States");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToView("Student");

                entity.Property(e => e.BuildingDescription).IsFixedLength();
            });

            modelBuilder.Entity<StudentNewsExpiration>(entity =>
            {
                entity.HasKey(e => e.SNID)
                    .HasName("PK_StudentNewsExpiration_SNID");

                entity.Property(e => e.SNID).ValueGeneratedNever();
            });

            modelBuilder.Entity<_360_SLIDER>(entity =>
            {
                entity.ToView("360_SLIDER");
            });

            modelBuilder.HasSequence("Information_Change_Request_Seq");

            OnModelCreatingGeneratedProcedures(modelBuilder);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}