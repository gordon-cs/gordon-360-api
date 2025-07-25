﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT.Context;

public partial class CCTContext : DbContext
{
    public CCTContext(DbContextOptions<CCTContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ACCOUNT> ACCOUNT { get; set; }

    public virtual DbSet<ACT_INFO> ACT_INFO { get; set; }

    public virtual DbSet<AccountPhotoURL> AccountPhotoURL { get; set; }

    public virtual DbSet<ActionsTaken> ActionsTaken { get; set; }

    public virtual DbSet<ActionsTakenData> ActionsTakenData { get; set; }

    public virtual DbSet<Activity> Activity { get; set; }

    public virtual DbSet<ActivityStatus> ActivityStatus { get; set; }

    public virtual DbSet<ActivityType> ActivityType { get; set; }

    public virtual DbSet<Affiliation> Affiliation { get; set; }

    public virtual DbSet<AffiliationPoints> AffiliationPoints { get; set; }

    public virtual DbSet<Alumni> Alumni { get; set; }

    public virtual DbSet<CM_SESSION_MSTR> CM_SESSION_MSTR { get; set; }

    public virtual DbSet<CUSTOM_PROFILE> CUSTOM_PROFILE { get; set; }

    public virtual DbSet<ChapelEvent> ChapelEvent { get; set; }

    public virtual DbSet<Clifton_Strengths> Clifton_Strengths { get; set; }

    public virtual DbSet<Config> Config { get; set; }

    public virtual DbSet<Countries> Countries { get; set; }

    public virtual DbSet<CurrentTasks> CurrentTasks { get; set; }

    public virtual DbSet<Current_On_Call> Current_On_Call { get; set; }

    public virtual DbSet<CustomParticipant> CustomParticipant { get; set; }

    public virtual DbSet<Daily_RA_Events> Daily_RA_Events { get; set; }

    public virtual DbSet<DiningInfo> DiningInfo { get; set; }

    public virtual DbSet<ERROR_LOG> ERROR_LOG { get; set; }

    public virtual DbSet<EmergencyContact> EmergencyContact { get; set; }

    public virtual DbSet<FacStaff> FacStaff { get; set; }

    public virtual DbSet<FoundActionsTaken> FoundActionsTaken { get; set; }

    public virtual DbSet<FoundActionsTakenData> FoundActionsTakenData { get; set; }

    public virtual DbSet<FoundGuest> FoundGuest { get; set; }

    public virtual DbSet<FoundItemData> FoundItemData { get; set; }

    public virtual DbSet<FoundItems> FoundItems { get; set; }

    public virtual DbSet<Graduation> Graduation { get; set; }

    public virtual DbSet<GuestUsers> GuestUsers { get; set; }

    public virtual DbSet<Hall_Assignment_Ranges> Hall_Assignment_Ranges { get; set; }

    public virtual DbSet<Hall_Task_Occurrence> Hall_Task_Occurrence { get; set; }

    public virtual DbSet<Hall_Tasks> Hall_Tasks { get; set; }

    public virtual DbSet<Halls> Halls { get; set; }

    public virtual DbSet<Housing_Applicants> Housing_Applicants { get; set; }

    public virtual DbSet<Housing_Applications> Housing_Applications { get; set; }

    public virtual DbSet<Housing_HallChoices> Housing_HallChoices { get; set; }

    public virtual DbSet<Housing_Halls> Housing_Halls { get; set; }

    public virtual DbSet<Information_Change_Request> Information_Change_Request { get; set; }

    public virtual DbSet<InvolvementOffering> InvolvementOffering { get; set; }

    public virtual DbSet<ItemCategory> ItemCategory { get; set; }

    public virtual DbSet<ItemCondition> ItemCondition { get; set; }

    public virtual DbSet<ItemStatus> ItemStatus { get; set; }

    public virtual DbSet<MEMBERSHIP> MEMBERSHIP { get; set; }

    public virtual DbSet<Mailboxes> Mailboxes { get; set; }

    public virtual DbSet<Majors> Majors { get; set; }

    public virtual DbSet<Match> Match { get; set; }

    public virtual DbSet<MatchBracket> MatchBracket { get; set; }

    public virtual DbSet<MatchParticipant> MatchParticipant { get; set; }

    public virtual DbSet<MatchStatus> MatchStatus { get; set; }

    public virtual DbSet<MatchTeam> MatchTeam { get; set; }

    public virtual DbSet<MatchTeamStatus> MatchTeamStatus { get; set; }

    public virtual DbSet<MembershipView> MembershipView { get; set; }

    public virtual DbSet<Minors> Minors { get; set; }

    public virtual DbSet<MissingItemData> MissingItemData { get; set; }

    public virtual DbSet<MissingReports> MissingReports { get; set; }

    public virtual DbSet<PART_DEF> PART_DEF { get; set; }

    public virtual DbSet<Participant> Participant { get; set; }

    public virtual DbSet<ParticipantActivity> ParticipantActivity { get; set; }

    public virtual DbSet<ParticipantNotification> ParticipantNotification { get; set; }

    public virtual DbSet<ParticipantStatus> ParticipantStatus { get; set; }

    public virtual DbSet<ParticipantStatusHistory> ParticipantStatusHistory { get; set; }

    public virtual DbSet<ParticipantTeam> ParticipantTeam { get; set; }

    public virtual DbSet<ParticipantView> ParticipantView { get; set; }

    public virtual DbSet<Post> Post { get; set; }

    public virtual DbSet<PostImage> PostImage { get; set; }

    public virtual DbSet<PostedItem> PostedItem { get; set; }

    public virtual DbSet<Poster> Poster { get; set; }

    public virtual DbSet<PosterStatus> PosterStatus { get; set; }

    public virtual DbSet<PrivType> PrivType { get; set; }

    public virtual DbSet<RA_Assigned_Ranges_View> RA_Assigned_Ranges_View { get; set; }

    public virtual DbSet<RA_On_Call> RA_On_Call { get; set; }

    public virtual DbSet<RA_Pref_Contact> RA_Pref_Contact { get; set; }

    public virtual DbSet<RA_Status_Events> RA_Status_Events { get; set; }

    public virtual DbSet<RA_Students> RA_Students { get; set; }

    public virtual DbSet<RD_Info> RD_Info { get; set; }

    public virtual DbSet<RD_OnCall_Today> RD_OnCall_Today { get; set; }

    public virtual DbSet<RD_On_Call> RD_On_Call { get; set; }

    public virtual DbSet<REQUEST> REQUEST { get; set; }

    public virtual DbSet<RequestView> RequestView { get; set; }

    public virtual DbSet<ResRooms> ResRooms { get; set; }

    public virtual DbSet<ResidentialStatus_View> ResidentialStatus_View { get; set; }

    public virtual DbSet<RoleType> RoleType { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<SeriesSchedule> SeriesSchedule { get; set; }

    public virtual DbSet<SeriesStatus> SeriesStatus { get; set; }

    public virtual DbSet<SeriesSurface> SeriesSurface { get; set; }

    public virtual DbSet<SeriesTeam> SeriesTeam { get; set; }

    public virtual DbSet<SeriesTeamView> SeriesTeamView { get; set; }

    public virtual DbSet<SeriesType> SeriesType { get; set; }

    public virtual DbSet<Slider_Images> Slider_Images { get; set; }

    public virtual DbSet<Sport> Sport { get; set; }

    public virtual DbSet<States> States { get; set; }

    public virtual DbSet<Statistic> Statistic { get; set; }

    public virtual DbSet<StuLifePhoneNumbers> StuLifePhoneNumbers { get; set; }

    public virtual DbSet<Student> Student { get; set; }

    public virtual DbSet<StudentNewsExpiration> StudentNewsExpiration { get; set; }

    public virtual DbSet<Surface> Surface { get; set; }

    public virtual DbSet<Team> Team { get; set; }

    public virtual DbSet<TeamStatus> TeamStatus { get; set; }

    public virtual DbSet<Unassigned_Rooms> Unassigned_Rooms { get; set; }

    public virtual DbSet<UserCourses> UserCourses { get; set; }

    public virtual DbSet<YearTermTable> YearTermTable { get; set; }
    
    public virtual DbSet<CourseRegistrationDate> CourseRegistrationDates { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ACCOUNT>(entity =>
        {
            entity.ToView("ACCOUNT", "dbo");
        });

        modelBuilder.Entity<ACT_INFO>(entity =>
        {
            entity.HasKey(e => e.ACT_CDE).HasName("PK_Activity_Info");

            entity.Property(e => e.ACT_CDE).IsFixedLength();
            entity.Property(e => e.ACT_DESC).IsFixedLength();
            entity.Property(e => e.ACT_TYPE).IsFixedLength();
            entity.Property(e => e.ACT_TYPE_DESC).IsFixedLength();
        });

        modelBuilder.Entity<AccountPhotoURL>(entity =>
        {
            entity.ToView("AccountPhotoURL", "dbo");
        });

        modelBuilder.Entity<ActionsTaken>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ActionsT__3214EC2722EC236F");

            entity.HasOne(d => d.missing).WithMany(p => p.ActionsTaken)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActionsTa__missi__365CE7DF");
        });

        modelBuilder.Entity<ActionsTakenData>(entity =>
        {
            entity.ToView("ActionsTakenData", "LostAndFound");
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Activity__3214EC2752162A10");

            entity.HasOne(d => d.SeriesSchedule).WithMany(p => p.Activity).HasConstraintName("FK_Activity_SeriesSchedule");

            entity.HasOne(d => d.Sport).WithMany(p => p.Activity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_Sport");

            entity.HasOne(d => d.Status).WithMany(p => p.Activity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_ActivityStatus");

            entity.HasOne(d => d.Type).WithMany(p => p.Activity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Activity_ActivityType");
        });

        modelBuilder.Entity<Affiliation>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK_Affiliations");
        });

        modelBuilder.Entity<AffiliationPoints>(entity =>
        {
            entity.HasOne(d => d.AffiliationNameNavigation).WithMany(p => p.AffiliationPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AffiliationPoints_Affiliations");

            entity.HasOne(d => d.Series).WithMany(p => p.AffiliationPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AffiliationPoints_Series");

            entity.HasOne(d => d.Team).WithMany(p => p.AffiliationPoints)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AffiliationPoints_Team");
        });

        modelBuilder.Entity<Alumni>(entity =>
        {
            entity.ToView("Alumni", "dbo");

            entity.Property(e => e.grad_student).IsFixedLength();
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
            entity.HasKey(e => new { e.ID_NUM, e.ACCESS_CODE }).HasName("PK_CliftonStrengths");

            entity.Property(e => e.Private).HasComment("Whether the user wants their strengths to be private (not shown to other users)");
        });

        modelBuilder.Entity<Countries>(entity =>
        {
            entity.ToView("Countries", "dbo");

            entity.Property(e => e.CTY).IsFixedLength();
        });

        modelBuilder.Entity<CurrentTasks>(entity =>
        {
            entity.ToView("CurrentTasks", "Housing");
        });

        modelBuilder.Entity<Current_On_Call>(entity =>
        {
            entity.ToView("Current_On_Call", "Housing");

            entity.Property(e => e.Hall_Name).IsFixedLength();
        });

        modelBuilder.Entity<CustomParticipant>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__CustomPa__536C85E5A0FDE2AE");

            entity.Property(e => e.ID).ValueGeneratedOnAdd();

            entity.HasOne(d => d.UsernameNavigation).WithOne(p => p.CustomParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomPar__Usern__70D3A237");
        });

        modelBuilder.Entity<Daily_RA_Events>(entity =>
        {
            entity.ToView("Daily_RA_Events", "Housing");

            entity.Property(e => e.Status_ID).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<DiningInfo>(entity =>
        {
            entity.ToView("DiningInfo", "dbo");

            entity.Property(e => e.ChoiceDescription).IsFixedLength();
            entity.Property(e => e.SessionCode).IsFixedLength();
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

        modelBuilder.Entity<FoundActionsTaken>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tmp_ms_x__3214EC2744C7D4F3");

            entity.HasOne(d => d.found).WithMany(p => p.FoundActionsTaken)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FoundActi__found__0000D72E");
        });

        modelBuilder.Entity<FoundActionsTakenData>(entity =>
        {
            entity.ToView("FoundActionsTakenData", "LostAndFound");
        });

        modelBuilder.Entity<FoundGuest>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__FoundGue__3214EC275E423367");
        });

        modelBuilder.Entity<FoundItemData>(entity =>
        {
            entity.ToView("FoundItemData", "LostAndFound");
        });

        modelBuilder.Entity<FoundItems>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tmp_ms_x__3214EC2792138BD9");

            entity.HasOne(d => d.matchingMissing).WithMany(p => p.FoundItems).HasConstraintName("FK__FoundItem__match__37510C18");
        });

        modelBuilder.Entity<Graduation>(entity =>
        {
            entity.ToView("Graduation", "dbo");

            entity.Property(e => e.GRAD_FLAG).IsFixedLength();
        });

        modelBuilder.Entity<GuestUsers>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__GuestUse__3214EC2774F2F95F");

            entity.HasOne(d => d.missing).WithMany(p => p.GuestUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GuestUser__missi__3568C3A6");
        });

        modelBuilder.Entity<Hall_Assignment_Ranges>(entity =>
        {
            entity.HasKey(e => e.Range_ID).HasName("PK__Hall_Ass__918829E78BFD242E");
        });

        modelBuilder.Entity<Hall_Task_Occurrence>(entity =>
        {
            entity.HasKey(e => e.Occur_ID).HasName("PK__Hall_Tas__0A37E7864ADCDBF0");

            entity.HasOne(d => d.Task).WithMany(p => p.Hall_Task_Occurrence)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hall_Task_Occurrence_Task_ID");
        });

        modelBuilder.Entity<Hall_Tasks>(entity =>
        {
            entity.HasKey(e => e.Task_ID).HasName("PK__Hall_Tas__716F4ACDF2AEF15C");

            entity.Property(e => e.Created_Date).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Halls>(entity =>
        {
            entity.ToView("Halls", "Housing");

            entity.Property(e => e.BuildingCode).IsFixedLength();
            entity.Property(e => e.HallName).IsFixedLength();
        });

        modelBuilder.Entity<Housing_Applicants>(entity =>
        {
            entity.Property(e => e.SESS_CDE).IsFixedLength();

            entity.HasOne(d => d.HousingApp).WithMany(p => p.Housing_Applicants).HasConstraintName("FK_Applicants_HousingAppID");
        });

        modelBuilder.Entity<Information_Change_Request>(entity =>
        {
            entity.Property(e => e.TimeStamp).HasDefaultValueSql("(getdate())");
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

        modelBuilder.Entity<MEMBERSHIP>(entity =>
        {
            entity.HasKey(e => e.MEMBERSHIP_ID).HasName("PK_Membership");

            entity.Property(e => e.ACT_CDE).IsFixedLength();
            entity.Property(e => e.JOB_NAME).IsFixedLength();
            entity.Property(e => e.PART_CDE).IsFixedLength();
            entity.Property(e => e.SESS_CDE).IsFixedLength();
            entity.Property(e => e.USER_NAME).IsFixedLength();
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
            entity.HasKey(e => e.ID).HasName("PK__Match__3214EC27BB2F05C8");

            entity.Property(e => e.SurfaceID).HasDefaultValue(1);

            entity.HasOne(d => d.Series).WithMany(p => p.Match).HasConstraintName("FK_Match_Series");

            entity.HasOne(d => d.Status).WithMany(p => p.Match)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_MatchStatus");

            entity.HasOne(d => d.Surface).WithMany(p => p.Match)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Match_Surface");
        });

        modelBuilder.Entity<MatchBracket>(entity =>
        {
            entity.Property(e => e.MatchID).ValueGeneratedNever();

            entity.HasOne(d => d.Match).WithOne(p => p.MatchBracket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchBracket_Match");
        });

        modelBuilder.Entity<MatchParticipant>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__MatchPar__3214EC27DFD38F69");

            entity.HasOne(d => d.Match).WithMany(p => p.MatchParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchParticipant_Match");

            entity.HasOne(d => d.ParticipantUsernameNavigation).WithMany(p => p.MatchParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchParticipant_Participant");

            entity.HasOne(d => d.Team).WithMany(p => p.MatchParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchParticipant_Team");
        });

        modelBuilder.Entity<MatchTeam>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__MatchTea__3214EC2743BD85C3");

            entity.HasOne(d => d.Match).WithMany(p => p.MatchTeam).HasConstraintName("FK_MatchTeam_Match");

            entity.HasOne(d => d.Status).WithMany(p => p.MatchTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchTeam_MatchTeamStatus");

            entity.HasOne(d => d.Team).WithMany(p => p.MatchTeam).HasConstraintName("FK_MatchTeam_Team");
        });

        modelBuilder.Entity<MembershipView>(entity =>
        {
            entity.ToView("MembershipView", "dbo");

            entity.Property(e => e.ActivityDescription).IsFixedLength();
        });

        modelBuilder.Entity<Minors>(entity =>
        {
            entity.ToView("Minors", "dbo");
        });

        modelBuilder.Entity<MissingItemData>(entity =>
        {
            entity.ToView("MissingItemData", "LostAndFound");
        });

        modelBuilder.Entity<MissingReports>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__tmp_ms_x__3214EC271C4C78EB");

            entity.HasOne(d => d.matchingFound).WithMany(p => p.MissingReports).HasConstraintName("FK__MissingRe__match__38453051");
        });

        modelBuilder.Entity<PART_DEF>(entity =>
        {
            entity.ToView("PART_DEF", "dbo");

            entity.Property(e => e.PART_CDE).IsFixedLength();
            entity.Property(e => e.PART_DESC).IsFixedLength();
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.Property(e => e.AllowEmails).HasDefaultValue(true);
            entity.Property(e => e.ID).ValueGeneratedOnAdd();
            entity.Property(e => e.SpecifiedGender).IsFixedLength();
        });

        modelBuilder.Entity<ParticipantActivity>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Particip__3214EC27C3ECBD4A");

            entity.HasOne(d => d.Activity).WithMany(p => p.ParticipantActivity).HasConstraintName("FK_ParticipantActivity_Activity");

            entity.HasOne(d => d.ParticipantUsernameNavigation).WithMany(p => p.ParticipantActivity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticipantActivity_Participant");

            entity.HasOne(d => d.PrivType).WithMany(p => p.ParticipantActivity)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrivType_ParticipantActivity");
        });

        modelBuilder.Entity<ParticipantNotification>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Particip__3214EC2702C79C04");

            entity.HasOne(d => d.ParticipantUsernameNavigation).WithMany(p => p.ParticipantNotification)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticipantNotification_Participant");
        });

        modelBuilder.Entity<ParticipantStatusHistory>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Particip__3214EC2705A45A77");

            entity.HasOne(d => d.ParticipantUsernameNavigation).WithMany(p => p.ParticipantStatusHistory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticipantStatusHistory_Participant");

            entity.HasOne(d => d.Status).WithMany(p => p.ParticipantStatusHistory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticipantStatusHistory_ParticipantStatus");
        });

        modelBuilder.Entity<ParticipantTeam>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Particip__3214EC27CA13A740");

            entity.HasOne(d => d.ParticipantUsernameNavigation).WithMany(p => p.ParticipantTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticipantTeam_Participant");

            entity.HasOne(d => d.RoleType).WithMany(p => p.ParticipantTeam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleType_ParticipantTeam");

            entity.HasOne(d => d.Team).WithMany(p => p.ParticipantTeam).HasConstraintName("FK_ParticipantTeam_Team");
        });

        modelBuilder.Entity<ParticipantView>(entity =>
        {
            entity.ToView("ParticipantView", "RecIM");

            entity.Property(e => e.Hall).IsFixedLength();
            entity.Property(e => e.SpecifiedGender).IsFixedLength();
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToView("Post", "Marketplace");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasOne(d => d.PostedItem).WithMany(p => p.PostImage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostImage_PostedItem");
        });

        modelBuilder.Entity<PostedItem>(entity =>
        {
            entity.Property(e => e.PostedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.PostedItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostedItem_ItemCategory");

            entity.HasOne(d => d.Condition).WithMany(p => p.PostedItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostedItem_ItemCondition");

            entity.HasOne(d => d.Status).WithMany(p => p.PostedItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostedItem_ItemStatus");
        });

        modelBuilder.Entity<RA_Assigned_Ranges_View>(entity =>
        {
            entity.ToView("RA_Assigned_Ranges_View", "Housing");

            entity.Property(e => e.Hall_Name).IsFixedLength();
        });

        modelBuilder.Entity<RA_On_Call>(entity =>
        {
            entity.HasKey(e => e.Record_ID).HasName("PK__tmp_ms_x__603A0C60046D3031");
        });

        modelBuilder.Entity<RA_Pref_Contact>(entity =>
        {
            entity.HasKey(e => e.Ra_ID).HasName("PK__RA_Pref___9530636C18757ADA");
        });

        modelBuilder.Entity<RA_Status_Events>(entity =>
        {
            entity.HasKey(e => e.Status_ID).HasName("PK__RA_Statu__519009ACBC52F0C7");

            entity.Property(e => e.Created_Date).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<RA_Students>(entity =>
        {
            entity.ToView("RA_Students", "Housing");

            entity.Property(e => e.Dorm).IsFixedLength();
        });

        modelBuilder.Entity<RD_Info>(entity =>
        {
            entity.ToView("RD_Info", "Housing");

            entity.Property(e => e.BuildingCode).IsFixedLength();
            entity.Property(e => e.HallName).IsFixedLength();
        });

        modelBuilder.Entity<RD_OnCall_Today>(entity =>
        {
            entity.ToView("RD_OnCall_Today", "Housing");
        });

        modelBuilder.Entity<RD_On_Call>(entity =>
        {
            entity.HasKey(e => e.Record_ID).HasName("PK__RD_On_Ca__603A0C605596CACA");

            entity.Property(e => e.Created_Date).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Poster>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_Posters");

            entity.Property(e => e.ACT_CDE).IsFixedLength();

            entity.HasOne(d => d.ACT_CDENavigation).WithMany(p => p.Poster).HasConstraintName("FK_Posters_ACT_INFO");

            entity.HasOne(d => d.Status).WithMany(p => p.Poster)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posters_PosterStatus");
        });

        modelBuilder.Entity<REQUEST>(entity =>
        {
            entity.HasKey(e => e.REQUEST_ID).HasName("PK_Request");

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

        modelBuilder.Entity<ResidentialStatus_View>(entity =>
        {
            entity.ToView("ResidentialStatus_View", "Housing");

            entity.Property(e => e.Latest_Session).IsFixedLength();
            entity.Property(e => e.Residency_Status).IsFixedLength();
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Series__3214EC27544E6A66");

            entity.HasOne(d => d.Activity).WithMany(p => p.Series)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Series_Activity");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Series)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeriesSchedule_Series");

            entity.HasOne(d => d.Status).WithMany(p => p.Series)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeriesStatus_Series");

            entity.HasOne(d => d.Type).WithMany(p => p.Series)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Series_SeriesType");

            entity.HasOne(d => d.Winner).WithMany(p => p.Series).HasConstraintName("FK_Series_Team");
        });

        modelBuilder.Entity<SeriesSurface>(entity =>
        {
            entity.HasOne(d => d.Series).WithMany(p => p.SeriesSurface).HasConstraintName("FK_SeriesSurface_Series");

            entity.HasOne(d => d.Surface).WithMany(p => p.SeriesSurface).HasConstraintName("FK_SeriesSurface_Surface");
        });

        modelBuilder.Entity<SeriesTeam>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__SeriesTe__3214EC27F3AD6274");

            entity.HasOne(d => d.Series).WithMany(p => p.SeriesTeam).HasConstraintName("FK_SeriesTeam_Series");

            entity.HasOne(d => d.Team).WithMany(p => p.SeriesTeam).HasConstraintName("FK_SeriesTeam_Team");
        });

        modelBuilder.Entity<SeriesTeamView>(entity =>
        {
            entity.ToView("SeriesTeamView", "RecIM");
        });

        modelBuilder.Entity<SeriesType>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__SeriesTy__3214EC270DA32B2F");
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Sport__3214EC276FB2D545");
        });

        modelBuilder.Entity<States>(entity =>
        {
            entity.ToView("States", "dbo");
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Statisti__3214EC2707DA8F6B");

            entity.HasOne(d => d.ParticipantTeam).WithMany(p => p.Statistic)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Statistic_ParticipantTeam");
        });

        modelBuilder.Entity<StuLifePhoneNumbers>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__StuLifeP__3214EC2744511943");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToView("Student", "dbo");

            entity.Property(e => e.BuildingDescription).IsFixedLength();
        });

        modelBuilder.Entity<StudentNewsExpiration>(entity =>
        {
            entity.HasKey(e => e.SNID).HasName("PK_StudentNewsExpiration_SNID");

            entity.Property(e => e.SNID).ValueGeneratedNever();
        });

        modelBuilder.Entity<Surface>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Surface__3214EC27629CF90A");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Team__3214EC27A55EA0D3");

            entity.HasOne(d => d.Activity).WithMany(p => p.Team).HasConstraintName("FK_Team_Activity");

            entity.HasOne(d => d.AffiliationNavigation).WithMany(p => p.Team)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Team_Affiliations");

            entity.HasOne(d => d.Status).WithMany(p => p.Team)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Team_TeamStatus");
        });

        modelBuilder.Entity<Unassigned_Rooms>(entity =>
        {
            entity.ToView("Unassigned_Rooms", "Housing");
        });

        modelBuilder.Entity<CourseRegistrationDate>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("CourseRegistrationDates", "dbo");
        });

        modelBuilder.Entity<UserCourses>(entity =>
        {
            entity.ToView("UserCourses", "dbo");

            entity.Property(e => e.BLDG_CDE).IsFixedLength();
            entity.Property(e => e.CRS_CDE).IsFixedLength();
            entity.Property(e => e.CRS_TITLE).IsFixedLength();
            entity.Property(e => e.FRIDAY_CDE).IsFixedLength();
            entity.Property(e => e.MONDAY_CDE).IsFixedLength();
            entity.Property(e => e.ROOM_CDE).IsFixedLength();
            entity.Property(e => e.SATURDAY_CDE).IsFixedLength();
            entity.Property(e => e.SUBTERM_DESC).IsFixedLength();
            entity.Property(e => e.SUNDAY_CDE).IsFixedLength();
            entity.Property(e => e.THURSDAY_CDE).IsFixedLength();
            entity.Property(e => e.TRM_CDE).IsFixedLength();
            entity.Property(e => e.TUESDAY_CDE).IsFixedLength();
            entity.Property(e => e.WEDNESDAY_CDE).IsFixedLength();
            entity.Property(e => e.YR_CDE).IsFixedLength();
        });
        modelBuilder.HasSequence("Information_Change_Request_Seq", "dbo");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
