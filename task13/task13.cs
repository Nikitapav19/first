using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; }
    public int Grade { get; set; }
}

public class Student
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }
    
    public List<Subject> Grades { get; set; }
}

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string dateString = reader.GetString();
        return DateTime.ParseExact(dateString, "dd.MM.yyyy", null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("dd.MM.yyyy"));
    }
}

public static class StudentSerializer
{
    static JsonSerializerOptions options = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true 
    };

    public static string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, options);
    }

    public static Student Deserialize(string json)
    {
        Student student = JsonSerializer.Deserialize<Student>(json, options);

        if (string.IsNullOrEmpty(student.FirstName) || string.IsNullOrEmpty(student.LastName))
        {
            throw new JsonException("Имя и фамилия пустые");
        }
        if (student.BirthDate > DateTime.Now)
        {
            throw new JsonException("Дата рождения в будущем");
        }

        return student;
    }

    public static void SaveToFile(Student student, string filePath)
    {
        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}