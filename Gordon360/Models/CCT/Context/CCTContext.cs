﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Gordon360.Models.CCT;
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

    public virtual DbSet<CustomParticipant> CustomParticipant { get; set; }

    public virtual DbSet<DiningInfo> DiningInfo { get; set; }

    public virtual DbSet<ERROR_LOG> ERROR_LOG { get; set; }

    public virtual DbSet<EmergencyContact> EmergencyContact { get; set; }

    public virtual DbSet<FacStaff> FacStaff { get; set; }

    public virtual DbSet<Housing_Applicants> Housing_Applicants { get; set; }

    public virtual DbSet<Housing_Applications> Housing_Applications { get; set; }

    public virtual DbSet<Housing_HallChoices> Housing_HallChoices { get; set; }

    public virtual DbSet<Housing_Halls> Housing_Halls { get; set; }

    public virtual DbSet<Information_Change_Request> Information_Change_Request { get; set; }

    public virtual DbSet<InvolvementOffering> InvolvementOffering { get; set; }

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

    public virtual DbSet<PART_DEF> PART_DEF { get; set; }

    public virtual DbSet<Participant> Participant { get; set; }

    public virtual DbSet<ParticipantActivity> ParticipantActivity { get; set; }

    public virtual DbSet<ParticipantNotification> ParticipantNotification { get; set; }

    public virtual DbSet<ParticipantStatus> ParticipantStatus { get; set; }

    public virtual DbSet<ParticipantStatusHistory> ParticipantStatusHistory { get; set; }

    public virtual DbSet<ParticipantTeam> ParticipantTeam { get; set; }

    public virtual DbSet<ParticipantView> ParticipantView { get; set; }

    public virtual DbSet<PrivType> PrivType { get; set; }

    public virtual DbSet<REQUEST> REQUEST { get; set; }

    public virtual DbSet<RequestView> RequestView { get; set; }

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

    public virtual DbSet<Student> Student { get; set; }

    public virtual DbSet<StudentNewsExpiration> StudentNewsExpiration { get; set; }

    public virtual DbSet<Surface> Surface { get; set; }

    public virtual DbSet<Team> Team { get; set; }

    public virtual DbSet<TeamStatus> TeamStatus { get; set; }

    public virtual DbSet<UserCourses> UserCourses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ACCOUNT>(entity =>
        {
            entity.ToView("ACCOUNT");
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
            entity.ToView("AccountPhotoURL");
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
            entity.ToView("Alumni");

            entity.Property(e => e.grad_student).IsFixedLength();
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
            entity.HasKey(e => new { e.ID_NUM, e.ACCESS_CODE }).HasName("PK_CliftonStrengths");

            entity.Property(e => e.Private).HasComment("Whether the user wants their strengths to be private (not shown to other users)");
        });

        modelBuilder.Entity<Countries>(entity =>
        {
            entity.ToView("Countries");

            entity.Property(e => e.CTY).IsFixedLength();
        });

        modelBuilder.Entity<CustomParticipant>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__CustomPa__536C85E5A0FDE2AE");

            entity.Property(e => e.ID).ValueGeneratedOnAdd();

            entity.HasOne(d => d.UsernameNavigation).WithOne(p => p.CustomParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CustomPar__Usern__70D3A237");
        });

        modelBuilder.Entity<DiningInfo>(entity =>
        {
            entity.ToView("DiningInfo");

            entity.Property(e => e.ChoiceDescription).IsFixedLength();
            entity.Property(e => e.SessionCode).IsFixedLength();
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
            entity.ToView("InvolvementOffering");

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
            entity.ToView("Mailboxes");
        });

        modelBuilder.Entity<Majors>(entity =>
        {
            entity.ToView("Majors");
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
            entity.ToView("MembershipView");

            entity.Property(e => e.ActivityDescription).IsFixedLength();
        });

        modelBuilder.Entity<Minors>(entity =>
        {
            entity.ToView("Minors");
        });

        modelBuilder.Entity<PART_DEF>(entity =>
        {
            entity.ToView("PART_DEF");

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

            entity.Property(e => e.SpecifiedGender).IsFixedLength();
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
            entity.ToView("RequestView");

            entity.Property(e => e.ActivityDescription).IsFixedLength();
            entity.Property(e => e.ParticipationDescription).IsFixedLength();
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
            entity.ToView("States");
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Statisti__3214EC2707DA8F6B");

            entity.HasOne(d => d.ParticipantTeam).WithMany(p => p.Statistic)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Statistic_ParticipantTeam");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToView("Student");

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

        modelBuilder.Entity<UserCourses>(entity =>
        {
            entity.ToView("UserCourses");

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
        modelBuilder.HasSequence("Information_Change_Request_Seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}