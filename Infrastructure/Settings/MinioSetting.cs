using Minio.DataModel.Args;

namespace Infrastructure.Settings;

public class MinioSetting
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public bool UseSSL { get; set; }
}