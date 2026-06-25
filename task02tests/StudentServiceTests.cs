using System.Collections.Generic;
using System.Linq;
using Xunit;
using task02;

namespace task02tests;

public class StudentServiceTests
{
    private List<Student> _testStudents;
    private StudentService _service;

    public StudentServiceTests()
    {
        _testStudents = new List<Student>
        {
            new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
            new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
            new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } }
        };
        _service = new StudentService(_testStudents);
    }

    // 1. Тест фильтрации по факультету
    [Fact]
    public void GetStudentsByFaculty_ReturnsCorrectStudents()
    {
        var result = _service.GetStudentsByFaculty("ФИТ").ToList();
        
        Assert.Equal(2, result.Count); 
        Assert.True(result.All(s => s.Faculty == "ФИТ")); 
    }

    // 2. Тест нахождения лучшего факультета
    [Fact]
    public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
    {
        var result = _service.GetFacultyWithHighestAverageGrade();
        
        Assert.Equal("Экономика", result);
    }

    // 3. Тест фильтрации по среднему баллу
    [Fact]
    public void GetStudentsWithMinAverageGrade_ReturnsFilteredStudents()
    {
        var result = _service.GetStudentsWithMinAverageGrade(4.0).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "Иван");
        Assert.Contains(result, s => s.Name == "Петр");
        Assert.DoesNotContain(result, s => s.Name == "Анна");
    }

    // 4. Тест сортировки по имени по алфавиту
    [Fact]
    public void GetStudentsOrderedByName_ReturnsAlphabeticalOrder()
    {
        var result = _service.GetStudentsOrderedByName().ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("Анна", result[0].Name);
        Assert.Equal("Иван", result[1].Name);
        Assert.Equal("Петр", result[2].Name); 
    }

    // 5. Тест группировки студентов по факультетам
    [Fact]
    public void GroupStudentsByFaculty_ReturnsCorrectLookup()
    {
        var result = _service.GroupStudentsByFaculty();

        Assert.Equal(2, result.Count);
        Assert.Equal(2, result["ФИТ"].Count());
        Assert.Single(result["Экономика"]); 
    }
}