using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using ReadIraq.Authorization;
using ReadIraq.Authorization.Roles;
using ReadIraq.Authorization.Users;
using ReadIraq.Countries;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Countries;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Advertisiments;
using ReadIraq.Domain.Units;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.EntityFrameworkCore.Seed.Host
{
    public class InitialSampleDataBuilder
    {
        private readonly ReadIraqDbContext _context;
        private readonly Random _random = new Random();

        // Static URLs to simulate "downloading once" for all records
        private const string SampleImageUrl = "https://picsum.photos/id/237/400/300"; // Static dog image
        private const string SampleVideoUrl = "https://www.w3schools.com/html/mov_bbb.mp4";

        public InitialSampleDataBuilder(ReadIraqDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            Console.WriteLine("--- Starting InitialSampleDataBuilder ---");
            //DeleteAllData();
            CreateAdminUser();
            CreateCountryAndCities();
            CreateTeacherFeatures();
            CreateEducationalStructure();
            CreateAdvertisiments();
            Console.WriteLine("--- InitialSampleDataBuilder Completed Successfully ---");
        }

        private void DeleteAllData()
        {
            Console.WriteLine("Cleaning up existing data...");

            // Nullify user references to avoid foreign key constraints preventing deletion of Cities and Grades
            try
            {
                _context.Database.ExecuteSqlRaw("UPDATE AbpUsers SET GovernorateId = NULL, GradeId = NULL");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not nullify AbpUsers references: {ex.Message}");
            }

            var tables = new[]
            {
                "LessonSessionAttachments",
                "Questions",
                "QuizAttempts",
                "Quizzes",
                "UserSessionProgresses",
                "UserPreferredSubjects",
                "TeacherReviews",
                "LessonSessions",
                "Units",
                "TeacherFeatureMap",
                "TeacherFeatures",
                "TeacherSubjects",
                "TeacherProfiles",
                "GradeSubjects",
                "Translations",
                "Subjects",
                "Grades",
                "GradeGroups",
                "RegionTranslations",
                "Regions",
                "Mediator",
                "CityTranslations",
                "Cities",
                "CountryTranslations",
                "Countries",
                "Advertisiments",
                "Attachments"
            };

            foreach (var table in tables)
            {
                try
                {
                    _context.Database.ExecuteSqlRaw($"DELETE FROM {table}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not delete from {table}: {ex.Message}");
                }
            }
            
            try
            {
                _context.Database.ExecuteSqlRaw("DELETE FROM AbpUsers WHERE Type = 3");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not delete sample users: {ex.Message}");
            }

            _context.SaveChanges();
            Console.WriteLine("Cleanup done.");
        }

        private void CreateAdminUser()
        {
            Console.WriteLine("Checking Admin User...");
            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.UserName == "admin123");
            if (adminUser == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = "admin123",
                    Name = "Admin",
                    Surname = "Admin",
                    EmailAddress = "admin123@readiraq.com",
                    IsEmailConfirmed = true,
                    IsActive = true,
                    Type = UserType.SuperAdmin,
                    PIN = "123456"
                };

                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, "admin123");
                user.SetNormalizedNames();

                _context.Users.Add(user);
                _context.SaveChanges();

                var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.Name == StaticRoleNames.Host.Admin);
                if (adminRole != null)
                {
                    _context.UserRoles.Add(new UserRole(null, user.Id, adminRole.Id));
                    _context.SaveChanges();
                }
                Console.WriteLine("Admin user created.");
            }
        }

        private void CreateCountryAndCities()
        {
            Console.WriteLine("Creating Countries and Cities...");
            var iraq = _context.Countries.Include(c => c.Translations).FirstOrDefault(c => c.Translations.Any(t => t.Name == "Iraq"));
            if (iraq == null)
            {
                iraq = new Country
                {
                    DialCode = "+964",
                    IsActive = true,
                    Translations = new List<CountryTranslation>
                    {
                        new CountryTranslation { Name = "Iraq", Language = "en" },
                        new CountryTranslation { Name = "العراق", Language = "ar" }
                    }
                };
                _context.Countries.Add(iraq);
                _context.SaveChanges();
            }

            var cities = new[]
            {
                new { En = "Baghdad", Ar = "بغداد" },
                new { En = "Basra", Ar = "البصرة" },
                new { En = "Erbil", Ar = "أربيل" },
                new { En = "Najaf", Ar = "النجف" }
            };

            foreach (var city in cities)
            {
                if (!_context.Cities.Include(c => c.Translations).Any(c => c.CountryId == iraq.Id && c.Translations.Any(t => t.Name == city.En)))
                {
                    _context.Cities.Add(new City
                    {
                        CountryId = iraq.Id,
                        IsActive = true,
                        Translations = new List<CityTranslation>
                        {
                            new CityTranslation { Name = city.En, Language = "en" },
                            new CityTranslation { Name = city.Ar, Language = "ar" }
                        }
                    });
                }
            }
            _context.SaveChanges();
        }

        private void CreateTeacherFeatures()
        {
            Console.WriteLine("Creating Teacher Features...");
            var features = new[]
            {
                new { En = "PhD Degree", Ar = "شهادة دكتوراه" },
                new { En = "10+ Years Experience", Ar = "خبرة أكثر من 10 سنوات" },
                new { En = "Certified Trainer", Ar = "مدرب معتمد" },
                new { En = "Expert in Online Education", Ar = "خبير في التعليم عن بعد" }
            };

            foreach (var feature in features)
            {
                if (!_context.TeacherFeatures.Any(f => f.Name == feature.En))
                {
                    _context.TeacherFeatures.Add(new TeacherFeature
                    {
                        Name = feature.En,
                        Description = feature.Ar,
                        IsActive = true
                    });
                }
            }
            _context.SaveChanges();
        }

        private void CreateEducationalStructure()
        {
            Console.WriteLine("Creating Educational Structure...");
            var stages = new[]
            {
                new { NameEn = "Primary School", NameAr = "المرحلة الابتدائية", Grades = new[] { 1 , 2 , 3  } }
            };

            var subjects = new[]
            {
                new { En = "Arabic", Ar = "اللغة العربية" },
                new { En = "English", Ar = "اللغة الإنجليزية" }
            };

            var teacherNames = new[] { "أحمد علي", "فاطمة حسن" };

            foreach (var stage in stages)
            {
                Console.WriteLine($"Processing Stage: {stage.NameEn}");
                var group = _context.GradeGroups.Include(g => g.Name).FirstOrDefault(g => g.Name.Any(t => t.Name == stage.NameEn));
                if (group == null)
                {
                    group = new GradeGroup
                    {
                        Priority = stage.Grades[0],
                        Name = new List<Translation>
                        {
                            new Translation { Code = "en", Name = stage.NameEn },
                            new Translation { Code = "ar", Name = stage.NameAr }
                        }
                    };
                    _context.GradeGroups.Add(group);
                    _context.SaveChanges();
                }

                foreach (var gradeNum in stage.Grades)
                {
                    var gradeNameEn = $"Grade {gradeNum}";
                    var gradeNameAr = $"الصف {GetArabicNumber(gradeNum)}";
                    Console.WriteLine($"  Processing {gradeNameEn}...");
                    var grade = _context.Grades.Include(g => g.Name).FirstOrDefault(g => g.GradeGroupId == group.Id && g.Name.Any(t => t.Name == gradeNameEn));
                    if (grade == null)
                    {
                        grade = new Grade
                        {
                            GradeGroupId = group.Id,
                            Priority = gradeNum,
                            IsActive = true,
                            Name = new List<Translation>
                            {
                                new Translation { Code = "en", Name = gradeNameEn },
                                new Translation { Code = "ar", Name = gradeNameAr }
                            }
                        };
                        _context.Grades.Add(grade);
                        _context.SaveChanges();
                    }

                    foreach (var sub in subjects)
                    {
                        Console.WriteLine($"    Processing Subject: {sub.En}");
                        var subject = _context.Subjects.Include(s => s.Name).FirstOrDefault(s => s.Name.Any(t => t.Name == sub.En));
                        if (subject == null)
                        {
                            var subjectId = Guid.NewGuid();
                            var attachment = CreateAttachment(subjectId.ToString(), AttachmentRefType.Subject, MediaType.Image);

                            subject = new Subject
                            {
                                Id = subjectId,
                                IsActive = true,
                                Name = new List<Translation>
                                {
                                    new Translation { Code = "en", Name = sub.En },
                                    new Translation { Code = "ar", Name = sub.Ar }
                                },
                                Level = SubjectLevel.Semester,
                                AttachmentId = attachment.Id
                            };
                            _context.Subjects.Add(subject);
                            _context.SaveChanges();
                        }

                        if (!_context.GradeSubjects.Any(gs => gs.GradeId == grade.Id && gs.SubjectId == subject.Id))
                        {
                            _context.GradeSubjects.Add(new GradeSubject { GradeId = grade.Id, SubjectId = subject.Id });
                        }

                        // Create Units for this subject
                        var units = CreateUnits(subject.Id, sub.Ar);

                        var teacherName = teacherNames[_random.Next(teacherNames.Length)];
                        var teacherProfile = CreateTeacher(teacherName, subject.Id, grade.Id);

                        CreateLessons(teacherProfile, subject.Id, units);

                        _context.SaveChanges();
                    }
                }
            }
        }

        private List<Unit> CreateUnits(Guid subjectId, string subjectNameAr)
        {
            var units = new List<Unit>();
            for (int i = 1; i <= 2; i++)
            {
                var unitNameEn = $"Unit {i}";
                var unitNameAr = $"الوحدة {i}";
                var unit = _context.Units.Include(u => u.Name).FirstOrDefault(u => u.SubjectId == subjectId && u.Name.Any(t => t.Name == unitNameEn));
                if (unit == null)
                {
                    unit = new Unit
                    {
                        Id = Guid.NewGuid(),
                        SubjectId = subjectId,
                        Order = i,
                        IsActive = true,
                        Name = new List<Translation>
                        {
                            new Translation { Code = "en", Name = unitNameEn },
                            new Translation { Code = "ar", Name = unitNameAr }
                        },
                        Description = $"وصف {unitNameAr} لمادة {subjectNameAr}"
                    };
                    _context.Units.Add(unit);
                }
                units.Add(unit);
            }
            _context.SaveChanges();
            return units;
        }

        private string GetArabicNumber(int n)
        {
            string[] numbers = { "", "الأول", "الثاني", "الثالث", "الرابع", "الخامس", "السادس", "السابع", "الثامن", "التاسع", "العاشر", "الحادي عشر", "الثاني عشر" };
            return n > 0 && n < numbers.Length ? numbers[n] : n.ToString();
        }

        private TeacherProfile CreateTeacher(string arabicName, Guid subjectId, int gradeId)
        {
            var teacherUser = _context.Users.FirstOrDefault(u => u.Name == arabicName && u.Type == UserType.Teacher);

            if (teacherUser == null)
            {
                var userName = "teacher_" + arabicName.Replace(" ", "_") + "_" + _random.Next(100, 999);
                teacherUser = new User
                {
                    TenantId = null,
                    UserName = userName,
                    Name = arabicName,
                    Surname = "Teacher",
                    EmailAddress = userName + "@readiraq.com",
                    IsEmailConfirmed = true,
                    IsActive = true,
                    Type = UserType.Teacher,
                    PIN = "123456"
                };
                teacherUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(teacherUser, "Teacher123!");
                teacherUser.SetNormalizedNames();
                _context.Users.Add(teacherUser);
                _context.SaveChanges();
            }

            var profile = _context.TeacherProfiles.FirstOrDefault(p => p.UserId == teacherUser.Id);
            if (profile == null)
            {
                var profileId = Guid.NewGuid();
                var attachment = CreateAttachment(profileId.ToString(), AttachmentRefType.TeacherProfile, MediaType.Image);

                profile = new TeacherProfile
                {
                    Id = profileId,
                    UserId = teacherUser.Id,
                    Name = arabicName,
                    Bio = $"أستاذ خبير متخصص في مادة {arabicName}",
                    IsActive = true,
                    AverageRating = 4.8m + (decimal)(_random.NextDouble() * 0.2),
                    AttachmentId = attachment.Id,
                    Specialization = arabicName
                };
                _context.TeacherProfiles.Add(profile);
                _context.SaveChanges();

                // Assign random features
                var allFeatures = _context.TeacherFeatures.ToList();
                var randomFeatures = allFeatures.OrderBy(x => Guid.NewGuid()).Take(2);
                foreach (var feature in randomFeatures)
                {
                    _context.TeacherFeaturesMap.Add(new TeacherFeatureMap
                    {
                        TeacherProfileId = profile.Id,
                        TeacherFeatureId = feature.Id
                    });
                }
            }

            if (!_context.TeacherSubjects.Any(ts => ts.TeacherProfileId == profile.Id && ts.SubjectId == subjectId && ts.GradeId == gradeId))
            {
                _context.TeacherSubjects.Add(new TeacherSubject { TeacherProfileId = profile.Id, SubjectId = subjectId, GradeId = gradeId });
            }

            return profile;
        }

        private void CreateLessons(TeacherProfile teacher, Guid subjectId, List<Unit> units)
        {
            var subjectNameAr = _context.Translations
                .Where(t => t.Code == "ar" && EF.Property<Guid>(t, "SubjectId") == subjectId)
                .Select(t => t.Name)
                .FirstOrDefault() ?? "المادة";

            foreach (var unit in units)
            {
                var unitNameAr = unit.Name.FirstOrDefault(t => t.Code == "ar")?.Name ?? "الوحدة";
                int lessonCount = _random.Next(1, 3);
                for (int i = 1; i <= lessonCount; i++)
                {
                    var lessonTitle = $"{unitNameAr} - الدرس {i}: مقدمة في {subjectNameAr}";
                    var lesson = _context.LessonSessions.FirstOrDefault(l => l.Title == lessonTitle && l.SubjectId == subjectId && l.TeacherProfileId == teacher.Id);

                    if (lesson == null)
                    {
                        var lessonId = Guid.NewGuid();
                        var thumb = CreateAttachment(lessonId.ToString(), AttachmentRefType.LessonSessionThumbnail, MediaType.Image);
                        var video = CreateAttachment(lessonId.ToString(), AttachmentRefType.LessonSessionVideo, MediaType.Video);

                        lesson = new LessonSession
                        {
                            Id = lessonId,
                            Title = lessonTitle,
                            Description = $"في هذا الدرس، سنقوم بشرح المفاهيم الأساسية للدرس {i} في {unitNameAr} بأسلوب مبسط وشيق.",
                            SubjectId = subjectId,
                            UnitId = unit.Id,
                            TeacherProfileId = teacher.Id,
                            DurationSeconds = _random.Next(900, 2700),
                            Order = i,
                            IsActive = true,
                            IsFree = i == 1,
                            ThumbnailAttachmentId = thumb.Id,
                            VideoAttachmentId = video.Id
                        };
                        _context.LessonSessions.Add(lesson);

                        _context.LessonSessionAttachments.Add(new LessonSessionAttachment { LessonSessionId = lesson.Id, Attachment = thumb });
                        _context.LessonSessionAttachments.Add(new LessonSessionAttachment { LessonSessionId = lesson.Id, Attachment = video });

                        CreateQuizForLesson(lesson);
                    }
                }
            }
        }

        private void CreateQuizForLesson(LessonSession lesson)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.SessionId == lesson.Id);
            if (quiz == null)
            {
                var quizId = Guid.NewGuid();
                var attachment = CreateAttachment(quizId.ToString(), AttachmentRefType.Other, MediaType.Image);

                quiz = new Quiz
                {
                    Id = quizId,
                    Title = $"اختبار: {lesson.Title}",
                    Description = $"اختبار قصير لتقييم فهمك لمحتوى {lesson.Title}",
                    SubjectId = lesson.SubjectId,
                    SessionId = lesson.Id,
                    TeacherId = lesson.TeacherProfileId,
                    DurationSeconds = 600,
                    TotalMarks = 100,
                    AttachmentId = attachment.Id
                };
                _context.Quizzes.Add(quiz);

                int questionCount = _random.Next(2, 4);
                for (int i = 1; i <= questionCount; i++)
                {
                    bool isMcq = _random.Next(0, 2) == 0;
                    var questionId = Guid.NewGuid();
                    var qAttachment = CreateAttachment(questionId.ToString(), AttachmentRefType.Question, MediaType.Image);

                    var question = new Question
                    {
                        Id = questionId,
                        QuizId = quiz.Id,
                        Type = isMcq ? QuestionType.MCQ : QuestionType.TrueFalse,
                        Text = isMcq ? $"السؤال {i}: اختر الإجابة الصحيحة بناءً على ما ورد في {lesson.Title}" : $"السؤال {i}: هل العبارة التالية صحيحة بخصوص موضوع الدرس؟",
                        Options = isMcq ? "الخيار الأول,الخيار الثاني,الخيار الثالث,الخيار الرابع" : "صح,خطأ",
                        CorrectAnswer = isMcq ? "الخيار الأول" : "صح",
                        AnswerDescription = "الإجابة مستمدة من الشرح التفصيلي الوارد في الفيديو التعليمي.",
                        Marks = 100 / questionCount,
                        AttachmentId = qAttachment.Id
                    };
                    _context.Questions.Add(question);
                }
            }
        }

        private void CreateAdvertisiments()
        {
            Console.WriteLine("Creating Advertisements...");
            for (int i = 1; i <= 3; i++)
            {
                var ad = new Advertisiment
                {
                    IsActive = true,
                    Link = "https://readiraq.com",
                    ForSettings = false
                };
                _context.Advertisiments.Add(ad);
                _context.SaveChanges();

                CreateAttachment(ad.Id.ToString(), AttachmentRefType.Advertisiment, MediaType.Image);
            }
        }

        private Attachment CreateAttachment(string refId, AttachmentRefType refType, MediaType mediaType)
        {
            var attachment = new Attachment
            {
                RefId = refId,
                RefType = refType,
                Type = mediaType,
                Url = mediaType == MediaType.Video ? SampleVideoUrl : SampleImageUrl,
                StorageKey = Guid.NewGuid().ToString(),
                FileName = mediaType == MediaType.Video ? "video.mp4" : "image.jpg",
                Size = 1024
            };
            _context.Attachments.Add(attachment);
            _context.SaveChanges();
            return attachment;
        }
    }
}
