using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {

            int entriesFound = 0;
            Course[] courses = { };
            string path2 = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
                @"App_Data\Courses.csv");

            

            using (var textReader = new StreamReader(path2))
            {
                string line = textReader.ReadLine();
                int skipCount = 0;
                while (line != null && skipCount < 1)
                {
                    line = textReader.ReadLine();
                    skipCount++;
                }
                
                while (line != null)
                {
                    string[] columns = line.Split(',');
                    entriesFound++;
                    line = textReader.ReadLine();
                    if (line != null)
                    {
                        var course = CreateCourse(line);
                        //courses.Add(course);
                        Array.Resize(ref courses,courses.Length+1);
                        courses[courses.Length - 1] = course;
                    }
                    
                }

                
            }

            PrintCourseList(courses);

            PrintGroupedCourses(courses);

            int entriesFound1 = 0;
            var courses1 = new List<Course>();
            string path3 = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
                @"App_Data\Instructors.csv");

            var instructors = new List<Instructor>();

            using (var textReader = new StreamReader(path3))
            {
                string line = textReader.ReadLine();
                int skipCount = 0;
                while (line != null && skipCount < 1)
                {
                    line = textReader.ReadLine();
                    skipCount++;
                }

                while (line != null)
                {
                    string[] columns = line.Split(',');
                    entriesFound1++;
                    line = textReader.ReadLine();
                    if (line != null)
                    {
                        var instructor = CreateInstructor(line);
                        instructors.Add(instructor);
                    }

                }


            }

            PrintCoursesWithEmail(courses, instructors);
            Console.ReadLine();
        }

        private static void PrintCoursesWithEmail(Course[] courses, List<Instructor> instructors)
        {
            var result = courses.Where(x => x.CourseNumber >= 200 && x.CourseNumber < 300).GroupJoin(instructors,
                c => c.Instructor, i => i.InstructorName, (c, i) => new
                {
                    Course = c,
                    Instructor = i
                }).SelectMany(c => c.Instructor.DefaultIfEmpty(), (c, i) => new
            {
                CourseName = c.Course.Title,
                CourseCode = $"{c.Course.Subject} {c.Course.CourseNumber}",
                Email = i?.Email
            });
            
            foreach (var item in result)
            {
                Console.WriteLine($"{item.CourseName},{item.CourseCode},{item.Email}");
            }
            Console.WriteLine("---------------------------------------------");
        }

        private static Instructor CreateInstructor(string line)
        {
            Instructor instructor = new Instructor();
            var fields = line.Split(',');
            instructor.InstructorName = fields[0];
            instructor.OfficeNumber = fields[1];
            instructor.Email = fields[2];

            return instructor;
        }

        private static void PrintGroupedCourses(Course[] courses)
        {
            var result = courses.GroupBy(x => new
            {
                x.Subject
            }).Select(x => new
            {
                x.Key.Subject,
                Courses = x.GroupBy(c => c.CourseNumber)
            }).Where(y => y.Courses.Count() >= 2);
            

            foreach (var item in result)
            {
                Console.WriteLine($"{item.Subject}");
                foreach (var course in item.Courses)
                {
                    Console.WriteLine($"\t{course.Key}");
                }
            }
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("1.5: Print the results for all 200 level courses: \n");
        }



        private static void PrintCourseList(Course[] courses)
        {
            var result = courses.Where(x => x.CourseNumber >= 300)
                .OrderBy(y => y.Instructor).Select(z => new
                {
                    CourseTitle = z.Title,
                    Instructor = z.Instructor
                });

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("1.2a: The result set must be sorted by instructor and in ascending order: \n");
            
            foreach (var item in result)
            {
                Console.WriteLine($"{item.CourseTitle},{item.Instructor}");
            }

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("1.2b: Print the groups that have at least two courses in the second level group: \n ");
        }

        private static Course CreateCourse(string line)
        {
            Course course = new Course();
            var fields = line.Split(',');
            var subjectFields = fields[0].Split(' ');
            course.Subject = subjectFields[0];
            course.CourseNumber = int.Parse(GetCourseNumber(fields[0]));
            course.Title = fields[1];
            //int parsed = 0;
            //if (int.TryParse(fields[2].Trim(), out parsed))
            //{
                course.CourseId = fields[2];
            //}
            course.Instructor = fields[3];
            course.Days = fields[4];
            course.Start = fields[5];
            course.End = fields[6];
            course.Location = fields[7];
            course.Dates = fields[8];
            int parsed = 0;
            if (int.TryParse(fields[9].Trim(), out parsed))
            {
                course.Units = parsed;
            }

            course.Enrollment = fields[10];

            return course;
        }

        private static string GetCourseNumber(string subject)
        {
            var fields = subject.Split(' ');
            return fields[1];
        }
    }
}
