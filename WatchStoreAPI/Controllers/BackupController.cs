using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

namespace WatchStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly string _backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "backups");

        public BackupController()
        {
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        [HttpPost("create-backup")]
        public IActionResult CreateBackup()
        {
            try
            {
                string fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.dump";
                string backupPath = Path.Combine(_backupDirectory, fileName);

                // Задаем команду для pg_dump
               string pgDumpCommand = $"PGPASSWORD=Lmntrix1 pg_dump -h localhost -p 5433 -U aidam -d watchstore-db -Fc > \"{backupPath}\"";

                // Используем bash для выполнения команды
                Process process = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{pgDumpCommand}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    return StatusCode(500, $"Ошибка создания бэкапа: {error}");
                }

                return Ok($"Бэкап создан: {fileName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost("restore-backup")]
        public IActionResult RestoreBackup([FromBody] string backupFileName)
        {
            try
            {
                string backupPath = Path.Combine(_backupDirectory, backupFileName);

                if (!System.IO.File.Exists(backupPath))
                {
                    return BadRequest($"Файл {backupFileName} не найден.");
                }

               string pgRestoreCommand = $"PGPASSWORD=Lmntrix1 pg_restore -h localhost -p 5433 -U aidam -d watchstore-db -c \"{backupPath}\"";

                Process process = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{pgRestoreCommand}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    return StatusCode(500, $"Ошибка восстановления: {error}");
                }

                return Ok("Восстановление успешно завершено.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("backup-files")]
        public IActionResult GetBackupFiles()
        {
            var files = Directory.GetFiles(_backupDirectory).Select(Path.GetFileName).ToList();
            return Ok(files);
        }
    }

}
