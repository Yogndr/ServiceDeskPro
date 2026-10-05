using System.ComponentModel.DataAnnotations;

namespace ServiceDeskPro.API.DTOs.Comments;

public class AddCommentDto
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
}