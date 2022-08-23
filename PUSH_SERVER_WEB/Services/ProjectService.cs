using PUSH_SERVER_WEB.Helpers;
using PUSH_SERVER_WEB.Models;
using System.Linq;

namespace PUSH_SERVER_WEB.Services
{
    public interface IProjectService
    {
        ProjectInfo? ProjectInfo { get; }
        public Task<ProjectInfo[]?> GetProjects();
        public Task<ProjectInfo?> GetProject(string id);
        Task AddProject(ProjectAddInfo projectAddInfo);
        void ChangeProjectName(string name);
        void ChangeProjectInfo(ProjectInfo changedProjectInfo);
        void DeleteProject();
        void RefreshClientKey();
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

        public Task<ProjectInfo[]?> GetProjects()
        {
            return _httpService.Get<ProjectServerInfo[]>("/api/project/all").ContinueWith(
                response => {
                    if (response.Result == null)
                        return default;
                    ProjectInfo[] projectInfos = new ProjectInfo[response.Result.Length];
                    foreach(var item in response.Result.Select((value, index) => (value, index)))
                    {
                        projectInfos[item.index] = new ProjectInfo {
                            Id=item.value.id,
                            ProjectId=item.value.project_id,
                            ProjectName=item.value.project_name,
                            ClientKey=item.value.client_key,
                            Credentials=item.value.credentials,
                            CreatedAt=item.value.created_at,
                            UpdatedAt=item.value.updated_at
                        };
                    }
                    return projectInfos;
                }
            );
            
        }
        public Task<ProjectInfo?> GetProject(string id)
        {
            return _httpService.Get<ProjectServerInfo>($"/api/project/{id}").ContinueWith(
                response => {
                        if (response.Result == null)
                            return default;
                        var TempProjectInfo = new ProjectInfo {
                            Id=response.Result.id,
                            ProjectId=response.Result.project_id,
                            ProjectName=response.Result.project_name,
                            ClientKey=response.Result.client_key,
                            Credentials=Base64Util.Base64Decode(response.Result.credentials),
                            CreatedAt=response.Result.created_at,
                            UpdatedAt=response.Result.updated_at
                        };
                        ProjectInfo = TempProjectInfo;
                        return TempProjectInfo;
                    }
            );
            
        }

        public Task AddProject(ProjectAddInfo projectAddInfo)
        {
            return _httpService.Post<ProjectInfo>($"/api/project", new ProjectServerAddInfo{
                project_id=projectAddInfo.FirebaseId,
                project_name=projectAddInfo.Title,
                credentials=Base64Util.Base64Encode(projectAddInfo.Credentials)
            });
        }

        public async void ChangeProjectName(string name) 
        {
            if (ProjectInfo == null)
            {
                return;
            }
            ProjectInfo.ProjectName = name;
            await _httpService.Patch<object>($"/api/project/{ProjectInfo.Id}", new ProjectServerAddInfo{
                project_name=name,
                project_id=ProjectInfo.ProjectId,
                credentials=Base64Util.Base64Encode(ProjectInfo.Credentials)
            });

        }

        public async void ChangeProjectInfo(ProjectInfo changedProjectInfo) 
        {
            if (ProjectInfo == null)
            {
                return;
            }
            ProjectInfo.ProjectId = changedProjectInfo.ProjectId;
            ProjectInfo.Credentials = changedProjectInfo.Credentials;
            await _httpService.Patch<object>($"/api/project/{ProjectInfo.Id}", new ProjectServerAddInfo{
                project_name=ProjectInfo.ProjectName,
                project_id=changedProjectInfo.ProjectId,
                credentials=Base64Util.Base64Encode(changedProjectInfo.Credentials)
            });
        }

        public async void DeleteProject() 
        {
            if (ProjectInfo == null)
            {
                return;
            }
            await _httpService.Delete<object>($"/api/project/{ProjectInfo.Id}");
            ProjectInfo = null;
        }

        public async void RefreshClientKey()
        {
            if (ProjectInfo == null)
            {
                return;
            }
            await _httpService.Patch<ClientKeyInfo>($"/api/project/{ProjectInfo.Id}/client-key", new Object{})
            .ContinueWith(
                response =>
                {
                    if (response.Result == null)
                    {
                        return;
                    }
                    ProjectInfo.ClientKey = response.Result.client_key;
                }
            );
        }
    }

}