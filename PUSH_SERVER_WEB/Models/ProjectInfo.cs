
namespace PUSH_SERVER_WEB.Models
{
    public class ProjectAddInfo
    {
        public int Id;
        public string Title { get; set; } = "";
        public string FirebaseId { get; set; } = "";
        public string Credentials { get; set; } = "";
    }
    public class ProjectServerAddInfo
    {
        public string project_name { get; set; } = "";
        public string project_id { get; set; } = "";
        public string credentials { get; set; } = "";
    }
    public class ProjectInfo : ICloneable {
        public int Id{ get; set; } = 1;
        //Firebase ID
        public string ProjectId { get; set; } = "";
        //Project Name
        public string ProjectName { get; set; } = "";
        // API Client Key
        public string ClientKey { get; set; } = "";
        public string Credentials { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string UpdatedAt { get; set; } = "";
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class ProjectServerInfo {
        public int id { get; set; } = 1;
        //Firebase ID
        public string project_id { get; set; } = "";
        //Project Name
        public string project_name { get; set; } = "";
        // API Client Key
        public string client_key { get; set; } = "";
        public string credentials { get; set; } = "";
        public string created_at { get; set; } = "";
        public string updated_at { get; set; } = "";
    }
    public class ClientKeyInfo {
        public string client_key { get; set; } = "";
    }
}