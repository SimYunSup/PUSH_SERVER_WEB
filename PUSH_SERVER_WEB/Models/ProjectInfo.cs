
namespace PUSH_SERVER_WEB.Models
{
    public class ProjectAddInfo
    {
        public int Id;
        public string Title { get; set; } = "";
        public string FirebaseId { get; set; } = "";
        public string Credentials { get; set; } = "";
    }
    public class ProjectInfo {
        public int Id;
        //Firebase ID
        public string ProjectId { get; set; } = "";
        //Project Name
        public string ProjectName { get; set; } = "";
        // API Client Key
        public string ClientKey { get; set; } = "";
        public string Credentials { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string UpdatedAt { get; set; } = "";
    }
}