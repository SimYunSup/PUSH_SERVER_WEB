using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PUSH_SERVER_WEB.Helpers
{
    public class FakeBackendHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var users = new[] { new { id = 1, username = "string", password = "stringst" } };
            var projects = new[] { 
                new { 
                    id = 1, 
                    project_name = "string", 
                    project_id = "stringst", 
                    created_at = "2022-08-23 18:42:40", 
                    updated_at = "2022-08-29 18:42:50", 
                    client_key="7FBAD50A2C0B47D9804084079FA17FDD",
                    credentials= "eyJ0ZXN0IjogInRlc3QifQ==",
                },
                new { 
                    id = 2, 
                    project_name = "test", 
                    project_id = "testst", 
                    created_at = "2022-10-23 18:42:40", 
                    updated_at = "2022-11-29 05:42:50", 
                    client_key="7FBAD50A2C0B47D98AA0840AAFA17FDD",
                    credentials= "eyJ0ZXN0IjogInRlc3QifQ==",
                },
            };
            var path = request.RequestUri?.AbsolutePath;
            var method = request.Method;
            var projectRegex = new System.Text.RegularExpressions.Regex(@"\/api\/project\/(\d+)$");

            if (projectRegex.IsMatch(path ?? "")) {
                var projectMatches = projectRegex.Match(path ?? "");
                if (method == HttpMethod.Get)
                    return await getProject(int.Parse(projectMatches.Groups[1].Value));
                if (method == HttpMethod.Delete)
                    return await deleteProject(int.Parse(projectMatches.Groups[1].Value));
                if (method == HttpMethod.Patch)
                    return await patchProject(int.Parse(projectMatches.Groups[1].Value));
            }
            var projectClientKeyRegex = new System.Text.RegularExpressions.Regex(@"\/api\/project\/(\d+)/client-key$");

            if (projectClientKeyRegex.IsMatch(path ?? "") && method == HttpMethod.Patch) {
                var projectMatches = projectClientKeyRegex.Match(path ?? "");
                if (method == HttpMethod.Patch)
                    return await patchProjectClientKey(int.Parse(projectMatches.Groups[1].Value));
            }

            if (path == "/api/user/login" && method == HttpMethod.Post)
            {
                return await authenticate();
            }
            else if (path == "/api/user/logout" && method == HttpMethod.Get)
            {
                    return await jsonResponse(HttpStatusCode.NoContent, "");
            }
            else if (path == "/api/user/refresh" && method == HttpMethod.Post)
            {
                return await refresh();
            }
            else if (path == "/api/project/all" && method == HttpMethod.Get)
            {
                return await getProjects();
            }
            else
            {
                // pass through any requests not handled above
                return await base.SendAsync(request, cancellationToken);
            }

            // route functions
            
            async Task<HttpResponseMessage> authenticate()
            {
                var bodyJson = await request.Content!.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyJson)!;
                var user = users.FirstOrDefault(x => x.username == body["username"] && x.password == body["password"]);

                if (user == null)
                    return await error("아이디나 패스워드가 잘못되었습니다.");

                return await ok(new {
                    refresh_token= "aaa.bbb.ccc",
                    access_token= "ddd.eee.qqq"
                });
            }
            async Task<HttpResponseMessage> refresh() {
                if (request.Headers.Authorization?.Parameter != "Bearer aaa.bbb.ccc") 
                {
                    return await jsonResponse(HttpStatusCode.Conflict, "token이 유효하지 않습니다.");;
                }

                return await jsonResponse(HttpStatusCode.Created, new {
                    refresh_token= "aaa.bbb.ccc",
                    access_token= "ddd.eee.qqq"
                });
            }

            async Task<HttpResponseMessage> getProjects()
            {
                if (!isLoggedIn()) return await unauthorized();
                return await ok(projects);
            }
            async Task<HttpResponseMessage> getProject(int id)
            {
                if (!isLoggedIn()) return await unauthorized();
                var project = projects.FirstOrDefault(x => x.id == id);
                if (project != null) {
                    return await ok(project);
                }
                return await jsonResponse(HttpStatusCode.NotFound, "프로젝트를 찾지 못했습니다");
            }
            async Task<HttpResponseMessage> deleteProject(int id)
            {
                if (!isLoggedIn()) return await unauthorized();
                var project = projects.FirstOrDefault(x => x.id == id);
                if (project != null) {
                    projects = projects.Where((val) => val.id != id).ToArray(); 
                    return await jsonResponse(HttpStatusCode.NoContent, "");
                }
                return await jsonResponse(HttpStatusCode.NotFound, "프로젝트를 찾지 못했습니다");
            }
            async Task<HttpResponseMessage> patchProject(int id)
            {
                if (!isLoggedIn()) return await unauthorized();
                var bodyJson = await request.Content!.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyJson)!;
                var project = projects.FirstOrDefault(x => x.id == id);
                if (project != null) {
                    var newProject = new  { 
                        id = project.id, 
                        project_name = body["project_name"] ?? project.project_name, 
                        project_id = body["project_id"] ?? project.project_id,
                        created_at = project.created_at, 
                        updated_at = project.updated_at, 
                        client_key = project.client_key,
                        credentials= body["project_credentials"] ?? project.credentials,
                    };
                    projects = projects.Where((val) => val.id != id).ToArray().Concat(new[]{ newProject }).ToArray(); 
                    return await jsonResponse(HttpStatusCode.NoContent, "");
                }
                return await jsonResponse(HttpStatusCode.NotFound, "프로젝트를 찾지 못했습니다");
            }            
            async Task<HttpResponseMessage> patchProjectClientKey(int id)
            {
                if (!isLoggedIn()) return await unauthorized();
                var bodyJson = await request.Content!.ReadAsStringAsync();
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(bodyJson)!;
                var project = projects.FirstOrDefault(x => x.id == id);
                if (project != null) {
                    Random random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var newKey = new string(Enumerable.Repeat(chars, 30).Select(s => s[random.Next(s.Length)]).ToArray());
                    var newProject = new  { 
                        id = project.id, 
                        project_name = project.project_name, 
                        project_id = project.project_id,
                        created_at = project.created_at, 
                        updated_at = project.updated_at, 
                        client_key = newKey,
                        credentials= project.credentials,
                    };
                    projects = projects.Where((val) => val.id != id).ToArray().Concat(new[]{ newProject }).ToArray(); 
                    return await ok(new { client_key= newKey });
                }
                return await jsonResponse(HttpStatusCode.NotFound, "프로젝트를 찾지 못했습니다");
            }

            // helper functions

            async Task<HttpResponseMessage> ok(object body)
            {
                return await jsonResponse(HttpStatusCode.OK, body);
            }

            async Task<HttpResponseMessage> error(string message)
            {
                return await jsonResponse(HttpStatusCode.BadRequest, new { message });
            }

            async Task<HttpResponseMessage> unauthorized()
            {
                return await jsonResponse(HttpStatusCode.Unauthorized, new { message = "Unauthorized" });
            }

            async Task<HttpResponseMessage> jsonResponse(HttpStatusCode statusCode, object content)
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                };
                
                // delay to simulate real api call
                await Task.Delay(500);

                return response;
            }

            bool isLoggedIn()
            {
                return request.Headers.Authorization?.Parameter == "ddd.eee.qqq";
            } 
        }
    }
}