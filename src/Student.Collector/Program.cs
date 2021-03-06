﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Student.Models;
using Student.Repository;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Packaging;

namespace Student.Collector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var course = new Course {GroupCount = 5};


            //UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-1-gr/", 1);
            //UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-2-gr/", 2);
            //UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-3-gr/", 3);
            //UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-4-gr/", 4);
            //UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-5-gr/", 5);

            //RefactorCourse(course);

            var context = new StudentContext(new DbContextOptionsBuilder().UseSqlServer("Server=(localdb)\\MSSQLLocalDb;Database=TheStudentDb;Trusted_Connection=true;MultipleActiveResultSets=true;").Options);
            //context.Courses.Add(course);
            // context.SaveChanges();
            //ILoggerFactory loggerFactory = new LoggerFactory()
            //    .AddDebug()
            //    .AddConsole();

            var serviceProvider = context.GetInfrastructure<IServiceProvider>();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddDebug()
                .AddConsole();




            var course = context.Courses.Include(c=>c.Subjects)
                .ThenInclude(s => s.Groups)
                .ThenInclude(g => g.Subgroups)
                .ThenInclude(sg => sg.Lectures)
                .FirstOrDefault();

            var user = new User {Email = "dainius.kavoliunas@vu.mif.lt", Pasword = "12345678"};
            user.Subscriptions.AddRange(course.Subjects.SelectMany(x => x.Groups).SelectMany(x => x.Subgroups).Where(x=>x.Group.Value == 1 && x.Value < 2).Select(x=>new Subscription { Subgroup = x}));

            //context.Users.Add(user);
            //context.SaveChanges();

            var u = context.Users.Include(x => x.Subscriptions)
                .ThenInclude(x => x.Subgroup)
                .ThenInclude(x => x.Lectures);
        }

        private static void RefactorCourse(Course course)
        {
            var subgroups = course.Subjects.SelectMany(x => x.Groups).SelectMany(x=>x.Subgroups).ToArray();

            foreach (var sg in subgroups)
            {
                foreach (var lecture in sg.Lectures)
                {
                    var subgroupsWithDuplicateLectures = subgroups.Where(x=>x.Lectures.Any(l=>l.Lecturer == lecture.Lecturer && l.EndTime == lecture.EndTime && l.StartTime == lecture.StartTime && l.Id!= lecture.Id));
                    foreach (var sgd in subgroupsWithDuplicateLectures)
                    {
                        sgd.Lectures.Remove(sgd.Lectures.First(l => l.Lecturer == lecture.Lecturer && l.EndTime == lecture.EndTime && l.StartTime == lecture.StartTime && l.Id != lecture.Id));
                        sgd.Lectures.Add(lecture);
                        Debug.WriteLine(lecture.Lecturer);
                    }
                }
            }

        }

        private static void UpdateCourse(Course course, string url, int groupValue)
        {
            var client = new HttpClient();
            var response = client.GetAsync(url).Result;

            var content = response.Content.ReadAsStringAsync().Result;

            var html = new HtmlDocument();
            html.LoadHtml(content);

            var scriptText = html.DocumentNode.SelectSingleNode("//body/script[last()]").FirstChild.InnerHtml;
            var s = scriptText.Substring(scriptText.IndexOf("events", StringComparison.Ordinal));
            s = s.Substring(s.IndexOf("[", StringComparison.Ordinal));
            s = s.Remove(s.IndexOf("]", StringComparison.Ordinal) + 1);

            var jsonLectures = JArray.Parse(s);

            foreach (var l in jsonLectures)
            {
                var data = l["title"].Value<string>();
                var startDate = ConvertToTimestamp(l["start"].Value<DateTime>().ToUniversalTime());
                var endDate = ConvertToTimestamp(l["end"].Value<DateTime>().ToUniversalTime());

                html.LoadHtml(data);
                var dataNullable = html.DocumentNode.FirstChild.Attributes["data-content"];

                if (dataNullable == null)
                {
                    Debug.WriteLine(data);
                    continue;
                }

                var info = dataNullable.Value.Split(new[] { "</a><hr>" }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList()
                    .Select(x => x.Split('>').Last().Replace("(", "").Replace(")", ""))
                    .ToArray();

                var subjectName = info[0];
                var lecturer = info[1];
                var location = info[2];
                var additionalInfo = info.Length == 4 ? info[3] : "";

                var longTitle = html.DocumentNode.SelectSingleNode("/a").InnerHtml.ToLower();

                var subjectType = GetCourseType(longTitle);
                var lectureType = GetLectureType(longTitle);

                var subgroupValue = 0;

                if (longTitle.Contains("pogrupiai:"))
                {
                    subgroupValue = int.Parse(longTitle.ToCharArray().Last().ToString());
                }

                if (course.Subjects.FirstOrDefault(x => x.Name == subjectName) == null)
                {
                    course.Subjects.Add(new Subject { Name = subjectName, Type = subjectType });
                }

                var subject = course.Subjects.First(x => x.Name == subjectName);

                if (subject.Groups.FirstOrDefault(x => x.Value == groupValue) == null)
                {
                    subject.Groups.Add(new Group { Value = groupValue, Subject = subject });
                }

                var group = subject.Groups.First(x => x.Value == groupValue);

                if (group.Subgroups.FirstOrDefault(x => x.Value == subgroupValue) == null)
                {
                    group.Subgroups.Add(new Subgroup { Group = group, Value = subgroupValue });
                }

                var subgroup = group.Subgroups.First(x => x.Value == subgroupValue);

                subgroup.Lectures.Add(new Lecture { Subgroup = subgroup, EndTime = endDate, StartTime = startDate, LectureType = lectureType, Location = location, Lecturer = lecturer });
            }
        }

        private static SubjectType GetCourseType(string longTitle)
        {
            if (longTitle.Contains("(privalomasis)") && !longTitle.Contains("(pasirenkamasis)"))
            {
                return SubjectType.Mandatory;
            }
            if (longTitle.Contains("(pasirenkamasis)") && !longTitle.Contains("privalomasis)"))
            {
                return SubjectType.Optional;
            }
            throw new Exception("No course type");
        }
        private static LectureType GetLectureType(string longTitle)
        {

            if (longTitle.Contains("(paskaita)") && !longTitle.Contains("(pratybos)") &&
                !longTitle.Contains("(paskaitos ir pratybos)"))
            {
                return LectureType.Theory;
            }
            if (longTitle.Contains("(paskaitos ir pratybos)") && !longTitle.Contains("(paskaita)") &&
                     !longTitle.Contains("(pratybos)"))
            {
                return LectureType.Practice | LectureType.Theory;
            }
            if (longTitle.Contains("(pratybos)") && !longTitle.Contains("(paskaitos ir pratybos)") &&
                     !longTitle.Contains("(paskaita)"))
            {
                return LectureType.Practice;
            }

            throw new Exception("No lecture type");

        }

        private static long ConvertToTimestamp(DateTime value)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var elapsedTime = value - epoch;
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
