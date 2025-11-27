using System.ComponentModel.DataAnnotations;

namespace DATT.API.DTOs.Task
{
	public class TaskUpdateDto
	{
		[Required(ErrorMessage = "Tiêu đề không được để trống")]
		[MaxLength(100)]
		public string Title { get; set; }

		[MaxLength(255)]
		public string? Description { get; set; }

		public bool IsCompleted { get; set; }
		public DateTime? Deadline { get; set; }

	}
}
