using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

class Program
{
  static void Main()
  {
    int port = 5000;

    var server = new Server(port);

    Console.WriteLine("The server is running");
    Console.WriteLine($"Main Page: http://localhost:{port}/website/pages/index.html");

    var database = new Database();

    if (!database.Groups.Any())
    {
      database.Subjects.Add(new Subject("Web Development", "Dvir Arazi"));
      database.Subjects.Add(new Subject("C# Fundamentals", "Ofer Shafir"));
      database.Subjects.Add(new Subject("Physics", "Anna Yelkin"));
      database.Subjects.Add(new Subject("English", "Liran Roimi"));
      
      database.Groups.Add(new Group("Class A"));
      database.Groups.Add(new Group("Class B"));
      database.Groups.Add(new Group("Class C"));
      database.SaveChanges();

      database.Students.Add(new Student("Mika", 1));
      database.Students.Add(new Student("Dennis", 1));
      database.Students.Add(new Student("Yuval", 1));
      database.Students.Add(new Student("Adelina", 2));
      database.Students.Add(new Student("Yarona", 2));
      database.Students.Add(new Student("Aviv", 2));
      database.Students.Add(new Student("Maya G", 2));
      database.Students.Add(new Student("Maya R", 3));
      database.Students.Add(new Student("Noam", 3));
      database.Students.Add(new Student("Nikol", 3));
      database.SaveChanges();

      database.Grades.Add(new Grade(92, 1, 1));
      database.Grades.Add(new Grade(95, 1, 3));
      database.Grades.Add(new Grade(95, 1, 4));

      database.Grades.Add(new Grade(99, 2, 1));
      database.Grades.Add(new Grade(95, 2, 2));

      database.Grades.Add(new Grade(81, 3, 1));
      database.Grades.Add(new Grade(85, 3, 2));
      database.Grades.Add(new Grade(87, 3, 3));
      database.Grades.Add(new Grade(97, 3, 4));

      database.Grades.Add(new Grade(83, 4, 1));
      database.Grades.Add(new Grade(92, 4, 3));
      database.Grades.Add(new Grade(84, 4, 4));
      
      database.Grades.Add(new Grade(91, 5, 3));

      database.Grades.Add(new Grade(87, 6, 1));
      database.Grades.Add(new Grade(80, 6, 1));
      database.Grades.Add(new Grade(97, 6, 1));

      database.Grades.Add(new Grade(98, 7, 1));
      database.Grades.Add(new Grade(96, 7, 2));
      database.Grades.Add(new Grade(84, 7, 4));
      
      database.Grades.Add(new Grade(85, 8, 1));
      database.Grades.Add(new Grade(95, 8, 2));
      
      database.Grades.Add(new Grade(87, 9, 2));
      
      database.Grades.Add(new Grade(98, 10, 1));
      database.Grades.Add(new Grade(90, 10, 2));
      database.Grades.Add(new Grade(89, 10, 3));
      database.SaveChanges();
    }


    while (true)
    {
      (var request, var response) = server.WaitForRequest();

      Console.WriteLine($"Recieved a request with the path: {request.Path}");

      if (File.Exists(request.Path))
      {
        var file = new File(request.Path);
        response.Send(file);
      }
      else if (request.ExpectsHtml())
      {
        var file = new File("website/pages/404.html");
        response.SetStatusCode(404);
        response.Send(file);
      }
      else
      {
        try
        {
          if (request.Path == "getGroups")
          {
            var groups = database.Groups.ToArray();

            response.Send(groups);
          }
          if (request.Path == "getStudents")
          {
            var groupId = request.GetBody<int>();

            var students = database.Students.Where(student => student.GroupId == groupId);

            response.Send(students);
          }
          if (request.Path == "getGrades")
          {
            var studentId = request.GetBody<int>();
            var grades = database.Grades.Where(grade => grade.StudentId == studentId);

            response.Send(grades);
          }
          else
          {
            response.SetStatusCode(405);
          }

          database.SaveChanges();
        }
        catch (Exception exception)
        {
          Log.WriteException(exception);
        }
      }

      response.Close();
    }
  }
}


class Database() : DbBase("database")
{
  public DbSet<Subject> Subjects { get; set; } = default!;
  public DbSet<Group> Groups { get; set; } = default!;
  public DbSet<Student> Students { get; set; } = default!;
  public DbSet<Grade> Grades { get; set; } = default!;
}

class Group(string name)
{
  [Key] public int Id { get; set; } = default!;
  public string Name { get; set; } = name;
}

class Student(string name, int groupId)
{
  [Key] public int Id { get; set; } = default!;

  public string Name { get; set; } = name;

  public int GroupId { get; set; } = groupId;
  [ForeignKey("GroupId")] public Group Group { get; set; } = default!;
}

class Subject(string name, string teacher)
{
  [Key] public int Id { get; set; } = default!;
  public string Name { get; set; } = name;
  public string Teacher { get; set; } = teacher;
}

class Grade(int score, int studentId, int subjectId)
{
  [Key] public int Id { get; set; } = default!;
  public int Score { get; set; } = score;
  public int SubjectId { get; set; } = subjectId;
  [ForeignKey("SubjectId")] public Subject Subject { get; set; } = default!;
  public int StudentId { get; set; } = studentId;
  [ForeignKey("StudentId")] public Student Student { get; set; } = default!;
}