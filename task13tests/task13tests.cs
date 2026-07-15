using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using task13;
using Xunit;

namespace task13tests;

public class JsonTests
{
    [Fact]
    public void Test_Serialize()
    {
        Student student = new Student();
        student.FirstName = "Иван";
        student.LastName = "Иванов";
        student.BirthDate = new DateTime(2005, 5, 15);
        student.Grades = null;

        string json = StudentSerializer.Serialize(student);

        Assert.Contains("15.05.2005", json);
        Assert.DoesNotContain("Grades", json);
    }

    [Fact]
    public void Test_Deserialize()
    {
        string json = "{\"FirstName\":\"Петр\",\"LastName\":\"Петров\",\"BirthDate\":\"10.10.2006\"}";

        Student student = StudentSerializer.Deserialize(json);

        Assert.Equal("Петр", student.FirstName);
        Assert.Equal(new DateTime(2006, 10, 10), student.BirthDate);
    }

    [Fact]
    public void Test_ErrorWhenNoLastName()
    {
        string json = "{\"FirstName\":\"Анна\",\"BirthDate\":\"10.10.2006\"}";

        Assert.Throws<JsonException>(() => StudentSerializer.Deserialize(json));
    }

    [Fact]
    public void Test_FileSaveAndLoad()
    {
        string filePath = "test_student.json";

        Student student = new Student();
        student.FirstName = "Мария";
        student.LastName = "Смирнова";
        student.BirthDate = new DateTime(2004, 8, 20);

        Subject sub = new Subject();
        sub.Name = "Math";
        sub.Grade = 5;

        student.Grades = new List<Subject>();
        student.Grades.Add(sub);

        StudentSerializer.SaveToFile(student, filePath);
        Student loadedStudent = StudentSerializer.LoadFromFile(filePath);

        Assert.Equal(student.FirstName, loadedStudent.FirstName);
        
        Assert.Equal(1, loadedStudent.Grades.Count);
        Assert.Equal("Math", loadedStudent.Grades[0].Name);

        File.Delete(filePath);
    }
}