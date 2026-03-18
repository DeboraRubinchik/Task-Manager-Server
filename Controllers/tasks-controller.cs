using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using TaskManagerServerCS.Models;

namespace TaskManagerServerCS.Controllers;

[ApiController]
[Route("tasks")] 
public class TasksController : ControllerBase
{
    private readonly string _filePath = "tasks.json";

    //JSON יתעדכן עם אותיות קטנות ויעבוד טוב בערבית - לא ישמור גיבריש
    private JsonSerializerOptions GetJsonOptions() => new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    //פונקציית עזר לקריאה מהקובץ
    private List<TaskItem> GetTasksFromFile()
    {
        if (!System.IO.File.Exists(_filePath)) return new List<TaskItem>();
        try {
            var json = System.IO.File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json, GetJsonOptions()) ?? new List<TaskItem>();
        } catch { return new List<TaskItem>(); }
    }

    // פונקציית עזר לשמירה לקובץ
    private void SaveTasksToFile(List<TaskItem> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, GetJsonOptions());
        System.IO.File.WriteAllText(_filePath, json);
    }

    //שולף את כל המשימות
    [HttpGet]
    public IActionResult Get() => Ok(GetTasksFromFile());

    //מוסיף משימה חדשה
    [HttpPost]
    public IActionResult Post([FromBody] TaskItem task)
    {
        var tasks = GetTasksFromFile();
        task.Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
        tasks.Add(task);
        SaveTasksToFile(tasks);
        return Ok(task);
    }

    //מעדכן משימה קיימת לפי הID שלה
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] TaskItem updatedTask)
    {
        var tasks = GetTasksFromFile();
        var index = tasks.FindIndex(t => t.Id == id);
        if (index == -1) return NotFound();

        updatedTask.Id = id; 
        tasks[index] = updatedTask;
        SaveTasksToFile(tasks);
        return Ok(updatedTask);
    }

    //מוחק משימה לפי ID
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var tasks = GetTasksFromFile();
        var taskToRemove = tasks.FirstOrDefault(t => t.Id == id);
        if (taskToRemove == null) return NotFound();

        tasks.Remove(taskToRemove);
        SaveTasksToFile(tasks);
        return NoContent();
    }
}