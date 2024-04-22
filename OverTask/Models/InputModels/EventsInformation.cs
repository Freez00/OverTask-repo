namespace OverTask.Models.InputModels
{
	public class EventsInformation
	{
		public int day { get; set; }
		public int month { get; set; }
		public int year { get; set; }
		public PersonalUserEventInput[] events { get; set; }
	}
}
