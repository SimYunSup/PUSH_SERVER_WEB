using PUSH_SERVER_WEB.Models;

namespace PUSH_SERVER_WEB.Services
{
    public interface IProjectService
    {
        ProjectInfo? ProjectInfo { get; }
        void GetProject(int id);
        void ChangeProjectName(string name);
    }

    public class ProjectService : IProjectService
    {
        private IHttpService _httpService;
        public ProjectInfo? ProjectInfo { get; private set; }

        public ProjectService(
            IHttpService httpService
        ) {
            _httpService = httpService;
        }
        public void GetProject(int id)
        {
            // ProjectInfo = await _httpService.Get<ProjectInfo>("/")
            ProjectInfo = new ProjectInfo{
                Id= 1, 
                ProjectName= "코인",
                ClientKey= "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
                ProjectId= "ABCDEFGH",
                CreatedAt = "2019-08-01",
                UpdatedAt = "2022-08-08"
            };
        }

        public void ChangeProjectName(string name) 
        {
            if (ProjectInfo == null)
            {
                return;
            }
            ProjectInfo.ProjectName = name;
        }
    }

}