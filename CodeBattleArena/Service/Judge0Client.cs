using CodeBattleArena.Models;
using CodeBattleArena.Models.DBModels;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeBattleArena.Service
{
    public class Judge0Client
    {
        private static readonly HttpClient client = new HttpClient();

        static Judge0Client()
        {
            client.DefaultRequestHeaders.Add("x-rapidapi-host", "judge0-ce.p.rapidapi.com");
            client.DefaultRequestHeaders.Add("x-rapidapi-key", "399144393dmshbd5c43f06da7a99p108f45jsnc4ad2b38c0b4");
        }

        public static async Task<ExecutionResult> Check(string source_code, string language_id, string stdin, string expected_output)
        {
            var payload = new
            {
                source_code = source_code,
                language_id = language_id,
                stdin = stdin,
                expected_output = expected_output
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;

            try
            {
                response = await client.PostAsync("https://judge0-ce.p.rapidapi.com/submissions?base64_encoded=false&wait=true", content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
                return new ExecutionResult
                {
                    CompileOutput = $"Error: Request failed with message: {ex.Message}"
                };
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    var root = doc.RootElement;

                    // Проверка статуса выполнения
                    var status = root.GetProperty("status").GetProperty("id").GetInt32();
                    if (status == 3) // Успешный статус (Accepted)
                    {
                        string time = root.TryGetProperty("time", out var timeProp) ? timeProp.GetString() : "N/A";
                        string memory = root.TryGetProperty("memory", out var memoryProp) ? memoryProp.GetInt32().ToString() : "N/A";

                        return new ExecutionResult
                        {
                            Time = time,
                            Memory = memory,
                            CompileOutput = null // Нет ошибок компиляции
                        };
                    }
                    else
                    {
                        string error = root.TryGetProperty("stderr", out var stderrProp) ? stderrProp.GetString() : "No stderr output";
                        string compileOutput = root.TryGetProperty("compile_output", out var compileOutputProp) ? compileOutputProp.GetString() : "No compile output";

                        return new ExecutionResult
                        {
                            Time = null,
                            Memory = null,
                            CompileOutput = $"Error: {error}\nCompile Output: {compileOutput}"
                        };
                    }
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ExecutionResult
                {
                    CompileOutput = $"Error: {response.StatusCode}\nDetails: {errorContent}"
                };
            }
        }
    }
}