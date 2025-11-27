using System.ComponentModel.DataAnnotations;

namespace DATT.API.DTOs.Task
{
	public class TaskCreateDto
	{
		[Required(ErrorMessage = "Tiêu đề không được để trống")]
		[MaxLength(100, ErrorMessage = "Tiêu đề tối đa 100 ký tự")]
		public string Title { get; set; }

		[MaxLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự")]
		public string? Description { get; set; }

		public bool IsCompleted { get; set; } = false;

		public DateTime? Deadline { get; set; }
	}
}
