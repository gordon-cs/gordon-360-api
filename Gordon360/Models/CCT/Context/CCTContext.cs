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
        public virtual DbSet<AccountPhotoURL> AccountPhotoURL { get; set; }
        public virtual DbSet<Activity> Activity { get; set; }
        public virtual DbSet<ActivityStatus> ActivityStatus { get; set; }
        public virtual DbSet<ActivityType> ActivityType { get; set; }
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
        public virtual DbSet<Housing_Applicants> Housing_Applicants { get; set; }
        public virtual DbSet<Housing_Applications> Housing_Applications { get; set; }
        public virtual DbSet<Housing_HallChoices> Housing_HallChoices { get; set; }
        public virtual DbSet<Housing_Halls> Housing_Halls { get; set; }
        public virtual DbSet<Information_Change_Request> Information_Change_Request { get; set; }
        public virtual DbSet<Internships_as_Involvements> Internships_as_Involvements { get; set; }
        public virtual DbSet<InvolvementOffering> InvolvementOffering { get; set; }
        public virtual DbSet<JENZ_ACT_CLUB_DEF> JENZ_ACT_CLUB_DEF { get; set; }
        public virtual DbSet<JNZB_ACTIVITIES> JNZB_ACTIVITIES { get; set; }
        public virtual DbSet<MEMBERSHIP> MEMBERSHIP { get; set; }
        public virtual DbSet<MYSCHEDULE> MYSCHEDULE { get; set; }
        public virtual DbSet<Mailboxes> Mailboxes { get; set; }
        public virtual DbSet<Majors> Majors { get; set; }
        public virtual DbSet<Match> Match { get; set; }
        public virtual DbSet<MatchParticipant> MatchParticipant { get; set; }
        public virtual DbSet<MatchStatus> MatchStatus { get; set; }
        public virtual DbSet<MatchTeam> MatchTeam { get; set; }
        public virtual DbSet<MatchTeamStatus> MatchTeamStatus { get; set; }
        public virtual DbSet<MembershipView> MembershipView { get; set; }
        public virtual DbSet<Message_Rooms> Message_Rooms { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Minors> Minors { get; set; }
        public virtual DbSet<PART_DEF> PART_DEF { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<ParticipantActivity> ParticipantActivity { get; set; }
        public virtual DbSet<ParticipantNotification> ParticipantNotification { get; set; }
        public virtual DbSet<ParticipantStatus> ParticipantStatus { get; set; }
        public virtual DbSet<ParticipantStatusHistory> ParticipantStatusHistory { get; set; }
        public virtual DbSet<ParticipantTeam> ParticipantTeam { get; set; }
        public virtual DbSet<Police> Police { get; set; }
        public virtual DbSet<PrivType> PrivType { get; set; }
        public virtual DbSet<REQUEST> REQUEST { get; set; }
        public virtual DbSet<RequestView> RequestView { get; set; }
        public virtual DbSet<RoleType> RoleType { get; set; }
        public virtual DbSet<RoomAssign> RoomAssign { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<Save_Bookings> Save_Bookings { get; set; }
        public virtual DbSet<Save_Rides> Save_Rides { get; set; }
        public virtual DbSet<Schedule_Control> Schedule_Control { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<SeriesSchedule> SeriesSchedule { get; set; }
        public virtual DbSet<SeriesStatus> SeriesStatus { get; set; }
        public virtual DbSet<SeriesSurface> SeriesSurface { get; set; }
        public virtual DbSet<SeriesTeam> SeriesTeam { get; set; }
        public virtual DbSet<SeriesType> SeriesType { get; set; }
        public virtual DbSet<Slider_Images> Slider_Images { get; set; }
        public virtual DbSet<Sport> Sport { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<Statistic> Statistic { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentNewsExpiration> StudentNewsExpiration { get; set; }
        public virtual DbSet<Surface> Surface { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<TeamStatus> TeamStatus { get; set; }
        public virtual DbSet<Timesheets_Clock_In_Out> Timesheets_Clock_In_Out { get; set; }
        public virtual DbSet<UserPrivacy_Fields> UserPrivacy_Fields { get; set; }
        public virtual DbSet<UserPrivacy_Settings> UserPrivacy_Settings { get; set; }
        public virtual DbSet<UserPrivacy_Visibility_Groups> UserPrivacy_Visibility_Groups { get; set; }
        public virtual DbSet<User_Connection_Ids> User_Connection_Ids { get; set; }
        public virtual DbSet<User_Rooms> User_Rooms { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ACCOUNT>(entity =>
            {
                entity.ToView("ACCOUNT", "dbo");
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

            modelBuilder.Entity<AccountPhotoURL>(entity =>
            {
                entity.ToView("AccountPhotoURL", "dbo");
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasOne(d => d.SeriesSchedule)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.SeriesScheduleID)
                    .HasConstraintName("FK_Activity_SeriesSchedule");

                entity.HasOne(d => d.Sport)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.SportID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Activity_Sport");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Activity_ActivityStatus");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.TypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Activity_ActivityType");
            });

            modelBuilder.Entity<Alumni>(entity =>
            {
                entity.ToView("Alumni", "dbo");

                entity.Property(e => e.grad_student).IsFixedLength();
            });

            modelBuilder.Entity<Birthdays>(entity =>
            {
                entity.ToView("Birthdays", "dbo");
            });

            modelBuilder.Entity<Buildings>(entity =>
            {
                entity.ToView("Buildings", "dbo");

                entity.Property(e => e.BLDG_CDE).IsFixedLength();

                entity.Property(e => e.BUILDING_DESC).IsFixedLength();
            });

            modelBuilder.Entity<CM_SESSION_MSTR>(entity =>
            {
                entity.ToView("CM_SESSION_MSTR", "dbo");

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<ChapelEvent>(entity =>
            {
                entity.ToView("ChapelEvent", "dbo");
            });

            modelBuilder.Entity<Clifton_Strengths>(entity =>
            {
                entity.HasKey(e => new { e.ID_NUM, e.ACCESS_CODE })
                    .HasName("PK_CliftonStrengths");

                entity.Property(e => e.Private).HasComment("Whether the user wants their strengths to be private (not shown to other users)");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.ToView("Countries", "dbo");

                entity.Property(e => e.CTY).IsFixedLength();
            });

            modelBuilder.Entity<DiningInfo>(entity =>
            {
                entity.ToView("DiningInfo", "dbo");

                entity.Property(e => e.ChoiceDescription).IsFixedLength();

                entity.Property(e => e.SessionCode).IsFixedLength();
            });

            modelBuilder.Entity<Dining_Meal_Choice_Desc>(entity =>
            {
                entity.ToView("Dining_Meal_Choice_Desc", "dbo");

                entity.Property(e => e.Meal_Choice_Desc).IsFixedLength();

                entity.Property(e => e.Meal_Choice_Id).IsFixedLength();
            });

            modelBuilder.Entity<Dining_Meal_Plan_Change_History>(entity =>
            {
                entity.ToView("Dining_Meal_Plan_Change_History", "dbo");

                entity.Property(e => e.OLD_PLAN_DESC).IsFixedLength();
            });

            modelBuilder.Entity<Dining_Meal_Plan_Id_Mapping>(entity =>
            {
                entity.ToView("Dining_Meal_Plan_Id_Mapping", "dbo");
            });

            modelBuilder.Entity<Dining_Mealplans>(entity =>
            {
                entity.ToView("Dining_Mealplans", "dbo");
            });

            modelBuilder.Entity<Dining_Student_Meal_Choice>(entity =>
            {
                entity.ToView("Dining_Student_Meal_Choice", "dbo");

                entity.Property(e => e.MEAL_CHOICE_ID).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<EmergencyContact>(entity =>
            {
                entity.ToView("EmergencyContact", "dbo");

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
                entity.ToView("FacStaff", "dbo");

                entity.Property(e => e.BuildingDescription).IsFixedLength();
            });

            modelBuilder.Entity<Graduation>(entity =>
            {
                entity.ToView("Graduation", "dbo");
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

            modelBuilder.Entity<Information_Change_Request>(entity =>
            {
                entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Internships_as_Involvements>(entity =>
            {
                entity.ToView("Internships_as_Involvements", "dbo");

                entity.Property(e => e.SESS_CDE).IsFixedLength();

                entity.Property(e => e.TRM_CDE).IsFixedLength();

                entity.Property(e => e.YR_CDE).IsFixedLength();
            });

            modelBuilder.Entity<InvolvementOffering>(entity =>
            {
                entity.ToView("InvolvementOffering", "dbo");

                entity.Property(e => e.ActivityCode).IsFixedLength();

                entity.Property(e => e.ActivityDescription).IsFixedLength();

                entity.Property(e => e.ActivityType).IsFixedLength();

                entity.Property(e => e.ActivityTypeDescription).IsFixedLength();

                entity.Property(e => e.SessionCode).IsFixedLength();
            });

            modelBuilder.Entity<JENZ_ACT_CLUB_DEF>(entity =>
            {
                entity.ToView("JENZ_ACT_CLUB_DEF", "dbo");

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
                entity.ToView("Mailboxes", "dbo");
            });

            modelBuilder.Entity<Majors>(entity =>
            {
                entity.ToView("Majors", "dbo");
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.Property(e => e.SurfaceID).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Series)
                    .WithMany(p => p.Match)
                    .HasForeignKey(d => d.SeriesID)
                    .HasConstraintName("FK_Match_Series");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Match)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Match_MatchStatus");

                entity.HasOne(d => d.Surface)
                    .WithMany(p => p.Match)
                    .HasForeignKey(d => d.SurfaceID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Match_Surface");
            });

            modelBuilder.Entity<MatchParticipant>(entity =>
            {
                entity.HasOne(d => d.Match)
                    .WithMany(p => p.MatchParticipant)
                    .HasForeignKey(d => d.MatchID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchParticipant_Match");

                entity.HasOne(d => d.ParticipantUsernameNavigation)
                    .WithMany(p => p.MatchParticipant)
                    .HasForeignKey(d => d.ParticipantUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchParticipant_Participant");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.MatchParticipant)
                    .HasForeignKey(d => d.TeamID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchParticipant_Team");
            });

            modelBuilder.Entity<MatchTeam>(entity =>
            {
                entity.HasOne(d => d.Match)
                    .WithMany(p => p.MatchTeam)
                    .HasForeignKey(d => d.MatchID)
                    .HasConstraintName("FK_MatchTeam_Match");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.MatchTeam)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MatchTeam_MatchTeamStatus");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.MatchTeam)
                    .HasForeignKey(d => d.TeamID)
                    .HasConstraintName("FK_MatchTeam_Team");
            });

            modelBuilder.Entity<MembershipView>(entity =>
            {
                entity.ToView("MembershipView", "dbo");

                entity.Property(e => e.ActivityDescription).IsFixedLength();
            });

            modelBuilder.Entity<Message_Rooms>(entity =>
            {
                entity.HasKey(e => e.room_id)
                    .HasName("PK__Message___19675A8A3E781488");
            });

            modelBuilder.Entity<Minors>(entity =>
            {
                entity.ToView("Minors", "dbo");
            });

            modelBuilder.Entity<PART_DEF>(entity =>
            {
                entity.ToView("PART_DEF", "dbo");

                entity.Property(e => e.PART_CDE).IsFixedLength();

                entity.Property(e => e.PART_DESC).IsFixedLength();
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ParticipantActivity>(entity =>
            {
                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ParticipantActivity)
                    .HasForeignKey(d => d.ActivityID)
                    .HasConstraintName("FK_ParticipantActivity_Activity");

                entity.HasOne(d => d.ParticipantUsernameNavigation)
                    .WithMany(p => p.ParticipantActivity)
                    .HasForeignKey(d => d.ParticipantUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantActivity_Participant");

                entity.HasOne(d => d.PrivType)
                    .WithMany(p => p.ParticipantActivity)
                    .HasForeignKey(d => d.PrivTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrivType_ParticipantActivity");
            });

            modelBuilder.Entity<ParticipantNotification>(entity =>
            {
                entity.HasOne(d => d.ParticipantUsernameNavigation)
                    .WithMany(p => p.ParticipantNotification)
                    .HasForeignKey(d => d.ParticipantUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantNotification_Participant");
            });

            modelBuilder.Entity<ParticipantStatusHistory>(entity =>
            {
                entity.HasOne(d => d.ParticipantUsernameNavigation)
                    .WithMany(p => p.ParticipantStatusHistory)
                    .HasForeignKey(d => d.ParticipantUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantStatusHistory_Participant");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ParticipantStatusHistory)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantStatusHistory_ParticipantStatus");
            });

            modelBuilder.Entity<ParticipantTeam>(entity =>
            {
                entity.HasOne(d => d.ParticipantUsernameNavigation)
                    .WithMany(p => p.ParticipantTeam)
                    .HasForeignKey(d => d.ParticipantUsername)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantTeam_Participant");

                entity.HasOne(d => d.RoleType)
                    .WithMany(p => p.ParticipantTeam)
                    .HasForeignKey(d => d.RoleTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleType_ParticipantTeam");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.ParticipantTeam)
                    .HasForeignKey(d => d.TeamID)
                    .HasConstraintName("FK_ParticipantTeam_Team");
            });

            modelBuilder.Entity<Police>(entity =>
            {
                entity.ToView("Police", "dbo");
            });

            modelBuilder.Entity<REQUEST>(entity =>
            {
                entity.HasKey(e => e.REQUEST_ID)
                    .HasName("PK_Request");

                entity.Property(e => e.ACT_CDE).IsFixedLength();

                entity.Property(e => e.PART_CDE).IsFixedLength();

                entity.Property(e => e.SESS_CDE).IsFixedLength();
            });

            modelBuilder.Entity<RequestView>(entity =>
            {
                entity.ToView("RequestView", "dbo");

                entity.Property(e => e.ActivityDescription).IsFixedLength();

                entity.Property(e => e.ParticipationDescription).IsFixedLength();
            });

            modelBuilder.Entity<RoomAssign>(entity =>
            {
                entity.ToView("RoomAssign", "dbo");

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

            modelBuilder.Entity<Series>(entity =>
            {
                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Series)
                    .HasForeignKey(d => d.ActivityID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Series_Activity");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Series)
                    .HasForeignKey(d => d.ScheduleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SeriesSchedule_Series");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Series)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SeriesStatus_Series");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Series)
                    .HasForeignKey(d => d.TypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Series_SeriesType");
            });

            modelBuilder.Entity<SeriesSurface>(entity =>
            {
                entity.HasOne(d => d.Series)
                    .WithMany(p => p.SeriesSurface)
                    .HasForeignKey(d => d.SeriesID)
                    .HasConstraintName("FK_SeriesSurface_Series");

                entity.HasOne(d => d.Surface)
                    .WithMany(p => p.SeriesSurface)
                    .HasForeignKey(d => d.SurfaceID)
                    .HasConstraintName("FK_SeriesSurface_Surface");
            });

            modelBuilder.Entity<SeriesTeam>(entity =>
            {
                entity.HasOne(d => d.Series)
                    .WithMany(p => p.SeriesTeam)
                    .HasForeignKey(d => d.SeriesID)
                    .HasConstraintName("FK_SeriesTeam_Series");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.SeriesTeam)
                    .HasForeignKey(d => d.TeamID)
                    .HasConstraintName("FK_SeriesTeam_Team");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.ToView("States", "dbo");
            });

            modelBuilder.Entity<Statistic>(entity =>
            {
                entity.HasOne(d => d.ParticipantTeam)
                    .WithMany(p => p.Statistic)
                    .HasForeignKey(d => d.ParticipantTeamID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Statistic_ParticipantTeam");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToView("Student", "dbo");

                entity.Property(e => e.BuildingDescription).IsFixedLength();
            });

            modelBuilder.Entity<StudentNewsExpiration>(entity =>
            {
                entity.HasKey(e => e.SNID)
                    .HasName("PK_StudentNewsExpiration_SNID");

                entity.Property(e => e.SNID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.ActivityID)
                    .HasConstraintName("FK_Team_Activity");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.StatusID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Team_TeamStatus");
            });

            modelBuilder.Entity<UserPrivacy_Fields>(entity =>
            {
                entity.HasKey(e => e.Field)
                    .HasName("Field");
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserPrivacy_Settings>(entity =>
            {
                entity.HasOne(d => d.FieldNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Field)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPrivacy_Settings_UserPrivacy_Fields");

                entity.HasOne(d => d.VisibilityNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Visibility)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPrivacy_Settings_UserPrivacy_Visibility_Groups");
            });

            modelBuilder.Entity<UserPrivacy_Visibility_Groups>(entity =>
            {
                entity.HasKey(e => e.Group)
                    .HasName("Group");
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
            });

            modelBuilder.HasSequence("Information_Change_Request_Seq", "dbo");

            OnModelCreatingGeneratedProcedures(modelBuilder);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}