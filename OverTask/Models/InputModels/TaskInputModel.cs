namespace OverTask.Models.InputModels
{
	public class TaskInputModel
	{
		public int? Id { get; set; }
		public int? CategoryID { get; set; }
		public string Title { get; set; }
		public bool Completed { get; set; }
	}
}
