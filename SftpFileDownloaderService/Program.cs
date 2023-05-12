using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using SftpFileDownloaderService.database;
using SftpFileDownloaderService.Models;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var sftpService = host.Services.GetRequiredService<SftpService>();
        var dbContext = host.Services.GetRequiredService<AppDbContext>();

        // Start the service to connect to SFTP and download new files
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var serviceTask = sftpService.StartAsync(cancellationToken);

        // Gracefully stop the service and clean up resources
        cancellationTokenSource.Cancel();
        serviceTask.GetAwaiter().GetResult();

        dbContext.Dispose();
        host.Dispose();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(hostContext.Configuration.GetConnectionString("YourConnectionStringName")));

                services.AddScoped<SftpService>();

                // Configure other services and dependencies here
            });
}

public class SftpService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public SftpService(IConfiguration configuration, AppDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // Connect to SFTP server
            var sftpClient = ConnectToSftpServer();

            // Get the list of files on the server
            var files = sftpClient.ListDirectory(GetSftpDirectoryPath());

            // Check for new files and download them
            foreach (var file in files)
            {
                if (IsNewFile(file))
                {
                    DownloadFile(sftpClient, file);
                    SaveFileToDatabase(file);
                }
            }

            // Disconnect from SFTP server
            sftpClient.Disconnect();

            // Wait for 1 minute before checking again
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private SftpClient ConnectToSftpServer()
    {

        var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        var sftpConfig = configuration.GetSection("Sftp");

        var sftpHost = sftpConfig.GetValue<string>("Host");
        var sftpPort = sftpConfig.GetValue<int>("Port");
        var sftpUsername = sftpConfig.GetValue<string>("Username");
        var sftpPassword = sftpConfig.GetValue<string>("Password");

        var sftpClient = new SftpClient(sftpHost, sftpPort, sftpUsername, sftpPassword);
        sftpClient.Connect();

        return sftpClient;
    }

    private string GetSftpDirectoryPath()
    {
        return _configuration["Sftp:DirectoryPath"];
    }

    private bool IsNewFile(SftpFile file)
    {
        var existingFile = _dbContext.YourModels.FirstOrDefault(f => f.FileName == file.Name);
        return existingFile == null;
    }

    private void DownloadFile(SftpClient sftpClient, SftpFile file)
    {
        var localPath = _configuration["Local:Path"];
        var localFilePath = Path.Combine(localPath, file.Name);

        using (var fileStream = File.OpenWrite(localFilePath))
        {
            sftpClient.DownloadFile(file.FullName, fileStream);
        }
    }

    private void SaveFileToDatabase(SftpFile file)
    {
        var newFile = new FileModel
        {
            FileName = file.Name,
            FilePath = file.FullName,
            CreatedAt = file.LastWriteTime,
        };

        _dbContext.YourModels.Add(newFile);
        _dbContext.SaveChanges();
    }
}
