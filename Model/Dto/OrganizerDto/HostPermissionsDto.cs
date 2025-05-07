namespace PubQuizBackend.Model.Dto.OrganizerDto
{
    public class HostPermissionsDto
    {
        public bool CreateEdition { get; set; } = false;
        public bool EditEdition { get; set; } = false;
        public bool DeleteEdition { get; set; } = false;
        public bool CreateQuiz { get; set; } = false;
        public bool EditQuiz { get; set; } = false;
        public bool DeleteQuiz { get; set; } = false;
    }
}
