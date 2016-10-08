using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Student.Models;

namespace Student.Collector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var course = new Course {GroupCount = 5};


            UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-1-gr/", 1);
            UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-2-gr/", 2);
            UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-3-gr/", 3);
            UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-4-gr/", 4);
            UpdateCourse(course, "https://mif.vu.lt/timetable/mif/groups/612i30001-programu-sistemos-3-k-5-gr/", 5);

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
