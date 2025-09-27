﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QuestionService.Models;

public class Answer
{
    [MaxLength(36)] public string Id { get; set; } = Guid.NewGuid().ToString();
    [MaxLength(5000)] public string Content { get; set; }
    [MaxLength(50)] public string UserId { get; set; }
    [MaxLength(1000)] public string UserDisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool Accepted { get; set; }
    [MaxLength(36)] public required string QuestionId { get; set; }
    [JsonIgnore] public Question Question { get; set; } = null!;

    public static Answer Create(string content, string userId, string userDisplayName, string questionId)
    {
        return new Answer
        {
            Content = content,
            UserId = userId,
            UserDisplayName = userDisplayName,
            QuestionId = questionId,
        };
    }
    
}