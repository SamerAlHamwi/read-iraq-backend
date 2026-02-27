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
using static ReadIraq.Enums.Enum;

namespace ReadIraq.EntityFrameworkCore.Seed.Host
{
    public class InitialSampleDataBuilder
    {
        private readonly ReadIraqDbContext _context;
        private readonly Random _random = new Random();

        public InitialSampleDataBuilder(ReadIraqDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateAdminUser();
            CreateCountryAndCities();
            CreateEducationalStructure();
        }

        private void CreateAdminUser()
        {
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
            }
        }

        private void CreateCountryAndCities()
        {
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

        private void CreateEducationalStructure()
        {
            var stages = new[]
            {
                new { NameEn = "Primary School", NameAr = "المرحلة الابتدائية", Grades = new[] { 1, 2, 3, 4, 5, 6 } },
                new { NameEn = "Middle School", NameAr = "المرحلة المتوسطة", Grades = new[] { 7, 8, 9 } },
                new { NameEn = "High School", NameAr = "المرحلة الإعدادية", Grades = new[] { 10, 11, 12 } }
            };

            var subjects = new[]
            {
                new { En = "Arabic", Ar = "اللغة العربية" },
                new { En = "English", Ar = "اللغة الإنجليزية" },
                new { En = "Mathematics", Ar = "الرياضيات" },
                new { En = "Physics", Ar = "الفيزياء" },
                new { En = "Chemistry", Ar = "الكيمياء" },
                new { En = "Biology", Ar = "الأحياء" },
                new { En = "Islamic Studies", Ar = "التربية الإسلامية" },
                new { En = "History", Ar = "التاريخ" },
                new { En = "Geography", Ar = "الجغرافيا" },
                new { En = "Computer Science", Ar = "الحاسوب" }
            };

            var teacherNames = new[] { "أحمد علي", "فاطمة حسن", "محمد جاسم", "زينب كاظم", "عمر محمود", "سارة إبراهيم", "حسين عبود", "مريم يوسف", "مصطفى شاكر", "ليلى مراد" };

            foreach (var stage in stages)
            {
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
                        var subject = _context.Subjects.Include(s => s.Name).FirstOrDefault(s => s.Name.Any(t => t.Name == sub.En));
                        if (subject == null)
                        {
                            subject = new Subject
                            {
                                IsActive = true,
                                Name = new List<Translation>
                                {
                                    new Translation { Code = "en", Name = sub.En },
                                    new Translation { Code = "ar", Name = sub.Ar }
                                },
                                Level = SubjectLevel.Semester
                            };
                            _context.Subjects.Add(subject);
                            _context.SaveChanges();

                            // Add Subject Image
                            CreateAttachment(subject.Id.ToString(), AttachmentRefType.Subject, MediaType.Image, $"https://picsum.photos/seed/{subject.Id}/400/300");
                        }

                        if (!_context.GradeSubjects.Any(gs => gs.GradeId == grade.Id && gs.SubjectId == subject.Id))
                        {
                            _context.GradeSubjects.Add(new GradeSubject { GradeId = grade.Id, SubjectId = subject.Id });
                            _context.SaveChanges();
                        }

                        var teacherName = teacherNames[_random.Next(teacherNames.Length)];
                        var teacherProfile = CreateTeacher(teacherName, subject.Id);

                        CreateLessons(teacherProfile, subject.Id);
                    }
                }
            }
        }

        private string GetArabicNumber(int n)
        {
            string[] numbers = { "", "الأول", "الثاني", "الثالث", "الرابع", "الخامس", "السادس", "السابع", "الثامن", "التاسع", "العاشر", "الحادي عشر", "الثاني عشر" };
            return n > 0 && n < numbers.Length ? numbers[n] : n.ToString();
        }

        private TeacherProfile CreateTeacher(string arabicName, Guid subjectId)
        {
            var userName = "teacher_" + arabicName.Replace(" ", "_") + "_" + _random.Next(100, 999);
            var teacherUser = _context.Users.FirstOrDefault(u => u.Name == arabicName && u.Type == UserType.Teacher);

            if (teacherUser == null)
            {
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
                profile = new TeacherProfile
                {
                    UserId = teacherUser.Id,
                    Name = arabicName,
                    Bio = $"أستاذ خبير متخصص في مادة {arabicName}",
                    IsActive = true,
                    AverageRating = 4.8m + (decimal)(_random.NextDouble() * 0.2)
                };
                _context.TeacherProfiles.Add(profile);
                _context.SaveChanges();

                // Add Teacher Image
                CreateAttachment(profile.Id.ToString(), AttachmentRefType.TeacherProfile, MediaType.Image, $"https://i.pravatar.cc/300?u={profile.Id}");
            }

            if (!_context.TeacherSubjects.Any(ts => ts.TeacherProfileId == profile.Id && ts.SubjectId == subjectId))
            {
                _context.TeacherSubjects.Add(new TeacherSubject { TeacherProfileId = profile.Id, SubjectId = subjectId });
                _context.SaveChanges();
            }

            return profile;
        }

        private void CreateLessons(TeacherProfile teacher, Guid subjectId)
        {
            var subjectNameAr = _context.Translations
                .Where(t => t.Code == "ar" && EF.Property<Guid>(t, "SubjectId") == subjectId)
                .Select(t => t.Name)
                .FirstOrDefault() ?? "المادة";

            int lessonCount = _random.Next(5, 11);
            for (int i = 1; i <= lessonCount; i++)
            {
                var lessonTitle = $"الدرس {i}: مقدمة شاملة في {subjectNameAr}";
                var lesson = _context.LessonSessions.FirstOrDefault(l => l.Title == lessonTitle && l.SubjectId == subjectId && l.TeacherProfileId == teacher.Id);

                if (lesson == null)
                {
                    lesson = new LessonSession
                    {
                        Title = lessonTitle,
                        Description = $"في هذا الدرس، سنقوم بشرح المفاهيم الأساسية للدرس {i} في {subjectNameAr} بأسلوب مبسط وشيق.",
                        SubjectId = subjectId,
                        TeacherProfileId = teacher.Id,
                        DurationSeconds = _random.Next(900, 2700),
                        Order = i,
                        IsActive = true,
                        IsFree = i == 1
                    };
                    _context.LessonSessions.Add(lesson);
                    _context.SaveChanges();

                    // Add Lesson Thumbnail
                    var thumb = CreateAttachment(lesson.Id.ToString(), AttachmentRefType.LessonSessionThumbnail, MediaType.Image, $"https://picsum.photos/seed/{lesson.Id}/400/225");
                    
                    // Add Lesson Video
                    var video = CreateAttachment(lesson.Id.ToString(), AttachmentRefType.LessonSessionVideo, MediaType.Video, "https://www.w3schools.com/html/mov_bbb.mp4");

                    // Link to LessonSessionAttachments table
                    if (!_context.LessonSessionAttachments.Any(la => la.LessonSessionId == lesson.Id && la.AttachmentId == thumb.Id))
                    {
                        _context.LessonSessionAttachments.Add(new LessonSessionAttachment { LessonSessionId = lesson.Id, AttachmentId = thumb.Id });
                    }
                    if (!_context.LessonSessionAttachments.Any(la => la.LessonSessionId == lesson.Id && la.AttachmentId == video.Id))
                    {
                        _context.LessonSessionAttachments.Add(new LessonSessionAttachment { LessonSessionId = lesson.Id, AttachmentId = video.Id });
                    }
                    _context.SaveChanges();

                    CreateQuizForLesson(lesson);
                }
            }
        }

        private void CreateQuizForLesson(LessonSession lesson)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.SessionId == lesson.Id);
            if (quiz == null)
            {
                quiz = new Quiz
                {
                    Title = $"اختبار: {lesson.Title}",
                    Description = $"اختبار قصير لتقييم فهمك لمحتوى {lesson.Title}",
                    SubjectId = lesson.SubjectId,
                    SessionId = lesson.Id,
                    TeacherId = lesson.TeacherProfileId,
                    DurationSeconds = 600,
                    TotalMarks = 100
                };
                _context.Quizzes.Add(quiz);
                _context.SaveChanges();

                // Add Quiz Image (Using AttachmentRefType.Other as no specific Quiz type exists)
                CreateAttachment(quiz.Id.ToString(), AttachmentRefType.Other, MediaType.Image, $"https://picsum.photos/seed/{quiz.Id}/400/300");
                
                // Add Quiz Video
                CreateAttachment(quiz.Id.ToString(), AttachmentRefType.Other, MediaType.Video, "https://www.w3schools.com/html/movie.mp4");

                int questionCount = _random.Next(5, 11);
                for (int i = 1; i <= questionCount; i++)
                {
                    bool isMcq = _random.Next(0, 2) == 0;
                    var question = new Question
                    {
                        QuizId = quiz.Id,
                        Type = isMcq ? QuestionType.MCQ : QuestionType.TrueFalse,
                        Text = isMcq ? $"السؤال {i}: اختر الإجابة الصحيحة بناءً على ما ورد في {lesson.Title}" : $"السؤال {i}: هل العبارة التالية صحيحة بخصوص موضوع الدرس؟",
                        Options = isMcq ? "[\"الخيار الأول\", \"الخيار الثاني\", \"الخيار الثالث\", \"الخيار الرابع\"]" : "[\"صح\", \"خطأ\"]",
                        CorrectAnswer = isMcq ? "\"الخيار الأول\"" : "\"صح\"",
                        AnswerDescription = "الإجابة مستمدة من الشرح التفصيلي الوارد في الفيديو التعليمي.",
                        Marks = 100 / questionCount
                    };
                    _context.Questions.Add(question);
                    _context.SaveChanges();

                    // Add Question Image
                    CreateAttachment(question.Id.ToString(), AttachmentRefType.Question, MediaType.Image, $"https://picsum.photos/seed/{question.Id}/200/200");
                }
            }
        }

        private Attachment CreateAttachment(string refId, AttachmentRefType refType, MediaType mediaType, string url)
        {
            var attachment = _context.Attachments.IgnoreQueryFilters().FirstOrDefault(a => a.RefId == refId && a.RefType == refType && a.Type == mediaType);
            if (attachment == null)
            {
                attachment = new Attachment
                {
                    RefId = refId,
                    RefType = refType,
                    Type = mediaType,
                    Url = url,
                    StorageKey = Guid.NewGuid().ToString(),
                    FileName = url.Split('/').Last(),
                    Size = 1024
                };
                _context.Attachments.Add(attachment);
                _context.SaveChanges();
            }
            return attachment;
        }
    }
}
